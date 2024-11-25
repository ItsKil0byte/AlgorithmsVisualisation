using System.Windows.Controls;

namespace AlgorithmsVisualisation.SortingAlgorithms
{
    public class MergeSort : ISorting
    {
        // То же самое что и у быстрой сортировки. TODO: переделать объяснение.
        public async Task Sort(Canvas canvas, List<int> array, CancellationToken token, Func<Task> onStep, Func<CancellationToken, Task> onDynamicDelay, Action<string> onLog, Action<string> onExplain, Action<int, int> onHighlight)
        {
            await MergeSortRecursive(array, 0, array.Count - 1, token, onStep, onDynamicDelay, onLog, onExplain, onHighlight);
            onExplain?.Invoke("Сортировка завершена!");
        }

        private async Task MergeSortRecursive(List<int> array, int left, int right, CancellationToken token, Func<Task> onStep, Func<CancellationToken, Task> onDynamicDelay, Action<string> onLog, Action<string> onExplain, Action<int, int> onHighlight)
        {
            if (left >= right || token.IsCancellationRequested) return;

            int middle = (left + right) / 2;

            onLog($"Делим массив: [{left}:{middle}] и [{middle + 1}:{right}].");
            onExplain?.Invoke($"Разделяем массив на две части: с {left}-го по {middle}-й элемент и с {middle + 1}-го по {right}-й элемент.");

            await onDynamicDelay(token);

            await MergeSortRecursive(array, left, middle, token, onStep, onDynamicDelay, onLog, onExplain, onHighlight);
            await MergeSortRecursive(array, middle + 1, right, token, onStep, onDynamicDelay, onLog, onExplain, onHighlight);

            await Merge(array, left, middle, right, token, onStep, onDynamicDelay, onLog, onExplain, onHighlight);
        }

        private async Task Merge(List<int> array, int left, int middle, int right, CancellationToken token, Func<Task> onStep, Func<CancellationToken, Task> onDynamicDelay, Action<string> onLog, Action<string> onExplain, Action<int, int> onHighlight)
        {
            onLog($"Слияние частей: [{left}:{middle}] и [{middle + 1}:{right}].");
            onExplain?.Invoke($"Начинаем слияние отсортированных частей: с {left}-го по {middle}-й элемент и с {middle + 1}-го по {right}-й элемент.");

            int[] leftArray = array.GetRange(left, middle - left + 1).ToArray();
            int[] rightArray = array.GetRange(middle + 1, right - middle).ToArray();

            int i = 0, j = 0, k = left;

            while (i < leftArray.Length && j < rightArray.Length)
            {
                if (token.IsCancellationRequested) return;

                onHighlight(k, k);
                onLog($"Сравниваем: {leftArray[i]} и {rightArray[j]}.");
                onExplain?.Invoke($"Сравниваем элемент {leftArray[i]} из левой части с элементом {rightArray[j]} из правой части.");

                await onDynamicDelay(token);

                if (leftArray[i] <= rightArray[j])
                {
                    onLog($"Добавляем {leftArray[i]} в результат.");
                    onExplain?.Invoke($"Элемент {leftArray[i]} меньше или равен {rightArray[j]}, добавляем его в массив.");
                    array[k++] = leftArray[i++];
                }
                else
                {
                    onLog($"Добавляем {rightArray[j]} в результат.");
                    onExplain?.Invoke($"Элемент {rightArray[j]} меньше {leftArray[i]}, добавляем его в массив.");
                    array[k++] = rightArray[j++];
                }

                await onStep();
            }

            while (i < leftArray.Length)
            {
                if (token.IsCancellationRequested) return;

                onHighlight(k, k);
                onLog($"Добавляем оставшийся элемент {leftArray[i]} из левой части.");
                onExplain?.Invoke($"В левой части остались элементы. Добавляем {leftArray[i]} в массив.");
                array[k++] = leftArray[i++];
                await onStep();
            }

            while (j < rightArray.Length)
            {
                if (token.IsCancellationRequested) return;

                onHighlight(k, k);
                onLog($"Добавляем оставшийся элемент {rightArray[j]} из правой части.");
                onExplain?.Invoke($"В правой части остались элементы. Добавляем {rightArray[j]} в массив.");
                array[k++] = rightArray[j++];
                await onStep();
            }

            onLog($"Слияние завершено для диапазона [{left}:{right}].");
            onExplain?.Invoke($"Части массива с {left}-го по {right}-й элемент объединены в отсортированную последовательность.");
        }
    }
}
