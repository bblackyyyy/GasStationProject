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
    /// Interaction logic for DetailsWindow.xaml
    /// </summary>
    public partial class DetailsWindow : Window
    {
        private readonly work.DataBaseService _dbService = new work.DataBaseService();
        private readonly long _stationId;
        private readonly long _pumpId;

        public DetailsWindow(string adminEmail, long stationId, long pumpId)
        {
            InitializeComponent();
            _stationId = stationId;
            _pumpId = pumpId;
            LoadTransactions();
        }

        private async void LoadTransactions()
        {
            var transactions = await _dbService.GetTransactionsByGroupAsync(_stationId, _pumpId);
            if (transactions == null || transactions.Count == 0)
            {
                MessageBox.Show("No transactions found for this group.");
            }
            TransactionsDataGrid.ItemsSource = transactions;
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

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedRadio = new[] {
                SortByTransactionId, SortByDataTime, SortByQuantityLiters,
                SortByMoney, SortByFuelType, SortByStationId, SortByPumpId
            }.FirstOrDefault(rb => rb.IsChecked == true);

            if (selectedRadio == null)
                return;

            string sortProperty = selectedRadio.Tag.ToString();

            var items = TransactionsDataGrid.ItemsSource as IEnumerable<Transaction>;
            if (items == null)
                return;

            var sorted = items.OrderBy(x => x.GetType().GetProperty(sortProperty).GetValue(x, null)).ToList();
            TransactionsDataGrid.ItemsSource = sorted;
        }
    }
}
