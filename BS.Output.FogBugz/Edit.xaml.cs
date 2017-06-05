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
      
      NameTextBox.Text = output.Name;
      UrlTextBox.Text = output.Url;
      
    }

    public string OutputName
    {
      get { return NameTextBox.Text; }
    }

    public string Url
    {
      get { return UrlTextBox.Text; }
    }
    
    private void OK_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
      this.DialogResult = false;
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
      base.OnPreviewKeyDown(e);

      switch (e.Key)
      {
        case Key.Enter:
          OK_Click(this, e);
          break;
        case Key.Escape:
          Cancel_Click(this, e);
          break;
      }
            
    }

  }
}
