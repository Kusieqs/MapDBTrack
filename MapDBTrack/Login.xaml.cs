using System.Text;
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

namespace MapDBTrack
{
    public partial class Login : Window
    {
        public Registration Registration;
        public Login()
        {
            InitializeComponent();
        }

        private void SignIn(object sender, RoutedEventArgs e)
        {
            HelpingClass.NetworkCheck(this);

            string login = Signin.Text;
            string password = Signup.Text;

            if (!CheckingEmptyText(login, password))
                return;

            if (!CheckingLog(login, password))
                return;

            // Opening new window
            Close();

        }

        private void SignUp(object sender, RoutedEventArgs e)
        {
            // open new window
        }

        private void LoginChanged(object sender, RoutedEventArgs e)
        {
            if (LoginFailed.Text != "")
                LoginFailed.Text = "";
        } // if login box will change, red text will disappear

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PasswordFailed.Text != "")
                PasswordFailed.Text = "";
        } // if password box will change, red text will disappear

        private void CheckNetwork(object sender, RoutedEventArgs e)
        {
            HelpingClass.NetworkCheck(this);
        } // checking whether the computer is connected to the Internet

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

            string sqlQueryLog = $"Select Username From Users Where Username = @login";
            string sqlQueryPass = $"Select Password, Username From Users Where Password = @password and Username = @login";

            SqlConnection sql = new SqlConnection();
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
                reader.Close();
                sql.Close();
                return false;
            }
            reader.Close();
            sql.Close();
            return true;

        } // Checking correct of password and login
    }
}