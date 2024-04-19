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

        private void Back(object sender, RoutedEventArgs e)
        {
            this.Close();
        } // close window

        private void CheckNetwork(object sender, RoutedEventArgs e)
        {
            HelpingClass.NetworkCheck(this);
        }  // checking network connection

        private void SignUp(object sender, RoutedEventArgs e)
        {
            HelpingClass.NetworkCheck(this);

            if (!CheckingEmptyString())
                return;

            SqlConnection sql = new SqlConnection(HelpingClass.connectString);
            sql.Open();

            if (!CheckAllParameters(sql))
                return;

            string sqlQuery = "Insert Into Employee (login, password, email)";
            string values = $"\nValues ('{Login.Text}','{Password.Text}','{Email.Text}')";
            sqlQuery += values;
            SqlCommand command = new SqlCommand(sqlQuery, sql);
            command.ExecuteNonQuery();

            sql.Close();
            MessageBox.Show("Correct registration", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        } // registration new account

        private bool CheckingEmptyString()
        {
            bool correct = true;
            if (string.IsNullOrEmpty(Login.Text))
            {
                LoginFailed.Text = "Login is empty";
                correct = false;
            }

            if (string.IsNullOrEmpty(Password.Text))
            {
                PasswordFailed.Text = "Password is empty";
                correct = false;
            }

            if (string.IsNullOrEmpty(RepeatPassword.Text))
            {
                RepeatPasswordFailed.Text = "Password repeat is empty";
                correct = false;
            }

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

        #region Parameters Check
        private bool LoginCheck(SqlConnection sql)
        {
            if (Login.Text.Length < 4)
            {
                LoginFailed.Text = "Login is too short";
                return false;
            }

            if (Login.Text.Length > 20)
            {
                LoginFailed.Text = "Login is too long";
                return false;
            }

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
            if (Password.Text.Length < 6)
            {
                PasswordFailed.Text = "Password is too short";
                return false;
            }

            if (PasswordFailed.Text.Length > 20)
            {
                PasswordFailed.Text = "Password is to long";
                return false;
            }

            return true;

        } // Checking password (correct length)
        private bool RepeatPasswordCheck()
        {
            if (RepeatPassword.Text.Trim() != Password.Text.Trim())
            {
                RepeatPasswordFailed.Text = "Passwords are not the same";
                return false;
            }
            return true;
        } // checkiong 2 passwords whether they are same
        private bool EmailCheck(SqlConnection sql)
        {
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
        }

        private void EmailChanged(object sender, RoutedEventArgs e)
        {
            EmailFailed.Text = string.Empty;
        }            

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordFailed.Text = string.Empty;
        }

        private void RepeatPasswordChanged(object sender, RoutedEventArgs e)
        {
            RepeatPasswordFailed.Text = string.Empty;
        }
        #endregion


    }
}
