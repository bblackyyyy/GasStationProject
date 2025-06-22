using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace work.Pages
{
    public partial class LoginWindow : Window
    {
        private readonly DataBaseService _dbService = new DataBaseService();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password;

            bool success = await _dbService.LoginAsync(email, password);

            if (success)
            {
                AdminPanelWindow adminPanel = new AdminPanelWindow(email);
                adminPanel.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid email or password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(PasswordBox.Password)
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EmailPlaceholder.Visibility = string.IsNullOrEmpty(EmailTextBox.Text)
                ? Visibility.Visible
                : Visibility.Hidden;
        }


    }
}