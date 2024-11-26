using System;
using System.Diagnostics; 
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using AlgorithmsVisualisation.SortingAlgorithms;

namespace AlgorithmsVisualisation.Windows
{
    public partial class WordSortWindow : Window
    {
        private TextSorter textSorter = null!; // Используйте `null!`, если вы уверены, что инициализируете позже
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

                    var filteredWords = textSorter.Words
                        .Where(word => !string.IsNullOrWhiteSpace(word)) 
                        .ToList();

                    var resultText = new StringBuilder();
                    resultText.Append(sortMethod + Environment.NewLine); 

                    foreach (var word in filteredWords)
                    {
                        resultText.Append(word + Environment.NewLine);
                    }

                    ResultTextBox.Clear();
                    ResultTextBox.AppendText(resultText.ToString());

                    UpdateWordStatistics(filteredWords);

                    stopwatch.Stop();
                    TimeSpan timeTaken = stopwatch.Elapsed; 
                    TimeTakenTextBlock.Text = $"Время выполнения: {timeTaken.TotalSeconds:F3} секунд"; 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сортировки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateWordStatistics(List<string> words)
        {
            var totalWords = words.Count;
            var uniqueWords = words.Distinct().ToList();

            TotalWordsTextBlock.Text = $"Общее количество слов: {totalWords}";
            UniqueWordsTextBlock.Text = $"Количество уникальных слов: {uniqueWords.Count}";

            // Подсчет повторений уникальных слов
            var wordCounts = uniqueWords.ToDictionary(word => word, word => words.Count(w => w.Equals(word, StringComparison.OrdinalIgnoreCase)));

            // Обновление TextBox с уникальными словами и их количеством
            UniqueWordsTextBox.Clear();
            foreach (var kvp in wordCounts)
            {
                UniqueWordsTextBox.AppendText($"{kvp.Key}: {kvp.Value}\n");
            }
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

        private void FontComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ResultTextBox != null && FontComboBox.SelectedItem is ComboBoxItem selectedFont)
            {
                ResultTextBox.FontFamily = new System.Windows.Media.FontFamily(selectedFont.Content.ToString());
            }
        }
    }
}
