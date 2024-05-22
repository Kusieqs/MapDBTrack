using Microsoft.Maps.MapControl.WPF;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace MapDBTrack
{
    public partial class AddingCustomer : Window
    {
        bool correctClose = false;
        private Location location;
        private Pushpin Pushpin;
        private Map map;
        private Window mainWindow;
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
        public void AcceptClick(object sender, RoutedEventArgs e)
        {
            if(FirstNameExceptions() & 
                LastNameExceptions() & 
                ContactExceptions() & 
                EmailExceptions() &
                ProvinceExceptions() & 
                CityExceptions() & 
                PostalExceptions() & 
                StreetExceptions())
            {
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

                MainWindow.places.Add(place);
                HelpingClass.AddingNewCustomer(place);
                correctClose = true;
                MainWindow.acceptOverridePin = true;
                this.Close();
            }
            else
                return;

            //obsluga dodania danych do bazy danych
        }
        private void LoadingData()
        {
            try
            {
                LongitudeBox.Text = location.Longitude.ToString();
                LatitudeBox.Text = location.Latitude.ToString();

                RootObject rootObject = HelpingClass.ReadLocation(location);
                string numberRoad = rootObject.display_name;

                if (numberRoad == null)
                    throw new FormatException();
                else if (int.TryParse(numberRoad.Split(',')[0], out int x))
                    StreetBox.Text = $"{rootObject.address.road} {x}";
                else
                    StreetBox.Text = $"{rootObject.address.road}";

                ProvinceBox.Text = rootObject.address.state;
                CityBox.Text = rootObject.address.city;
                PostalCodeBox.Text = rootObject.address.postcode;
            }
            catch (Exception)
            {
                return;
            }

        } // Loading inforamtion about place where pinn was inputed
        public void DeleteClick(object sender, RoutedEventArgs e)
        {
            map.Children.Remove(Pushpin);
            Close();
        } // deleting pinn from map
        public void CloseWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!correctClose)
                map.Children.Remove(Pushpin);
        } // Ovveriding method when window is closing

        #region Excpetions to infomration about customer
        private bool FirstNameExceptions()
        {
            if (FirstNameBox.Text.Trim().Length == 0)
            {
                FirstNameError.Text = HelpingClass.Exceptions(0);
                return false;
            }
            else if (!Regex.IsMatch(FirstNameBox.Text.Trim(), @"^[\p{L}\p{M}]+$"))
            {
                FirstNameError.Text = HelpingClass.Exceptions(1);
                return false;
            }
            else if (FirstNameBox.Text.Trim().Length > 50)
            {
                FirstNameError.Text = HelpingClass.Exceptions(2);
                return false;
            }

            return true;
        }
        private bool LastNameExceptions()
        {
            if (LastNameBox.Text.Trim().Length > 0)
                if (!Regex.IsMatch(LastNameBox.Text.Trim(), @"^[\p{L}\p{M}\s-]+$"))
                {
                    LastNameError.Text = HelpingClass.Exceptions(1);
                    return false;
                }
                else if (LastNameBox.Text.Trim().Length > 50)
                {
                    LastNameError.Text = HelpingClass.Exceptions(2);
                    return false;
                }

            return true;
        }
        private bool ContactExceptions()
        {


            if (ContactBox.Text.Trim().Length == 0)
            {
                ContactError.Text = HelpingClass.Exceptions(0);
                return false;
            }
            else if (!Regex.IsMatch(ContactBox.Text.Trim(), @"^\+?\d{1,}$"))
            {
                ContactError.Text = HelpingClass.Exceptions(1);
                return false;
            }
            else if (ContactBox.Text.Trim().Length > 20)
            {
                ContactError.Text = HelpingClass.Exceptions(2);
                return false;
            }
            return true;
        }
        private bool EmailExceptions()
        {
            string pattern = @"^[^\.\s][.\w]*@[.\w]+\.[a-zA-Z]{2,4}$";

            if (EmailBox.Text.Trim().Length != 0 && !Regex.IsMatch(EmailBox.Text.Trim(), pattern))
            {
                MailError.Text = HelpingClass.Exceptions(1);
                return false;
            }
            return true;

        }
        private bool ProvinceExceptions()
        {
            if (ProvinceBox.Text.Trim().Length > 0)
                if (!Regex.IsMatch(ProvinceBox.Text.Trim(), @"^[\p{L}\p{M}\s-]+$"))
                {
                    ProvinceError.Text = HelpingClass.Exceptions(1); 
                    return false;
                }
                else if (ProvinceBox.Text.Trim().Length > 50)
                {
                    ProvinceError.Text = HelpingClass.Exceptions(2);
                    return false;
                }
            return true;
        }
        private bool CityExceptions()
        {
            if (CityBox.Text.Trim().Length == 0)
            {
                CityError.Text = HelpingClass.Exceptions(0);
                return false;
            }
            else if (!Regex.IsMatch(CityBox.Text.Trim(), @"^[\p{L}\p{M}\s-]+$"))
            {
                CityError.Text = HelpingClass.Exceptions(1);
                return false;
            }
            else if (CityBox.Text.Trim().Length > 50)
            {
                CityError.Text = HelpingClass.Exceptions(2);
                return false;
            }
            return true;
        }
        private bool PostalExceptions()
        {
            if (PostalCodeBox.Text.Trim().Length == 0)
            {
                PostalError.Text = HelpingClass.Exceptions(0);
                return false;
            }
            else if (!Regex.IsMatch(PostalCodeBox.Text.Trim(), @"^\d+-\d+$"))
            {
                PostalError.Text = HelpingClass.Exceptions(1);
                return false;
            }
            else if (PostalCodeBox.Text.Trim().Length > 10)
            {
                PostalError.Text = HelpingClass.Exceptions(2);
                return false;
            }
            return true;
        }
        private bool StreetExceptions()
        {
            if (StreetBox.Text.Trim().Length == 0)
            {
                StreetError.Text = HelpingClass.Exceptions(0);
                return false;
            }
            else if (StreetBox.Text.Trim().Length > 100)
            {
                StreetError.Text = HelpingClass.Exceptions(2);
                return false;
            }
            else if (!Regex.IsMatch(StreetBox.Text.Trim(), @"^[\p{L}\p{M}0-9\s-]+$"))
            {
                StreetError.Text = HelpingClass.Exceptions(1);
                return false;
            }
            return true;
        }

        private void FirstChanged(object sender, EventArgs e)
        {
            FirstNameError.Text = string.Empty;
        }
        private void LastChanged(object sender, EventArgs e)
        {
            LastNameError.Text = string.Empty;
        }
        private void NumberChanged(object sender, EventArgs e)
        {
            ContactError.Text = string.Empty;
        }
        private void EmailChanged(object sender, EventArgs e)
        {
            MailError.Text = string.Empty;
        }
        private void ProvinceChanged(object sender, EventArgs e)
        {
            ProvinceError.Text = string.Empty;
        }
        private void CityChanged(object sender, EventArgs e)
        {
            CityError.Text = string.Empty;
        }
        private void PostalChanged(object sender, EventArgs e)
        {
            PostalError.Text = string.Empty;
        }
        private void StreetChanged(object sender, EventArgs e)
        {
            StreetError.Text = string.Empty;
        }

        #endregion

    }
}

