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
using work.Pdf;

namespace work.Pages
{
    /// <summary>
    /// Interaction logic for ReportWindow.xaml
    /// </summary>
    public partial class ReportWindow : Window
    {
        public ReportWindow(string adminEmail)
        {
            InitializeComponent();
            EmailTextBlock.Text = adminEmail;
        }

        public ReportWindow()
        {
            InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var monitoringWindow = new MonitoringWindow(EmailTextBlock.Text);
            this.Close();
            monitoringWindow.Show();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }

        private async void GenerateButton_Click(object sender, RoutedEventArgs e)
        {
            DateTime? fromDate = FromDatePicker.SelectedDate;
            DateTime? toDate = ToDatePicker.SelectedDate;

            if (fromDate == null || toDate == null)
            {
                MessageBox.Show("Please select both From and To dates.", "Missing Dates", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (fromDate > toDate)
            {
                MessageBox.Show("'From' date cannot be after 'To' date.", "Invalid Dates", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var dbService = new DataBaseService();
                var transactions = (await dbService.GetMonitoringRowsAsync())
                    .Where(t => t.Date >= fromDate && t.Date <= toDate).ToList();

                var allTransactions = (await dbService.GetTransactionsAsync(fromDate.Value, toDate.Value));

                FuelReportPdfGenerator.Generate(allTransactions, EmailTextBlock.Text, fromDate.Value, toDate.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
