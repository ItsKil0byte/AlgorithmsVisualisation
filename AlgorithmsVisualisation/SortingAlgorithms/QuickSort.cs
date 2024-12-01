using System.Windows.Controls;

namespace AlgorithmsVisualisation.SortingAlgorithms
{
    public class QuickSort : ISorting
    {
        public async Task Sort(Canvas canvas, List<int> array, CancellationToken token, Func<Task> onStep, Func<CancellationToken, Task> onDynamicDelay, Action<string> onLog, Action<string> onExplain, Action<int, int> onHighlight, Action<int, int> onHighlightRange)
        {
            await QuickSortRecursive(array, 0, array.Count - 1, token, onStep, onDynamicDelay, onLog, onExplain, onHighlight, onHighlightRange);
            onExplain?.Invoke("Сортировка завершена!");
        }

        private async Task QuickSortRecursive(List<int> array, int low, int high, CancellationToken token, Func<Task> onStep, Func<CancellationToken, Task> onDynamicDelay, Action<string> onLog, Action<string> onExplain, Action<int, int> onHighlight, Action<int, int> onHighlightRange)
        {
            if (low >= high || token.IsCancellationRequested) return;

            onHighlightRange(low, high);
            onLog($"Работаем с массивом: [{low}:{high}].");
            onExplain?.Invoke($"Работаем с массивом от элемента {low+1} до элемента {high}.");
            onHighlight(-1, -1);

            await onDynamicDelay(token);

            int pivotIndex = await Partition(array, low, high, token, onStep, onDynamicDelay, onLog, onExplain, onHighlight);

            await QuickSortRecursive(array, low, pivotIndex - 1, token, onStep, onDynamicDelay, onLog, onExplain, onHighlight, onHighlightRange);
            await QuickSortRecursive(array, pivotIndex + 1, high, token, onStep, onDynamicDelay, onLog, onExplain, onHighlight, onHighlightRange);
        }

        private async Task<int> Partition(List<int> array, int low, int high, CancellationToken token, Func<Task> onStep, Func<CancellationToken, Task> onDynamicDelay, Action<string> onLog, Action<string> onExplain, Action<int, int> onHighlight)
        {
            int pivot = array[high];
            onLog($"Выбран опорный элемент - {high}.");
            onExplain?.Invoke($"Опорный элемент {pivot} выбран из конца массива.");

            onHighlight(high, high);
            await onDynamicDelay(token);

            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (token.IsCancellationRequested) return -1;

                onHighlight(j, high);
                onLog($"Сравниваем: {array[j]} и {pivot}.");
                onExplain?.Invoke($"Сравниваем {array[j]} с опорным элементом {pivot}.");

                await onDynamicDelay(token);

                if (array[j] <= pivot)
                {
                    i++;

                    if (i != j)
                    {
                        onLog($"Перестановка: {array[i]} и {array[j]}.");
                        onExplain?.Invoke($"Элемент {array[j]} меньше или равен опорному. Меняем его с элементом {array[i]}, что бы он оказался в левой части.");
                        (array[i], array[j]) = (array[j], array[i]);

                        onHighlight(i, high);
                        await onStep();
                    }
                    else
                    {
                        onHighlight(i, j);
                    }
                }
            }

            onLog($"Опорный элемент перемещён на позицию {i + 1}.");
            onExplain?.Invoke($"Опорный элемент {pivot} перемещён в левую часть массива.");
            (array[i + 1], array[high]) = (array[high], array[i + 1]);

            onHighlight(i + 1, i + 1);
            await onStep();

            return i + 1;
        }
    }
}
