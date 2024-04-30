
using Microsoft.Maps.MapControl.WPF;
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

namespace MapDBTrack
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Grid mapGrid;
        private Map map;
        private bool pinned = false;
        public MainWindow()
        {
            InitializeComponent();
            CreateMap();

        }
        private void CreateMap()
        {
            Border mapBorder = new Border();
            mapBorder.Background = new SolidColorBrush("#FFFFF7FC".ToColor()); 
            Grid.SetColumn(mapBorder, 2);
            Grid.SetRow(mapBorder, 0);

            mapGrid = new Grid();
            mapGrid.Name = "Content";

            map = new Map();
            map.CredentialsProvider = new ApplicationIdCredentialsProvider("Omi6chwps4qZse8uKKBz~1UQ7_Si2p2ODSsRCrCKxYw~AtFhG_SEo0Y6l_NRkUEVAf435kpg50sefPGfqdzhncRljdpipYSsjHci_BtcWM73");
            // Ustawienie trybu mapy na AerialMode
            map.Mode = new AerialMode(true);
            map.Center = new Location(52.2387, 19.0478);
            map.ZoomLevel = 6.7;
            map.MouseLeftButtonDown += MapPutting;

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

            Grid buttonGrid = new Grid();

            Button addButton = new Button();
            addButton.Style = FindResource("ButtonsAddPins") as Style; 
            addButton.Click += AddPin;

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

            buttonGrid.Children.Add(addButton);
            buttonGrid.Children.Add(plusText);
            buttonBorder.Child = buttonGrid;

            MainGrid.Children.Add(mapBorder);
            MainGrid.Children.Add(buttonBorder);

        }
        private void Information(object sender, RoutedEventArgs e)
        {
            // information about app and version
        }

        private void AddPin(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
            pinned = true;
        } // set true for added new pin to map
        private void MapPutting(object sender, MouseButtonEventArgs e)
        {
            if (pinned)
            {
                // Pobierz pozycję kliknięcia na mapie
                Point mousePosition = e.GetPosition(mapGrid);
                Location pinLocation = map.ViewportPointToLocation(mousePosition);

                // Stwórz nową pinezkę na mapie w miejscu kliknięcia
                Pushpin pin = new Pushpin();
                pin.Location = pinLocation;
                pin.Background = Brushes.Red; // Dostosuj kolor pinezki według potrzeb
                map.Children.Add(pin);

                // Zakończ dodawanie pinezki i przywróć standardowy kursor
                pinned = false;
                Mouse.OverrideCursor = Cursors.Arrow;

            }
        }
    }
    public static class StringExtensions
    {
        public static Color ToColor(this string color)
        {
            return (Color)ColorConverter.ConvertFromString(color);
        }
    }
}
