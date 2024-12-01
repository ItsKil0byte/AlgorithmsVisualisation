using AlgorithmsVisualisation.SortingAlgorithms;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;

namespace AlgorithmsVisualisation.Windows
{
    /// <summary>
    /// Логика взаимодействия для ExternalSortWindow.xaml
    /// </summary>
    public partial class ExternalSortWindow : Window
    {
        private readonly Dictionary<string, IExternalSorting>? algorithms = [];
        private readonly Dictionary<string, int>? columns = [];
        private readonly Dictionary<string, Label>? lables = [];
        private List<int> array = [];
        private readonly Random random = new();
        private int sampleCount = 10;
        private int delay;
        private string selectedFilePath;

        private CancellationTokenSource? cancellationTokenSource;
        private bool isWorking = false;

        string outputFile = "C:\\Users\\zxcursedfan\\Desktop\\otec\\output.csv";

        public ExternalSortWindow()
        {
            InitializeComponent();

            algorithms!["Прямая сотртировка"] = new StraightMergeSort();
            //algorithms!["Сортировка слиянием"] = new MergeSort();

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
                    //Fill = (i == highlightIndex1 || i == highlightIndex2) ? Brushes.Red : Brushes.Black,
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

        private async Task RunSort(IExternalSorting algorithm)
        {
            try
            {
                await algorithm.Sort(
                    SelectedFile.Text,
                    columns[AttrSelector.SelectedItem.ToString()],
                    cancellationTokenSource!.Token,
                    AddValueToColumn,
                    DeleteValueFromColumn,
                    token => DynamicDelay(token),
                    log => Log(log),
                    message => Dispatcher.Invoke(() => StepsTextBox.Text = message)
                );
            }
            finally
            {
                ResetColumnColors();
            }
        }

        private void ResetColumnColors()
        {
            //highlightIndex1 = -1;
            //highlightIndex2 = -1;
            //DrawSamples(AlgCanvas, array);
        }

        private void HighlightColumns(int index1, int index2)
        {
            //highlightIndex1 = index1;
            //highlightIndex2 = index2;

            //DrawSamples(AlgCanvas, array);
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
            SelectFileButton.IsEnabled = enable;
            AlgSelector.IsEnabled = enable;
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            Shuffle(array);
            //DrawSamples(AlgCanvas, array);
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            string? algorithmName = AlgSelector.SelectedItem.ToString();
            IExternalSorting? algorithm = algorithms![algorithmName!];

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
                    await Log($"{algorithm.GetType().Name} запущена"); //TODO

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
            //DrawSamples(AlgCanvas, array);
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            SelectedFile.Text = string.Empty;
            openFileDialog.Title = "Выберите файл";
            openFileDialog.Filter = "Текстовые файлы (*.csv)|*.csv";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == true)
            {
                SelectedFile.Text = openFileDialog.FileName;
                SelectedFile_LostFocus(new object(), new RoutedEventArgs());
            }
        }

        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            delay = (int)(4005 - SpeedSlider.Value);
        }

        private void SelectedFile_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                using (TextReader reader = File.OpenText(SelectedFile.Text))
                {
                    try
                    {
                        int i = 0;
                        foreach (var columnName in reader.ReadLine()!.Split(";"))
                        {
                            columns.Add(columnName, i); 
                            i++;
                        }
                    }
                    catch { }
                    AttrSelector.ItemsSource = columns.Keys;
                }
            }
            catch { }
        }

        private void DeleteValueFromColumn(string columnName, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
               
                switch (columnName)
                {
                    case "A":
                        ColumnA.Children.Remove(lables[value]);
                        break;
                    case "B":
                        ColumnB.Children.Remove(lables[value]);
                        break;
                    case "C":
                        ColumnC.Children.Remove(lables[value]);
                        break;
                    case "D":
                        ColumnD.Children.Remove(lables[value]);
                        break;
                }
            }
        }

        private void AddValueToColumn(string columnName, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                Label l = new Label();
                l.Content = value;
                lables[value] = l;
                switch (columnName) 
                {
                    case "A":
                        ColumnA.Children.Add(l);
                        break;
                    case "B":
                        ColumnB.Children.Add(l);
                        break;
                    case "C":
                        ColumnC.Children.Add(l);
                        break;
                    case "D":
                        ColumnD.Children.Add(l);
                        break;
                }
            }
        }
    }
}
