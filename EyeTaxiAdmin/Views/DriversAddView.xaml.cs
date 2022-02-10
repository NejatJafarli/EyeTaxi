using EyeTaxiAdmin.ViewModel;
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

namespace EyeTaxiAdmin.Views
{
    /// <summary>
    /// Interaction logic for DriversAddView.xaml
    /// </summary>
    public partial class DriversAddView : Window
    {
        public DriversAddView()
        {
            InitializeComponent();

        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void MyMap_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            DriversAddViewModel.TaxiPoint = Mouse.GetPosition(MyMap);
            //= MyMapView.ScreenToLocation(mousePoint);
        }
        /*private void ColorPicker_MouseMove(object sender, MouseEventArgs e)
{
   var converter = new System.Windows.Media.BrushConverter();
   var brush = (Brush)converter.ConvertFromString(ColorPicker.Color.ToString());

   Border.Background = brush;
}*/
    }
}
