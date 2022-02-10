using Esri.ArcGISRuntime.Geometry;
using EyeTaxi.Command;
using EyeTaxi.Models;
using EyeTaxiAdmin.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace EyeTaxiAdmin.ViewModel
{
    public class AdminPanelViewModel:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyRaised([CallerMemberName] string propertyname = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        public RelayCommand AdminPanelViewFrameLoad { get; set; }
        public RelayCommand DriversListPageButtonClickCommand { get; set; }
        public RelayCommand PricePageButtonClickCommand { get; set; }
        public RelayCommand StatisticsPageButtonClickCommand { get; set; }
        public RelayCommand DrawerCloseCommand { get; set; }
        public RelayCommand CloseButtonClickCommand { get; set; }
        public AdminPanelViewModel()
        {

            CloseButtonClickCommand = new RelayCommand(s =>
            {
                if (s is Window window)
                {
                    window.Close();
                }
            });

            PricePageButtonClickCommand = new RelayCommand(s =>
            {
                var frame = s as Frame;

                var Content = new PricePage();

                frame.Content = Content;

            });

            DriversListPageButtonClickCommand = new RelayCommand(s =>
            {
                var frame = s as Frame;

                var Content = new DriversListPage();


                frame.Content = Content;
            });

            AdminPanelViewFrameLoad = new RelayCommand(s =>
            {
                var frame = s as Frame;

                var Content = new DriversListPage();

               

                frame.Content = Content;

            });

            StatisticsPageButtonClickCommand = new RelayCommand(s =>
            {
                var frame = s as Frame;

                var Content = new StatisticsView();

                frame.Content = Content;
                

            });

            DrawerCloseCommand = new RelayCommand(s =>
            {
                var temp = s as ToggleButton;
                temp.IsChecked = false;
            });

        }
    }
}
