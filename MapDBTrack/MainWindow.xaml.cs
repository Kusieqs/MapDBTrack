
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
        private AddingCustomer addingCustomer;
        private Grid mapGrid;
        private Map map;
        private bool pinned = false;

        public MainWindow(int id, string login)
        {
            InitializeComponent();
            idOfEmployee = id;
            loginOfEmployee = login;
            LoadingMapScreen();

            // Text on the top of menu buttons
            LoginName.Text = "Welcome " + loginOfEmployee; 
        }

        private void MapClick(object sender, RoutedEventArgs e)
        {
            LoadingMapScreen();
        } // button to load map
        private void LoadingMapScreen()
        {
            // Checking network connection
            HelpingClass.NetworkCheck(this); 

            // creating grid for map
            mapGrid = new Grid();
            mapGrid.Name = "Content";

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

            // Putting border on grid
            mapGrid.Children.Add(map);

            // creating border for map grid
            Border mapBorder = new Border();
            mapBorder.Background = new SolidColorBrush("#FFFFF7FC".ToColor());
            Grid.SetColumn(mapBorder, 2);
            Grid.SetRow(mapBorder, 0);
            mapBorder.Child = mapGrid;

            // creating border for button
            Border buttonBorder = new Border()
            {
                Width = 60,
                Height = 60,
                Margin = new Thickness(0, 650, 0, 0),
                Background = new SolidColorBrush("#FF2C3C96".ToColor()),

            };
            buttonBorder.CornerRadius = new CornerRadius(100);
            Grid.SetColumn(buttonBorder, 2);
            Grid.SetRow(buttonBorder, 0);

            // creating button to add pin
            adding = new Button();
            adding.Style = FindResource("ButtonsAddPins") as Style; 
            adding.Click += AddPin;

            // Setting textblock on button
            TextBlock plusText = new TextBlock()
            {
                Text = "+",
                FontSize = 71,
                FontWeight = FontWeights.DemiBold,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(6,-26,0,0),
                Width = 42,
                IsHitTestVisible = false,
            };

            // creating grid for all objects and adding to main window
            Grid buttonGrid = new Grid();
            buttonGrid.Children.Add(adding);
            buttonGrid.Children.Add(plusText);
            buttonBorder.Child = buttonGrid;

            MainGrid.Children.Add(mapBorder);
            MainGrid.Children.Add(buttonBorder);

            LoadingPinns();

        } // loading map 
        private void LoadingCustomerScreen()
        {
            HelpingClass.NetworkCheck(this); // Checking network connection

            #region Creating grid
            Border mainWindowBorder = new Border();
            Grid.SetColumn(mainWindowBorder, 2);
            Grid.SetRow(mainWindowBorder, 0);
            MainGrid.Children.Add(mainWindowBorder);
            
            Grid mainWindowBorderGrid = new Grid();
            mainWindowBorderGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(180) });
            mainWindowBorderGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(2) });
            mainWindowBorderGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60) });
            mainWindowBorderGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            mainWindowBorder.Child = mainWindowBorderGrid;

            Border menuBorder = new Border();
            Grid.SetColumn(menuBorder, 0);
            Grid.SetRow(menuBorder, 0);
            mainWindowBorderGrid.Children.Add(menuBorder);


            Grid menuBorderGrid = new Grid();
            menuBorderGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            menuBorderGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            menuBorderGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(202) });
            menuBorderGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(207) });
            menuBorderGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(190) });
            menuBorderGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(90) });
            menuBorderGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            menuBorderGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            menuBorder.Child = menuBorderGrid;

            #endregion Creating grid

            #region Creaing menu (sroting searching)
            TextBlock searching = new TextBlock()
            {
                Text = "Searching:",
                FontWeight = FontWeights.Bold,
                FontSize = 30,
                Foreground = new SolidColorBrush("#FF2F5588".ToColor()),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Height = 40,
                Width = 147
            };
            Grid.SetColumn(searching, 0);
            Grid.SetRow(searching, 0);

            TextBlock sorting  = new TextBlock()
            {
                Text = "Sorting:",
                FontWeight = FontWeights.Bold,
                FontSize = 30,
                Foreground = new SolidColorBrush("#FF2F5588".ToColor()),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Height = 40,
                Width = 147
            };
            Grid.SetColumn(sorting, 0);
            Grid.SetRow(sorting, 1);

            Button removeSearching = new Button()
            {
                Style = FindResource("ButtonRoundedRemove") as Style,
                Margin = new Thickness(26,25,24,25),
            };
            removeSearching.Click += RemoveSearching;
            Grid.SetColumn(removeSearching, 3);
            Grid.SetRow(removeSearching, 0);

            TextBlock x = new TextBlock()
            {
                Text = "x",
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 30,
                FontWeight = FontWeights.Bold,
                Height = 41,
                Width = 16,
                IsHitTestVisible = false,
                Margin = new Thickness(0,21,0,0)
            };
            Grid.SetColumn(x, 3);
            Grid.SetRow(x, 0);

            Button removeSorting = new Button()
            {
                Style = FindResource("ButtonRoundedRemove") as Style,
                Margin = new Thickness(26, 25, 24, 25),
            };
            removeSorting.Click += RemoveSorting;
            Grid.SetColumn(removeSorting, 3);
            Grid.SetRow(removeSorting, 1);

            TextBlock x1 = new TextBlock()
            {
                Text = "x",
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 30,
                FontWeight = FontWeights.Bold,
                Height = 41,
                Width = 16,
                IsHitTestVisible = false,
                Margin = new Thickness(0, 21, 0, 0)
            };
            Grid.SetColumn(x1, 3);
            Grid.SetRow(x1, 1);

            TextBox sortingBox = new TextBox()
            {
                Name = "Sorting",
                Style = FindResource("RoundedTextBox") as Style,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Height = 40,
                Width = 318
            };
            sortingBox.TextChanged += SortingChanged;
            Grid.SetColumnSpan(sortingBox, 2);
            Grid.SetColumn(sortingBox, 1);
            Grid.SetRow(sortingBox, 0);

            ComboBox comboName = new ComboBox()
            {
                Name = "ComboName",
                Style = FindResource("RoundedComboBox") as Style,
                Margin = new Thickness(41,25,40,25)
            };
            comboName.SelectionChanged += ComboBoxChanged;
            Grid.SetColumnSpan(comboName, 2);
            Grid.SetColumn(comboName, 1);
            Grid.SetRow(comboName, 1);

            Button raport = new Button();
            raport.Style = FindResource("ButtonRounded") as Style;
            raport.Click += RaportClick;
            Grid.SetColumn(raport, 4);
            Grid.SetRow(raport, 0);

            Button clear = new Button();
            clear.Style = FindResource("ButtonRounded") as Style;
            clear.Click += ClearClick;
            Grid.SetColumn(clear, 4);
            Grid.SetRow(clear, 1);

            Button mode = new Button();
            mode.Style = FindResource("ButtonRounded") as Style;
            mode.Click += ModeClick;
            Grid.SetColumn(mode, 5);
            Grid.SetRow(mode, 0);

            Button test = new Button();
            test.Style = FindResource("ButtonRounded") as Style;
            test.Click += TestClick;
            Grid.SetColumn(test, 5);
            Grid.SetRow(test, 1);

            TextBlock raportText = new TextBlock()
            {
                Text = "Raport",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = Brushes.White,
                FontSize = 30,
                IsHitTestVisible = false,
                FontWeight = FontWeights.DemiBold
            };
            Grid.SetColumn(raportText, 4);
            Grid.SetRow(raportText, 0);

            TextBlock clearText = new TextBlock()
            {
                Text = "Clear",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = Brushes.White,
                FontSize = 30,
                IsHitTestVisible = false,
                FontWeight = FontWeights.DemiBold
            };
            Grid.SetColumn(clearText, 4);
            Grid.SetRow(clearText, 1);

            TextBlock modeText = new TextBlock()
            {
                Text = "Mode",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = Brushes.White,
                FontSize = 30,
                IsHitTestVisible = false,
                FontWeight = FontWeights.DemiBold
            };
            Grid.SetColumn(modeText, 5);
            Grid.SetRow(modeText, 0);

            TextBlock testText = new TextBlock()
            {
                Text = "Test",
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = Brushes.White,
                FontSize = 30,
                IsHitTestVisible = false,
                FontWeight = FontWeights.DemiBold
            };
            Grid.SetColumn(testText, 5);
            Grid.SetRow(testText, 1);
            #endregion

            #region Creating line
            Border line = new Border();
            line.Background = new SolidColorBrush("#FF2C3C96".ToColor());
            Grid.SetColumn(line, 0);
            Grid.SetRow(line, 1);
            mainWindowBorderGrid.Children.Add(line);
            #endregion

            #region Theme of category
            Border themeBorder = new Border();
            Grid.SetRow(themeBorder, 2);
            Grid.SetColumn(themeBorder, 0);

            Grid themeGrid = new Grid();
            themeGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            themeGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            themeGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            themeGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            themeGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            themeGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            themeBorder.Child = themeGrid;


            for(int i = 0; i < 6; i++)
            {
                TextBlock theme = new TextBlock()
                {
                    Text =  i==0 ? "Added by" : i == 1 ? "Name" : i == 2 ? "Last name" : i == 3 ? "City" : i == 4 ? "Street" : "Number",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                Grid.SetColumn(theme, i);
                Grid.SetRow(theme, 0);
                themeGrid.Children.Add(theme);
            }
            mainWindowBorderGrid.Children.Add(themeBorder);
            #endregion

            menuBorderGrid.Children.Add(searching);
            menuBorderGrid.Children.Add(sorting);
            menuBorderGrid.Children.Add(removeSearching);
            menuBorderGrid.Children.Add(removeSorting);
            menuBorderGrid.Children.Add(x);
            menuBorderGrid.Children.Add(x1);
            menuBorderGrid.Children.Add(sortingBox);
            menuBorderGrid.Children.Add(comboName);
            menuBorderGrid.Children.Add(raport);
            menuBorderGrid.Children.Add(clear);
            menuBorderGrid.Children.Add(raportText);
            menuBorderGrid.Children.Add(clearText);
            menuBorderGrid.Children.Add(mode);
            menuBorderGrid.Children.Add(modeText);
            menuBorderGrid.Children.Add(test);
            menuBorderGrid.Children.Add(testText);

        } // loading customer list
        private void TestClick(object sender, EventArgs e)
        {

        }
        private void ModeClick(object sender, EventArgs e)
        {

        }
        private void ComboBoxChanged(object sender, EventArgs e)
        {

        }
        private void RaportClick(object sender, EventArgs e)
        {

        }
        private void ClearClick(object sender, EventArgs e)
        {

        }
        private void SortingChanged(object sender, EventArgs e)
        {

        }
        private void RemoveSearching(object sender, EventArgs e)
        {

        }
        private void RemoveSorting(object sender, EventArgs e)
        {

        }
        private void CustomersClick(object sender, RoutedEventArgs e)
        {
            HelpingClass.CleanGrid(MainGrid);
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
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        } // close window
        private void Information(object sender, RoutedEventArgs e)
        {
            string info = "Version: 1.0\nContact: kus.konrad1@gmail.com\nLicense: MapDBTrack Commercial Use License";
            MessageBox.Show(info, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        } // special MessageBox with version etc.
        private void AddPin(object sender, RoutedEventArgs e)
        {
            if(pinned == true)
            {
                pinned = false;
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else 
            {
                pinned = true;
                Mouse.OverrideCursor = Cursors.Hand;
            }
        } // set true for added new pin to map or false
        private void MapPuttingPins(object sender, MouseButtonEventArgs e)
        {
            HelpingClass.NetworkCheck(this);

            if (pinned)
            {
                Point mousePosition = e.GetPosition(mapGrid);
                Location pinLocation = map.ViewportPointToLocation(mousePosition);


                Pushpin pin = SetPinns(pinLocation);
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
                };
            }
        } // puting pins on map when pinned is true
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
                pin.ToolTip = tooltip; 
                tooltip.IsOpen = true;
            }
        }

        private void PinMouseLeave(object sender, MouseEventArgs e)
        {
            Pushpin pin = sender as Pushpin;
            ToolTip tooltip = pin.ToolTip as ToolTip;
            tooltip.IsOpen = false;
        }


        private Pushpin SetPinns(Location pinLocation)
        {
            Pushpin pin = new Pushpin();
            pin.Location = pinLocation;
            pin.Background = Brushes.DarkBlue;
            pin.MouseEnter += PinMouseEnter;
            pin.MouseLeave += PinMouseLeave;
            return pin;
        } // Setting options for pin

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if(addingCustomer != null && addingCustomer.IsVisible)
                e.Cancel = true;
        } // blocking window 

    }
    public static class StringExtensions
    {
        public static Color ToColor(this string color)
        {
            return (Color)ColorConverter.ConvertFromString(color);
        }
    }
}
