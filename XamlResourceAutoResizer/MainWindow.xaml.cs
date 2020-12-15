using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using XamlResourceAutoResizer.Properties;
using XamlResourceAutoResizer.ViewModel;

namespace XamlResourceAutoResizer
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private readonly MainWindowViewModel _dc = new MainWindowViewModel();

    public MainWindow()
    {
      DataContext = _dc;
      InitializeComponent();
      PathTb.Text = Settings.Default.InputPathSetting;
    }

    /// <summary>Raises the <see cref="E:System.Windows.Window.Closing" /> event.</summary>
    /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs" /> that contains the event data.</param>
    protected override void OnClosing(CancelEventArgs e)
    {
      base.OnClosing(e);
      Settings.Default.InputPathSetting = PathTb.Text;
      Settings.Default.Save();
      DataContext = null;
    }

    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
      _dc.PopulateResults(PathTb.Text, false);
    }
  }
}
