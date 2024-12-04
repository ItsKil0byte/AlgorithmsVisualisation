using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace AlgorithmsVisualisation.SortingAlgorithms
{
    public class MultiWayMergeSort : IExternalSorting
    {
        private string? _headers;
        private int _chosenField;
        private long _iterations, _segments;

        Dictionary<long, Color> colorDictionary = new Dictionary<long, Color>
        {
            { 1, Color.FromArgb(255, 255, 0, 0) },  // Красный
            { 2, Color.FromArgb(255, 200, 0, 0) },  // Темнее красного
            { 3, Color.FromArgb(255, 150, 0, 0) },  // Ещё темнее

            { 4, Color.FromArgb(255, 0, 255, 0) },  // Зеленый
            { 5, Color.FromArgb(255, 0, 200, 0) },  // Темнее зеленого
            { 6, Color.FromArgb(255, 0, 150, 0) },  // Ещё темнее

            { 7, Color.FromArgb(255, 0, 0, 255) },  // Синий
            { 8, Color.FromArgb(255, 0, 0, 200) },  // Темнее синего
            { 9, Color.FromArgb(255, 0, 0, 150) },  // Ещё темнее

            { 10, Color.FromArgb(255, 255, 165, 0) },  // Оранжевый
            { 11, Color.FromArgb(255, 255, 140, 0) },  // Темнее оранжевого
            { 12, Color.FromArgb(255, 255, 115, 0) },  // Ещё темнее

            { 13, Color.FromArgb(255, 128, 0, 128) },  // Фиолетовый
            { 14, Color.FromArgb(255, 102, 0, 102) },  // Темнее фиолетового
            { 15, Color.FromArgb(255, 76, 0, 76) }     // Ещё темнее
        };

        private static int GetNextFileIndexToWrite(int currentIndex)
        => currentIndex switch
        {
            0 => 1,
            1 => 2,
            2 => 0,
            _ => throw new Exception("Что-то вышло из под контроля. Будем разбираться")
        };
        private bool CheckElement(string? element, int counter)
        => element is null || counter == _iterations;

        private static int GetMinInt(IReadOnlyList<int> elements)
        {
            if (elements.Count == 1)
            {
                return 0;
            }

            var min = elements[0];
            var minIndex = 0;
            for (var i = 1; i < elements.Count; i++)
            {
                if (elements[i] > min)
                {
                    continue;
                }

                min = elements[i];
                minIndex = i;
            }

            return minIndex;
        }
        private int GetMinString(IReadOnlyList<string> elements)
        {
            if (elements.Count == 1)
            {
                return 0;
            }

            var min = elements[0].Split(';')[_chosenField];
            var minIndex = 0;
            for (var i = 1; i < elements.Count; i++)
            {
                if (string.Compare(elements[i].Split(';')[_chosenField], min, StringComparison.Ordinal) > 0)
                {
                    continue;
                }

                min = elements[i].Split(';')[_chosenField];
                minIndex = i;
            }

            return minIndex;
        }
        private int GetMinOfElements(params string?[] elements)
        {
            if (elements.Contains(null))
            {
                switch (elements.Length)
                {
                    case 2:
                        return elements[0] is null ? 1 : 0;
                    case 3 when elements[0] is null && elements[1] is null:
                        return 2;
                    case 3 when elements[0] is null && elements[2] is null:
                        return 1;
                    case 3 when elements[1] is null && elements[2] is null:
                        return 0;
                }
            }

            int i;
            if (int.TryParse(elements[0].Split(";")[_chosenField], out i))
            {
                return GetMinInt(elements
                .Select(s => s is null ? int.MaxValue : int.Parse(s.Split(';')[_chosenField]))
                .ToArray());
            }
            else
            {
                return GetMinString(elements!);
            }


        }

        public async Task Sort(string inputFilePath, int key, CancellationToken token, Action<string, string, Color> addToColumn, Action<string, string> removeFromColumn, Func<CancellationToken, Task> onDynamicDelay, Action<string> onLog, Action<string> onExplain)
        {
            this._chosenField = key;
            _iterations = 1;
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
                _segments = 1;
                using var fileA = new StreamReader(inputFilePath);
                _headers = fileA.ReadLine();

                using var fileB = new StreamWriter("B.csv");
                using var fileC = new StreamWriter("C.csv");
                using var fileD = new StreamWriter("D.csv");
                string? currentRecord = fileA.ReadLine();
                //переменная flag поменяла свой тип с bool на int, т.к. теперь нам нужно больше
                //двух значений
                int flag = 0;
                int counter = 0;
                while (currentRecord is not null)
                {
                    if (counter == _iterations)
                    {
                        //Случай, когда мы дошли до конца цепочки
                        counter = 0;
                        flag = GetNextFileIndexToWrite(flag);
                        _segments++;
                    }

                    switch (flag)
                    {
                        case 0:
                            fileB.WriteLine(currentRecord);
                            onExplain.Invoke($"Раскладывем A в отсальные столбцы, последовательностями по {_iterations} элементов");
                            onLog("Записали из A в B");
                            removeFromColumn("A", currentRecord);
                            addToColumn("B", currentRecord, colorDictionary[_segments]);
                            await onDynamicDelay(token);
                            
                            break;
                        case 1:
                            fileC.WriteLine(currentRecord);
                            onExplain.Invoke($"Раскладывем A в отсальные столбцы, последовательностями по {_iterations} элементов");
                            onLog("Записали из A в C");
                            removeFromColumn("A", currentRecord);
                            addToColumn("C", currentRecord, colorDictionary[_segments]);
                            await onDynamicDelay(token);
                            
                            break;
                        case 2:
                            fileD.WriteLine(currentRecord);
                            onExplain.Invoke($"Раскладывем A в отсальные столбцы, последовательностями по {_iterations} элементов");
                            onLog("Записали из A в D");
                            removeFromColumn("A", currentRecord);
                            addToColumn("D", currentRecord, colorDictionary[_segments]);
                            await onDynamicDelay(token);
                            
                            break;
                    }

                    currentRecord = fileA.ReadLine();
                    counter++;
                }

                if (_segments == 1)
                {
                    break;
                }

                fileA.Close();
                fileB.Close();
                fileC.Close();
                fileD.Close();


                using var writerA = new StreamWriter(inputFilePath);

                using var readerB = new StreamReader("B.csv");
                using var readerC = new StreamReader("C.csv");
                using var readerD = new StreamReader("D.csv");

                writerA.WriteLine(_headers);

                string? elementB = readerB.ReadLine();
                string? elementC = readerC.ReadLine();
                string? elementD = readerD.ReadLine();

                int counterB = 0;
                int counterC = 0;
                int counterD = 0;
                while (elementB is not null || elementC is not null || elementD is not null)
                {
                    string? currentRecord2;
                    int flag2;

                    if (CheckElement(elementB, counterB) && !CheckElement(elementC, counterC) && !CheckElement(elementD, counterD))
                    {
                        //Случай, когда цепочка закончилась только в файле B
                        (currentRecord2, flag2) = GetMinOfElements(
                                elementC,
                                elementD) switch
                        {
                            0 => (elementC, 1),
                            1 => (elementD, 2)
                        };
                    }
                    else if (CheckElement(elementC, counterC) && !CheckElement(elementB, counterB) && !CheckElement(elementD, counterD))
                    {
                        //Случай, когда цепочка закончилась только в файле С
                        (currentRecord2, flag2) = GetMinOfElements(
                                elementB,
                                elementD) switch
                        {
                            0 => (elementB, 0),
                            1 => (elementD, 2)
                        };
                    }
                    else if (CheckElement(elementD, counterD) && !CheckElement(elementB, counterB) && !CheckElement(elementC, counterC))
                    {
                        //Случай, когда цепочка закончилась только в файле D
                        (currentRecord2, flag2) = GetMinOfElements(
                                elementB,
                                elementC) switch
                        {
                            0 => (elementB, 0),
                            1 => (elementC, 1)
                        };
                    }
                    else if (counterB == _iterations && counterC == _iterations)
                    {
                        //Случай, когда цепочки закончились в файлах В и С
                        currentRecord2 = elementD;
                        flag2 = 2;
                    }
                    else if (counterB == _iterations && counterD == _iterations)
                    {
                        //Случай, когда цепочки закончились в файлах В и D
                        currentRecord2 = elementC;
                        flag2 = 1;
                    }
                    else if (counterC == _iterations && counterD == _iterations)
                    {
                        //Случай, когда цепочки закончились в файлах C и D
                        currentRecord2 = elementB;
                        flag2 = 0;
                    }
                    else
                    {
                        //Случай, когда не закончилась ни одна из 3 цепочек
                        (currentRecord2, flag2) = GetMinOfElements(
                                elementB,
                                elementC,
                                elementD) switch
                        {
                            0 => (elementB, 0),
                            1 => (elementC, 1),
                            2 => (elementD, 2)
                        };
                    }

                    switch (flag2)
                    {
                        case 0:
                            writerA.WriteLine(currentRecord2);
                            elementB = readerB.ReadLine();
                            counterB++;
                            onLog("Записали элемент из B в A");
                            onExplain.Invoke($"Смотрим на наши последовательности, минимальное это {currentRecord2.Split(";")[_chosenField]}, записываем его в A");
                            removeFromColumn("B", currentRecord2);
                            addToColumn("A", currentRecord2, Colors.Black);
                            await onDynamicDelay(token);
                            
                            break;
                        case 1:
                            writerA.WriteLine(currentRecord2);
                            elementC = readerC.ReadLine();
                            counterC++;
                            onLog("Записали элемент из C в A");
                            onExplain.Invoke($"Смотрим на наши последовательности, минимальное это {currentRecord2.Split(";")[_chosenField]}, записываем его в A");
                            removeFromColumn("C", currentRecord2);
                            addToColumn("A", currentRecord2, Colors.Black);
                            await onDynamicDelay(token);
                            
                            break;
                        case 2:
                            writerA.WriteLine(currentRecord2);
                            elementD = readerD.ReadLine();
                            counterD++;
                            onLog("Записали элемент из D в A");
                            onExplain.Invoke($"Смотрим на наши последовательности, минимальное это {currentRecord2.Split(";")[_chosenField]}, записываем его в A");
                            removeFromColumn("D", currentRecord2);
                            addToColumn("A", currentRecord2, Colors.Black);
                            await onDynamicDelay(token);
                            
                            break;
                    }

                    if (counterB != _iterations || counterC != _iterations || counterD != _iterations)
                    {
                        continue;
                    }

                    //Обнуляем все 3 счётчика, если достигли конца всех цепочек во всех файлах
                    counterC = 0;
                    counterB = 0;
                    counterD = 0;
                }

                _iterations *= 3;
            }
            onExplain.Invoke("Осталась одна отсортированная последовательность, это результат");

            using (var reader = new StreamReader("B.csv"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    removeFromColumn("B", line);
                    addToColumn("A", line, Colors.Black);
                }
            }
            
            await onDynamicDelay(token);
        }
    }
}
