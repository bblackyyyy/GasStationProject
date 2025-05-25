using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace work
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DataBaseService _dbService;

        public MainWindow()
        {
            InitializeComponent();
            _dbService = new DataBaseService();

            // Wywołaj zapytanie asynchroniczne po załadowaniu okna
            Loaded += async (s, e) => await _dbService.GetDataAsync();
            MessageBox.Show("MainWindow uruchomione!");
        }
    }
}