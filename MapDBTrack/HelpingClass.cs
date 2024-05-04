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

namespace MapDBTrack
{
    public static class HelpingClass
    {
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
                    {
                        continue;
                    }
                    else
                    {
                        window.Close();
                    }
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
    }
}
