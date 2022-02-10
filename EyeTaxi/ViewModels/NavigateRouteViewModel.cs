using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Location;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Navigation;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.NetworkAnalysis;
using Esri.ArcGISRuntime.UI;
using Color = System.Drawing.Color;
using System.Speech.Synthesis;
using Esri.ArcGISRuntime.Portal;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Esri.ArcGISRuntime.UI.Controls;
using EyeTaxi.Command;
using EyeTaxi.Views;
using Esri.ArcGISRuntime.Tasks.Geocoding;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.ObjectModel;
using System.Text.Json;
using EyeTaxi.Models;

namespace EyeTaxi.ViewModels
{
    public class NavigateRouteViewModel : INotifyPropertyChanged
    {


        private RouteTracker _tracker;

        public TestLoading TestLoading;

        private RouteResult _routeResult;
        private Route _route;

        // List of driving directions for the route.
        private IReadOnlyList<DirectionManeuver> _directionsList;

        // Speech synthesizer to play voice guidance audio.
        private SpeechSynthesizer _speechSynthesizer = new SpeechSynthesizer();

        // Graphics to show progress along the route.
        private Graphic _routeAheadGraphic;
        private Graphic _routeTraveledGraphic;

        // Taxi Point
        public MapPoint PointOne;// = new MapPoint(5571783.59037844, 4933881.61886646, SpatialReferences.Wgs84);

        //Point One
        public MapPoint PointTwo;// = new MapPoint(49.848390, 40.376800, SpatialReferences.Wgs84);

        // Point Two.
        public MapPoint PointThree;// = new MapPoint(49.811980, 40.412180, SpatialReferences.Wgs84);
        public NavigateRouteView View { get; set; }

