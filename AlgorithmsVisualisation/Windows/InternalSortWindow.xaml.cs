using AlgorithmsVisualisation.SortingAlgorithms;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AlgorithmsVisualisation.Windows
{
    /// <summary>
    /// Логика взаимодействия для InternalSortingWindow.xaml
    /// </summary>
    public partial class InternalSortWindow : Window
    {
        private readonly Dictionary<string, ISorting>? algorithms = [];

        private List<int> array = [];
        private readonly Random random = new();
        private int sampleCount = 10;
        private int delay;

        private CancellationTokenSource? cancellationTokenSource;
        private bool isWorking = false;

        private int highlightIndex1 = -1;
        private int highlightIndex2 = -1;

        public InternalSortWindow()
        {
            InitializeComponent();

            algorithms!["Сортировка пузырьком"] = new BubbleSort();
            // ...

            AlgSelector.ItemsSource = algorithms.Keys;
        }

        private void DrawSamples(Canvas canvas, List<int> array)
        {
            canvas.Children.Clear();
            double barWidth = canvas.ActualWidth / array.Count;

            for (int i = 0; i < array.Count; i++)
            {
                double barHeight = (double)array[i] / array.Max() * canvas.ActualHeight;

                Rectangle rectangle = new()
                {
                    Width = barWidth - 2,
                    Height = barHeight,
                    Fill = (i == highlightIndex1 || i == highlightIndex2) ? Brushes.Red : Brushes.Black,
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

        private async Task RunSort(ISorting algorithm)
        {
            try
            {
                await algorithm.Sort(
                    AlgCanvas,
                    array,
                    cancellationTokenSource!.Token,
                    async () =>
                    {
                        DrawSamples(AlgCanvas, array);
                        await DynamicDelay(cancellationTokenSource.Token);
                    },
                    token => DynamicDelay(token),
                    log => Log(log),
                    message => Dispatcher.Invoke(() => StepsTextBox.Text = message),
                    HighlightColumns
                );
            }
            finally
            {
                ResetColumnColors();
            }
        }

        // Я начинаю терять веру в человечество.
        private void ResetColumnColors()
        {
            highlightIndex1 = -1;
            highlightIndex2 = -1;
            DrawSamples(AlgCanvas, array);
        }

        private void HighlightColumns(int index1, int index2)
        {
            highlightIndex1 = index1;
            highlightIndex2 = index2;

            DrawSamples(AlgCanvas, array);
        }

        private async Task Log(string message)
        {
            Dispatcher.Invoke(() =>
            {
                LogsTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] : {message}\n");
                LogsTextBox.ScrollToEnd();
            });

            string logFile = "logs.txt";
            await File.AppendAllTextAsync(logFile, $"[{DateTime.Now:HH:mm:ss}] : {message}\n");
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
            string? algorithmName = AlgSelector.SelectedItem.ToString();
            ISorting? algorithm = algorithms![algorithmName!];

            if (isWorking)
            {
                await Log($"{algorithm.GetType().Name} прервана пользователем.");

                cancellationTokenSource?.Cancel();
                isWorking = false;
                StartButton.Content = "Запустить";
            }
            else
            {
                cancellationTokenSource = new();
                isWorking = true;
                StartButton.Content = "Остановить";
                bool wasCancelled = false;

                EnableUI(false);

                try
                {
                    await Log($"{algorithm.GetType().Name} запущена для массива из {array.Count} элементов.");

                    await RunSort(algorithm);
                }
                catch (TaskCanceledException)
                {
                    wasCancelled = true;
                }
                finally
                {
                    isWorking = false;
                    StartButton.Content = "Запустить";

                    if (!wasCancelled)
                    {
                        await Log($"{algorithm.GetType().Name} завершилась.");
                    }

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
            delay = (int)(4005 - SpeedSlider.Value); // Над зедержкой ещё надо будет подумать.
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
