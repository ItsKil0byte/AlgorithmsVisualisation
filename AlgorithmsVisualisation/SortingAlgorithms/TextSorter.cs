using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace AlgorithmsVisualisation.SortingAlgorithms
{
    public class TextSorter
    {
        private List<string> words;

        public TextSorter(string filePath)
        {
            LoadTextFromFile(filePath);
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
                Console.WriteLine($"Error reading file: {ex.Message}");
                words = new List<string>();
            }
        }

        private string CleanWord(string word)
        {
            return Regex.Replace(word, @"[^\wа-яА-ЯёЁ0-9]", "");
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
                leftHalf.Add(CleanWord(words[left + i]));
            for (int j = 0; j < n2; j++)
                rightHalf.Add(CleanWord(words[mid + 1 + j]));

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
            var abcWords = words.Select(CleanWord)
                .Where(word => word.Length > 0 && 
                (char.IsLetter(word[0]) || char.IsDigit(word[0]) || 
                (word[0] >= 'А' && word[0] <= 'я'))).ToList();

            abcWords.Sort((x, y) => string.Compare(x, y, StringComparison.CurrentCultureIgnoreCase));

            words = abcWords;
        }

        public List<string> Words => new List<string>(words);
    }
}
