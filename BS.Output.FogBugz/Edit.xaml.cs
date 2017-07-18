using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
