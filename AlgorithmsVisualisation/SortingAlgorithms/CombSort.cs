using System.Windows.Controls;

namespace AlgorithmsVisualisation.SortingAlgorithms
{
    public class CombSort : ISorting
    {
        public async Task Sort(Canvas canvas, List<int> array, CancellationToken token, Func<Task> onStep, Func<CancellationToken, Task> onDynamicDelay, Action<string> onLog, Action<string> onExplain, Action<int, int> onHighlight)
        {
            int gap = array.Count;
            bool swapped = true;
            double shrinkFactor = 1.3;

            while (gap > 1 || swapped)
            {
                if (token.IsCancellationRequested) return;

                gap = (int)(gap / shrinkFactor);

                if (gap < 1) gap = 1;

                swapped = false;

                for (int i = 0; i + gap < array.Count; i++)
                {
                    if (token.IsCancellationRequested) return;

                    int j = i + gap;

                    onHighlight(i, j);
                    onLog($"Сравнение: {array[i]} и {array[j]}.");
                    onExplain?.Invoke($"Сравниваем столбец {array[i]} и столбец {array[j]} на расстоянии {gap}.");

                    await onDynamicDelay(token);

                    if (array[i] > array[j])
                    {
                        onLog($"Перестановка: {array[i]} и {array[j]}.");
                        onExplain?.Invoke($"Столбец {array[i]} больше, чем столбец {array[j]}, меняем их местами.");

                        (array[i], array[j]) = (array[j], array[i]);
                        swapped = true;

                        onHighlight(i, j);

                        await onStep();
                    }
                    else
                    {
                        onExplain?.Invoke($"Столбец {array[i]} меньше, чем столбец {array[j]}. Ничего не меняем.");
                        await onDynamicDelay(token);
                    }
                }
            }

            onExplain?.Invoke("Сортировка завершена!");

            await onDynamicDelay(token);
        }
    }
}
