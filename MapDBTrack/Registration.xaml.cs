using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

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

            using(SqlConnection sql = new SqlConnection(HelpingClass.connectString))
            {
                sql.Open();
                // Checking all paramters with corrections
                if (!CheckAllParameters(sql))
                    return;

                // special query to sql
                string sqlQuery = $"Insert Into Employee (login, password, email)\nValues ('{Login.Text.Trim()}','{Password.Password.Trim()}','{Email.Text.Trim()}')";
                SqlCommand command = new SqlCommand(sqlQuery, sql);
                command.ExecuteNonQuery();
            }

            //message box about to provide of registation
            MessageBox.Show("Correct registration", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        } // registration new account
        private bool CheckingEmptyString()
        {
            bool correct = true;

            // checking empty login
            if (string.IsNullOrEmpty(Login.Text.Trim()))
            {
                LoginFailed.Text = "Login is empty";
                correct = false;
            }

            // checking empty password
            if (string.IsNullOrEmpty(Password.Password.Trim()))
            {
                PasswordFailed.Text = "Password is empty";
                correct = false;
            }

            // checking empty repeatpassword
            if (string.IsNullOrEmpty(RepeatPassword.Password.Trim()))
            {
                RepeatPasswordFailed.Text = "Password repeat is empty";
                correct = false;
            }

            // checking empty email
            if (string.IsNullOrEmpty(Email.Text.Trim()))
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
        private void BorderClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        } // feature to moving window
        private void InfoClick(object sender, RoutedEventArgs e)
        {
            // MessageBox with informations
            HelpingClass.InformationsAboutProgram();
        } // information button
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        } // Exit button

        #region Parameters Check
        private bool LoginCheck(SqlConnection sql)
        {
            // checking length of login (min 4 characters)
            if (Login.Text.Trim().Length < 4)
            {
                LoginFailed.Text = "Login is too short";
                return false;
            }

            // checking length of login (max 20 characters)
            if (Login.Text.Trim().Length > 20)
            {
                LoginFailed.Text = "Login is too long";
                return false;
            }

            // special query to search same login
            string sqlQuery = $"Select login From Employee Where login = @login";
            SqlCommand command = new SqlCommand(sqlQuery, sql);
            command.Parameters.AddWithValue("@login", Login.Text.Trim());

            using(SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    LoginFailed.Text = "This login is locked";
                    return false;
                }
                return true;
            }

        } // Checking login (new login can not be in db before)
        private bool PasswordCheck()
        {
            // checking length of password (min 6)
            if (Password.Password.Trim().Length < 6)
            {
                PasswordFailed.Text = "Password is too short";
                return false;
            }

            // checking length of password (max 20)
            if (PasswordFailed.Text.Trim().Length > 20)
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
            // special query to search same email in DB
            string sqlQuery = $"Select email From Employee Where email = @email";
            SqlCommand command = new SqlCommand(sqlQuery, sql);
            command.Parameters.AddWithValue("@email", Email.Text.Trim());

            using(SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    EmailFailed.Text = "This email is locked";
                    return false;
                }
            }
            string pattern = @"^[^\.\s][.\w]*@[.\w]+\.[a-zA-Z]{2,4}$"; // special pattern to check email

            // checking email with pattern
            if (!Regex.IsMatch(Email.Text.Trim(), pattern))
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
            RepeatPasswordFailed.Text = string.Empty;
        } // error Password is disappearing     
        private void RepeatPasswordChanged(object sender, RoutedEventArgs e)
        {
            RepeatPasswordFailed.Text = string.Empty;
        } // error repeat password is disappearing     
        #endregion


    }
}
