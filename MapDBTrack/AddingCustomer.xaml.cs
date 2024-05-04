using Microsoft.Maps.MapControl.WPF;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public partial class AddingCustomer : Window
    {
        private Pushpin Pushpin;
        private Map map;
        public AddingCustomer(Location location, Window window, Pushpin pin, Map map)
        {
            InitializeComponent();
            LongitudeBox.Text = location.Longitude.ToString();
            LatitudeBox.Text = location.Latitude.ToString();
            this.Pushpin = pin;
            this.map = map;
            this.Closing += CloseWindow;
        }
        public void AcceptClick(object sender, RoutedEventArgs e)
        {

            if (FirstNameBox.Text.Trim().Length == 0)
                MessageBox.Show("Blad1");
            else if (!Regex.IsMatch(FirstNameBox.Text.Trim(), @"^[A-Za-ząćęłńóśźżĄĆĘŁŃÓŚŹŻ]+$"))
                MessageBox.Show("Blad2");
            else if (FirstNameBox.Text.Trim().Length > 50)
                MessageBox.Show("Blad3");



            if (LastNameBox.Text.Trim().Length > 0)
                if (!Regex.IsMatch(LastNameBox.Text.Trim(), @"^[A-Za-ząćęłńóśźżĄĆĘŁŃÓŚŹŻ]+- ?[A-Za-ząćęłńóśźżĄĆĘŁŃÓŚŹŻ]+$"))
                    MessageBox.Show("Blad4");
                else if (LastNameBox.Text.Trim().Length > 50)
                    MessageBox.Show("Blad5");


            if (ContactBox.Text.Trim().Length == 0)
                MessageBox.Show("Blad6");
            if (!Regex.IsMatch(ContactBox.Text.Trim(), @"^\+?\d{1,}$"))
                MessageBox.Show("Blad7");
            else if (ContactBox.Text.Trim().Length > 20)
                MessageBox.Show("Blad8");

            string pattern = @"^[^\.\s][.\w]*@[.\w]+\.[a-zA-Z]{2,4}$";

            if (EmailBox.Text.Trim().Length != 0 || !Regex.IsMatch(EmailBox.Text.Trim(), pattern))
                MessageBox.Show("Blad9");

            if (ProvinceBox.Text.Trim().Length > 0)
                if (!Regex.IsMatch(ProvinceBox.Text.Trim(), @"^[A-Za-ząćęłńóśźżĄĆĘŁŃÓŚŹŻ]+- ?[A-Za-ząćęłńóśźżĄĆĘŁŃÓŚŹŻ]+$"))
                    MessageBox.Show("Blad10");
                else if (ProvinceBox.Text.Trim().Length > 50)
                    MessageBox.Show("Blad11");


            if (CityBox.Text.Trim().Length == 0)
                MessageBox.Show("Blad12");
            else if (CityBox.Text.Trim().Length > 50)
                MessageBox.Show("Blad13");
            else if (!Regex.IsMatch(CityBox.Text.Trim(), @"^[A-Za-ząćęłńóśźżĄĆĘŁŃÓŚŹŻ]+- ?[A-Za-ząćęłńóśźżĄĆĘŁŃÓŚŹŻ]+$"))
                MessageBox.Show("Blad14");

            if (PostalCodeBox.Text.Trim().Length == 0)
                MessageBox.Show("Blad15");
            else if (PostalCodeBox.Text.Trim().Length > 10)
                MessageBox.Show("Blad16");
            else if (!Regex.IsMatch(PostalCodeBox.Text.Trim(), @"^\d+-\d+$"))
                MessageBox.Show("Blad17");


            if (StreetBox.Text.Trim().Length == 0)
                MessageBox.Show("Blad18");
            else if (StreetBox.Text.Trim().Length > 100)
                MessageBox.Show("Blad19");
            else if (!Regex.IsMatch(StreetBox.Text.Trim(), @"^[A-Za-ząćęłńóśźżĄĆĘŁŃÓŚŹŻ]+- ?[A-Za-ząćęłńóśźżĄĆĘŁŃÓŚŹŻ]+\d+$"))
                MessageBox.Show("Blad20");

            //obsluga dodania danych do bazy danych
        }
        public void DeleteClick(object sender, RoutedEventArgs e)
        {
            map.Children.Remove(Pushpin);
            Close();
        }
        public void CloseWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            map.Children.Remove(Pushpin);
        } // Ovveriding method when window is closing
    }
}

