using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BS.Output.FogBugz
{
  public class OutputAddIn: V3.OutputAddIn<Output>
  {

    protected override string Name
    {
      get { return "FogBugz"; }
    }

    protected override Image Image64x32
    {
      get  { return Properties.Resources.logo_64x32; }
    }

    protected override Image Image16x16
    {
      get { return Properties.Resources.logo_16x16 ; }
    }

    protected override bool Editable
    {
      get { return true; }
    }

    protected override string Description
    {
      get { return "Attach screenshots to FogBugz Cases."; }
    }
    
    protected override Output CreateOutput(IWin32Window Owner)
    {

      Output output = new Output(Name, String.Empty, 1);

      return EditOutput(Owner, output);

    }

    protected override Output EditOutput(IWin32Window Owner, Output Output)
    {

      Edit edit = new Edit(Output);

      var ownerHelper = new System.Windows.Interop.WindowInteropHelper(edit);
      ownerHelper.Owner = Owner.Handle;
      
      if (edit.ShowDialog() == true) {

        return new Output(edit.OutputName,
                          edit.Url,
                          Output.LastCaseID);
      }
      else
      {
        return null; 
      }

    }

    protected override OutputValueCollection SerializeOutput(Output Output)
    {

      OutputValueCollection outputValues = new OutputValueCollection();

      outputValues.Add(new OutputValue("Name", Output.Name));
      outputValues.Add(new OutputValue("Url", Output.Url));
      outputValues.Add(new OutputValue("LastCaseID", Convert.ToString(Output.LastCaseID)));

      return outputValues;
      
    }

    protected override Output DeserializeOutput(OutputValueCollection OutputValues)
    {

      return new Output(OutputValues["Name", this.Name].Value,
                        OutputValues["Url", ""].Value, 
                        Convert.ToInt32(OutputValues["LastCaseID", "1"].Value));

    }

    protected async override Task<V3.SendResult> Send(Output Output, V3.ImageData ImageData)
    {
      try
      {

        Send send = new Send(Output.Url, Output.LastCaseID);

        if (send.ShowDialog() != true)
        {
          return new V3.SendResult(V3.Result.Canceled);
        }

        string filePath = Path.Combine(Path.GetTempPath(), "SendToFogBugz.html");

        CreateSendFile(filePath, Output.Url, ImageData.GetImage(), send.Type, send.CaseID);

        System.Diagnostics.Process.Start(filePath);

        if (send.Type == SendType.AttachToCase || send.Type == SendType.ReplyToCase)
        {
          return new V3.SendResult(V3.Result.Success, new Output(Output.Name, Output.Url, send.CaseID));
        }
        else
        {
          return new V3.SendResult(V3.Result.Success);
        }
        
      }
      catch (Exception ex)
      {
        return new V3.SendResult(V3.Result.Failed, ex.Message);
      }
      
    }

    private void CreateSendFile(string filePath, string url, Image image, SendType sendType, int caseID)
    {

      string fileBase64;
      using (MemoryStream stream = new MemoryStream())
      {
        image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
        fileBase64 = Convert.ToBase64String(stream.ToArray());
      }
    
      Int32 fileFragmentCount = Convert.ToInt32(Math.Max(1, Math.Ceiling((decimal)fileBase64.Length / 100000)));

      StringBuilder fileData = new StringBuilder();
      fileData.AppendFormat("<input type=\"hidden\" name=\"cImageFragments\" value=\"{0}\">", fileFragmentCount).AppendLine();
      
      for (Int32 fragmentIndex = 1; fragmentIndex <= fileFragmentCount; fragmentIndex++)
      {
        Int32 fragmentStart = (fragmentIndex - 1) * 100000;
        string fragmentData = fileBase64.Substring(fragmentStart, Math.Min(100000, fileBase64.Length - fragmentStart));

        fileData.AppendFormat("<input type=\"hidden\" name=\"base64png{0}\" value=\"{1}\">", fragmentIndex, fragmentData).AppendLine();

      }

      StringBuilder formData = new StringBuilder();
      switch (sendType)
      {
        case SendType.NewCase:
          formData.AppendLine("<input type=\"hidden\" name=\"fEmail\" value=\"0\">");
          formData.AppendLine("<input type=\"hidden\" name=\"fNewCase\" value=\"1\">");
          formData.Append(fileData);
          formData.Append("<input type=\"hidden\" name=\"dest\" value=\"pg=pgSubmitScreenshot&fNewCase=1&fEmail=0\">");
          break;
        case SendType.NewEmail:
          formData.AppendLine("<input type=\"hidden\" name=\"fEmail\" value=\"1\">");
          formData.AppendLine("<input type=\"hidden\" name=\"fNewCase\" value=\"1\">");
          formData.Append(fileData);
          formData.Append("<input type=\"hidden\" name=\"dest\" value=\"pg=pgSubmitScreenshot&fNewCase=1&fEmail=1\">");
          break;
        case SendType.AttachToCase:
          formData.AppendLine("<input type=\"hidden\" name=\"fEmail\" value=\"0\">");
          formData.AppendLine("<input type=\"hidden\" name=\"fNewCase\" value=\"0\">");
          formData.AppendFormat("<input type=\"hidden\" name=\"ixBug\" value=\"{0}\">", caseID).AppendLine();
          formData.Append(fileData);
          formData.AppendFormat("<input type=\"hidden\" name=\"dest\" value=\"pg=pgSubmitScreenshot&fNewCase=0&fEmail=0&ixBug={0}\">", caseID);
          break;
        case SendType.ReplyToCase:
          formData.AppendLine("<input type=\"hidden\" name=\"fEmail\" value=\"1\">");
          formData.AppendLine("<input type=\"hidden\" name=\"fNewCase\" value=\"0\">");
          formData.AppendFormat("<input type=\"hidden\" name=\"ixBug\" value=\"{0}\">", caseID).AppendLine();
          formData.Append(fileData);
          formData.AppendFormat("<input type=\"hidden\" name=\"dest\" value=\"pg=pgSubmitScreenshot&fNewCase=0&fEmail=1&ixBug={0}\">", caseID);
          break;
      }
      
      StringBuilder fileContent = new StringBuilder(Properties.Resources.SendToFogBugz);
      fileContent.Replace("{TITLE}", "Sending Screenshot to FogBugz...");
      fileContent.Replace("{URL}", url);
      fileContent.Replace("{FORM_DATA}", formData.ToString());

      using (StreamWriter writer = new StreamWriter(filePath))
      {
        writer.WriteLine(fileContent);
        writer.Close();
      }

    }

  }

  public enum SendType
  {
    NewCase, AttachToCase, NewEmail, ReplyToCase
  }

}