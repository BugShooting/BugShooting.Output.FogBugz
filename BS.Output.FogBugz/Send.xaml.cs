using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace BS.Output.FogBugz
{
  partial class Send : Window
  {

    public Send(string url, int lastCaseID)
    {
      InitializeComponent();

      UrlLabel.Text = url;
      AttachToCaseID.Text = lastCaseID.ToString();
      ReplyToCaseID.Text = lastCaseID.ToString();

    }

    public SendType Type
    {
      get
      {

        if (NewCase.IsChecked.Value)
        {
          return SendType.NewCase;
        }
        else if (AttachToCase.IsChecked.Value)
        {
          return SendType.AttachToCase;
        }
        else if (NewEmail.IsChecked.Value)
        {
          return SendType.NewEmail;
        }
        else
        {
          return SendType.ReplyToCase;
        }
      
      }
    }

    public int CaseID
    {
      get
      {

        if (AttachToCase.IsChecked.Value)
        {
          return Convert.ToInt32(AttachToCaseID.Text);
        }
        else
        {
          return Convert.ToInt32(ReplyToCaseID.Text);
        }
        
      }
    }

    private void OK_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
    }

    private void AttachToCase_Checked(object sender, RoutedEventArgs e)
    {
      AttachToCaseID.Focus();
    }

    private void AttachToCaseID_GotFocus(object sender, RoutedEventArgs e)
    {
      AttachToCase.IsChecked = true;
      AttachToCaseID.SelectAll();
    }

    private void ReplyToCase_Checked(object sender, RoutedEventArgs e)
    {
      ReplyToCaseID.Focus();
    }

    private void ReplyToCaseID_GotFocus(object sender, RoutedEventArgs e)
    {
      ReplyToCase.IsChecked = true;
      ReplyToCaseID.SelectAll();
    }

    private void Numeric_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
      e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
    }
     
  }

}
