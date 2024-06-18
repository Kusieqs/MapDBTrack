using Microsoft.Maps.MapControl.WPF;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
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

namespace MapDBTrack
{
    public partial class MainWindow : Window
    {

        public static int idOfEmployee;
        public static string loginOfEmployee;
        public static List<Place> places = new List<Place>();
        private Button adding;
        private Button removing;
        private AddingCustomer addingCustomer;
        private Map map;
        private bool pinned = false;
        private bool removed = false;
        public static bool acceptOverridePin = false;
        public static bool customerMode = true;


        public MainWindow(int id, string login)
        {
            InitializeComponent();
            idOfEmployee = id;
            loginOfEmployee = login;
            LoadingMapScreen();

            // Text on the top of menu buttons
            LoginName.Text = "Hi, " + loginOfEmployee;
        }

        private void MapClick(object sender, RoutedEventArgs e)
        {
            HelpingClass.CleanGrid(mapBorder);
            LoadingMapScreen();
        } // button to load map
        private void LoadingMapScreen()
        {
            // Checking network connection
            HelpingClass.NetworkCheck(this);

            // setting grid row definitions
            mapBorder.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(4, GridUnitType.Star) });
            mapBorder.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            // Creating map
            map = new Map()
            {
                CredentialsProvider = new ApplicationIdCredentialsProvider(HelpingClass.connectMap), // Api key
                Mode = new AerialMode(true), // setting satelite map
                Center = new Location(52.2387, 19.0478), // setting center of map
                Culture = "en-US", // setting language
                ZoomLevel = 6.7, // setting starting zoom
            };

            // Special method to putting pins on map
            map.MouseLeftButtonDown += MapPuttingPins;
            Grid.SetRowSpan(map, 2);
            Grid.SetRow(map, 0);
            mapBorder.Children.Add(map);

            // creating border for button
            Border buttonBorder = new Border()
            {
                Width = 150,
                Height = 60,
                Margin = new Thickness(0, 0, 0, 0),
                Background = new SolidColorBrush("#FF7B4BA5".ToColor()),

            };
            buttonBorder.CornerRadius = new CornerRadius(20);
            Grid.SetColumn(buttonBorder, 0);
            Grid.SetRow(buttonBorder, 1);

