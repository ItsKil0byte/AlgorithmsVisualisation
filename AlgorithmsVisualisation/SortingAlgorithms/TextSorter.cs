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
            words.Sort((x, y) => string.Compare(x, y, StringComparison.CurrentCultureIgnoreCase));
        }

        public List<string> Words => new List<string>(words);
    }
}
