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

namespace work.Pages
{
    /// <summary>
    /// Interaction logic for FuelStorageWindow.xaml
    /// </summary>
    public partial class AddFuelWindow : Window
    {


        public AddFuelWindow(string adminEmail)
        {
            InitializeComponent();
            EmailTextBlock.Text = adminEmail;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var monitoring = new MonitoringWindow(EmailTextBlock.Text);
            this.Close();
            monitoring.Show();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}
