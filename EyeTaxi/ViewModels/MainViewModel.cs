using Esri.ArcGISRuntime.Geometry;
using EyeTaxi.Command;
using EyeTaxi.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace EyeTaxi.ViewModels
{
    public class MainViewModel
    {
        public RelayCommand GetMapGridCommand { get; set; }
        public MainViewModel()
        {
            GetMapGridCommand = new RelayCommand(s =>
            {
                NavigateRouteView navigateRoute = new NavigateRouteView();
                var frame = s as Frame;

                frame.Content = navigateRoute;
            });


        }
    }
}
