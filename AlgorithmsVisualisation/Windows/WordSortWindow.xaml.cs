using System;
using System.Windows;
using System.Windows.Controls;
using AlgorithmsVisualisation.SortingAlgorithms;
using Microsoft.Win32;

namespace AlgorithmsVisualisation.Windows
{
    public partial class WordSortWindow : Window
    {
        private TextSorter textSorter;

        public WordSortWindow()
        {
            InitializeComponent();
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
                if (SortMethodComboBox.SelectedItem is ComboBoxItem selectedItem)
                {
                    if (selectedItem.Content.ToString() == "Merge Sort")
                    {
                        textSorter.MergeSort();
                    }
                    else if (selectedItem.Content.ToString() == "ABC Sort")
                    {
                        textSorter.ABCSort();
                    }

                    ResultTextBox.Clear();
                    foreach (var word in textSorter.Words)
                    {
                        ResultTextBox.AppendText(word + Environment.NewLine);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка сортировки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
        
        private void FontComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ResultTextBox != null && FontComboBox.SelectedItem is ComboBoxItem selectedFont)
            {
                ResultTextBox.FontFamily = new System.Windows.Media.FontFamily(selectedFont.Content.ToString());
            }
        }

    }
}
