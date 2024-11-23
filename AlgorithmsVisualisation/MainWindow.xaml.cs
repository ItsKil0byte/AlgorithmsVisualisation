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
        private int sampleCount = 10;
        private int delay;

        private CancellationTokenSource? cancellationTokenSource;
        private bool isWorking = false;

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

        // NOTE: Это просто заглушечка, но работает нормально.
        // Попытаюсь все алгосы вынести как отдельный класс.
        private async Task BubbleSort(Canvas canvas, List<int> array, CancellationToken token)
        {
            for (int i = 0; i < array.Count; i++)
            {
                for (int j = 0; j <  array.Count - 1 - i; j++)
                {
                    if (array[j] > array[j + 1])
                    {
                        if (token.IsCancellationRequested) return;

                        (array[j], array[j + 1]) = (array[j + 1], array[j]);

                        DrawSamples(canvas, array);
                        await DynamicDelay(token);
                    }
                }
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

        private async Task DynamicDelay(CancellationToken token)
        {
            int delay = this.delay;
            int elapsed = 0;

            while (elapsed < delay)
            {
                if (token.IsCancellationRequested) return;

                await Task.Delay(5, token);
                elapsed += 10;
                delay = this.delay;
            }
        }

        private void EnableUI(bool enable)
        {
            ShuffleButton.IsEnabled = enable;
            SampleSlider.IsEnabled = enable;
            AlgSelector.IsEnabled = enable;
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            Shuffle(array);
            DrawSamples(AlgCanvas, array);
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (isWorking)
            {
                cancellationTokenSource?.Cancel();
                isWorking = false;
                StartButton.Content = "Запустить";
            }
            else
            {
                cancellationTokenSource = new();
                isWorking = true;
                StartButton.Content = "Остановить";

                EnableUI(false);

                try
                {
                    await BubbleSort(AlgCanvas, array, cancellationTokenSource.Token);
                }
                catch (TaskCanceledException)
                {
                    // IGNORE
                }
                finally
                {
                    isWorking = false;
                    StartButton.Content = "Запустить";

                    EnableUI(true);
                    cancellationTokenSource.Dispose();
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            array = Enumerable.Range(1, sampleCount).ToList();
            DrawSamples(AlgCanvas, array);
        }

        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            delay = (int) (4005 - SpeedSlider.Value); // Над зедержкой ещё надо будет подумать.
        }

        private void SampleSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (AlgCanvas.ActualWidth > 0 && AlgCanvas.ActualHeight > 0)
            {
                sampleCount = (int)SampleSlider.Value;
                array = Enumerable.Range(1, sampleCount).ToList();
                DrawSamples(AlgCanvas, array);
            }
        }
    }
}