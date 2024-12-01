using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AlgorithmsVisualisation.SortingAlgorithms
{
    public class StraightMergeSort : IExternalSorting
    {
        string outputFile = "C:\\Users\\zxcursedfan\\Desktop\\otec\\output.csv";


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

        private string? _headers;
        private long _iterations, _segments;
        private int _chosenField;
        private string _filePath;
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

        public async Task Sort(string inputFilePath, int sortKeyIndex,CancellationToken token, Action<string, string, Color> addToColumn, Action<string, string> removeFromColumn, Func<CancellationToken, Task> onDynamicDelay, Action<string> onLog, Action<string> onExplain)
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
                    addToColumn("A", line, Colors.Black);
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
                        addToColumn("B", currentRecord, colorDictionary[_segments]);
                        await onDynamicDelay(token);
                    }
                    else
                    {
                        //Запись отправляется в подфайл С
                        fileC.WriteLine(currentRecord);
                        onLog($"Положили {currentRecord} в подфайл C");
                        removeFromColumn("A", currentRecord);
                        addToColumn("C", currentRecord, colorDictionary[_segments]);
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
                        addToColumn("A", elementC, Colors.Black);
                        await onDynamicDelay(token);
                        currentRecord2 = elementC;
                    }
                    else if (elementC is null || counterC == _iterations) //аналогично предыдущему блоку if, но для подфайла С
                    {
                        currentRecord2 = elementB;
                        onExplain.Invoke("В подфайле C закончились пары, поэтому перекладываем все остальные значения из B");
                        onLog("Переложили всё оставшееся значения из B");
                        removeFromColumn("B", elementB);
                        addToColumn("A", elementB, Colors.Black);
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
                            addToColumn("A", elementB, Colors.Black);
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
                            addToColumn("A", elementC, Colors.Black);
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
