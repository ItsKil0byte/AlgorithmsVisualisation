﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace AlgorithmsVisualisation.SortingAlgorithms
{
    public class StraightMergeSort : IExternalSorting
    {
        string outputFile = "C:\\Users\\zxcursedfan\\Desktop\\otec\\output.csv";

        private string? _headers;
        private long _iterations, _segments;
        private int _chosenField;
        private string _filePath;



        /*private async void SplitToFiles(Action<string> onLog, Action<string> onExplain, Func<CancellationToken, Task> onDynamicDelay, CancellationToken token)
        {
            _segments = 1;
            using var fileA = new StreamReader(_filePath);
            _headers = fileA.ReadLine()!;

            using var fileB = new StreamWriter("C:\\Users\\zxcursedfan\\Desktop\\otec\\B.csv");
            using var fileC = new StreamWriter("C:\\Users\\zxcursedfan\\Desktop\\otec\\C.csv");
            //В этой переменной будет храниться очередная считанная из исходного файла запись
            string? currentRecord = fileA.ReadLine();
            
            bool flag = true;
            int counter = 0;
            //цикл прекратится, когда мы дойдём до конца исходного файла
            while (currentRecord is not null)
            {
                //дошли до конца цепочки, переключаемся на запись новой
                if (counter == _iterations)
                {
                    counter = 0;
                    flag = !flag;
                    _segments++;
                }

                if (flag)
                {
                    //Запись отправляется в подфайл В
                    fileB.WriteLine(currentRecord);
                    onLog($"Положили {currentRecord} в подфайл B");
                    await onDynamicDelay(token);
                }
                else
                {
                    //Запись отправляется в подфайл С
                    fileC.WriteLine(currentRecord);
                    onLog($"Положили {currentRecord} в подфайл C");
                    await onDynamicDelay(token);
                }

                //считываем следующую запись
                currentRecord = fileA.ReadLine();
                counter++;
            }
            return;
        }*/

        /*private async void MergePairs(Action<string> onLog, Action<string> onExplain, Func<CancellationToken, Task> onDynamicDelay, CancellationToken token)
        {
            using var writerA = new StreamWriter(_filePath);
            using var readerB = new StreamReader("C:\\Users\\zxcursedfan\\Desktop\\otec\\B.csv");
            using var readerC = new StreamReader("C:\\Users\\zxcursedfan\\Desktop\\otec\\C.csv");

            //Не забываем вернуть заголовки таблицы на своё место, в начало исходного файла
            writerA.WriteLine(_headers);

            string? elementB = readerB.ReadLine();
            string? elementC = readerC.ReadLine();

            int counterB = 0;
            int counterC = 0;
            //Итерации будут происходить, когда 
            while (elementB is not null || elementC is not null)
            {
                string? currentRecord;
                bool flag = false;

                //Обрабатываем случай, когда закончился весь файл B, или цепочка из данной пары 
                //в нём
                if (elementB is null || counterB == _iterations)
                {
                    onExplain.Invoke("В подфайле B закончились записи, поэтому перекладываем всё оставшееся из C");
                    onLog("Переложили всё оставшееся из C");
                    await onDynamicDelay(token);
                    currentRecord = elementC;
                }
                else if (elementC is null || counterC == _iterations) //аналогично предыдущему блоку if, но для подфайла С
                {
                    currentRecord = elementB;
                    onExplain.Invoke("В подфайле C закончились записи, поэтому перекладываем всё оставшееся из B");
                    onLog("Переложили всё оставшееся из B");
                    await onDynamicDelay(token);
                    flag = true;
                }
                else
                {
                    //Если оба подфайла ещё не закончились, то сравниваем записи по нужному полю
                    if (CompareElements(elementB, elementC))
                    {
                        onExplain.Invoke($"{elementB.Split(';')[_chosenField]} < {elementC.Split(';')[_chosenField]}, значит кладём в A элемент из B");
                        onLog($"Переложили {elementB.Split(';')[_chosenField]} из B в A");
                        await onDynamicDelay(token);
                        //Если запись из файла В оказалась меньше
                        currentRecord = elementB;
                        flag = true;
                    }
                    else
                    {
                        onExplain.Invoke($"{elementC.Split(';')[_chosenField]} <= {elementB.Split(';')[_chosenField]}, значит кладём в A элемент из C");
                        onLog($"Переложили {elementB.Split(';')[_chosenField]} из C в A");
                        await onDynamicDelay(token);
                        //Если запись из файла С оказалась меньше
                        currentRecord = elementC;
                    }
                }

                //Записываем в исходный файл выбранную нами запись
                writerA.WriteLine(currentRecord);

                if (flag)
                {
                    elementB = readerB.ReadLine();
                    counterB++;
                }
                else
                {
                    elementC = readerC.ReadLine();
                    counterC++;
                }

                if (counterB != _iterations || counterC != _iterations)
                {
                    continue;
                }

                //Если серии в обоих файлах закончились, то обнуляем соответствующие счётчики
                counterC = 0;
                counterB = 0;
            }

            _iterations *= 2;
            return;
        }*/

        //Метод сравнения записей по выбранному полю, учитывая его тип данных
        //Вернёт true, если element1 меньше element2
        private bool CompareElements(string? element1, string? element2)
        {
            try
            {
                return int.Parse(element1!.Split(';')[_chosenField])
                          .CompareTo(int.Parse(element2!.Split(';')[_chosenField])) < 0;
            }
            catch {}
            
            

            return string.Compare(element1!.Split(';')[_chosenField],
                                  element2!.Split(';')[_chosenField],
                                  StringComparison.Ordinal) < 0;
        }

        public async Task Sort(string inputFilePath, int sortKeyIndex,CancellationToken token, Action<string, string> addToColumn, Action<string, string> removeFromColumn, Func<CancellationToken, Task> onDynamicDelay, Action<string> onLog, Action<string> onExplain)
        {
            //надеюсь это никто никогда не увидит
            _iterations = 1;
            _chosenField = sortKeyIndex;
            _filePath = inputFilePath;

            using (StreamReader reader = new StreamReader(inputFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    addToColumn("A", line);
                }
            }
            while (true)
            {
                //Разбиваем записи на подфайлы
                


                _segments = 1;
                using var fileA = new StreamReader(_filePath);
                _headers = fileA.ReadLine()!;

                using var fileB = new StreamWriter("B.csv");
                using var fileC = new StreamWriter("C.csv");
                //В этой переменной будет храниться очередная считанная из исходного файла запись
                string? currentRecord = fileA.ReadLine();

                bool flag = true;
                int counter = 0;
                //цикл прекратится, когда мы дойдём до конца исходного файла
                while (currentRecord is not null)
                {
                    //дошли до конца цепочки, переключаемся на запись новой
                    if (counter == _iterations)
                    {
                        counter = 0;
                        flag = !flag;
                        _segments++;
                    }

                    if (flag)
                    {
                        //Запись отправляется в подфайл В
                        fileB.WriteLine(currentRecord);
                        onLog($"Положили {currentRecord} в подфайл B");
                        removeFromColumn("A", currentRecord);
                        addToColumn("B", currentRecord);
                        await onDynamicDelay(token);
                    }
                    else
                    {
                        //Запись отправляется в подфайл С
                        fileC.WriteLine(currentRecord);
                        onLog($"Положили {currentRecord} в подфайл C");
                        removeFromColumn("A", currentRecord);
                        addToColumn("C", currentRecord);
                        await onDynamicDelay(token);
                    }

                    //считываем следующую запись
                    currentRecord = fileA.ReadLine();
                    counter++;
                }

                fileA.Close();
                fileB.Close();
                fileC.Close();

                //Если после разделения цепочка осталась одна, значит, записи в файле отсортированы
                if (_segments == 1)
                {
                    onExplain.Invoke("После разделения осталась одна цепочка, значит все файлы отсортированы");
                    break;
                }

                //Сливаем вместе цепочки из под файлов
                using var writerA = new StreamWriter(_filePath);
                using var readerB = new StreamReader("B.csv");
                using var readerC = new StreamReader("C.csv");

                //Не забываем вернуть заголовки таблицы на своё место, в начало исходного файла
                writerA.WriteLine(_headers);

                string? elementB = readerB.ReadLine();
                string? elementC = readerC.ReadLine();

                int counterB = 0;
                int counterC = 0;
                //Итерации будут происходить, когда 
                while (elementB is not null || elementC is not null)
                {
                    string? currentRecord2;
                    bool flag2 = false;

                    //Обрабатываем случай, когда закончился весь файл B, или цепочка из данной пары 
                    //в нём
                    if (elementB is null || counterB == _iterations)
                    {
                        onExplain.Invoke("В подфайле B закончились пары, поэтому перекладываем все остальные значения из C");
                        onLog("Переложили всё оставшееся значения из C");
                        removeFromColumn("C", elementC);
                        addToColumn("A", elementC);
                        await onDynamicDelay(token);
                        currentRecord2 = elementC;
                    }
                    else if (elementC is null || counterC == _iterations) //аналогично предыдущему блоку if, но для подфайла С
                    {
                        currentRecord2 = elementB;
                        onExplain.Invoke("В подфайле C закончились пары, поэтому перекладываем все остальные значения из B");
                        onLog("Переложили всё оставшееся значения из B");
                        removeFromColumn("B", elementB);
                        addToColumn("A", elementB);
                        await onDynamicDelay(token);
                        flag2 = true;
                    }
                    else
                    {
                        //Если оба подфайла ещё не закончились, то сравниваем записи по нужному полю
                        if (CompareElements(elementB, elementC))
                        {
                            onExplain.Invoke($"{elementB.Split(';')[_chosenField]} < {elementC.Split(';')[_chosenField]}, значит кладём в A элемент из B");
                            onLog($"Переложили {elementB.Split(';')[_chosenField]} из B в A");
                            removeFromColumn("B", elementB);
                            addToColumn("A", elementB);
                            await onDynamicDelay(token);
                            //Если запись из файла В оказалась меньше
                            currentRecord2 = elementB;
                            flag2 = true;
                        }
                        else
                        {
                            onExplain.Invoke($"{elementC.Split(';')[_chosenField]} <= {elementB.Split(';')[_chosenField]}, значит кладём в A элемент из C");
                            onLog($"Переложили {elementB.Split(';')[_chosenField]} из C в A");
                            removeFromColumn("C", elementC);
                            addToColumn("A", elementC);
                            await onDynamicDelay(token);
                            //Если запись из файла С оказалась меньше
                            currentRecord2 = elementC;
                        }
                    }

                    //Записываем в исходный файл выбранную нами запись
                    writerA.WriteLine(currentRecord2);

                    if (flag2)
                    {
                        elementB = readerB.ReadLine();
                        counterB++;
                    }
                    else
                    {
                        elementC = readerC.ReadLine();
                        counterC++;
                    }

                    if (counterB != _iterations || counterC != _iterations)
                    {
                        continue;
                    }

                    //Если серии в обоих файлах закончились, то обнуляем соответствующие счётчики
                    counterC = 0;
                    counterB = 0;
                }

                _iterations *= 2;
            }



            await onDynamicDelay(token);

        }

        
    }
}