        // Feature service for routing in World.
        private readonly Uri _routingUri = new Uri("https://route-api.arcgis.com/arcgis/rest/services/World/Route/NAServer/Route_World");

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyRaised([CallerMemberName] string propertyname = null)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
            }
        }

        static public MapView MyMapView { get; set; }

        private bool _StartNavigationButtonIsEnabled;

        public bool TripDetailsButtonIsEnabled
        {
            get { return _StartNavigationButtonIsEnabled; }
            set { _StartNavigationButtonIsEnabled = value; OnPropertyRaised(); }
        }
        private bool _searchNavigationButtonIsEnabled = true;

        public bool SearchNavigationButtonIsEnabled
        {
            get { return _searchNavigationButtonIsEnabled; }
            set { _searchNavigationButtonIsEnabled = value; OnPropertyRaised(); }
        }
        private bool _RecenterButtonIsEnabled;

        public bool RecenterButtonIsEnabled
        {
            get { return _RecenterButtonIsEnabled; }
            set { _RecenterButtonIsEnabled = value; OnPropertyRaised(); }
        }

        private string _MessagesTextBlockText;

        public string MessagesTextBlockText
        {
            get { return _MessagesTextBlockText; }
            set { _MessagesTextBlockText = value; OnPropertyRaised(); }
        }

        private string pointOneText;

        public string PointOneText
        {
            get { return pointOneText; }
            set { pointOneText = value; OnPropertyRaised(); }
        }

        private string pointTwoText;

        public string PointTwoText
        {
            get { return pointTwoText; }
            set { pointTwoText = value; OnPropertyRaised(); }
        }

        private string _priceText = "";

        public string PriceText
        {
            get { return _priceText; }
            set { _priceText = value; OnPropertyRaised(); }
        }


        static public NavigateRouteViewModel CommandCreatedObject { get; set; }

        public RelayCommand MapViewCommand { get; set; }
        public RelayCommand SearchBtnClickCommand { get; set; }
        public RelayCommand HistoryButton { get; set; }
        public RelayCommand WindowClosingCommand { get; set; }
        public RelayCommand TripDetailsButtonIsCommand { get; set; }
        public RelayCommand RecenterButtonCommand { get; set; }
        public RelayCommand ViewLoadCommand { get; set; }
        public ObservableCollection<Driver> Drivers { get; set; } = JsonSerializer.Deserialize<ObservableCollection<Driver>>(File.ReadAllText($@"C:\Users\{Environment.UserName}\source\repos\EyeTaxi\EyeTaxi\Json Files\Drivers.json"));

        private LocatorTask _geocoder;
        public void InitTaxies(Driver DoNotShowThisDriver = null)
        {

            Assembly currentAssembly = GetType().GetTypeInfo().Assembly;

            var uri = new Uri($@"C:\Users\{Environment.UserName}\source\repos\EyeTaxi\EyeTaxi\Assets\cab.png");
            var converted = uri.AbsoluteUri;
            PictureMarkerSymbol CabSymbol = new PictureMarkerSymbol(new Uri(converted));

            if (MyMapView.GraphicsOverlays == null) return;
            CabSymbol.Width = 65;
            CabSymbol.Height = 65;

            foreach (var d in Drivers)
            {
                if (DoNotShowThisDriver is null)
                    MyMapView.GraphicsOverlays[1].Graphics.Add(new Graphic(
                        new MapPoint(d.Location.X, d.Location.Y, SpatialReferences.Wgs84),
                        CabSymbol));
                else if (!(DoNotShowThisDriver is null))
                {
                    if (DoNotShowThisDriver != d)
                        MyMapView.GraphicsOverlays[1].Graphics.Add(new Graphic(
                            new MapPoint(d.Location.X, d.Location.Y, SpatialReferences.Wgs84),
                            CabSymbol));
                }

            }
        }
        public double Distance { get; set; }

        public void GpsResponse()
        {
            PointTwo = new MapPoint(MyMapView.LocationDisplay.Location.Position.X, MyMapView.LocationDisplay.Location.Position.Y, SpatialReferences.Wgs84);
        }
        public DriverInfo DriverInfoWindow { get; set; }
        public static User CurrentUser { get; set; }

        private string userUsername;

        public string UserUsername
        {
            get { return userUsername; }
            set { userUsername = value; OnPropertyRaised(); }
        }

        private string userEmail;

        public string UserEmail
        {
            get { return userEmail; }
            set { userEmail = value; OnPropertyRaised(); }
        }

        public NavigateRouteViewModel()
        {
            WindowClosingCommand = new RelayCommand(s =>
            {
                System.Environment.Exit(0);
                if (!(DriverInfoWindow is null))
                    DriverInfoWindow.Close();
                //ratingView.Close();
            });

            HistoryButton = new RelayCommand(s =>
            {
                var temp2 = CurrentUser;
                var temp = new HistoryView(temp2);
                temp.ShowDialog();
            });

            MapViewCommand = new RelayCommand(s =>
            {
                MyMapView = s as MapView;
                MyMapView.GraphicsOverlays.Add(new GraphicsOverlay());
                MyMapView.GraphicsOverlays.Add(new GraphicsOverlay());


                UserUsername = CurrentUser.Username;
                UserEmail = CurrentUser.Email;

                Initialize();

                MyMapView.LocationDisplay.IsEnabled = true;

                //System.Threading.Thread.Sleep(5000);
                while (MyMapView.LocationDisplay.Started == false)
                {
                    TestLoading.Visibility = Visibility.Visible;
                }
                if (MyMapView.LocationDisplay.Started)
                {
                    PointTwo = new MapPoint(MyMapView.LocationDisplay.Location.Position.X, MyMapView.LocationDisplay.Location.Position.Y, SpatialReferences.Wgs84);
                }
                else
                {
                    MessageBox.Show("Please enable your location settings to show current location or enter manually");
                }

                InitTaxies();
                //for (int i = 0; i < MyMapView.GraphicsOverlays[0].Graphics.Count; i++)
                //Taxies.Add(MyMapView.GraphicsOverlays[0].Graphics[i]);
                //PointTwo = new MapPoint(MyMapView.LocationDisplay.Location.Position.X, MyMapView.LocationDisplay.Location.Position.Y, SpatialReferences.Wgs84);
                //MyMapView.LocationDisplay.IsEnabled = true;

                //MyMapView.LocationDisplay.IsEnabled = true;
                //PointTwo = MyMapView.LocationDisplay.;


                //POINT TWO ya Current Lokasya verilmelidir!!!


                GetCurrentPointAddressName();
            });


            RecenterButtonCommand = new RelayCommand(s =>
            {
                MyMapView.LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.Navigation;
            });

            TripDetailsButtonIsCommand = new RelayCommand(s =>
            {
                DriverInfoWindow.ShowDialog();
                if (DriverInfo.IsAccept)
                {
                    DriverInfo.IsAccept = false;
                    StartNavigation();
                }
            });

            ViewLoadCommand = new RelayCommand(s =>
            {
                View = s as NavigateRouteView;
            });

            SearchBtnClickCommand = new RelayCommand(s =>
            {
                if (!(MyMapView is null))
                    if (!string.IsNullOrWhiteSpace(PointTwoText))
                    {
                        TripDetailsButtonIsEnabled = false;
                        PriceText = "";
                        MyMapView.GraphicsOverlays.Clear();
                        Temp(); //calculate 2 difrent location route
                    }
            });

            CommandCreatedObject = this;

        }

        public string CurrentPlaceName { get; set; } = "Any Problem";
        public async void GetCurrentPointAddressName()
        {
            Uri Link = new Uri("https://geocode-api.arcgis.com/arcgis/rest/services/World/GeocodeServer");
            try
            {
                _geocoder = await LocatorTask.CreateAsync(Link);
                //var a = MyMapView.LocationDisplay.Location;

                IReadOnlyList<GeocodeResult> addresses = await _geocoder.ReverseGeocodeAsync(PointTwo);
                GeocodeResult address = addresses.First();
                PointOneText = address.Attributes["Address"].ToString();
                CurrentPlaceName = address.Attributes["Address"].ToString();
                if (string.IsNullOrWhiteSpace(PointOneText))
                {
                    PointOneText = "   ";
                    CurrentPlaceName = "   ";
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error");
            }
        }
        public Driver SelectedDriver { get; set; }
        public List<Graphic> Taxies { get; set; } = new List<Graphic>();
        public RatingView ratingView { get; set; }
        public async void Temp()
        {
            try
            {
                MyMapView.GraphicsOverlays.Clear();
                Uri Link = new Uri("https://geocode-api.arcgis.com/arcgis/rest/services/World/GeocodeServer");
                _geocoder = await LocatorTask.CreateAsync(Link);


                if (CurrentPlaceName != PointOneText)
                {
                    IReadOnlyList<SuggestResult> suggestionsOne = await _geocoder.SuggestAsync(PointOneText);
                    SuggestResult firstSuggestion = suggestionsOne.First();
                    var addressesOne = await _geocoder.GeocodeAsync(firstSuggestion.Label);
                    var mapPointOne = addressesOne.First().DisplayLocation;
                    PointTwo = mapPointOne;
                }
                else
                    GetCurrentPointAddressName();
                IReadOnlyList<SuggestResult> suggestionsTwo = await _geocoder.SuggestAsync(PointTwoText);

                SuggestResult SecondSuggestion = suggestionsTwo.First();

                var addressesTwo = await _geocoder.GeocodeAsync(SecondSuggestion.Label);

                var mapPointTwo = addressesTwo.First().DisplayLocation;


                PointThree = mapPointTwo;

                List<double> driversdistance = new List<double>();
                for (int i = 0; i < Drivers.Count; i++)
                {
                    RouteTask routeTaskk = await RouteTask.CreateAsync(new Uri("https://route-api.arcgis.com/arcgis/rest/services/World/Route/NAServer/Route_World"));

                    // Get the default route parameters.
                    RouteParameters routeParamss = await routeTaskk.CreateDefaultParametersAsync();

                    // Explicitly set values for parameters.
                    routeParamss.ReturnDirections = true;
                    routeParamss.ReturnStops = true;
                    routeParamss.ReturnRoutes = true;
                    routeParamss.OutputSpatialReference = SpatialReferences.Wgs84;

                    // Create stops for each location.
                    Stop stopp1 = new Stop(new MapPoint(Drivers[i].Location.X, Drivers[i].Location.Y, SpatialReferences.Wgs84));
                    Stop stopp2 = new Stop(PointTwo);

                    // Assign the stops to the route parameters.
                    List<Stop> stopPointss = new List<Stop> { stopp1, stopp2 };
                    routeParamss.SetStops(stopPointss);

                    // Get the route results.
                    RouteResult result = await routeTaskk.SolveRouteAsync(routeParamss);
                    Route route = result.Routes[0];

                    driversdistance.Add(route.TotalLength / 1000);

                }
                var Min = driversdistance[0];
                SelectedDriver = Drivers[0];
                for (int i = 0; i < driversdistance.Count; i++)
                {
                    if (driversdistance[i] < Min)
                    {
                        Min = driversdistance[i];
                        SelectedDriver = Drivers[i];
                    }
                }
                PointOne = new MapPoint(SelectedDriver.Location.X, SelectedDriver.Location.Y, SpatialReferences.Wgs84);
                //Taxi Locations This
                //PointOne = new MapPoint(P1.X, P1.Y, SpatialReferences.WebMercator);


                //PointTwo = new MapPoint(P1.X, P1.Y, SpatialReferences.WebMercator);
                //PointThree = new MapPoint(P2.X, P2.Y, SpatialReferences.WebMercator);
                //CalculateRoute();


                // Create the route task, using the online routing service.
                RouteTask routeTask = await RouteTask.CreateAsync(_routingUri);

                // Get the default route parameters.
                RouteParameters routeParams = await routeTask.CreateDefaultParametersAsync();

                // Explicitly set values for parameters.
                routeParams.ReturnDirections = true;
                routeParams.ReturnStops = true;
                routeParams.ReturnRoutes = true;
                routeParams.OutputSpatialReference = SpatialReferences.Wgs84;

                // Create stops for each location.
                Stop stops = new Stop(PointOne);
                Stop stops1 = new Stop(PointTwo);
                Stop stops2 = new Stop(PointThree);

                // Assign the stops to the route parameters.
                List<Stop> stopPoints = new List<Stop> { stops, stops1, stops2 };
                routeParams.SetStops(stopPoints);

                // Get the route results.
                _routeResult = await routeTask.SolveRouteAsync(routeParams);
                _route = _routeResult.Routes[0];

                Distance = _route.TotalLength / 1000;


                // Add a graphics overlay for the route graphics.
                MyMapView.GraphicsOverlays.Clear();
                MyMapView.GraphicsOverlays.Add(new GraphicsOverlay());
                MyMapView.GraphicsOverlays.Add(new GraphicsOverlay());


                // Add graphics for the stops.
                SimpleMarkerSymbol stopSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Diamond, Color.OrangeRed, 20);
                MyMapView.GraphicsOverlays[0].Graphics.Add(new Graphic(PointTwo, stopSymbol));
                MyMapView.GraphicsOverlays[0].Graphics.Add(new Graphic(PointThree, stopSymbol));

                // Create a graphic (with a dashed line symbol) to represent the route.
                _routeAheadGraphic = new Graphic(_route.RouteGeometry) { Symbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, Color.FromArgb(71, 96, 243), 5) };

                // Create a graphic (solid) to represent the route that's been traveled (initially empty).
                _routeTraveledGraphic = new Graphic { Symbol = new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, Color.FromArgb(163, 175, 249), 3) };

                // Add the route graphics to the map view.
                MyMapView.GraphicsOverlays[0].Graphics.Add(_routeAheadGraphic);
                MyMapView.GraphicsOverlays[0].Graphics.Add(_routeTraveledGraphic);

                // Set the map viewpoint to show the entire route.
                await MyMapView.SetViewpointGeometryAsync(_route.RouteGeometry, 100);

                // Enable the navigation button.
                TripDetailsButtonIsEnabled = true;




                //1.521878123

                //   (0.5*10)=5.21878123
                //   convert int 5
                //   5/10=0.5
                //   1+0.5=1.5

                var temp5 = (Distance * double.Parse(File.ReadAllText($@"C:\Users\{Environment.UserName}\source\repos\EyeTaxi\EyeTaxi\Json Files\PricePer-KM.json")));

                PriceText = $"{(double)((int)temp5) + ((double)((int)((((temp5 - (int)(temp5)) * 10)) / 10)))} Manat Tuttu baslamaq Ucun Start basin";
                InitTaxies();

                DriverInfoViewModel.MyDriver = SelectedDriver;
                DriverInfoWindow = new DriverInfo(temp5.ToString());
                DriverInfoWindow.ShowDialog();
                if (DriverInfo.IsAccept)
                {
                    DriverInfo.IsAccept = false;
                    StartNavigation();
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                MyMapView.GraphicsOverlays.Clear();
                MyMapView.GraphicsOverlays.Add(new GraphicsOverlay());
                MyMapView.GraphicsOverlays.Add(new GraphicsOverlay());
                InitTaxies();
            }
        }

        public async void Initialize()
        {
            try
            {
                // Add event handler for when this sample is unloaded.
                MyMapView.Unloaded += SampleUnloaded;

                // Create a portal. If a URI is not specified, www.arcgis.com is used by default.
                ArcGISPortal portal = await ArcGISPortal.CreateAsync();

                // Get the portal item for a web map using its unique item id.
                PortalItem mapItem = await PortalItem.CreateAsync(portal, "41281c51f9de45edaf1c8ed44bb10e30");

                // Create the map from the item.
                Map map = new Map(mapItem);

                map.InitialViewpoint = new Viewpoint(40.409264, 49.867092, 100000);
                map.Basemap = Basemap.CreateOpenStreetMap();

                // Create the map view.
                MyMapView.Map = map;


            }
            catch (Exception e)
            {
                if (e.Message != "Value cannot be null.\r\nParameter name: point")
                    MessageBox.Show(e.Message, "Error");
            }
        }

        private void StartNavigation()
        {
            try
            {

                //InitTaxies(SelectedDriver);
                //MyMapView.GraphicsOverlays[0].Graphics.Remove(new Graphic(
                //            new MapPoint(SelectedDriver.Location.X, SelectedDriver.Location.Y, SpatialReferences.Wgs84),
                //            CabSymbol);

                MyMapView.GraphicsOverlays[1].Graphics.Clear();
                InitTaxies(SelectedDriver);

                NavigateRouteView.IsNagivateStart = false;
                // Disable the start navigation button.

                TripDetailsButtonIsEnabled = false;
                SearchNavigationButtonIsEnabled = false;

                // Get the directions for the route.
                _directionsList = _route.DirectionManeuvers;

                // Create a route tracker.
                _tracker = new RouteTracker(_routeResult, 0, true);
                _tracker.NewVoiceGuidance += SpeakDirection;

                // Handle route tracking status changes.
                _tracker.TrackingStatusChanged += TrackingStatusUpdated;

                // Turn on navigation mode for the map view.
                MyMapView.LocationDisplay.AutoPanMode = LocationDisplayAutoPanMode.Navigation;
                MyMapView.LocationDisplay.AutoPanModeChanged += AutoPanModeChanged;

                // Add a data source for the location display.
                // Speed
                var simulationParameters = new SimulationParameters(DateTimeOffset.Now, 300);
                var simulatedDataSource = new SimulatedLocationDataSource();
                simulatedDataSource.SetLocationsWithPolyline(_route.RouteGeometry, simulationParameters);
                MyMapView.LocationDisplay.DataSource = new RouteTrackerDisplayLocationDataSource(simulatedDataSource, _tracker);

                // Use this instead if you want real location:
                // MyMapView.LocationDisplay.DataSource = new RouteTrackerLocationDataSource(new SystemLocationDataSource(), _tracker);

                // Enable the location display (this wil start the location data source).
                MyMapView.LocationDisplay.IsEnabled = true;
            }
            catch (Exception e)
            {
                MessageBox.Show($"ERROR MESSAGE:{e.Message}\nYou cannot reach to this location", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                TripDetailsButtonIsEnabled = true;
                SearchNavigationButtonIsEnabled = true;
            }
        }
        public int DestinationCounter { get; set; } = 0;
        public List<User> Users { get; set; } = JsonSerializer.Deserialize<List<User>>(File.ReadAllText($@"C:\Users\{Environment.UserName}\source\repos\EyeTaxi\EyeTaxi\Json Files\Users.json"));

        private void TrackingStatusUpdated(object sender, RouteTrackerTrackingStatusChangedEventArgs e)
        {
            TrackingStatus status = e.TrackingStatus;

            // Start building a status message for the UI.
            System.Text.StringBuilder statusMessageBuilder = new System.Text.StringBuilder("Route Status:\n");

            // Check the destination status.
            if (status.DestinationStatus == DestinationStatus.NotReached || status.DestinationStatus == DestinationStatus.Approaching)
            {
                statusMessageBuilder.AppendLine("Distance remaining: " +
                                            status.RouteProgress.RemainingDistance.DisplayText + " " +
                                            status.RouteProgress.RemainingDistance.DisplayTextUnits.PluralDisplayName);

                statusMessageBuilder.AppendLine("Time remaining: " +
                                                status.RouteProgress.RemainingTime.ToString(@"hh\:mm\:ss"));


                if (status.CurrentManeuverIndex + 1 < _directionsList.Count)
                {
                    statusMessageBuilder.AppendLine("Next direction: " + _directionsList[status.CurrentManeuverIndex + 1].DirectionText);
                }

                // Set geometries for progress and the remaining route.
                _routeAheadGraphic.Geometry = status.RouteProgress.RemainingGeometry;
                _routeTraveledGraphic.Geometry = status.RouteProgress.TraversedGeometry;
                SearchNavigationButtonIsEnabled = false;

            }
            else if (status.DestinationStatus == DestinationStatus.Reached)
            {
                statusMessageBuilder.AppendLine("Destination reached.");
                SearchNavigationButtonIsEnabled = true;

                DestinationCounter += 1;

                if (DestinationCounter >= 2)
                {
                    SelectedDriver.Location = new Point(PointThree.X, PointThree.Y);

                    double Price = 0;
                    if (!string.IsNullOrWhiteSpace(PriceText))
                    {
                        var temp = (Distance * double.Parse(File.ReadAllText($@"C:\Users\{Environment.UserName}\source\repos\EyeTaxi\EyeTaxi\Json Files\PricePer-KM.json")));
                        //1.55636
                        var temp2 = temp - (int)temp;
                        //0.55636
                        temp2 = temp2 * 100;
                        //55.5636
                        temp2 = (int)temp2;
                        //55
                        temp2 = temp2 / 100;
                        //0.55

                        temp = (int)temp;
                        //1
                        SelectedDriver.Balance += temp + temp2;
                        Price = temp + temp2;
                        SelectedDriver.CompanyBenefit = SelectedDriver.Balance * 15 / 100;

                        SelectedDriver.DriverBenefit = SelectedDriver.Balance - SelectedDriver.CompanyBenefit;
                    }
                    SelectedDriver.CountTravel += 1;

                    DestinationCounter = 0;
                    _tracker.TrackingStatusChanged -= TrackingStatusUpdated;

                    foreach (var item in Users)
                    {
                        if (item.Username == CurrentUser.Username)
                        {
                            item.History.Add(new History
                            {
                                DriverName = SelectedDriver.Name,
                                DriverSurname = SelectedDriver.Surname,
                                Price = Price,
                                PointOneText = PointOneText,
                                PointTwoText = PointTwoText
                            });
                            CurrentUser = item;
                            break;
                        }
                    }

                    var TextJso1n = JsonSerializer.Serialize(Users, new JsonSerializerOptions() { WriteIndented = true });
                    File.WriteAllText($@"C:\Users\{Environment.UserName}\source\repos\EyeTaxi\EyeTaxi\Json Files\Users.json", TextJso1n);


                    View.Dispatcher.Invoke(() =>
                    {
                        MyMapView.GraphicsOverlays[1].Graphics.Clear();
                        InitTaxies();
                        MyMapView.SetViewpointRotationAsync(0);

                        RatingViewModel.SelectedDriver = SelectedDriver;
                        ratingView = new RatingView();
                        ratingView.ShowDialog();

                        var TextJson = JsonSerializer.Serialize(Drivers, new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText($@"C:\Users\{Environment.UserName}\source\repos\EyeTaxi\EyeTaxi\Json Files\Drivers.json", TextJson);
                    });
                }


                // Set the route geometries to reflect the completed route.
                _routeAheadGraphic.Geometry = null;
                _routeTraveledGraphic.Geometry = status.RouteResult.Routes[0].RouteGeometry;

                // Navigate to the next stop (if there are stops remaining).
                if (status.RemainingDestinationCount > 1)
                {
                    _tracker.SwitchToNextDestinationAsync();
                }
                else
                {
                    View.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        // Stop the simulated location data source.
                        MyMapView.LocationDisplay.DataSource.StopAsync();
                    });
                }
            }

            View.Dispatcher.BeginInvoke((Action)delegate ()
            {
                // Show the status information in the UI.
                MessagesTextBlockText = statusMessageBuilder.ToString();
            });
        }

        private void SpeakDirection(object sender, RouteTrackerNewVoiceGuidanceEventArgs e)
        {
            // Say the direction using voice synthesis.
            // _speechSynthesizer.SpeakAsyncCancelAll();
            // _speechSynthesizer.SpeakAsync(e.VoiceGuidance.Text);
        }

        private void AutoPanModeChanged(object sender, LocationDisplayAutoPanMode e)
        {
            // Turn the recenter button on or off when the location display changes to or from navigation mode.
            RecenterButtonIsEnabled = e != LocationDisplayAutoPanMode.Navigation;
        }

        private void SampleUnloaded(object sender, RoutedEventArgs e)
        {
            // Stop the speech synthesizer.
            _speechSynthesizer.SpeakAsyncCancelAll();
            _speechSynthesizer.Dispose();

            // Stop the tracker.
            if (_tracker != null)
            {
                _tracker.TrackingStatusChanged -= TrackingStatusUpdated;
                _tracker.NewVoiceGuidance -= SpeakDirection;
                _tracker = null;
            }

            // Stop the location data source.
            MyMapView.LocationDisplay?.DataSource?.StopAsync();
        }
    }

    // This location data source uses an input data source and a route tracker.
    // The location source that it updates is based on the snapped-to-route location from the route tracker.
    public class RouteTrackerDisplayLocationDataSource : LocationDataSource
    {
        private LocationDataSource _inputDataSource;
        private RouteTracker _routeTracker;

        public RouteTrackerDisplayLocationDataSource(LocationDataSource dataSource, RouteTracker routeTracker)
        {
            // Set the data source
            _inputDataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));

            // Set the route tracker.
            _routeTracker = routeTracker ?? throw new ArgumentNullException(nameof(routeTracker));

            // Change the tracker location when the source location changes.
            _inputDataSource.LocationChanged += InputLocationChanged;

            // Update the location output when the tracker location updates.
            _routeTracker.TrackingStatusChanged += TrackingStatusChanged;
        }

        private void InputLocationChanged(object sender, Location e)
        {
            // Update the tracker location with the new location from the source (simulation or GPS).
            _routeTracker.TrackLocationAsync(e);
        }

        private void TrackingStatusChanged(object sender, RouteTrackerTrackingStatusChangedEventArgs e)
        {
            // Check if the tracking status has a location.
            if (e.TrackingStatus.DisplayLocation != null)
            {
                // Call the base method for LocationDataSource to update the location with the tracked (snapped to route) location.
                UpdateLocation(e.TrackingStatus.DisplayLocation);
            }
        }

        protected override Task OnStartAsync() => _inputDataSource.StartAsync();

        protected override Task OnStopAsync() => _inputDataSource.StartAsync();
    }
}
