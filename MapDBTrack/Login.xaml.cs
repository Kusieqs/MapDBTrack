using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.SqlClient;

namespace MapDBTrack
{
    public partial class Login : Window
    {
        private int id; // od of employee
        private string login; // login of employee
        public Registration registration; // registration window

        public Login()
        {
            InitializeComponent();
        }
        private void SignIn(object sender, RoutedEventArgs e)
        {
            // checking network connection
            HelpingClass.NetworkCheck(this);

            string login = LoginBox.Text.Trim();
            string password = PasswordBox.Password.Trim();

            // checking empty text in textboxes
            if (!CheckingEmptyText(login, password))
                return;

            // checking login in DB
            if (!CheckingLog(login, password))
                return;

            // opening new window (main window)
            MainWindow mainWindow = new MainWindow(this.id, this.login);
            mainWindow.Show();
            this.Close();

        }// trying to login to account
        private void SignUp(object sender, RoutedEventArgs e)
        {
            // opening new window (registration window)
            registration = new Registration();
            this.Hide();
            registration.Show();
            registration.Closing += (s, args) => this.Show();
        } // opening registration window
        private bool CheckingEmptyText(string login, string password) // if box is empty red text under the box will show
        {
            bool log = true, pass = true;

            //checking empty login
            if (string.IsNullOrEmpty(login))
            {
                log = false;
                LogFailed.Text = "Username not provided";
            }

            //checking empty password
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
            // special queries
            string sqlQueryLog = $"Select login From Employee Where login = @login";
            string sqlQueryPass = $"Select password, login From Employee Where password = @password and login = @login";
            string sqlQueryId = "Select Id, Login From Employee Where login = @login";

            using(SqlConnection sql = new SqlConnection(HelpingClass.connectString))
            {

                sql.Open();
                SqlCommand command = new SqlCommand(sqlQueryLog, sql);

                // adding parameters to query
                command.Parameters.AddWithValue("@login", login);

                // checking about exist of login
                using(SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        LogFailed.Text = "User doesn't exist";
                        return false;
                    }
                }

                // adding parameters to query
                command = new SqlCommand(sqlQueryPass, sql);
                command.Parameters.AddWithValue("@password", password);
                command.Parameters.AddWithValue("@login", login);

                // Checking about exist of password to login
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        PasswordFailed.Text = "Password is not correct";
                        PasswordReminder.Text = "Forget password?";
                        Reset.IsEnabled = true;
                        Reset.IsHitTestVisible = true;
                        return false;
                    }
                }
                // adding parameters to query
                command = new SqlCommand(sqlQueryId, sql);
                command.Parameters.AddWithValue("@login", login);

                //Downwriting iformation about user
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        this.id = reader.GetInt32(0);
                        this.login = reader.GetString(1);
                    }
                }
            }
            return true;
        } // Checking correct of password and login
        private void ResetPassword(object sender, EventArgs e) 
        {
            // checking network conection
            HelpingClass.NetworkCheck(this);

            // Method to send special email to direct email
            HelpingClass.SendingPassword(LoginBox.Text.Trim());
            MessageBox.Show("Password was sent to your email", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        } // resseting password
        private void InfoClick(object sender, RoutedEventArgs e)
        {
            string info = $"Version: {HelpingClass.version}\nContact: kus.konrad1@gmail.com\nLicense: MapDBTrack Commercial Use License";
            MessageBox.Show(info, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        } // info click about version and license
        private void LoginTextChanged(object sender, RoutedEventArgs e)
        {
            LogFailed.Text = string.Empty;
            PasswordFailed.Text = string.Empty;
            Reset.IsEnabled = false;
            PasswordReminder.Text = string.Empty;
        } // disappearing text from textbox
        private void PassTextChanged(object sender, RoutedEventArgs e)
        {
            PasswordFailed.Text = string.Empty;
            Reset.IsEnabled = false;
            PasswordReminder.Text = string.Empty;
        }// disappearing text from passwordbox
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        } // Exit button
        private void BorderClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        } // feature to moving window
    }
}