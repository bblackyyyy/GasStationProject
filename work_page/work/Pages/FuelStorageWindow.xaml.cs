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
    public partial class FuelStorageWindow : Window
    {


        private readonly work.DataBaseService _dbService = new work.DataBaseService();

        public FuelStorageWindow(string adminEmail)
        {
            InitializeComponent();
            EmailTextBlock.Text = adminEmail;
            Loaded += FuelStorageWindow_Loaded;
        }

        private async void FuelStorageWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadFuelDataAsync();
        }

        private async Task LoadFuelDataAsync()
        {
            try
            {
                var inventoryList = await _dbService.GetInventoryAsync();
                var petrolTypeList = await _dbService.GetPetrolTypesAsync();

                var data = from inv in inventoryList
                           join pt in petrolTypeList on inv.petrol_name equals pt.petrol_name
                           select new FuelStorageRow
                           {
                               FuelType = inv.petrol_name,
                               Amount = inv.available,
                               Capacity = inv.max,
                               Price = pt.price_per_liter,
                               TankNumber = inv.inventory_id
                           };

                FuelDataGrid.ItemsSource = data.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load fuel data: " + ex.Message);
            }
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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedRow = FuelDataGrid.SelectedItem as FuelStorageRow;
            if (selectedRow != null)
            {
                var addFuel = new AddFuelWindow(EmailTextBlock.Text, selectedRow.TankNumber,(selectedRow.Capacity - selectedRow.Amount));
                this.Close();
                addFuel.Show();
            }
            else
            {
                MessageBox.Show("Please select a pump first.");
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedRow = FuelDataGrid.SelectedItem as FuelStorageRow;
            if (selectedRow != null)
            {
                var editFuel = new EditFuelWindow(EmailTextBlock.Text, selectedRow.FuelType);
                this.Close();
                editFuel.Show();
            }
            else
            {
                MessageBox.Show("Please select a pump first.");
            }
        }

        
    }

    public class FuelStorageRow
    {
        public string FuelType { get; set; }
        public double Amount { get; set; }
        public double Capacity { get; set; }
        public double Price { get; set; }
        public int TankNumber { get; set; }
    }
}
