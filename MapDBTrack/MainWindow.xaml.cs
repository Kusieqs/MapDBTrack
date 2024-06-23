using Microsoft.Maps.MapControl.WPF;
using Microsoft.VisualBasic;
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
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

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

        #region customer grid and border
        private Grid customerGrid;
        private Button personalBtn;
        private Button addressBtn;
        private StackPanel theme;
        private StackPanel customer;
        private ScrollViewer scrollViewer;
        private Border customerBorder;
        #endregion customer grid and border

        public MainWindow(int id, string login)
        {
            InitializeComponent();
            idOfEmployee = id;
            loginOfEmployee = login;
            LoadingMapScreen();

            // Text on the top of menu buttons
            LoginName.Text = "Hi, " + loginOfEmployee;
        }


        #region Menu buttons
        private void MapClick(object sender, RoutedEventArgs e)
        {
            HelpingClass.CleanGrid(mapBorder);
            LoadingMapScreen();
        } // map button
        private void CustomersClick(object sender, RoutedEventArgs e)
        {
            HelpingClass.CleanGrid(mapBorder);
            LoadingCustomerScreen();
        } // Customer button
        private void HistoryClick(object sender, RoutedEventArgs e)
        {

        } // history button
        private void LogoutClick(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        } // logout button
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }  // exit button
        #endregion Menu buttons
        
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

            customerBorder = new Border()
            {
                CornerRadius = new CornerRadius(0, 15, 15, 0),
                Background = Brushes.White,
            };
            customerBorder.MouseDown += BorderClick;
            Grid.SetColumn(customerBorder, 1);

            //Creating grid

            customerGrid = new Grid()
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

            // Personal button
            personalBtn = new Button()
            {
                Content = "Personal",
                Style = FindResource("tabButton") as Style,
                Width = 78,
                Margin = new Thickness(0,0,20,0),
                FontFamily = new FontFamily("Calibri"),
                BorderBrush = Brushes.Transparent,
                Cursor = Cursors.Hand
            };
            personalBtn.Click += PersonalClick;

            // address button
            addressBtn = new Button()
            {
                Content = "Address",
                Style = FindResource("tabButton") as Style,
                Width = 78,
                FontFamily = new FontFamily("Calibri"),
                BorderBrush = Brushes.Transparent,
                Cursor = Cursors.Hand
            };
            addressBtn.Click += AddresClick;


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
                Margin = new Thickness(940, 8, 10, 17),
                Background = new SolidColorBrush("#FF7B4BA5".ToColor()),
                FontFamily = new FontFamily("Calibri"),
            };
            reportBtn.Click += ReportClick;

            Button deleteBtn = new Button()
            {
                Width = 110,
                Height = 45,
                Content = "Delete",
                FontSize = 20,
                Margin = new Thickness(798, 8, 152, 17),
                Background = new SolidColorBrush("#FF7B4BA5".ToColor()),
                FontFamily = new FontFamily("Calibri"),
            };
            deleteBtn.Click += DeleteClick;

            // Theme panel for scroll viewer
            theme = StackPanelMode();
            Grid.SetRow(theme, 2);

            // Creating scrollviewer
            scrollViewer = ScrollMode();
            Grid.SetRow(scrollViewer, 3);

            // creating search box
            TextBox searching = new TextBox()
            {
                Height = 30,
                Margin = new Thickness(16, 20, 800, 7),
                FontSize = 20,
                FontFamily = new FontFamily("Calibri"),
                Foreground = new SolidColorBrush("#FF7B4BA5".ToColor()),
                FontWeight = FontWeights.DemiBold,
                BorderBrush = new SolidColorBrush("#dae2ea".ToColor())
            };
            MaterialDesignThemes.Wpf.HintAssist.SetHint(searching, "Search");
            MaterialDesignThemes.Wpf.HintAssist.SetFloatingOffset(searching, new Point(0, -20));


            #region adding elements
            customerGrid.Children.Add(separator);
            stackPanel.Children.Add(personalBtn);
            stackPanel.Children.Add(addressBtn);
            customerGrid.Children.Add(stackPanel);
            customerGrid.Children.Add(deleteBtn);
            customerGrid.Children.Add(reportBtn);
            customerGrid.Children.Add(searching);

            customerGrid.Children.Add(theme);
            customerGrid.Children.Add(scrollViewer);

            customerBorder.Child = customerGrid;
            mapBorder.Children.Add(customerBorder);
            #endregion adding elements


        } // loading customer list
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
        private ScrollViewer ScrollMode()
        {
            ScrollViewer scrollViewer = new ScrollViewer()
            {

                Padding = new Thickness(10),
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            int loops = customerMode == true ? 5 : 4;
            customer = new StackPanel();

            for (int i = 0 ; i < places.Count; i++)
            {
                Border borderStackPanel = new Border()
                {
                    CornerRadius = new CornerRadius(10),
                    Background = Brushes.LightGray,
                    Height = 50,
                    Margin = new Thickness(0, 5, 3, 5)
                };

                StackPanel informations = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                };

                for (int j = 0 ; j < loops; j++)
                {
                    TextBlock info = new TextBlock()
                    {
                        FontSize = 17,
                        Width = 190,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        TextAlignment = TextAlignment.Center
                    };

                    if (!customerMode && (j == 1 || j == 3))
                        info.Width = 285;

                    info.Text = HelpingClass.DescritpionScrollView(customerMode, j, i);
                    informations.Children.Add(info);
                }

                Button menu = new Button()
                {
                    FontSize = 20,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(40, 0, 0, 0),
                    Background = new SolidColorBrush("#FF7B4BA5".ToColor())
                };


                informations.Children.Add(menu);
                borderStackPanel.Child = informations;
                customer.Children.Add(borderStackPanel);
            }
            scrollViewer.Content = customer;
            return scrollViewer;

        } // Creating ScrollViewer for mode
        private StackPanel StackPanelMode()
        {
            int loops = customerMode == true ? 5 : 4;
            StackPanel theme = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
            };

            for (int j = 0; j < loops; j++)
            {
                TextBlock info = new TextBlock()
                {
                    FontWeight = FontWeights.Bold,
                    FontSize = 17,
                    Width = 190,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Center,
                    Foreground = new SolidColorBrush("#FF7B4BA5".ToColor()),
                    FontFamily = new FontFamily("Calibri"),
                };

                if (!customerMode && (j == 1 || j == 3))
                    info.Width = 285;

                info.Text = HelpingClass.DescriptionStackPanel(customerMode, j);

                theme.Children.Add(info);
            }

            TextBlock but = new TextBlock()
            {
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                Text = "MENU",
                Margin = new Thickness(30, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                TextAlignment = TextAlignment.Center,
                Foreground = new SolidColorBrush("#FF7B4BA5".ToColor()),
                FontFamily = new FontFamily("Calibri"),
            };
            theme.Children.Add(but);

            return theme;
        } // Creating stack panel for Theme 
        private void AddresClick(object sender,  RoutedEventArgs e)
        {
            customerMode = false;

            personalBtn.BorderBrush = Brushes.Transparent;
            addressBtn.BorderBrush = new SolidColorBrush("#FF7B4BA5".ToColor());

            ChangingCustomerView();
        } // Address Click (customer view)
        private void PersonalClick(object sender, RoutedEventArgs e)
        {
            customerMode = true;

            personalBtn.BorderBrush = new SolidColorBrush("#FF7B4BA5".ToColor());
            addressBtn.BorderBrush = Brushes.Transparent;

            ChangingCustomerView();
        } // Personal Click (customer view)
        private void ChangingCustomerView()
        {
            var elementsToRemove = customerGrid.Children
                        .Cast<UIElement>()
                        .Where(e => Grid.GetRow(e) == 2 || Grid.GetRow(e) == 3)
                        .ToList();

            foreach (var element in elementsToRemove)
            {
                customerGrid.Children.Remove(element);
            }

            // Theme panel for scroll viewer
            theme = StackPanelMode();
            Grid.SetRow(theme, 2);
            // Creating scrollviewer
            scrollViewer = ScrollMode();
            Grid.SetRow(scrollViewer, 3);

            customerGrid.Children.Add(theme);
            customerGrid.Children.Add(scrollViewer);
        } // Switching mode between address and personal 
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (addingCustomer != null && addingCustomer.IsVisible)
                e.Cancel = true;
        } // blocking window 
        private void BorderClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        } // feature to moving window
        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            if (customer.Children.Count == 0)
                return;

            MessageBoxResult result = MessageBox.Show($"Are you sure to delete {customer.Children.Count}","Information",MessageBoxButton.YesNo,MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes )
            {
                for (int i = 0; i < customer.Children.Count; i++)
                {
                    Border border = customer.Children[i] as Border;
                    StackPanel panel = border.Child as StackPanel;
                    TextBlock id = panel.Children[0] as TextBlock;

                    if (places.Any(x => x.customer_id == id.Text))
                    {
                        Place place = places.Where(x => x.customer_id == id.Text).FirstOrDefault();
                        places.Remove(place);
                        HelpingClass.RemoveRecordFromDB(place);
                    }

                }
                LoadingCustomerScreen();
            }    
            
        }
        private void ReportClick(object sender, RoutedEventArgs e)
        {
            Button report = sender as Button;
            Report reportWindow = new Report();
            menuButtons.IsEnabled = false;
            report.IsEnabled = false;

            reportWindow.Closed += (s, args) =>
            {
                report.IsEnabled = true;
                menuButtons.IsEnabled = true;
            };
            reportWindow.Show();
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