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


        



        private int delay;
        private string selectedFilePath;

        private CancellationTokenSource? cancellationTokenSource;
        private bool isWorking = false;

     

        public ExternalSortWindow()
        {
            InitializeComponent();

            algorithms!["Прямая сотртировка"] = new StraightMergeSort();
            algorithms!["Натуральная сортировка"] = new NaturalMergeSort();
            //algorithms!["Сортировка слиянием"] = new MergeSort();

            AlgSelector.ItemsSource = algorithms.Keys;
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
               
            }
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

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            string? algorithmName = AlgSelector.SelectedItem.ToString();
            IExternalSorting? algorithm = algorithms![algorithmName!];

            if (isWorking)
            {
                await Log($"{algorithm.GetType().Name} прервана пользователем.");

                ClearAllColumns();
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
                    ClearAllColumns();

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
            
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedFile.Text = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog();

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
                        AttrSelector.ItemsSource = null;
                        columns.Clear();
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

        

        private void AddValueToColumn(string columnName, string value, Color color)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                Label l = new Label();
                l.Content = value;
                l.Foreground = new SolidColorBrush(color);
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

        private void AddValueToColumn(string columnName, string value)
        {
            AddValueToColumn(columnName, value, Colors.Black);
        }

        private void ClearAllColumns()
        {
            ColumnA.Children.Clear();
            ColumnB.Children.Clear();
            ColumnC.Children.Clear();
            ColumnD.Children.Clear();
        }
    }
}
