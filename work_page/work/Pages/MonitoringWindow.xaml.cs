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
    public partial class MonitoringWindow : Window
    {
        private readonly work.DataBaseService _dbService = new work.DataBaseService();

        public MonitoringWindow(string adminEmail)
        {
            InitializeComponent();
            EmailTextBlock.Text = adminEmail;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var rows = await _dbService.GetMonitoringRowsAsync();
            MonitoringDataGrid.ItemsSource = rows;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var AdminPanel = new AdminPanelWindow(EmailTextBlock.Text);
            this.Close();
            AdminPanel.Show();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }

        private void DetailButton_Click(object sender, RoutedEventArgs e)
        {
            if (MonitoringDataGrid.SelectedItem is work.MonitoringRow selectedRow)
            {
                if (selectedRow.StationId.HasValue && selectedRow.PumpId.HasValue)
                {
                    var detailsWindow = new DetailsWindow(
                        EmailTextBlock.Text,
                        selectedRow.StationId.Value,
                        selectedRow.PumpId.Value);
                    detailsWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Data Error.");
                }
            }
            else
            {
                MessageBox.Show("Please select a row first.");
            }
        }

        private void ReportButton_Click(object sender, RoutedEventArgs e)
        {
            var reportWindow = new ReportWindow(EmailTextBlock.Text);
            reportWindow.Show();
            this.Close();
        }
    }
}
