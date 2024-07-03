using Microsoft.Maps.MapControl.WPF;
using System.ComponentModel;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MapDBTrack
{
    public partial class MainWindow : Window
    {
        // Employee informations
        public static int idOfEmployee { get; set; } // Id of employee (number)
        public static string loginOfEmployee { get; set; } // Name of employee
        public static List<Place> listOfData { get; set; } = new List<Place>();// List of customers/listOfData

        // Map screen
        private Button adding; // Button to adding new customer
        private Button removing; // Button to removing customer
        private AddingCustomer addingCustomer; // Window to add customer
        private Map map; // Map
        private bool pinned = false; // It depends on clicked feature on map (add or remove pin)
        private bool removed = false; // It depends on clicked feature on map (add or remove pin)
        public static bool acceptOverridePin { get; set; } = false;// If it is true than pin will be put on map

        // Customer screen
        private bool customerMode = true; // It depends on which mode is active ( Personal or address)
        private Grid customerGrid; // Customer Gird
        private Button personalBtn; // Button to changed mode of list
        private Button addressBtn; // Button to changed mode of list
        private StackPanel theme; // Theme of column
        public static StackPanel customer { get; set; } // Customer panel 
        private ScrollViewer scrollViewer; // Scrollview for all customers
        private Border customerBorder; // Border for customers
        private TextBox search; // Textbox to searching in list
        private ContextMenu contextMenu; // Special contextmenu for each customer
        private int menuId; // Id of contextmenu

        public MainWindow(int id, string login)
        {
            loginOfEmployee = login;
            idOfEmployee = id;
            InitializeComponent();
            LoadingMapScreen();

            // Text on the top of menu buttons
            LoginName.Text = "Hi, " + loginOfEmployee;
        }


        #region Menu buttons
        private void MapClick(object sender, RoutedEventArgs e)
        {
            // cleaning grid
            HelpingClass.CleanGrid(mapBorder);

            // loading map screen
            LoadingMapScreen();
        } // Map button
        private void CustomersClick(object sender, RoutedEventArgs e)
        {
            // cleaning grid
            HelpingClass.CleanGrid(mapBorder);

            // Loading list of customers with diffrents features
            LoadingCustomerScreen();
        } // Customer button
        private void LogoutClick(object sender, RoutedEventArgs e)
        {
            // Opening new window to login
            Login login = new Login();
            login.Show();
            this.Close();
        } // Logout button
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }  // Exit button
        private void Information(object sender, RoutedEventArgs e)
        {
            // MessageBox with informations
            HelpingClass.InformationsAboutProgram();
        } // special button on the top of window with version etc.

        // Loading Map or Customer screen
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

            // Special figure
            PathGeometry clipGeometry = new PathGeometry();
            PathFigure pathFigure = new PathFigure
            {
                StartPoint = new Point(0, 0)
            };

            // Adding segmentation for figure
            pathFigure.Segments.Add(new LineSegment(new Point(1085, 0), true)); 
            pathFigure.Segments.Add(new ArcSegment(new Point(1100, 15), new Size(15, 15), 0, false, SweepDirection.Clockwise, true)); 
            pathFigure.Segments.Add(new LineSegment(new Point(1100, 785), true)); 
            pathFigure.Segments.Add(new ArcSegment(new Point(1085, 800), new Size(15, 15), 0, false, SweepDirection.Clockwise, true)); 
            pathFigure.Segments.Add(new LineSegment(new Point(0, 800), true)); 
            pathFigure.Segments.Add(new LineSegment(new Point(0, 0), true)); 
            clipGeometry.Figures.Add(pathFigure);
            map.Clip = clipGeometry;

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

            // Loading pins 
            LoadingPinns();

        } // Loading map 
        private void LoadingCustomerScreen()
        {
            // Checking network connection
            HelpingClass.NetworkCheck(this);

            // Creating border
            customerBorder = new Border()
            {
                CornerRadius = new CornerRadius(0, 15, 15, 0),
                Background = Brushes.White,
            };
            customerBorder.MouseDown += BorderClick;
            Grid.SetColumn(customerBorder, 1);

            // Creating grid
            customerGrid = new Grid()
            {
                Margin = new Thickness(20)
            };
            customerGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(70, GridUnitType.Pixel) });
            customerGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Pixel) });
            customerGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(50, GridUnitType.Pixel) });
            customerGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });

            // Creating line
            Separator separator = new Separator()
            {
                Background = new SolidColorBrush("#dae2ea".ToColor()),
                Margin = new Thickness(370, 55, 400, 14)
            };
            customerGrid.Children.Add(separator);

            // Creating stackpanel with buttons
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
                Margin = new Thickness(0, 0, 20, 0),
                FontFamily = new FontFamily("Calibri"),
                BorderBrush = Brushes.Transparent,
            };
            personalBtn.Click += PersonalClick;
            stackPanel.Children.Add(personalBtn);

            // Address button
            addressBtn = new Button()
            {
                Content = "Address",
                Style = FindResource("tabButton") as Style,
                Width = 78,
                FontFamily = new FontFamily("Calibri"),
                BorderBrush = Brushes.Transparent,
            };
            addressBtn.Click += AddressClick;
            stackPanel.Children.Add(addressBtn);

            // Adding stackpanel to main grid
            customerGrid.Children.Add(stackPanel);

            // Changing borderbrush color
            if (customerMode)
                personalBtn.BorderBrush = new SolidColorBrush("#FF7B4BA5".ToColor());
            else
                addressBtn.BorderBrush = new SolidColorBrush("#FF7B4BA5".ToColor());

            // Creating report button
            Button reportBtn = new Button()
            {
                Width = 110,
                Height = 45,
                Content = "Report",
                FontSize = 20,
                Margin = new Thickness(940, 8, 10, 17),
                Style = FindResource("ButtonSign") as Style
            };
            reportBtn.Click += ReportClick;
            customerGrid.Children.Add(reportBtn);

            // Creating delete button
            Button deleteBtn = new Button()
            {
                Width = 110,
                Height = 45,
                Content = "Delete",
                FontSize = 20,
                Margin = new Thickness(798, 8, 152, 17),
                Style = FindResource("ButtonSign") as Style
            };
            deleteBtn.Click += DeleteClick;
            customerGrid.Children.Add(deleteBtn);

            // Theme panel for scroll viewer
            theme = StackPanelMode();
            Grid.SetRow(theme, 2);
            customerGrid.Children.Add(theme);

            // Creating scrollviewer
            scrollViewer = ScrollMode(listOfData);
            Grid.SetRow(scrollViewer, 3);
            customerGrid.Children.Add(scrollViewer);

            // Creating search box
            search = new TextBox()
            {
                Height = 35,
                Margin = new Thickness(16, 14, 800, 7),
                FontSize = 20,
                FontFamily = new FontFamily("Calibri"),
                Foreground = new SolidColorBrush("#FF7B4BA5".ToColor()),
                FontWeight = FontWeights.DemiBold,
                BorderBrush = new SolidColorBrush("#dae2ea".ToColor())
            };
            search.TextChanged += SearchingScroll;
            MaterialDesignThemes.Wpf.HintAssist.SetHint(search, "Search");
            customerGrid.Children.Add(search);

            // Adding to main window
            customerBorder.Child = customerGrid;
            mapBorder.Children.Add(customerBorder);
        } // Loading customer list
        #endregion Menu buttons


        // Features to Map button
        private void MapPuttingPins(object sender, MouseButtonEventArgs e)
        {
            // Checking network connection
            HelpingClass.NetworkCheck(this);

            // if pinned is true than you can put pin on map 
            if (pinned)
            {
                // Adding Pushpin on map
                Point mousePosition = e.GetPosition(mapBorder);
                Location pinLocation = map.ViewportPointToLocation(mousePosition);
                Pushpin pin = SetPinns(pinLocation, true);
                map.Children.Add(pin);
                pinned = false;
                Mouse.OverrideCursor = Cursors.Arrow;

                // Creating new window and showing
                addingCustomer = new AddingCustomer(pinLocation, this, pin, map);
                addingCustomer.Show();
                menuButtons.IsEnabled = false;

                // When window will be closed than 
                addingCustomer.Closed += (s, args) =>
                {
                    menuButtons.IsEnabled = true;
                    if (acceptOverridePin)
                    {
                        map.Children.Remove(pin);

                        // Setting features to pin
                        pin.MouseEnter += PinMouseEnter;
                        pin.MouseLeave += PinMouseLeave;
                        pin.MouseLeftButtonDown += RemovePinFromMap;

                        map.Children.Add(pin);
                    }
                    acceptOverridePin = false;
                };
                removing.IsEnabled = true;
            }

        } // Puting pins on map when pinned is true
        private void AddPin(object sender, RoutedEventArgs e)
        {
            if (removed == false)
            {
                if (pinned == true)
                {
                    menuButtons.IsEnabled = true;
                    removing.IsEnabled = true;
                    pinned = false;
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
                else
                {
                    menuButtons.IsEnabled = false;
                    removing.IsEnabled = false;
                    pinned = true;
                    Mouse.OverrideCursor = Cursors.Hand;
                }
            }
        } // Seting true for add new pin to map 
        private void RemovePin(object sender, RoutedEventArgs e)
        {
            if (pinned == false)
            {
                if (removed == true)
                {
                    menuButtons.IsEnabled = true;
                    adding.IsEnabled = true;
                    removed = false;
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
                else
                {
                    menuButtons.IsEnabled = false;
                    adding.IsEnabled = false;
                    removed = true;
                    Mouse.OverrideCursor = Cursors.Hand;
                }
            }
        } // Seting true for remove pin from map
        private void LoadingPinns()
        {
            // Setting places by id of employee
            listOfData = HelpingClass.LoadingPlace(idOfEmployee);

            // Adding pins to map by latitude and logitude
            foreach (Place p in listOfData)
            {
                Location pinLocation = new Location(p.latitude, p.longitude);
                Pushpin pin = SetPinns(pinLocation);
                map.Children.Add(pin);
            }

        } // Loading all pins when map is close
        private Pushpin SetPinns(Location pinLocation, bool creating = false)
        {
            // Setting location for pin
            Pushpin pin = new Pushpin();
            pin.Location = pinLocation;
            pin.Background = new SolidColorBrush("#FF7B4BA5".ToColor());
            if (!creating)
            {
                // Adding features to pin
                pin.MouseEnter += PinMouseEnter;
                pin.MouseLeave += PinMouseLeave;
                pin.MouseLeftButtonDown += RemovePinFromMap;
            }
            return pin;
        } // Setting options for pin
        private void PinMouseEnter(object sender, MouseEventArgs e)
        {
            // Setting focus on a pin when mouse is over
            Pushpin pin = sender as Pushpin;
            double lat = pin.Location.Latitude;
            double lng = pin.Location.Longitude;
            Place customerToolTip = listOfData.FirstOrDefault(x => x.latitude == lat && x.longitude == lng);

            if (customerToolTip != null)
            {
                // Creating tooltip for pin
                ToolTip tooltip = new ToolTip()
                {
                    IsEnabled = true,
                    Content = HelpingClass.GetDescTool(customerToolTip),
                    PlacementTarget = pin,
                    Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse,
                    IsOpen = true
                };
                pin.ToolTip = tooltip;
            }
        } // Show tooltip when mouse is over
        private void PinMouseLeave(object sender, MouseEventArgs e)
        {
            Pushpin pin = sender as Pushpin;
            ToolTip tooltip = pin.ToolTip as ToolTip;
            tooltip.IsOpen = false;
        } // Tooltip will disapire if mouse will not be on pin
        private void RemovePinFromMap(object sender, MouseButtonEventArgs e)
        {
            // Checking network connection
            HelpingClass.NetworkCheck(this);

            // If removed is true than pin can be deleted from map
            if (removed)
            {
                Pushpin pushpin = sender as Pushpin;
                if (pushpin != null)
                {
                    // setting object when pin was clicked
                    e.Handled = true;
                    Place customerPin = listOfData.Where(x => x.latitude == pushpin.Location.Latitude && x.longitude == pushpin.Location.Longitude).FirstOrDefault();
                    MessageBoxResult result = MessageBox.Show($"Do you want remove this pin?\n\n{HelpingClass.GetDescTool(customerPin)}", "Inforamtion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    // If result of messagebox is "Yes" then customer will be deleted from DB and map
                    if (result == MessageBoxResult.Yes)
                    {
                        map.Children.Remove(pushpin);
                        HelpingClass.RemoveRecordFromDB(customerPin);
                        listOfData.Remove(customerPin);
                    }
                    adding.IsEnabled = true;
                    removed = false;
                    Mouse.OverrideCursor = Cursors.Arrow;
                }
            }
        } // Removing pin from map and DB


        // Features to Customer button
        private StackPanel StackPanelMode()
        {
            // Setting how many loop it will be depends on which mode is active
            int loops = customerMode == true ? 5 : 4;

            // Creating StackPanel for theme
            StackPanel theme = new StackPanel();
            theme.Orientation = Orientation.Horizontal;

            // Setting in which column special button to sort alphabetical
            for (int j = 0; j < loops; j++)
            {
                Button sortingBy = new Button()
                {
                    FontWeight = FontWeights.Bold,
                    FontSize = 17,
                    Width = 190,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = new SolidColorBrush("#FF7B4BA5".ToColor()),
                    Background = Brushes.Transparent,
                    FontFamily = new FontFamily("Calibri"),
                    BorderBrush = Brushes.Transparent,
                    Cursor = Cursors.Hand,
                };
                sortingBy.Click += SortByClick;

                if (!customerMode && (j == 1 || j == 3))
                    sortingBy.Width = 285;

                sortingBy.Content = HelpingClass.DescriptionStackPanel(customerMode, j);
                theme.Children.Add(sortingBy);
            }

            // Creating menu theme block
            TextBlock menu = new TextBlock()
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
            theme.Children.Add(menu);

            return theme;
        } // Creating stack panel for Theme 
        private ScrollViewer ScrollMode(List<Place> listOfCustomers)
        {
            // creating scroll viewer
            ScrollViewer scrollViewer = new ScrollViewer()
            {
                Padding = new Thickness(10),
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            // Setting how many loops (it depends on customerMode)
            int loops = customerMode == true ? 5 : 4;

            // Creating StackPanel
            customer = new StackPanel();

            for (int i = 0 ; i < listOfCustomers.Count; i++)
            {
                // creating border for each StackPanel
                Border borderStackPanel = new Border()
                {
                    CornerRadius = new CornerRadius(10),
                    Background = Brushes.LightGray,
                    Height = 50,
                    Margin = new Thickness(0, 5, 3, 5)
                };

                StackPanel informations = new StackPanel();
                informations.Orientation = Orientation.Horizontal;

                // Loop for each information depends on theme
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

                    info.Text = HelpingClass.DescritpionScrollView(listOfCustomers,customerMode, j, i);
                    informations.Children.Add(info);
                }

                // adding contextmenu for each customer
                contextMenu = new ContextMenu();
                for(int j = 0; j < 2; j ++)
                {
                    MenuItem menuItem = new MenuItem()
                    {
                        Header = j == 0 ? "Edit customer" : "Delete customer",
                    };
                    menuItem.Click += ContextMenu;
                    contextMenu.Items.Add(menuItem);
                }

                // Button for contextmenu
                Button menu = new Button()
                {
                    FontSize = 20,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(40, 0, 0, 0),
                    Background = new SolidColorBrush("#FF7B4BA5".ToColor()),
                    ContextMenu = contextMenu,
                    Name = "id" + i.ToString(),
                };
                menu.Click += OpenContextMenu;
                informations.Children.Add(menu);

                borderStackPanel.Child = informations;
                customer.Children.Add(borderStackPanel);
            }
            scrollViewer.Content = customer;
            return scrollViewer;

        } // Creating ScrollViewer for mode
        private void ChangingCustomerView(List<Place> customers)
        {
            // Selecting elements to remove
            var elementsToRemove = customerGrid.Children
                        .Cast<UIElement>()
                        .Where(e => Grid.GetRow(e) == 2 || Grid.GetRow(e) == 3)
                        .ToList();

            // Removing elements from grid
            foreach (var element in elementsToRemove)
            {
                customerGrid.Children.Remove(element);
            }

            // Theme panel for scroll viewer
            theme = StackPanelMode();
            Grid.SetRow(theme, 2);

            // Creating scrollviewer
            scrollViewer = ScrollMode(customers);
            Grid.SetRow(scrollViewer, 3);

            // Adding to customer screen
            customerGrid.Children.Add(theme);
            customerGrid.Children.Add(scrollViewer);
        } // Switching mode between address and personal
        private void ContextMenu(object sender, RoutedEventArgs e)
        {
            // Setting string (which options was choosen)
            string option = (sender as MenuItem).Header.ToString();
            Place place = listOfData[menuId];

            switch (option)
            {
                // Editing customer
                case "Edit customer":
                    AddingCustomer editCustomer = new AddingCustomer(place, true);
                    editCustomer.Show();
                    editCustomer.Closed += (sender, e) => SearchingScroll(null, null);
                    break;

                // Deleting customer
                case "Delete customer":
                    listOfData.RemoveAt(menuId);
                    HelpingClass.RemoveRecordFromDB(place);
                    break;
            }
            SearchingScroll(null, null);
        }  // Options for ContextMenu
        private void SearchingScroll(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(search.Text))
                ChangingCustomerView(listOfData);
            else
            {
                // Setting that word which is the most correct with searching
                var filter = listOfData.Where(x =>
                x.customer_id.Contains(search.Text) ||
                x.first_name.Contains(search.Text) ||
                x.last_name.Contains(search.Text) ||
                x.email.Contains(search.Text) ||
                x.contact_number.Contains(search.Text) ||
                x.city.Contains(search.Text) ||
                x.province.Contains(search.Text) ||
                x.street.Contains(search.Text) ||
                x.postal_code.Contains(search.Text) ||
                x.employee_id.ToString().Contains(search.Text)).ToList();

                ChangingCustomerView(filter);
            }
        } // Searching by special Box
        private void OpenContextMenu(object sender, RoutedEventArgs e)
        {
            menuId = int.Parse((sender as Button).Name.Split('d')[1]);
            contextMenu.IsOpen = true;
        } // opening context menu


        private void AddressClick(object sender,  RoutedEventArgs e)
        {
            customerMode = false;

            // Changing color of mode
            personalBtn.BorderBrush = Brushes.Transparent;
            addressBtn.BorderBrush = new SolidColorBrush("#FF7B4BA5".ToColor());

            SearchingScroll(null, null);

        } // Address Click (customer view)
        private void PersonalClick(object sender, RoutedEventArgs e)
        {
            customerMode = true;

            // Changing color of mode
            personalBtn.BorderBrush = new SolidColorBrush("#FF7B4BA5".ToColor());
            addressBtn.BorderBrush = Brushes.Transparent;

            SearchingScroll(null, null);
        } // Personal Click (customer view)
        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            if (customer.Children.Count == 0)
                return;

            // Sending MessageBox to conferm about deleting
            MessageBoxResult result = MessageBox.Show($"Are you sure to delete {customer.Children.Count} customers","Information",MessageBoxButton.YesNo,MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes )
            {
                // Deleting customers
                for (int i = 0; i < customer.Children.Count; i++)
                {
                    Border border = customer.Children[i] as Border;
                    StackPanel panel = border.Child as StackPanel;
                    TextBlock id = panel.Children[0] as TextBlock;

                    if (listOfData.Any(x => x.customer_id == id.Text))
                    {
                        Place place = listOfData.Where(x => x.customer_id == id.Text).FirstOrDefault();
                        listOfData.Remove(place);
                        HelpingClass.RemoveRecordFromDB(place);
                    }

                }
                SearchingScroll(null, null);
            }    
            
        } // Button to delete customers
        private void ReportClick(object sender, RoutedEventArgs e)
        {
            // Opening new window (to report)
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
        } // Button to report 
        private void SortByClick(object sender, RoutedEventArgs e)
        {
            // sorting by Theme
            Button button = sender as Button;
            string theme = button.Content.ToString();
            switch(theme)
            {
                case "ID":
                    listOfData = listOfData.OrderBy(x => x.customer_id).ToList();
                    break;
                case "NAME":
                    listOfData = listOfData.OrderBy(x => x.first_name).ToList();
                    break;
                case "SURNAME":
                    listOfData = listOfData.OrderBy(x => x.last_name).ToList();
                    break;
                case "EMAIL":
                    listOfData = listOfData.OrderBy(x => x.email).ToList();
                    break;
                case "NUMBER":
                    listOfData = listOfData.OrderBy(x => x.contact_number).ToList();
                    break;
                case "CITY":
                    listOfData = listOfData.OrderBy(x => x.city).ToList();
                    break;
                case "POSTAL CODE":
                    listOfData = listOfData.OrderBy(x => x.postal_code).ToList();
                    break;
                case "STREET":
                    listOfData = listOfData.OrderBy(x => x.street).ToList();
                    break;
            }
            SearchingScroll(null, null);
        } // Sorting button 


        // Features for the window
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
    }

    // Special string extension class for color of structures
    public static class StringExtensions
    {
        public static Color ToColor(this string color)
        {
            return (Color)ColorConverter.ConvertFromString(color);
        }
    }
}