            // creating grid for buttons
            Grid buttonsMap = new Grid();
            buttonsMap.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            buttonsMap.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            // creating button to add pin
            adding = new Button();
            adding.Style = FindResource("ButtonsAddPins") as Style;
            adding.Click += AddPin;
            System.Windows.Controls.Image imageAdd = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Pictures/add.png")),
                Width = 42,
                Height = 42,
            };
            RenderOptions.SetBitmapScalingMode(imageAdd, BitmapScalingMode.HighQuality);
            adding.Content = imageAdd;
            Grid.SetColumn(adding, 0);
            Grid.SetRow(adding, 0);
            buttonsMap.Children.Add(adding);

            // creating button do remove pin
            removing = new Button();
            removing.Style = FindResource("ButtonsRemovePins") as Style;
            removing.Click += RemovePin;
            System.Windows.Controls.Image imageRemove = new System.Windows.Controls.Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Pictures/bin.png")),
                Width = 42,
                Height = 42,
            };
            RenderOptions.SetBitmapScalingMode(imageRemove, BitmapScalingMode.HighQuality);
            removing.Content = imageRemove;
            Grid.SetColumn(removing, 1);
            Grid.SetRow(removing, 0);
            buttonsMap.Children.Add(removing);

            // creating grid for all objects and adding to main window
            buttonBorder.Child = buttonsMap;
            mapBorder.Children.Add(buttonBorder);

            LoadingPinns();

        } // loading map 
        private void LoadingCustomerScreen()
        {
            // Checking network connection
            HelpingClass.NetworkCheck(this);

            //Creating border

            Border customerBorder = new Border()
            {
                CornerRadius = new CornerRadius(0, 15, 15, 0),
                Background = Brushes.White,
            };
            Grid.SetColumn(customerBorder, 1);

            //Creating grid

            Grid customerGrid = new Grid()
            {
                Margin = new Thickness(20)
            };

            customerGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(70, GridUnitType.Pixel) });
            customerGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Pixel) });
            customerGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });
            customerGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });


            // creating line
            Separator separator = new Separator()
            {
                Background = new SolidColorBrush("#dae2ea".ToColor()),
                Margin = new Thickness(370, 55, 400, 14)
            };

            //creating stackpanel with buttons

            StackPanel stackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(425, 10, 457, 13)
            };
            Grid.SetRow(stackPanel, 0);


            Button personalBtn = new Button()
            {
                Content = "Personal",
                Style = FindResource("tabButton") as Style,
                Width = 78,
                Margin = new Thickness(0,0,20,0),
                FontFamily = new FontFamily("Calibri"),
                BorderBrush = Brushes.Transparent,
            };

            Button addressBtn = new Button()
            {
                Content = "Address",
                Style = FindResource("tabButton") as Style,
                Width = 78,
                FontFamily = new FontFamily("Calibri"),
                BorderBrush = Brushes.Transparent,
                
            };

            if(customerMode)
                personalBtn.BorderBrush = new SolidColorBrush("#FF7B4BA5".ToColor());
            else
                addressBtn.BorderBrush = new SolidColorBrush("#FF7B4BA5".ToColor());

            //creating buttons to Delete customer and creating report

            Button reportBtn = new Button()
            {
                Width = 110,
                Height = 45,
                Content = "Report",
                FontSize = 20,
                Margin = new Thickness(940,8,10,17),
                Background = new SolidColorBrush("#FF7B4BA5".ToColor())
            };

            Button deleteBtn = new Button()
            {
                Width = 110,
                Height = 45,
                Content = "Report",
                FontSize = 20,
                Margin = new Thickness(798, 8, 152, 17),
                Background = new SolidColorBrush("#FF7B4BA5".ToColor())
            };

            // Creating scrollviewer

            ScrollViewer scrollViewer = scrollMode();
            Grid.SetRow(scrollViewer, 3);

            


            #region adding elements
            customerGrid.Children.Add(separator);
            stackPanel.Children.Add(personalBtn);
            stackPanel.Children.Add(addressBtn);
            customerGrid.Children.Add(stackPanel);
            customerGrid.Children.Add(deleteBtn);
            customerGrid.Children.Add(reportBtn);

            customerGrid.Children.Add(scrollViewer);
            customerBorder.Child = customerGrid;
            mapBorder.Children.Add(customerBorder);
            #endregion adding elements


        } // loading customer list
        private void CustomersClick(object sender, RoutedEventArgs e)
        {
            HelpingClass.CleanGrid(mapBorder);
            LoadingCustomerScreen();
        } // Changing view to customer window
        private void HistoryClick(object sender, RoutedEventArgs e)
        {

        } // history button
        private void LogoutClick(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        } // button logout 
        private void Information(object sender, RoutedEventArgs e)
        {
            string info = $"{HelpingClass.version}\nContact: kus.konrad1@gmail.com\nLicense: MapDBTrack Commercial Use License";
            MessageBox.Show(info, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        } // special MessageBox with version etc.
        private void AddPin(object sender, RoutedEventArgs e)
        {
            if (removed == false)
            {
                if (pinned == true)
                {
                    removing.IsEnabled = true;
                    pinned = false;
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
                else
                {
                    removing.IsEnabled = false;
                    pinned = true;
                    Mouse.OverrideCursor = Cursors.Hand;
                }
            }
        } // set true for added new pin to map or false
        private void RemovePin(object sender, RoutedEventArgs e)
        {
            if (pinned == false)
            {
                if (removed == true)
                {
                    adding.IsEnabled = true;
                    removed = false;
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
                else
                {
                    adding.IsEnabled = false;
                    removed = true;
                    Mouse.OverrideCursor = Cursors.Hand;
                }
            }
        } // removing pin from map
        private void MapPuttingPins(object sender, MouseButtonEventArgs e)
        {
            HelpingClass.NetworkCheck(this);

            if (pinned)
            {
                Point mousePosition = e.GetPosition(mapBorder);
                Location pinLocation = map.ViewportPointToLocation(mousePosition);
                Pushpin pin = SetPinns(pinLocation, true);
                map.Children.Add(pin);

                pinned = false;
                Mouse.OverrideCursor = Cursors.Arrow;

                addingCustomer = new AddingCustomer(pinLocation, this, pin, map);
                addingCustomer.Show();


                Map.IsEnabled = false;
                Exit.IsEnabled = false;
                Logout.IsEnabled = false;
                adding.IsEnabled = false;

                addingCustomer.Closed += (s, args) =>
                {

                    Map.IsEnabled = true;
                    Exit.IsEnabled = true;
                    Logout.IsEnabled = true;
                    adding.IsEnabled = true;
                    if (acceptOverridePin)
                    {
                        map.Children.Remove(pin);
                        pin.MouseEnter += PinMouseEnter;
                        pin.MouseLeave += PinMouseLeave;
                        pin.MouseLeftButtonDown += RemovePinFromMap;
                        map.Children.Add(pin);
                    }
                    acceptOverridePin = false;
                };
                removing.IsEnabled = true;
            }

        } // puting pins on map when pinned is true
        private void RemovePinFromMap(object sender, MouseButtonEventArgs e)
        {
            HelpingClass.NetworkCheck(this);

            if (removed)
            {
                Pushpin pushpin = sender as Pushpin;
                if (pushpin != null)
                {
                    e.Handled = true;
                    Place p1 = places.Where(x => x.latitude == pushpin.Location.Latitude && x.longitude == pushpin.Location.Longitude).FirstOrDefault();
                    MessageBoxResult result = MessageBox.Show($"Do you want remove this pin?\n\n{HelpingClass.GetDescTool(p1)}", "Inforamtion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result == MessageBoxResult.Yes)
                    {
                        map.Children.Remove(pushpin);
                        HelpingClass.RemoveRecordFromDB(p1);
                        places.Remove(p1);
                    }
                    adding.IsEnabled = true;
                    removed = false;
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
        } // removing pin from map and DB
        private void LoadingPinns()
        {
            places = HelpingClass.LoadingPlace(idOfEmployee);

            foreach (Place p in places)
            {
                Location pinLocation = new Location(p.latitude, p.longitude);
                Pushpin pin = SetPinns(pinLocation);
                map.Children.Add(pin);
            }

        } // loading all pins when map is close
        private void PinMouseEnter(object sender, MouseEventArgs e)
        {
            Pushpin pin = sender as Pushpin;
            double lat = pin.Location.Latitude;
            double lng = pin.Location.Longitude;
            Place p1 = places.FirstOrDefault(x => x.latitude == lat && x.longitude == lng);

            if (p1 != null)
            {
                ToolTip tooltip = new ToolTip();
                tooltip.IsEnabled = true;
                tooltip.Content = HelpingClass.GetDescTool(p1);
                tooltip.PlacementTarget = pin;
                tooltip.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
                pin.ToolTip = tooltip;
                tooltip.IsOpen = true;
            }
        } // Show tooltip when mouse is over
        private void PinMouseLeave(object sender, MouseEventArgs e)
        {
            Pushpin pin = sender as Pushpin;
            ToolTip tooltip = pin.ToolTip as ToolTip;
            tooltip.IsOpen = false;
        } // tooltip is disapiring if mouse is not over
        private Pushpin SetPinns(Location pinLocation, bool creating = false)
        {
            Pushpin pin = new Pushpin();
            pin.Location = pinLocation;
            pin.Background = new SolidColorBrush("#FF7B4BA5".ToColor());
            if (!creating)
            {
                pin.MouseEnter += PinMouseEnter;
                pin.MouseLeave += PinMouseLeave;
                pin.MouseLeftButtonDown += RemovePinFromMap;
            }
            return pin;
        } // Setting options for pin
        private ScrollViewer scrollMode()
        {
            ScrollViewer scrollViewer = new ScrollViewer();
            int loops = customerMode == true ? 5 : 6;
            StackPanel customer = new StackPanel();

            for (int i = 0 ; i < places.Count; i++)
            {
                StackPanel informations = new StackPanel();
                for (int j = 0 ; j <  loops-1; j++)
                {
                    // dodawanie informacji
                }


            }

            return scrollViewer;
        } // Creating ScrollViewer for mode


        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (addingCustomer != null && addingCustomer.IsVisible)
                e.Cancel = true;
        } // blocking window 
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        } // close window
        private void BorderClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        } // feature to moving window
    }

    public static class StringExtensions
    {
        public static Color ToColor(this string color)
        {
            return (Color)ColorConverter.ConvertFromString(color);
        }
    }
}