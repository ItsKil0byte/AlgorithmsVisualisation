using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AlgorithmsVisualisation.SortingAlgorithms
{
    public class NaturalMergeSort : IExternalSorting
    {
        
        private string? _headers;
        private string? _filePath;
        private int _chosenField;

        Dictionary<long, Color> colorDictionary = new Dictionary<long, Color>
        {
            { 1, Color.FromArgb(255, 255, 0, 0) },  // Красный
            { 2, Color.FromArgb(255, 200, 0, 0) },  // Темно-красный
            { 3, Color.FromArgb(255, 0, 240, 0) },  // Зеленый
            { 4, Color.FromArgb(255, 0, 190, 0) },  // Темно-зеленый
            { 5, Color.FromArgb(255, 0, 0, 255) },  // Синий
            { 6, Color.FromArgb(255, 0, 0, 200) },  // Темно-синий
            { 7, Color.FromArgb(255, 255, 255, 0) },  // Желтый
            { 8, Color.FromArgb(255, 200, 200, 0) },  // Темно-желтый
            { 9, Color.FromArgb(255, 255, 165, 0) },  // Оранжевый
            { 10, Color.FromArgb(255, 200, 130, 0) },  // Темно-оранжевый
            { 11, Color.FromArgb(255, 128, 0, 128) },  // Пурпурный
            { 12, Color.FromArgb(255, 100, 0, 100) },  // Темно-пурпурный
            { 13, Color.FromArgb(255, 0, 255, 255) },  // Бирюзовый
            { 14, Color.FromArgb(255, 0, 200, 200) },  // Темно-бирюзовый
            { 15, Color.FromArgb(255, 255, 192, 203) },  // Розовый
            { 16, Color.FromArgb(255, 200, 150, 160) },  // Темно-розовый
        };

        private readonly List<int> _series = new List<int>();

        private bool CompareElements(string? element1, string? element2)
        {
            try
            {
                return int.Parse(element1!.Split(';')[_chosenField])
                          .CompareTo(int.Parse(element2!.Split(';')[_chosenField])) <= 0;
            }
            catch { }



            return string.Compare(element1!.Split(';')[_chosenField],
                                  element2!.Split(';')[_chosenField],
                                  StringComparison.Ordinal) <= 0;
        }

        public async Task Sort(string inputFilePath, int key, CancellationToken token, Action<string, string, Color> addToColumn, Action<string, string> removeFromColumn, Func<CancellationToken, Task> onDynamicDelay, Action<string> onLog, Action<string> onExplain)
        {
            _chosenField = key;
            _filePath = inputFilePath;
            using (StreamReader reader = new StreamReader(inputFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    addToColumn("A", line, Colors.Black);
                }
            }
            while (true)
            {
                _series.Clear();
                

                using var fileA = new StreamReader(_filePath);
                _headers = fileA.ReadLine();



                using var fileB = new StreamWriter("B.csv");
                using var fileC = new StreamWriter("C.csv");
                
                string? firstStr = fileA.ReadLine();
                string? secondStr = fileA.ReadLine();
                bool flag = true;
                int counter = 0;
                while (firstStr is not null)
                {
                    bool tempFlag = flag;
                    if (secondStr is not null)
                    {
                        if (CompareElements(firstStr, secondStr))
                        {
                            onExplain.Invoke($"{firstStr.Split(";")[_chosenField]} <= {secondStr.Split(";")[_chosenField]}, поэтому продолжаем записывать текущую серию");
                            counter++;
                        }
                        else
                        {
                            onExplain.Invoke($"{firstStr.Split(";")[_chosenField]} > {secondStr.Split(";")[_chosenField]}, поэтому заканчиваем записывать серию и переходим к следующей");
                            tempFlag = !tempFlag;
                            _series.Add(counter + 1);
                            counter = 0;
                        }
                    }

                    if (flag)
                    {
                        fileB.WriteLine(firstStr);
                        onLog($"Записали {firstStr} в подфайл B");
                        removeFromColumn("A", firstStr);
                        addToColumn("B", firstStr, colorDictionary[_series.Count+1]);
                        await onDynamicDelay(token);
                    }
                    else
                    {
                        fileC.WriteLine(firstStr);
                        onLog($"Записали {firstStr} в подфайл C");
                        removeFromColumn("A", firstStr);
                        addToColumn("C", firstStr, colorDictionary[_series.Count+1]);
                        await onDynamicDelay(token);
                    }
                    firstStr = secondStr;
                    secondStr = fileA.ReadLine();
                    flag = tempFlag;
                }

                _series.Add(counter + 1);

                fileA.Close();
                fileB.Close();
                fileC.Close();

                if (_series.Count == 1)
                {
                    onExplain("У нас осталась одна последовательность значит сортировка завершена");
                    break;
                }

                using var writerA = new StreamWriter(_filePath);
                using var readerB = new StreamReader("B.csv");
                using var readerC = new StreamReader("C.csv");


                writerA.WriteLine(_headers);

                int indexB = 0;
                int indexC = 1;

                int counterB = 0;
                int counterC = 0;
                string? elementB = readerB.ReadLine();
                string? elementC = readerC.ReadLine(); 
                while (elementB is not null || elementC is not null)
                {
                    if (counterB == _series[indexB] && counterC == _series[indexC])
                    {
                        counterB = 0;
                        counterC = 0;
                        indexB += 2;
                        indexC += 2;
                        continue;
                    }

                    if (indexB == _series.Count || counterB == _series[indexB])
                    {
                        writerA.WriteLine(elementC);
                        onLog($"Записали {elementC} в подфайл A из C");
                        onExplain.Invoke("В серии подфайла B закончились все элементы, поэтому записываем оставшмеся в файл A");
                        removeFromColumn("C", elementC);
                        addToColumn("A", elementC, Colors.Black);
      
                        await onDynamicDelay(token);
                        elementC = readerC.ReadLine();
                        counterC++;
                        continue;
                    }

                    if (indexC == _series.Count || counterC == _series[indexC])
                    {
                        writerA.WriteLine(elementB);
                        onLog($"Записали {elementB} в подфайл A из B");
                        onExplain.Invoke("В серии подфайла C закончились все элементы, поэтому записываем оставшмеся в файл A");
                        removeFromColumn("B", elementB);
                        addToColumn("A", elementB, Colors.Black);
                        await onDynamicDelay(token);
                        elementB = readerB.ReadLine();
                        counterB++;
                        continue;
                    }

                    if (CompareElements(elementB, elementC))
                    {
                        writerA.WriteLine(elementB);
                        removeFromColumn("B", elementB);
                        addToColumn("A", elementB, Colors.Black);
                        onLog($"Перенесли {elementB} в А из В");
                        onExplain.Invoke($"{elementB.Split(";")[_chosenField]} <= {elementC.Split(";")[_chosenField]}, поэтому записываем {elementB} в А");
                        await onDynamicDelay(token);
                        elementB = readerB.ReadLine();
                        counterB++;
                    }
                    else
                    {
                        writerA.WriteLine(elementC);
                        removeFromColumn("C", elementC);
                        addToColumn("A", elementC, Colors.Black);
                        onLog($"Перенесли {elementC} в А из C");
                        onExplain.Invoke($"{elementB.Split(";")[_chosenField]} > {elementC.Split(";")[_chosenField]}, поэтому записываем {elementC} в А");
                        await onDynamicDelay(token);
                        elementC = readerC.ReadLine();
                        counterC++;
                    }
                }
            }
            using (var reader = new StreamReader("B.csv"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    removeFromColumn("B", line);
                    addToColumn("A", line, Colors.Black);
                }
            }
            //))
            await onDynamicDelay(token);
            await onDynamicDelay(token);
            
        }
        


    }
}
