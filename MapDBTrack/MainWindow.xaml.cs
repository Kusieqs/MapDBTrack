
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
        public static List<Place> newCustomers = new List<Place>();
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
        }

        private void MapClick(object sender, RoutedEventArgs e)
        {
            LoadingMapScreen();
        } // button method
        private void LoadingMapScreen()
        {
            HelpingClass.NetworkCheck(this);

            LoginName.Text = "Welcome " + loginOfEmployee;

            Border mapBorder = new Border();
            mapBorder.Background = new SolidColorBrush("#FFFFF7FC".ToColor()); 
            Grid.SetColumn(mapBorder, 2);
            Grid.SetRow(mapBorder, 0);

            mapGrid = new Grid();
            mapGrid.Name = "Content";
            map = new Map();
            map.CredentialsProvider = new ApplicationIdCredentialsProvider(HelpingClass.connectMap);
            map.Mode = new AerialMode(true);
            map.Center = new Location(52.2387, 19.0478);
            map.Culture = "en-US";
            map.ZoomLevel = 6.7;
            map.MouseLeftButtonDown += MapPuttingPins;

            mapGrid.Children.Add(map);
            mapBorder.Child = mapGrid;

            Border buttonBorder = new Border();
            buttonBorder.Width = 60;
            buttonBorder.Height = 60;
            buttonBorder.Margin = new Thickness(0, 650, 0, 0);
            buttonBorder.Background = new SolidColorBrush("#FF2C3C96".ToColor()); 
            buttonBorder.CornerRadius = new CornerRadius(100);
            Grid.SetColumn(buttonBorder, 2);
            Grid.SetRow(buttonBorder, 0);

            adding = new Button();
            adding.Style = FindResource("ButtonsAddPins") as Style; 
            adding.Click += AddPin;

            TextBlock plusText = new TextBlock();
            plusText.Text = "+";
            plusText.FontSize = 71;
            plusText.FontWeight = FontWeights.DemiBold;
            plusText.Foreground = Brushes.White;
            plusText.VerticalAlignment = VerticalAlignment.Top;
            plusText.HorizontalAlignment = HorizontalAlignment.Left;
            plusText.Margin = new Thickness(6, -26, 0, 0);
            plusText.Width = 42;
            plusText.IsHitTestVisible = false;

            Grid buttonGrid = new Grid();
            buttonGrid.Children.Add(adding);
            buttonGrid.Children.Add(plusText);
            buttonBorder.Child = buttonGrid;

            MainGrid.Children.Add(mapBorder);
            MainGrid.Children.Add(buttonBorder);

            LoadingPinns();

        } // loading map 
        private void CustomersClick(object sender, RoutedEventArgs e)
        {
            HelpingClass.CleanGrid(MainGrid);
            // Changing  mainGird
        } // button customer
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
