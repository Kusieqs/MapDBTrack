using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        }

        private void SignUp(object sender, RoutedEventArgs e)
        {
            // checking network connect
            // checking empty string
        }

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
