using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Mail;
using System.Data.SqlClient;
using System.Printing;
using System.Drawing;
using System.Windows.Controls;
using Microsoft.Maps.MapControl.WPF;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Windows.Controls.Primitives;

namespace MapDBTrack
{

    public static class HelpingClass
    {
        public static string[] engExp = new string[]
        {
            "Field is empty",
            "Wrong field format",
            "String is too long",
        };
        


        public static bool NetworkCheck(Window window)
        {
            while(true)
            {
                var client = new WebClient();
                var stream = client.OpenRead("http://www.google.com");
                if (stream == null)
                {
                    client.Dispose();
                    stream.Close();
                    MessageBoxResult result = MessageBox.Show("No internet connection\nDo you want try again?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Error);

                    if(result == MessageBoxResult.Yes)
                        continue;
                    else
                        window.Close();
                }
                break;
            }
            return true;
        }
        public static void SendingPassword(string login)
        {
            string passwordReminder = "", employeeEmail = "";
            string query = $"Select password, email From Employee Where login = '{login}'";

            SqlConnection connect = new SqlConnection(connectString);
            connect.Open();
            SqlCommand command = new SqlCommand(query,connect);
            SqlDataReader reader = command.ExecuteReader();

            if(reader.Read())
            {
                passwordReminder = reader.GetString(0);
                employeeEmail = reader.GetString(1);
            }
            

            string mail = "t75102442@gmail.com";
            string pass = "gezf aeiw utwg ovnb";
            int smtpPort = 587;

            MailMessage message = new MailMessage()
            {
                From = new MailAddress(mail),
                Subject = "Password Reminder",
                Body = $"Your password to your account: {passwordReminder}"
            };
            message.To.Add(new MailAddress(employeeEmail));

            var smtClient = new SmtpClient("smtp.gmail.com")
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(mail, pass),
                EnableSsl = true
            };

            smtClient.Send(message);

        }
        public static void CleanGrid(Grid MainGrid)
        {
            for (int i = MainGrid.Children.Count - 1; i >= 0; i--)
            {
                UIElement child = MainGrid.Children[i];
                int column = Grid.GetColumn(child);
                if (column == 2)
                {
                    MainGrid.Children.Remove(child);
                }
            }
        }
        public static RootObject ReadLocation(Location location)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36");
            webClient.Headers.Add("Referer", "https://www.google.com");
            var jsonData = webClient.DownloadData($"http://nominatim.openstreetmap.org/reverse?format=json&lat={location.Latitude.ToString("0.000000", CultureInfo.InvariantCulture)}&lon={location.Longitude.ToString("0.000000", CultureInfo.InvariantCulture)}");
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(RootObject));
            RootObject rootObject = (RootObject)ser.ReadObject(new MemoryStream(jsonData));
            return rootObject;
        }
        public static void AddingNewCustomer(Place customer)
        {
            string queryCustomer = $"Insert Into Customer (employee_id, contact_number, first_name, last_name, description, email)";
            string valuesCustomer = $"\nValues ({customer.employee_id}, '{customer.contact_number}', '{customer.first_name}', '{customer.last_name}', '{customer.description}', '{customer.email}')";
            string queryPlace = $"INSERT INTO Place (Customer_Id, province, city, postal_code, street, latitude, longitude)";
            string valuesPlace = $"\nValues ({customer.customer_id}, '{customer.province}', '{customer.city}', '{customer.postal_code}', '{customer.street}', {customer.latitude.ToString().Replace(',','.')}, {customer.longitude.ToString().Replace(',','.')})";
            queryCustomer += valuesCustomer;
            queryPlace += valuesPlace;

            SqlConnection sqlConnection = new SqlConnection(connectString);
            sqlConnection.Open();
            SqlCommand command1 = new SqlCommand(queryPlace, sqlConnection);
            command1.ExecuteNonQuery();
            SqlCommand command = new SqlCommand(queryCustomer, sqlConnection);
            command.ExecuteNonQuery();
            sqlConnection.Close();

        }
        public static List<Place> LoadingPlace(int id)
        {
            List<Place> places = new List<Place>();
            string query = $"SELECT * FROM Customer c RIGHT JOIN Place p ON c.id = p.Customer_Id WHERE employee_id = 1";

            SqlConnection sqlConnection = new SqlConnection(connectString);
            sqlConnection.Open();
            SqlCommand command = new SqlCommand(query, sqlConnection);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                Place p1 = new Place(
                    reader.GetInt32(1),
                    reader.GetString(2),
                    reader.GetString(3),
                    reader.GetString(4),
                    reader.GetString(5),
                    reader.GetString(6),
                    reader.GetInt32(7),
                    reader.GetString(8),
                    reader.GetString(9),
                    reader.GetString(10),
                    reader.GetString(11),
                    Convert.ToDouble(reader.GetDecimal(12)),
                    Convert.ToDouble(reader.GetDecimal(13)));

                places.Add(p1);
            }

            reader.Close();
            sqlConnection.Close();

            if (places.Count == 0)
                return new List<Place>();
            else
                return places;

        }
        public static string Exceptions(int num)
        {
            string[] exp = engExp; // warunek czy pol czy eng
            return exp[num];
        }
    }
}
