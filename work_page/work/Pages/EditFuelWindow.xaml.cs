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
    public partial class EditFuelWindow : Window
    {
        private readonly work.DataBaseService _dbService = new work.DataBaseService();
        private readonly string _petrolName;
        private int _petrolId;

        public EditFuelWindow(string adminEmail, string petrolName)
        {
            InitializeComponent();
            EmailTextBlock.Text = adminEmail;
            _petrolName = petrolName;
            Loaded += EditFuelWindow_Loaded;
        }
        
        private void PriceTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            PricePlaceholder.Visibility = string.IsNullOrEmpty(PriceTextBox.Text)
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        private void TaxTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TaxPlaceholder.Visibility = string.IsNullOrEmpty(TaxTextBox.Text)
                ? Visibility.Visible
                : Visibility.Hidden;
        }
        

        private async void EditFuelWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var petrolTypes = await _dbService.GetPetrolTypesAsync();
            var petrol = petrolTypes.FirstOrDefault(pt => pt.petrol_name == _petrolName);
            if (petrol != null)
            {
                _petrolId = petrol.id;
                PriceTextBox.Text = petrol.price_per_liter.ToString();
                TaxTextBox.Text = petrol.tax.ToString();
            }
        }


        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(PriceTextBox.Text, out double price) ||
                !double.TryParse(TaxTextBox.Text, out double tax))
            {
                MessageBox.Show("Please enter valid numbers for price and tax.");
                return;
            }

            var petrolType = new PetrolType
            {
                id = _petrolId,
                petrol_name = _petrolName,
                price_per_liter = price,
                tax = tax
            };

            await _dbService.UpdatePetrolTypeAsync(petrolType);

            var fuelStorage = new FuelStorageWindow(EmailTextBlock.Text);
            this.Close();
            fuelStorage.Show();
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
    }
}
