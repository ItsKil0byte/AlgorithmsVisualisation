using System.Windows.Controls;

namespace AlgorithmsVisualisation.SortingAlgorithms
{
    public class MergeSort : ISorting
    {
        public async Task Sort(Canvas canvas, List<int> array, CancellationToken token, Func<Task> onStep, Func<CancellationToken, Task> onDynamicDelay, Action<string> onLog, Action<string> onExplain, Action<int, int> onHighlight, Action<int, int> onHighlightRange)
        {
            await MergeSortRecursive(array, 0, array.Count - 1, token, onStep, onDynamicDelay, onLog, onExplain, onHighlight, onHighlightRange);
            onExplain?.Invoke("Сортировка завершена!");
        }

        private async Task MergeSortRecursive(List<int> array, int left, int right, CancellationToken token, Func<Task> onStep, Func<CancellationToken, Task> onDynamicDelay, Action<string> onLog, Action<string> onExplain, Action<int, int> onHighlight, Action<int, int> onHighlightRange)
        {
            if (left >= right || token.IsCancellationRequested) return;

            int middle = (left + right) / 2;

            onHighlightRange(left, right);
            onLog($"Делим массив: [{left}:{middle}] и [{middle + 1}:{right}].");
            onExplain?.Invoke($"Делим массив на два подмассива.");

            await onDynamicDelay(token);

            await MergeSortRecursive(array, left, middle, token, onStep, onDynamicDelay, onLog, onExplain, onHighlight, onHighlightRange);
            await MergeSortRecursive(array, middle + 1, right, token, onStep, onDynamicDelay, onLog, onExplain, onHighlight, onHighlightRange);

            await Merge(array, left, middle, right, token, onStep, onDynamicDelay, onLog, onExplain, onHighlight, onHighlightRange);

            onHighlightRange(-1, -1);
        }

        private async Task Merge(List<int> array, int left, int middle, int right, CancellationToken token, Func<Task> onStep, Func<CancellationToken, Task> onDynamicDelay, Action<string> onLog, Action<string> onExplain, Action<int, int> onHighlight, Action<int, int> onHighlightRange)
        {
            onHighlightRange(left, right);
            onLog($"Слияние частей: [{left}:{middle}] и [{middle + 1}:{right}].");
            onExplain?.Invoke($"Начинаем слияние отсортированных частей.");

            await onDynamicDelay(token);

            int[] leftArray = array.GetRange(left, middle - left + 1).ToArray();
            int[] rightArray = array.GetRange(middle + 1, right - middle).ToArray();

            int i = 0, j = 0, k = left;

            while (i < leftArray.Length && j < rightArray.Length)
            {
                if (token.IsCancellationRequested) return;

                onHighlight(k, k);
                onLog($"Сравниваем: {leftArray[i]} и {rightArray[j]}.");
                onExplain?.Invoke($"Сравниваем элемент {leftArray[i]} из левого подмассива с элементом {rightArray[j]} из правого подмассива.");

                await onDynamicDelay(token);

                if (leftArray[i] <= rightArray[j])
                {
                    onLog($"Добавляем {leftArray[i]} в массив.");
                    onExplain?.Invoke($"Элемент {leftArray[i]} меньше или равен {rightArray[j]}, добавляем его в отсортированный массив.");
                    array[k++] = leftArray[i++];
                }
                else
                {
                    onLog($"Добавляем {rightArray[j]} в массив.");
                    onExplain?.Invoke($"Элемент {rightArray[j]} меньше {leftArray[i]}, добавляем его в отсортированный массив.");
                    array[k++] = rightArray[j++];
                }

                await onStep();
            }

            while (i < leftArray.Length)
            {
                if (token.IsCancellationRequested) return;

                onHighlight(k, k);
                onLog($"Добавляем оставшийся элемент {leftArray[i]} из левого подмассива.");
                onExplain?.Invoke($"В левом подмассиве остались элементы. Добавляем {leftArray[i]} в отсортированный массив.");
                array[k++] = leftArray[i++];
                await onStep();
            }

            while (j < rightArray.Length)
            {
                if (token.IsCancellationRequested) return;

                onHighlight(k, k);
                onLog($"Добавляем оставшийся элемент {rightArray[j]} из правого подмассива.");
                onExplain?.Invoke($"В правом подмассиве остались элементы. Добавляем {rightArray[j]} в отсортированный массив.");
                array[k++] = rightArray[j++];
                await onStep();
            }

            onLog($"Слияние завершено для диапазона [{left}:{right}].");
            onExplain?.Invoke($"Два подмассива объединены в отсортированную последовательность.");
            onHighlightRange(left, right);
            onHighlight(-1, -1);

            await onDynamicDelay(token);
        }
    }
}
