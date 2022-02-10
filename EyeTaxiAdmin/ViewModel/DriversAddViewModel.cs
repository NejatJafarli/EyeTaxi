using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Portal;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using Esri.ArcGISRuntime.UI.Controls;
using EyeTaxi.Command;
using EyeTaxi.Models;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Color = System.Drawing.Color;
using Window = System.Windows.Window;

namespace EyeTaxiAdmin.ViewModel
{

    public class DriversAddViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyRaised([CallerMemberName] string propertyname = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        private Map _map;
        public Map Map
        {
            get => _map;
            set
            {
                _map = value;
                OnPropertyRaised();
            }
        }
        private async Task SetupMap()
        {
            // Create a portal. If a URI is not specified, www.arcgis.com is used by default.
            ArcGISPortal portal = await ArcGISPortal.CreateAsync();

            // Get the portal item for a web map using its unique item id.
            PortalItem mapItem = await PortalItem.CreateAsync(portal, "41281c51f9de45edaf1c8ed44bb10e30");

            // Create the map from the item.
            Map map = new Map(mapItem);
            //Map map = new Map(SpatialReference.Create(4326));


            map.InitialViewpoint = new Viewpoint(40.409264, 49.867092, 1000000);
            map.Basemap = Basemap.CreateOpenStreetMap();

            // To display the map, set the MapViewModel.Map property, which is bound to the map view.
            this.Map = map;
        }

        public RelayCommand CloseButtonClickCommand { get; set; }
        public RelayCommand AddButtonClickCommand { get; set; }
        public RelayCommand MapRightClickCommand { get; set; }
        public RelayCommand MapLoadedCommand { get; set; }


        private string nameText;

        public string NameText
        {
            get { return nameText; }
            set { nameText = value; OnPropertyRaised(); }
        }

        private string surnameText;

        public string SurnameText
        {
            get { return surnameText; }
            set { surnameText = value; OnPropertyRaised(); }
        }
        private string phoneText;

        public string PhoneText
        {
            get { return phoneText; }
            set { phoneText = value; OnPropertyRaised(); }
        }

        private string carVendorText;

        public string CarVendorText
        {
            get { return carVendorText; }
            set { carVendorText = value; OnPropertyRaised(); }
        }

        private string carModelText;

        public string CarModelText
        {
            get { return carModelText; }
            set { carModelText = value; OnPropertyRaised(); }
        }
        private string carColor;

        public string CarColor
        {
            get { return carColor; }
            set { carColor = value; OnPropertyRaised(); }
        }


        private string carPlateText;

        public string CarPlateText
        {
            get { return carPlateText; }
            set { carPlateText = value; OnPropertyRaised(); }
        }
        public static Point TaxiPoint { get; set; } = new Point(-1, -1);
        public MapView MyMap { get; set; }
        public ObservableCollection<Driver> Drivers { get; set; } = JsonSerializer.Deserialize<ObservableCollection<Driver>>(File.ReadAllText($@"C:\Users\{Environment.UserName}\source\repos\EyeTaxi\EyeTaxi\Json Files\Drivers.json"));
        public static Point MetersToLatLon(Point m)
        {
            var OriginShift = 2 * Math.PI * 6378137 / 2;
            var ll = new Point();
            ll.X = (m.X / OriginShift) * 180;
            ll.Y = (m.Y / OriginShift) * 180;
            ll.Y = 180 / Math.PI * (2 * Math.Atan(Math.Exp(ll.Y * Math.PI / 180)) - Math.PI / 2);
            return ll;
        }
        public DriversAddViewModel()
        {
            _ = SetupMap();
            CloseButtonClickCommand = new RelayCommand(s =>
            {
                if (s is Window window)
                {
                    window.Close();
                }
            });


            MapLoadedCommand = new RelayCommand(s =>
            {
                MyMap = s as MapView;
            });
            MapRightClickCommand = new RelayCommand(s =>
            {
                MyMap.GraphicsOverlays.Clear();
                SimpleMarkerSymbol stopSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Diamond, Color.OrangeRed, 20);

                MyMap.GraphicsOverlays.Add(new GraphicsOverlay());
                var temp = MyMap.ScreenToLocation(TaxiPoint);

               var ConvertWGS84= MetersToLatLon(new Point(temp.X,temp.Y));

                MyMap.GraphicsOverlays[0].Graphics.Add(new Graphic(new MapPoint(ConvertWGS84.X,ConvertWGS84.Y,SpatialReferences.Wgs84), stopSymbol));
            });

            AddButtonClickCommand = new RelayCommand(s =>
            {
                if (!string.IsNullOrWhiteSpace(NameText))
                    if (!string.IsNullOrWhiteSpace(SurnameText))
                        if (!string.IsNullOrWhiteSpace(PhoneText))
                            if (!string.IsNullOrWhiteSpace(CarVendorText))
                                if (!string.IsNullOrWhiteSpace(CarModelText))
                                    if (!string.IsNullOrWhiteSpace(CarPlateText))
                                    {
                                        if (TaxiPoint != new Point(-1, -1))
                                        {
                                            StringBuilder color = new StringBuilder();
                                            for (int i = 2; i < carColor.Length; i++)
                                            {
                                                carColor.Append(carColor[i]);
                                            }
                                            //MapPoint PointTwo = new MapPoint(5571783.59037844, 4933881.61886646, SpatialReferences.WebMercator);

                                            var temp = MyMap.ScreenToLocation(TaxiPoint);
                                            var ConvertWGS84 = MetersToLatLon(new Point(temp.X, temp.Y));

                                            var NewDriver = new Driver(NameText, SurnameText, PhoneText, CarModelText, CarVendorText, CarPlateText, color.ToString(), ConvertWGS84);

                                            Drivers.Add(NewDriver);
                                            //Json Serialize
                                            var TextJson = JsonSerializer.Serialize(Drivers, new JsonSerializerOptions { WriteIndented = true });
                                            File.WriteAllText($@"C:\Users\{Environment.UserName}\source\repos\EyeTaxi\EyeTaxi\Json Files\Drivers.json", TextJson);

                                            NameText = "";
                                            SurnameText = "";
                                            PhoneText = "";
                                            CarVendorText = "";
                                            CarModelText = "";
                                            CarPlateText = "";
                                            MyMap.GraphicsOverlays.Clear();
                                            TaxiPoint = new Point(-1, -1);

                                            Growl.SuccessGlobal("Driver Added Success");
                                            var CloseWindow = s as Window;
                                            CloseWindow.Close();
                                        }
                                        else
                                            //throw handy control global warning right click the map select location
                                            Growl.WarningGlobal("Right click the map select location");
                                    }
            });
        }

    }
}
