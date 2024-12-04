using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AlgorithmsVisualisation.SortingAlgorithms
{
    public class TextSorter
    {
        private List<string> words;

        public TextSorter(string filePath)
        {
            LoadTextFromFile(filePath);
            CleanWords(); 
        }

        private void LoadTextFromFile(string filePath)
        {
            try
            {
                string text = File.ReadAllText(filePath);
                words = new List<string>(text.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка чтения файла: {ex.Message}");
                words = new List<string>();
            }
        }

        private void CleanWords()
        {
            for (int i = 0; i < words.Count; i++)
            {
                words[i] = CleanWord(words[i]);
            }
            words = words.Where(word => !string.IsNullOrEmpty(word)).ToList();
        }

        private string CleanWord(string word)
        {
            var cleanedWord = new StringBuilder();
            foreach (char c in word)
            {
                if (char.IsLetter(c) || char.IsDigit(c) || (c >= 'а' && c <= 'я') || (c >= 'А' && c <= 'Я'))
                {
                    cleanedWord.Append(c);
                }
            }
            return cleanedWord.ToString();
        }

        public void MergeSort()
        {
            MergeSort(words, 0, words.Count - 1);
        }

        private void MergeSort(List<string> words, int left, int right)
        {
            if (left < right)
            {
                int mid = (left + right) / 2;

                MergeSort(words, left, mid);
                MergeSort(words, mid + 1, right);
                Merge(words, left, mid, right);
            }
        }

        private void Merge(List<string> words, int left, int mid, int right)
        {
            int n1 = mid - left + 1;
            int n2 = right - mid;

            List<string> leftHalf = new List<string>(n1);
            List<string> rightHalf = new List<string>(n2);

            for (int i = 0; i < n1; i++)
                leftHalf.Add(words[left + i]);
            for (int j = 0; j < n2; j++)
                rightHalf.Add(words[mid + 1 + j]);

            int k = left, iIndex = 0, jIndex = 0;

            while (iIndex < n1 && jIndex < n2)
            {
                if (leftHalf[iIndex].CompareTo(rightHalf[jIndex]) <= 0)
                {
                    words[k] = leftHalf[iIndex];
                    iIndex++;
                }
                else
                {
                    words[k] = rightHalf[jIndex];
                    jIndex++;
                }
                k++;
            }

            while (iIndex < n1)
            {
                words[k] = leftHalf[iIndex];
                iIndex++;
                k++;
            }

            while (jIndex < n2)
            {
                words[k] = rightHalf[jIndex];
                jIndex++;
                k++;
            }
        }

        public void ABCSort()
        {
            words = ABCSort(words);
        }

        private List<string> ABCSort(List<string> inputCollection)
        {
            // Базовый случай - если коллекция пуста, возвращаем
            if (inputCollection.Count == 0)
                return new List<string>();

            // Создаем словарь для хранения слов и их рангов
            Dictionary<string, int> table = new Dictionary<string, int>();

            // Заполняем словарь словами и их рангами
            foreach (var str in inputCollection)
            {
                if (str.Length > 0)
                {
                    if (table.ContainsKey(str))
                    {
                        table[str]++;
                    }
                    else
                    {
                        table.Add(str, 1);
                    }
                }
            }

            // Сортируем словарь, сначала цифры, затем буквы
            var sortedTable = table.OrderBy(x =>
            {
                if (IsNumber(x.Key))
                {
                    return new Tuple<int, string>(0, x.Key);
                }
                else
                {
                    return new Tuple<int, string>(1, x.Key);
                }
            }, Comparer<Tuple<int, string>>.Create((a, b) => a.Item1.CompareTo(b.Item1) == 0 ? a.Item2.CompareTo(b.Item2) : a.Item1.CompareTo(b.Item1)));

            // Заполняем отсортированный список
            List<string> sortedWords = new List<string>();
            foreach (var item in sortedTable)
            {
                for (int i = 0; i < item.Value; i++)
                {
                    sortedWords.Add(item.Key);
                }
            }

            // Рекурсивно сортируем оставшиеся элементы
            List<string> remainingWords = inputCollection.Except(sortedWords).ToList();
            sortedWords.AddRange(ABCSort(remainingWords));

            return sortedWords;
        }

        private bool IsNumber(string str)
        {
            return int.TryParse(str, out _);
        }


        
        public List<string> Words => new List<string>(words);
    }
}
