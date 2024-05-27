using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    public partial class Registration : Window
    {
        public Registration()
        {
            InitializeComponent();
        }
        private void SignUp(object sender, RoutedEventArgs e)
        {
            //checking network connection
            HelpingClass.NetworkCheck(this);

            //checking if there is empty string
            if (!CheckingEmptyString())
                return;


            SqlConnection sql = new SqlConnection(HelpingClass.connectString);
            sql.Open();

            // Checking all paramters with corrections
            if (!CheckAllParameters(sql))
                return;

            // special query to sql
            string sqlQuery = "Insert Into Employee (login, password, email)";
            string values = $"\nValues ('{Login.Text}','{Password.Password}','{Email.Text}')";

            sqlQuery += values;
            SqlCommand command = new SqlCommand(sqlQuery, sql);
            command.ExecuteNonQuery();
            sql.Close();

            //message box about to provide of registation
            MessageBox.Show("Correct registration", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        } // registration new account
        private bool CheckingEmptyString()
        {
            bool correct = true;

            // checking empty login
            if (string.IsNullOrEmpty(Login.Text))
            {
                LoginFailed.Text = "Login is empty";
                correct = false;
            }

            // checking empty password
            if (string.IsNullOrEmpty(Password.Password))
            {
                PasswordFailed.Text = "Password is empty";
                correct = false;
            }

            // checking empty repeatpassword
            if (string.IsNullOrEmpty(RepeatPassword.Password))
            {
                RepeatPasswordFailed.Text = "Password repeat is empty";
                correct = false;
            }

            // checking empty email
            if (string.IsNullOrEmpty(Email.Text))
            {
                EmailFailed.Text = "Email is empty";
                correct = false;
            }
            return correct;

        } // checking empty area and writing message
        private bool CheckAllParameters(SqlConnection sql)
        {
            return LoginCheck(sql) & PasswordCheck() & RepeatPasswordCheck() & EmailCheck(sql);
        } // feature with checking 4 features
        private void Back(object sender, RoutedEventArgs e)
        {
            this.Close();
        } // close window
        private void BorderClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        } // feature to moving window
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        } // Exit button
        private void InfoClick(object sender, RoutedEventArgs e)
        {
            string info = $"{HelpingClass.version}\nContact: kus.konrad1@gmail.com\nLicense: MapDBTrack Commercial Use License";
            MessageBox.Show(info, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        } // information button

        #region Parameters Check
        private bool LoginCheck(SqlConnection sql)
        {
            // checking length of login (min 4)
            if (Login.Text.Length < 4)
            {
                LoginFailed.Text = "Login is too short";
                return false;
            }

            // checking length of login (max 20)
            if (Login.Text.Length > 20)
            {
                LoginFailed.Text = "Login is too long";
                return false;
            }

            // special query to search same login
            string sqlQuery = $"Select login From Employee Where login = @login";
            SqlCommand command = new SqlCommand(sqlQuery, sql);
            command.Parameters.AddWithValue("@login", Login.Text);
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                LoginFailed.Text = "This login is locked";
                reader.Close();
                return false;
            }
            reader.Close();
            return true;

        } // Checking login (new login can not be in db before)
        private bool PasswordCheck()
        {
            // checking length of password (min 6)
            if (Password.Password.Length < 6)
            {
                PasswordFailed.Text = "Password is too short";
                return false;
            }

            // checking length of password (max 20)
            if (PasswordFailed.Text.Length > 20)
            {
                PasswordFailed.Text = "Password is to long";
                return false;
            }

            return true;

        } // Checking password (correct length)
        private bool RepeatPasswordCheck()
        {
            // checking that 2 passwords are the same
            if (RepeatPassword.Password.Trim() != Password.Password.Trim())
            {
                RepeatPasswordFailed.Text = "Passwords are not the same";
                return false;
            }
            return true;
        } // checkiong 2 passwords whether they are same
        private bool EmailCheck(SqlConnection sql)
        {

            // special query to search same email on DB
            string sqlQuery = $"Select email From Employee Where email = @email";
            SqlCommand command = new SqlCommand(sqlQuery, sql);
            command.Parameters.AddWithValue("@email", Email.Text);
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                EmailFailed.Text = "This email is locked";
                reader.Close();
                return false;
            }
            reader.Close();
            string pattern = @"^[^\.\s][.\w]*@[.\w]+\.[a-zA-Z]{2,4}$";

            // checking email with pattern
            if (!Regex.IsMatch(Email.Text, pattern))
            {
                EmailFailed.Text = "This email is not correct";
                return false;
            }

            return true;
        } // Checking email (new email can not be in db before)
        #endregion

        #region Features to clean information about uncorrect registration
        private void LoginChanged(object sender, RoutedEventArgs e)
        {
            LoginFailed.Text = string.Empty;
        } // error login is disappearing

        private void EmailChanged(object sender, RoutedEventArgs e)
        {
            EmailFailed.Text = string.Empty;
        } // error Email is disappearing     

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordFailed.Text = string.Empty;
        } // error Password is disappearing     

        private void RepeatPasswordChanged(object sender, RoutedEventArgs e)
        {
            RepeatPasswordFailed.Text = string.Empty;
        } // error repeat password is disappearing     
        #endregion


    }
}
