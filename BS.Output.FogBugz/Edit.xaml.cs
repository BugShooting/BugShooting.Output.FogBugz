using System.Windows;
using System.Windows.Controls;

namespace BS.Output.FogBugz
{
  partial class Edit : Window
  {
    public Edit(Output output)
    {
      InitializeComponent();

      this.DataContext = this;

      OutputName = output.Name;
      Url = output.Url;

    }

    public string OutputName { get; set; }

    public string Url { get; set; }

    private void ValidateData(object sender, RoutedEventArgs e)
    {
      OK.IsEnabled = !Validation.GetHasError(NameTextBox) &&
                     !Validation.GetHasError(UrlTextBox);
    }

    private void OK_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
    }

  }
}
