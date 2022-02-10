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
using System.Windows.Shapes;

namespace EyeTaxi.Views
{
    /// <summary>
    /// Interaction logic for DriverInfo.xaml
    /// </summary>
    public partial class DriverInfo : Window
    {
       static public string RoutePriceText { get; set; }
        static public bool IsAccept { get; set; } = false;
        public DriverInfo(string PriceTexxt)
        {
            InitializeComponent();
            RoutePriceText = PriceTexxt;
        }
        private void window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
