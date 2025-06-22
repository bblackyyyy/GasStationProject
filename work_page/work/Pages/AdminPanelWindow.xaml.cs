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
    /// Interaction logic for AdminPanelWindow.xaml
    /// </summary>
    public partial class AdminPanelWindow : Window
    {
        public AdminPanelWindow(string adminEmail)
        {
            InitializeComponent();
            EmailTextBlock.Text = adminEmail;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }

        private void FuelStorageButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Fuel Storage clicked!");
        }

        private void MonitoringButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Monitoring clicked!");
        }
    }
}
