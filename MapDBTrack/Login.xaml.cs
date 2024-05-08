﻿using System.Text;
using System.Windows;
using System.Data;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.ComponentModel;

namespace MapDBTrack
{
    public partial class Login : Window
    {
        private int id; 
        private string login;
        public Registration registration;
        public Login()
        {
            InitializeComponent();
        }

        private void SignIn(object sender, RoutedEventArgs e)
        {
            HelpingClass.NetworkCheck(this);

            string login = LoginBox.Text;
            string password = PasswordBox.Text;

            if (!CheckingEmptyText(login, password))
                return;

            if (!CheckingLog(login, password))
                return;


            MainWindow mainWindow = new MainWindow(this.id, this.login);
            mainWindow.Show();
            this.Close();

        }// trying to login to account

        private void SignUp(object sender, RoutedEventArgs e)
        {
            registration = new Registration();
            this.Hide();
            registration.Show();
            registration.Closing += (s, args) => this.Show();
        } // opening registration window

        private bool CheckingEmptyText(string login, string password) // if box is empty red text under the box will show
        {

            bool log = true, pass = true;
            if (string.IsNullOrEmpty(login))
            {
                log = false;
                LoginFailed.Text = "Username not provided";
            }

            if (string.IsNullOrEmpty(password))
            {
                pass = false;
                PasswordFailed.Text = "Password not provided";
            }

            if (log && pass)
                return true;
            return false;

        }

        private bool CheckingLog(string login, string password)
        {

            string sqlQueryLog = $"Select login From Employee Where login = @login";
            string sqlQueryPass = $"Select password, login From Employee Where password = @password and login = @login";
            string sqlQueryId = "Select Id, Login From Employee Where login = @login";

            SqlConnection sql = new SqlConnection(HelpingClass.connectString);
            sql.Open();
            SqlCommand command = new SqlCommand(sqlQueryLog, sql);
            command.Parameters.AddWithValue("@login", login);
            SqlDataReader reader = command.ExecuteReader();

            if (!reader.HasRows)
            {
                LoginFailed.Text = "User doesn't exist";
                reader.Close();
                sql.Close();
                return false;
            }
            reader.Close();

            command = new SqlCommand(sqlQueryPass, sql);
            command.Parameters.AddWithValue("@password", password);
            command.Parameters.AddWithValue("@login", login);
            reader = command.ExecuteReader();

            if (!reader.HasRows)
            {
                PasswordFailed.Text = "Password is not correct";
                PasswordReminder.Text = "Click to to remind password";
                Reset.IsEnabled = true;
                reader.Close();
                sql.Close();
                return false;
            }
            reader.Close();

            command = new SqlCommand(sqlQueryId, sql);
            command.Parameters.AddWithValue("@login", login);
            reader = command.ExecuteReader();

            while(reader.Read())
            {
                this.id = reader.GetInt32(0);
                this.login = reader.GetString(1);
            }

            reader.Close();
            sql.Close();
            return true;
        } // Checking correct of password and login

        private void ResetPassword(object sender, EventArgs e) 
        {
            HelpingClass.NetworkCheck(this);
            HelpingClass.SendingPassword(LoginBox.Text);
            MessageBox.Show("Password was sent to your email", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        } // resseting password

        private void LoginChanged(object sender, RoutedEventArgs e)
        {
            LoginFailed.Text = string.Empty;
        } // if login box will change, red text will disappear

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordFailed.Text = string.Empty;
            Reset.IsEnabled = false;
            PasswordReminder.Text = string.Empty; 
        } // if password box will change, red text will disappear
    }
}