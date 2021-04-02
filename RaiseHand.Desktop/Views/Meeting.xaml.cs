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

namespace RaiseHand.Desktop.Views
{
    /// <summary>
    /// Interaction logic for Meeting.xaml
    /// </summary>
    public partial class Meeting : UserControl
    {
        public Meeting()
        {
            InitializeComponent();
        }

        private void chkAlwayOnTop_Checked(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            window.Topmost = chkAlwayOnTop.IsChecked == true;
        }
    }
}
