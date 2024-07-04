using System.Net;
using System.Windows;
using System.Net.Mail;
using System.Data.SqlClient;
using System.Windows.Controls;
using Microsoft.Maps.MapControl.WPF;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Json;

namespace MapDBTrack
{

    public static class HelpingClass
    {
        public const string connectString = ""; // Connect string to DB
        public const string connectMap = ""; // Key API to bing maps
        public const string version = "1.1"; // Version of program
        public readonly static string[] engExp =
        {
            "Field is empty",
            "Wrong field format",
            "String is too long",
            "Error",
        };  // String exceptions
        
        public static bool NetworkCheck(Window window)
        {
            while(true)
            {
                var client = new WebClient();

                // open stream google.com
                using(var stream = client.OpenRead("http://www.google.com"))
                {
                    // if it is no conncetion internet then messagebox will show
                    if (stream == null)
                    {
                        MessageBoxResult result = MessageBox.Show("No internet connection\nDo you want try again?", "Error", MessageBoxButton.YesNo, MessageBoxImage.Error);

                        // trying to connect one more time
                        if (result == MessageBoxResult.Yes)
                            continue;
                        else
                            window.Close();
                    }
                    break;
                }
            }
            return true;
        } // Checking connection with internet
        public static void SendingPassword(string login)
        {
            string passwordReminder = "", employeeEmail = "";
            string query = $"Select password, email From Employee Where login = '{login}'"; // query to DB
            string mail = "t75102442@gmail.com"; // Special admin mail to sending mail to employee
            string pass = "gezf aeiw utwg ovnb"; // Special key of mail
            int smtpPort = 587; // port of mail

            // open connection with DB
            using (SqlConnection connect = new SqlConnection(connectString))
            {
                connect.Open();
                SqlCommand command = new SqlCommand(query, connect);
                
                using(SqlDataReader reader = command.ExecuteReader())
                {
                    // reading informations about employee
                    if (reader.Read())
                    {
                        passwordReminder = reader.GetString(0);
                        employeeEmail = reader.GetString(1);
                    }
                }

            }

            // Theme of mail and body
            MailMessage message = new MailMessage()
            {
                From = new MailAddress(mail),
                Subject = "Password Reminder",
                Body = $"Your password to your account: {passwordReminder}"
            };
            message.To.Add(new MailAddress(employeeEmail));

            // Setting mail properties
            var smtClient = new SmtpClient("smtp.gmail.com")
            {
                Port = smtpPort,
                Credentials = new NetworkCredential(mail, pass),
                EnableSsl = true
            };

            // sending email to employee 
            smtClient.Send(message);

        } // Sending password reminder to user email
        public static void CleanGrid(Grid grid)
        {
            // Deleting all childrens from gird
            grid.Children.Clear();

            // Deleting all rows
            grid.RowDefinitions.Clear();

            // Deleting all columns
            grid.ColumnDefinitions.Clear();
        } // Cleaning Grid 
        public static RootObject ReadLocation(Location location)
        {
            WebClient webClient = new WebClient();
            webClient.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36");
            webClient.Headers.Add("Referer", "https://www.google.com");
            var jsonData = webClient.DownloadData($"http://nominatim.openstreetmap.org/reverse?format=json&lat={location.Latitude.ToString("0.000000", CultureInfo.InvariantCulture)}&lon={location.Longitude.ToString("0.000000", CultureInfo.InvariantCulture)}");
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(RootObject));
            RootObject rootObject = (RootObject)ser.ReadObject(new MemoryStream(jsonData));
            return rootObject;
        } // Reading locations when employee will click on map
        public static void AddingNewCustomer(Place customer)
        {
            string queryCustomer = $"Insert Into Customer (id, employee_id, contact_number, first_name, last_name, description, email)" + // Query to customer
            $"\nValues ('{customer.customer_id}', {customer.employee_id}, '{customer.contact_number}', '{customer.first_name}', '{customer.last_name}', '{customer.description}', '{customer.email}')";
            string queryPlace = $"INSERT INTO Place (customer_id, province, city, postal_code, street, latitude, longitude)" + // Query to employee 
            $"\nValues ('{customer.customer_id}', '{customer.province}', '{customer.city}', '{customer.postal_code}', '{customer.street}', {customer.latitude.ToString().Replace(',', '.')}, {customer.longitude.ToString().Replace(',', '.')})";

            // Open stream to DB
            using(SqlConnection sqlConnection = new SqlConnection(connectString))
            {
                sqlConnection.Open();
                SqlCommand commandCustomer = new SqlCommand(queryCustomer, sqlConnection);
                commandCustomer.ExecuteNonQuery();
                SqlCommand commandPlace = new SqlCommand(queryPlace, sqlConnection);
                commandPlace.ExecuteNonQuery();
            }

        } // Adding new customer to DB
        public static void EditCustomer(Place customer)
        {
            string updateCustomer = $"UPDATE Customer SET " +
                        $"employee_id = {customer.employee_id}, " +
                        $"contact_number = '{customer.contact_number}', " +
                        $"first_name = '{customer.first_name}', " +
                        $"last_name = '{customer.last_name}', " +
                        $"description = '{customer.description}', " +
                        $"email = '{customer.email}' " +
                        $"WHERE id = '{customer.customer_id}'"; // Query to edit customer

            string updatePlace = $"UPDATE Place SET " +
                     $"province = '{customer.province}', " +
                     $"city = '{customer.city}', " +
                     $"postal_code = '{customer.postal_code}', " +
                     $"street = '{customer.street}', " +
                     $"latitude = {customer.latitude.ToString().Replace(',', '.')}, " +
                     $"longitude = {customer.longitude.ToString().Replace(',', '.')}" +
                     $"WHERE customer_id = '{customer.customer_id}'"; // Query to edit place

            // open stream to DB
            using(SqlConnection sqlConnection = new SqlConnection(connectString))
            {
                sqlConnection.Open();
                SqlCommand commandCustomer = new SqlCommand(updateCustomer, sqlConnection);
                commandCustomer.ExecuteNonQuery();
                SqlCommand commandPlace = new SqlCommand(updatePlace, sqlConnection);
                commandPlace.ExecuteNonQuery();
            };
        } // Sending new infromations to DB
        public static List<Place> LoadingPlace(int id)
        {
            string query = $"SELECT * FROM Customer c RIGHT JOIN Place p ON c.id = p.Customer_Id WHERE employee_id = {id}"; // Query to join to tables
            List<Place> places = new List<Place>();

            // Open stream to DB
            using (SqlConnection sqlConnection = new SqlConnection(connectString))
            {
                sqlConnection.Open();
                SqlCommand command = new SqlCommand(query, sqlConnection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    // Adding new place to list
                    while (reader.Read())
                    {
                        Place p1 = new Place(
                            reader.GetInt32(1),
                            reader.GetString(2),
                            reader.GetString(3),
                            reader.GetString(4),
                            reader.GetString(5),
                            reader.GetString(6),
                            reader.GetString(7),
                            reader.GetString(8),
                            reader.GetString(9),
                            reader.GetString(10),
                            reader.GetString(11),
                            Convert.ToDouble(reader.GetDecimal(12)),
                            Convert.ToDouble(reader.GetDecimal(13)));

                        places.Add(p1);
                    }
                }
            }

            // returning list
            if (places.Count == 0)
                return new List<Place>();
            else
                return places;

        } // Loading data to list
        public static string GetDescTool(Place p1)
        {
            string info = $"Name: {p1.first_name}"; // name of customer
            info += string.IsNullOrEmpty(p1.last_name) ? "\n" : $"\nLast name: {p1.last_name}\n"; // last name of customer
            info += string.IsNullOrEmpty(p1.email) ? $"Nr: {p1.contact_number}\n" : $"Nr: {p1.contact_number}\nEmail: {p1.email}\n"; // Contact number and email of customer
            info += $"{p1.city} {p1.postal_code} {p1.street} {p1.province}"; // Address of customer

            // Setting description in fine format
            if(!string.IsNullOrEmpty(p1.description))
            {
                string desc = "";
                int x = 0;
                for (int i = 0; i < p1.description.Length; i++)
                {
                    desc += p1.description[i];
                    ++x;
                    if (x > 50)
                    {
                        desc += '\n';
                        x = 0;
                    }
                }
                info += "\n\n" + desc;
            }
            return info;
        } // Descripiton on the map
        public static string GetIdFromDB()
        {
            while (true)
            {
                Random random = new Random();
                string alfa = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"; // Chars to ID
                string id = ""; // id of customer

                // creating ID
                for (int i = 0; i < 8; i++)
                {
                    id += alfa[random.Next(0, alfa.Length)];
                }

                string query = $"Select id From Customer Where id = '{id}'"; // Query to search clone Id

                // Creating stream to DB
                using(SqlConnection sql = new SqlConnection(connectString))
                {
                    sql.Open();
                    SqlCommand sqlCommand = new SqlCommand(query, sql);
                    using(SqlDataReader read = sqlCommand.ExecuteReader())
                    {
                        // if ID is not in DB than we return correct ID
                        if (!read.HasRows)
                            return id;
                    }
                }
            }
        } // Setting random ID
        public static void RemoveRecordFromDB(Place p1)
        {
            string queryPlace = $"DELETE FROM Place WHERE customer_id = '{p1.customer_id}'" + 
            $"\nDELETE FROM Customer WHERE id = '{p1.customer_id}'"; // Query to Delete customer
            // Creating Stream to DB
            using (SqlConnection sql = new SqlConnection(connectString))
            {
                SqlCommand sqlCommand = new SqlCommand(queryPlace, sql);
                sql.Open();
                sqlCommand.ExecuteNonQuery();
            }
        } // Removing record from DB
        public static string DescritpionScrollView(List<Place> customers,bool mode, int loop, int positionOfPlace)
        {
            // Personal mode
            if (mode)
            {
                switch (loop)
                {
                    case 0:
                        return customers[positionOfPlace].customer_id;
                    case 1:
                        return customers[positionOfPlace].first_name;
                    case 2:
                        return customers[positionOfPlace].last_name;
                    case 3:
                        return customers[positionOfPlace].email;
                    case 4:
                        return customers[positionOfPlace].contact_number;
                    default:
                        return engExp[3];
                }
            }
            // Address mode
            else
            {
                switch (loop)
                {
                    case 0:
                        return customers[positionOfPlace].customer_id;
                    case 1:
                        return customers[positionOfPlace].city;
                    case 2:
                        return customers[positionOfPlace].postal_code;
                    case 3:
                        return customers[positionOfPlace].street;
                    default:
                        return engExp[3];
                }
            }
        } // Special information for scroll view
        public static string DescriptionStackPanel(bool mode, int loop)
        {
            // Personal mode
            if (mode)
            {
                switch (loop)
                {
                    case 0:
                        return "ID";
                    case 1:
                        return "NAME";
                    case 2:
                        return "SURNAME";
                    case 3:
                        return "EMAIL";
                    case 4:
                        return "NUMBER";
                    default:
                        return engExp[3];
                }
            }
            // Address mode
            else
            {
                switch (loop)
                {
                    case 0:
                        return "ID";
                    case 1:
                        return "CITY";
                    case 2:
                        return "POSTAL CODE";
                    case 3:
                        return "STREET";
                    default:
                        return engExp[3];
                }
            }
        } // Theme description for scroll view
        public static void InformationsAboutProgram()
        {
            string info = $"Version: {version}\nContact: kus.konrad1@gmail.com\nLicense: MapDBTrack Commercial Use License";
            MessageBox.Show(info, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        } // Information MessageBox
    }

}
