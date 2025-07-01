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


        private readonly int _inventoryId;
        private readonly double _capacity;

        public AddFuelWindow(string adminEmail, int inventoryId, double capacity)
        {
            InitializeComponent();
            EmailTextBlock.Text = adminEmail;
            _inventoryId = inventoryId;
            _capacity = capacity;
            CapacityTextBox.Text = $"capacity: {_capacity}";
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var fuelStorage = new FuelStorageWindow(EmailTextBlock.Text);
            this.Close();
            fuelStorage.Show();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }

        private void AmountTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AmountPlaceholder.Visibility = string.IsNullOrEmpty(AmountTextBox.Text)
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(AmountTextBox.Text, out double amountToAdd) || amountToAdd <= 0)
            {
                MessageBox.Show("Please enter a valid positive amount.");
                return;
            }

            var dbService = new work.DataBaseService();
            var inventoryList = await dbService.GetInventoryAsync();
            var inventory = inventoryList.FirstOrDefault(i => i.inventory_id == _inventoryId);

            if (inventory == null)
            {
                MessageBox.Show("Pump not found.");
                return;
            }

            
            if (amountToAdd > _capacity)
            {
                MessageBox.Show("Amount exceeds tank capacity!");
                return;
            }

            inventory.available += amountToAdd;
            await dbService.UpdateInventoryAsync(inventory);

            var fuelStorage = new FuelStorageWindow(EmailTextBlock.Text);
            this.Close();
            fuelStorage.Show();
        }
    }
}
