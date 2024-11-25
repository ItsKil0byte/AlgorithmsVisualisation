using System;
using System.Diagnostics; // Для использования Stopwatch
using System.Linq; // Для использования LINQ
using System.Windows;
using System.Windows.Controls;
using AlgorithmsVisualisation.SortingAlgorithms;

namespace AlgorithmsVisualisation.Windows
{
    public partial class WordSortWindow : Window
    {
        private TextSorter textSorter;
        private Stopwatch stopwatch;

        public WordSortWindow()
        {
            InitializeComponent();
            stopwatch = new Stopwatch(); 
        }

        private void LoadFile_Click(object sender, RoutedEventArgs e)
        {
            string filePath = FilePathTextBox.Text;

            if (string.IsNullOrWhiteSpace(filePath))
            {
                MessageBox.Show("Пожалуйста, введите путь к файлу.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                textSorter = new TextSorter(filePath);
                MessageBox.Show("Файл успешно загружен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки файла: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            if (textSorter == null)
            {
                MessageBox.Show("Сначала загрузите файл.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                stopwatch.Restart();

                string sortMethod = string.Empty;

                if (SortMethodComboBox.SelectedItem is ComboBoxItem selectedItem)
                {
                    if (selectedItem.Content.ToString() == "Merge Sort")
                    {
                        textSorter.MergeSort();
                        sortMethod = "Результат Merge Sort:\n";
                    }
                    else if (selectedItem.Content.ToString() == "ABC Sort")
                    {
                        textSorter.ABCSort();
                        sortMethod = "Результат ABC Sort:\n";
                    }

                    ResultTextBox.Clear();
                    ResultTextBox.AppendText(sortMethod + Environment.NewLine); 
                    
                    var filteredWords = textSorter.Words
                        .Where(word => !string.IsNullOrWhiteSpace(word)) 
                        .ToList();

                    foreach (var word in filteredWords)
                    {
                        ResultTextBox.AppendText(word + Environment.NewLine);
                    }
                    
                    UpdateWordStatistics(filteredWords);

                    stopwatch.Stop();
                    TimeSpan timeTaken = stopwatch.Elapsed; 
                    TimeTakenTextBlock.Text = $"Время выполнения: {timeTaken.TotalSeconds:F2} секунд"; 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сортировки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateWordStatistics(System.Collections.Generic.List<string> words)
        {
            var totalWords = words.Count;
            var uniqueWords = words.Distinct().Count();

            TotalWordsTextBlock.Text = $"Общее количество слов: {totalWords}";
            UniqueWordsTextBlock.Text = $"Количество уникальных слов: {uniqueWords}";
        }

        private void FontSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ResultTextBox != null)
            {
                ResultTextBox.FontSize = e.NewValue;
            }

            if (FontSizeValueTextBlock != null)
            {
                FontSizeValueTextBlock.Text = ((int)e.NewValue).ToString();
            }
        }

        private void FontComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ResultTextBox != null && FontComboBox.SelectedItem is ComboBoxItem selectedFont)
            {
                ResultTextBox.FontFamily = new System.Windows.Media.FontFamily(selectedFont.Content.ToString());
            }
        }
    }
}
