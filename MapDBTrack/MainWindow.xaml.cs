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
        private Grid mapGrid;
        private Map map;
        private bool pinned = false;
        private bool removed = false;
        public static bool acceptOverridePin = false;
        

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
                Width = 150,
                Height = 60,
                Margin = new Thickness(0, 650, 0, 0),
                Background = new SolidColorBrush("#FF2C3C96".ToColor()),

            };
            buttonBorder.CornerRadius = new CornerRadius(20);
            Grid.SetColumn(buttonBorder, 2);
            Grid.SetRow(buttonBorder, 0);

            // creating grid for buttons
            Grid buttonsMap = new Grid();
            buttonsMap.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            buttonsMap.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1) });
            buttonsMap.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            // creating line beetwen two buttons
            Border border = new Border();
            border.Background = Brushes.Gray;
            Grid.SetColumn(border, 1);
            Grid.SetRow(border, 0);
            buttonsMap.Children.Add(border);

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
            Grid.SetColumn(removing, 2);
            Grid.SetRow(removing, 2);
            buttonsMap.Children.Add(removing);

            // creating grid for all objects and adding to main window
            buttonBorder.Child = buttonsMap;
            MainGrid.Children.Add(mapBorder);
            MainGrid.Children.Add(buttonBorder);

            LoadingPinns();

        } // loading map 
        private void LoadingCustomerScreen()
        {
            HelpingClass.NetworkCheck(this); // Checking network connection

            // creating menu grid on the right side
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
            menuBorderGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            menuBorderGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            menuBorder.Child = menuBorderGrid;

            #endregion Creating grid

            // creating menu with diffrent buttons and features
            #region Creating menu (sorting searching)

            // textblock searching
            TextBlock searching = new TextBlock()
            {
                Text = "Searching:",
                FontFamily = new FontFamily("Calibri"),
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

            // textblock sorting
            TextBlock sorting = new TextBlock()
            {
                Text = "Sorting:",
                FontFamily = new FontFamily("Calibri"),
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

            // special place when user can put text to search by
            TextBox sortingBox = new TextBox()
            {
                Name = "Sorting",
                FontFamily = new FontFamily("Calibri"),
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

            // combo box to choose witch category user can sort
            ComboBox comboName = new ComboBox()
            {
                Name = "ComboName",
                Style = FindResource("RoundedComboBox") as Style,
                Margin = new Thickness(41, 25, 40, 25)
            };
            comboName.Items.Add("nwm");
            comboName.Items.Add("nws");
            comboName.Items.Add("nw5");
            comboName.SelectionChanged += ComboBoxChanged;
            Grid.SetColumnSpan(comboName, 2);
            Grid.SetColumn(comboName, 1);
            Grid.SetRow(comboName, 1);


            // button to open new window to choose mode to report
            Button report = new Button()
            {
                Width = 180,
            };
            report.Style = FindResource("ButtonRounded") as Style;
            report.Click += RaportClick;
            Grid.SetColumn(report, 3);
            Grid.SetRow(report, 0);

            // button to clear the list ????????????????
            Button clear = new Button()
            {
                Width = 180,
            };
            clear.Style = FindResource("ButtonRounded") as Style;
            clear.Click += ClearClick;
            Grid.SetColumn(clear, 3);
            Grid.SetRow(clear, 1);

            // button to see all places
            Button mode = new Button()
            {
                Width = 180,
            };
            mode.Style = FindResource("ButtonRounded") as Style;
            mode.Click += ModeClick;
            Grid.SetColumn(mode, 4);
            Grid.SetRow(mode, 0);

            // test button
            Button test = new Button()
            {
                Width = 180,
            };
            test.Style = FindResource("ButtonRounded") as Style;
            test.Click += TestClick;
            Grid.SetColumn(test, 4);
            Grid.SetRow(test, 1);

            // content to report button
            TextBlock reportText = new TextBlock()
            {
                Text = "Report",
            };
            reportText.Style = FindResource("TextBlocksCustomerButtons") as Style;
            Grid.SetColumn(reportText, 3);
            Grid.SetRow(reportText, 0);

            // content to clear button
            TextBlock clearText = new TextBlock()
            {
                Text = "Clear",
            };
            clearText.Style = FindResource("TextBlocksCustomerButtons") as Style;
            Grid.SetColumn(clearText, 3);
            Grid.SetRow(clearText, 1);

            // content to mode button
            TextBlock modeText = new TextBlock()
            {
                Text = "Mode",
            };
            modeText.Style = FindResource("TextBlocksCustomerButtons") as Style;
            Grid.SetColumn(modeText, 4);
            Grid.SetRow(modeText, 0);

            // content to test button
            TextBlock testText = new TextBlock()
            {
                Text = "Test",
            };
            testText.Style = FindResource("TextBlocksCustomerButtons") as Style;
            Grid.SetColumn(testText, 4);
            Grid.SetRow(testText, 1);

            Image reportImage = new Image()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Pictures/report.png")),            
            };
            reportImage.Style = FindResource("ImageCustomerView") as Style;
            RenderOptions.SetBitmapScalingMode(reportImage, BitmapScalingMode.HighQuality);
            Grid.SetColumn(reportImage, 3);
            Grid.SetRow(reportImage, 0);

            Image clearImage = new Image()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Pictures/clear.png")),
            };
            clearImage.Style = FindResource("ImageCustomerView") as Style;
            RenderOptions.SetBitmapScalingMode(clearImage, BitmapScalingMode.HighQuality);
            Grid.SetColumn(clearImage, 3);
            Grid.SetRow(clearImage, 1);

            Image modeImage = new Image()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Pictures/mode.png")),
            };
            modeImage.Style = FindResource("ImageCustomerView") as Style;
            modeImage.Width = 38;
            RenderOptions.SetBitmapScalingMode(modeImage, BitmapScalingMode.HighQuality);
            Grid.SetColumn(modeImage, 4);
            Grid.SetRow(modeImage, 0);

            /*
            Image testImage = new Image()
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Pictures/test.png")),
            };
            testImage.Style = FindResource("ImageCustomerView") as Style;
            RenderOptions.SetBitmapScalingMode(testImage, BitmapScalingMode.HighQuality);
            Grid.SetColumn(testImage, 4);
            Grid.SetRow(testImage, 1);
            */
            #endregion

            //creating line 
            #region Creating line
            Border line = new Border();
            line.Background = new SolidColorBrush("#FF2C3C96".ToColor());
            Grid.SetColumn(line, 0);
            Grid.SetRow(line, 1);
            mainWindowBorderGrid.Children.Add(line);
            #endregion

            // setting theme category to scroll viewer
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


            for (int i = 0; i < 6; i++)
            {
                TextBlock theme = new TextBlock()
                {
                    Text = i == 0 ? "Added by" : i == 1 ? "Name" : i == 2 ? "Last name" : i == 3 ? "City" : i == 4 ? "Street" : "Number",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                Grid.SetColumn(theme, i);
                Grid.SetRow(theme, 0);
                themeGrid.Children.Add(theme);
            }
            mainWindowBorderGrid.Children.Add(themeBorder);
            #endregion

            // creating scroll view with customers
            #region Creating Sroll view
            StackPanel panel = null;
            StackPanel mainPanel = new StackPanel();
            ScrollViewer list = new ScrollViewer();

            for (int i = 0; i < places.Count; i++)
            {
                panel = new StackPanel();
                panel.Orientation = Orientation.Horizontal;
                TextBlock info = null;

                for (int j = 0; j < 6; j++)
                {
                    // creating content to panel
                    info = new TextBlock()
                    {
                        Text = j == 0 ? places[i].employee_id.ToString() : j == 1 ? places[i].first_name : j == 2 ? places[i].last_name : j == 3 ? places[i].city : j == 4 ? places[i].street : places[i].contact_number,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Height = 50,
                        Width = 150,
                    };
                    panel.Children.Add(info);
                }

                // button to open menu of customer
                Button menu = new Button()
                {
                    Content = "click",
                };
                menu.Click += MenuOfCustomerClick;

                panel.Children.Add(menu);
                mainPanel.Children.Add(panel);
            }
            list.Content = mainPanel;
            Grid.SetColumn(list, 0);
            Grid.SetRow(list, 3);
            mainWindowBorderGrid.Children.Add(list);

            #endregion

            // adding all elemnts to gird
            #region Adding elemnts to grid
            menuBorderGrid.Children.Add(searching);
            menuBorderGrid.Children.Add(sorting);
            menuBorderGrid.Children.Add(sortingBox);
            menuBorderGrid.Children.Add(comboName);
            menuBorderGrid.Children.Add(report);
            menuBorderGrid.Children.Add(clear);
            menuBorderGrid.Children.Add(reportText);
            menuBorderGrid.Children.Add(clearText);
            menuBorderGrid.Children.Add(mode);
            menuBorderGrid.Children.Add(modeText);
            menuBorderGrid.Children.Add(test);
            menuBorderGrid.Children.Add(testText);

            menuBorderGrid.Children.Add(reportImage);
            menuBorderGrid.Children.Add(modeImage);
            menuBorderGrid.Children.Add(clearImage);
            #endregion

        } // loading customer list
        private void MenuOfCustomerClick(object sender, EventArgs e)
        {

        }
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
        private void Information(object sender, RoutedEventArgs e)
        {
            string info = $"{HelpingClass.version}\nContact: kus.konrad1@gmail.com\nLicense: MapDBTrack Commercial Use License";
            MessageBox.Show(info, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        } // special MessageBox with version etc.
        private void AddPin(object sender, RoutedEventArgs e)
        {
            if(removed == false)
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
            if(pinned == false)
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
        }

        private void MapPuttingPins(object sender, MouseButtonEventArgs e)
        {
            HelpingClass.NetworkCheck(this);

            if (pinned)
            {
                Point mousePosition = e.GetPosition(mapGrid);
                Location pinLocation = map.ViewportPointToLocation(mousePosition);
                Pushpin pin = SetPinns(pinLocation,true);
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
                    MessageBoxResult result = MessageBox.Show($"Do you want remove this pin?\n\n{HelpingClass.GetDescTool(p1)}", "Inforamtion",MessageBoxButton.YesNo,MessageBoxImage.Warning);

                    if(result == MessageBoxResult.Yes)
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
        private Pushpin SetPinns(Location pinLocation, bool creating = false)
        {
            Pushpin pin = new Pushpin();
            pin.Location = pinLocation;
            pin.Background = Brushes.DarkBlue;
            if (!creating)
            {
                pin.MouseEnter += PinMouseEnter;
                pin.MouseLeave += PinMouseLeave;
                pin.MouseLeftButtonDown += RemovePinFromMap;
            }
            return pin;
        } // Setting options for pin
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
    }

    public static class StringExtensions
    {
        public static Color ToColor(this string color)
        {
            return (Color)ColorConverter.ConvertFromString(color);
        }
    }
}