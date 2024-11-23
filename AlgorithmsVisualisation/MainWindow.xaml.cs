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

namespace AlgorithmsVisualisation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<int> array = [];
        private readonly Random random = new();
        private readonly int sampleCount = 150;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DrawSamples(Canvas canvas, List<int> array)
        {
            canvas.Children.Clear();
            double barWidth = canvas.ActualWidth / array.Count;

            for (int i = 0; i < array.Count; i++)
            {
                double barHeight = (double) array[i] / array.Max() * canvas.ActualHeight;
                
                Rectangle rectangle = new()
                {
                    Width = barWidth - 2,
                    Height = barHeight,
                    Fill = Brushes.Black,
                };

                Canvas.SetLeft(rectangle, i * barWidth);
                Canvas.SetTop(rectangle, canvas.ActualHeight - barHeight);

                canvas.Children.Add(rectangle);
            }
        }

        private void Shuffle(List<int> array)
        {
            for (int i = array.Count - 1; i > 0; i--)
            {
                int swapIndex = random.Next(i + 1);
                (array[i], array[swapIndex]) = (array[swapIndex], array[i]);
            }
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            Shuffle(array);
            DrawSamples(AlgCanvas, array);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            array = Enumerable.Range(1, sampleCount).ToList();
            DrawSamples(AlgCanvas, array);
        }
    }
}