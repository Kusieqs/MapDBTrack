using Microsoft.Maps.MapControl.WPF;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MapDBTrack
{
    public partial class AddingCustomer : Window
    {
        bool correctClose = false;
        private Location location;
        private Pushpin Pushpin;
        private Map map;
        private Window mainWindow;
        private bool edit = false;

        private static string id;
        private static Place customer;
        public AddingCustomer(Location location, Window window, Pushpin pin, Map map)
        {
            InitializeComponent();
            this.location = location;
            LoadingData();
            this.Pushpin = pin;
            this.map = map;
            Closing += CloseWindow;
            mainWindow = window;
        }
        public AddingCustomer(Place place,bool edit)
        {
            InitializeComponent();
            ThemeOfPanel.Text = "Edit customer";
            this.edit = true;
            FirstNameBox.Text = place.first_name;
            LastNameBox.Text = place.last_name;
            ContactBox.Text = place.contact_number;
            EmailBox.Text = place.email;
            ProvinceBox.Text = place.province;
            CityBox.Text = place.city;
            PostalCodeBox.Text = place.postal_code;
            StreetBox.Text = place.street;
            LatitudeBox.Text = place.latitude.ToString();
            LongitudeBox.Text = place.longitude.ToString();
            DescriptionBox.Text = place.description;
            id = place.customer_id;
            customer = place;

        }
        public void AcceptClick(object sender, RoutedEventArgs e)
        {
            // checking informations that are correct
            if(FirstNameExceptions() & 
                LastNameExceptions() & 
                ContactExceptions() & 
                EmailExceptions() &
                ProvinceExceptions() & 
                CityExceptions() & 
                PostalExceptions() & 
                StreetExceptions())
            {
                if (edit)
                {
                    // editing existing place
                    int x = MainWindow.listOfData.IndexOf(customer);
                    customer.first_name = FirstNameBox.Text;
                    customer.last_name = LastNameBox.Text;
                    customer.email = EmailBox.Text;
                    customer.contact_number = ContactBox.Text;
                    customer.province = ProvinceBox.Text;
                    customer.postal_code = PostalCodeBox.Text;
                    customer.street = StreetBox.Text;
                    customer.city = CityBox.Text;
                    customer.description = DescriptionBox.Text;

                    MainWindow.listOfData[x] = customer;

                    HelpingClass.EditCustomer(customer);
                }
                else
                {
                    // creating new object
                    string lastOne = HelpingClass.GetIdFromDB();
                    Place place = new Place(
                        MainWindow.idOfEmployee,
                        ContactBox.Text,
                        FirstNameBox.Text,
                        LastNameBox.Text,
                        DescriptionBox.Text,
                        EmailBox.Text,
                        lastOne,
                        ProvinceBox.Text,
                        CityBox.Text,
                        PostalCodeBox.Text,
                        StreetBox.Text,
                        double.Parse(LatitudeBox.Text),
                        double.Parse(LongitudeBox.Text)
                        );

                    // adding new object to list and map
                    MainWindow.listOfData.Add(place);
                    HelpingClass.AddingNewCustomer(place);
                    correctClose = true;
                    MainWindow.acceptOverridePin = true;
                }
                this.Close();
            }
            else
                return;

        } // Accepting iformations about customer
        public void DeleteClick(object sender, RoutedEventArgs e)
        {
            if(!edit)
                map.Children.Remove(Pushpin);

            Close();
        } // deleting pinn from map
        private void LoadingData()
        {
            try
            {
                LongitudeBox.Text = location.Longitude.ToString();
                LatitudeBox.Text = location.Latitude.ToString();

                // reading location on map
                RootObject rootObject = HelpingClass.ReadLocation(location);
                string numberRoad = rootObject.display_name;

                // setting information about road
                if (numberRoad == null)
                    throw new FormatException();
                else if (int.TryParse(numberRoad.Split(',')[0], out int x))
                    StreetBox.Text = $"{rootObject.address.road} {x}";
                else
                    StreetBox.Text = $"{rootObject.address.road}";

                // setting information about province, city and postal code
                ProvinceBox.Text = rootObject.address.state;
                CityBox.Text = rootObject.address.city;
                PostalCodeBox.Text = rootObject.address.postcode;
            }
            catch (Exception)
            {
                return;
            }

        } // Loading inforamtion about place where pinn was inputed
        public void CloseWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!correctClose)
                map.Children.Remove(Pushpin);
        } // Ovveriding method when window is closing
        private void BorderClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        } // feature to moving window

        #region Excpetions to infomration about customer
        private bool FirstNameExceptions()
        {
            if (FirstNameBox.Text.Trim().Length == 0)
            {
                FirstNameError.Text = HelpingClass.engExp[0];
                return false;
            }
            else if (!Regex.IsMatch(FirstNameBox.Text.Trim(), @"^[\p{L}\p{M}]+$"))
            {
                FirstNameError.Text = HelpingClass.engExp[1];
                return false;
            }
            else if (FirstNameBox.Text.Trim().Length > 50)
            {
                FirstNameError.Text = HelpingClass.engExp[2];
                return false;
            }

            return true;
        } // Error text when name is wrong
        private bool LastNameExceptions()
        {
            if (LastNameBox.Text.Trim().Length > 0)
                if (!Regex.IsMatch(LastNameBox.Text.Trim(), @"^[\p{L}\p{M}\s-]+$"))
                {
                    LastNameError.Text = HelpingClass.engExp[1];
                    return false;
                }
                else if (LastNameBox.Text.Trim().Length > 50)
                {
                    LastNameError.Text = HelpingClass.engExp[2];
                    return false;
                }

            return true;
        } // Error text when last name is wrong
        private bool ContactExceptions()
        {


            if (ContactBox.Text.Trim().Length == 0)
            {
                ContactError.Text = HelpingClass.engExp[0];
                return false;
            }
            else if (!Regex.IsMatch(ContactBox.Text.Trim(), @"^\+?\d{1,}$"))
            {
                ContactError.Text = HelpingClass.engExp[1];
                return false;
            }
            else if (ContactBox.Text.Trim().Length > 20)
            {
                ContactError.Text = HelpingClass.engExp[2];
                return false;
            }
            return true;
        } // Error text when contact is wrong
        private bool EmailExceptions()
        {
            string pattern = @"^[^\.\s][.\w]*@[.\w]+\.[a-zA-Z]{2,4}$";

            if (EmailBox.Text.Trim().Length != 0 && !Regex.IsMatch(EmailBox.Text.Trim(), pattern))
            {
                MailError.Text = HelpingClass.engExp[1];
                return false;
            }
            return true;

        } // Error text when email is wrong
        private bool ProvinceExceptions()
        {
            if (ProvinceBox.Text.Trim().Length > 0)
                if (!Regex.IsMatch(ProvinceBox.Text.Trim(), @"^[\p{L}\p{M}\s-]+$"))
                {
                    ProvinceError.Text = HelpingClass.engExp[1]; 
                    return false;
                }
                else if (ProvinceBox.Text.Trim().Length > 50)
                {
                    ProvinceError.Text = HelpingClass.engExp[2];
                    return false;
                }
            return true;
        } // Error text when province is wrong
        private bool CityExceptions()
        {
            if (CityBox.Text.Trim().Length == 0)
            {
                CityError.Text = HelpingClass.engExp[0];
                return false;
            }
            else if (!Regex.IsMatch(CityBox.Text.Trim(), @"^[\p{L}\p{M}\s-]+$"))
            {
                CityError.Text = HelpingClass.engExp[1];
                return false;
            }
            else if (CityBox.Text.Trim().Length > 50)
            {
                CityError.Text = HelpingClass.engExp[2];
                return false;
            }
            return true;
        } // Error text when city is wrong
        private bool PostalExceptions()
        {
            if (PostalCodeBox.Text.Trim().Length == 0)
            {
                PostalError.Text = HelpingClass.engExp[0];
                return false;
            }
            else if (!Regex.IsMatch(PostalCodeBox.Text.Trim(), @"^\d+-\d+$"))
            {
                PostalError.Text = HelpingClass.engExp[1];
                return false;
            }
            else if (PostalCodeBox.Text.Trim().Length > 10)
            {
                PostalError.Text = HelpingClass.engExp[2];
                return false;
            }
            return true;
        } // Error text when postal code is wrong
        private bool StreetExceptions()
        {
            if (StreetBox.Text.Trim().Length == 0)
            {
                StreetError.Text = HelpingClass.engExp[0];
                return false;
            }
            else if (!Regex.IsMatch(StreetBox.Text.Trim(), @"^[\p{L}\p{M}0-9\s-]+$"))
            {
                StreetError.Text = HelpingClass.engExp[1];
                return false;
            }
            else if (StreetBox.Text.Trim().Length > 100)
            {
                StreetError.Text = HelpingClass.engExp[2];
                return false;
            }
            return true;
        } // Error text when street is wrong
        private void FirstChanged(object sender, EventArgs e)
        {
            FirstNameError.Text = string.Empty;
        } // Error name text disappearing 
        private void LastChanged(object sender, EventArgs e)
        {
            LastNameError.Text = string.Empty;
        } // Error last name text disappearing 
        private void NumberChanged(object sender, EventArgs e)
        {
            ContactError.Text = string.Empty;
        } // Error number text disappearing 
        private void EmailChanged(object sender, EventArgs e)
        {
            MailError.Text = string.Empty;
        }  // Error email text disappearing 
        private void ProvinceChanged(object sender, EventArgs e)
        {
            ProvinceError.Text = string.Empty;
        } // Error province text disappearing 
        private void CityChanged(object sender, EventArgs e)
        {
            CityError.Text = string.Empty;
        } // Error city text disappearing 
        private void PostalChanged(object sender, EventArgs e)
        {
            PostalError.Text = string.Empty;
        } // Error postal text disappearing 
        private void StreetChanged(object sender, EventArgs e)
        {
            StreetError.Text = string.Empty;
        } // Error street text disappearing 
         
        #endregion

    }
}

