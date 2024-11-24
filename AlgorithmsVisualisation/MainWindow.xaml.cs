using AlgorithmsVisualisation.Windows;
using System.Windows;

namespace AlgorithmsVisualisation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InternalSortButton_Click(object sender, RoutedEventArgs e)
        {
            InternalSortWindow window = new();
            window.Show();
        }

        private void ExternalSortButton_Click(object sender, RoutedEventArgs e)
        {
            ExternalSortWindow window = new();
            window.Show();
        }

        private void WordSortButton_Click(object sender, RoutedEventArgs e)
        {
            WordSortWindow window = new();
            window.Show();
        }
    }
}