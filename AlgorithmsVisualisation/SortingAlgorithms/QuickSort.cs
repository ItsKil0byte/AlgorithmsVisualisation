using System.Windows.Controls;

namespace AlgorithmsVisualisation.SortingAlgorithms
{
    public class QuickSort : ISorting
    {
        // Тут так себе объяснение, но лучше пока не могу придумать. В общем TODO: переделать объяснение.
        public async Task Sort(Canvas canvas, List<int> array, CancellationToken token, Func<Task> onStep, Func<CancellationToken, Task> onDynamicDelay, Action<string> onLog, Action<string> onExplain, Action<int, int> onHighlight)
        {
            await QuickSortRecursive(array, 0, array.Count - 1, token, onStep, onDynamicDelay, onLog, onExplain, onHighlight);
            onExplain?.Invoke("Сортировка завершена!");
        }

        private async Task QuickSortRecursive(List<int> array, int low, int high, CancellationToken token, Func<Task> onStep, Func<CancellationToken, Task> onDynamicDelay, Action<string> onLog, Action<string> onExplain, Action<int, int> onHighlight)
        {
            if (low >= high || token.IsCancellationRequested) return;

            int pivotIndex = await Partition(array, low, high, token, onStep, onDynamicDelay, onLog, onExplain, onHighlight);

            await QuickSortRecursive(array, low, pivotIndex - 1, token, onStep, onDynamicDelay, onLog, onExplain, onHighlight);

            await QuickSortRecursive(array, pivotIndex + 1, high, token, onStep, onDynamicDelay, onLog, onExplain, onHighlight);
        }

        private async Task<int> Partition(List<int> array, int low, int high, CancellationToken token, Func<Task> onStep, Func<CancellationToken, Task> onDynamicDelay, Action<string> onLog, Action<string> onExplain, Action<int, int> onHighlight)
        {
            int pivot = array[high];
            int i = low - 1;

            onLog($"Выбран опорный элемент: {pivot}.");
            onExplain?.Invoke($"Последний элемент выбирается в качестве опорного: {pivot}.");

            await onDynamicDelay(token);

            for (int j = low; j < high; j++)
            {
                if (token.IsCancellationRequested) return -1;

                onHighlight(j, high);

                onLog($"Сравнение: {array[j]} и {pivot}.");
                onExplain?.Invoke($"Сравниваем элемент {array[j]} с опорным элементом {pivot}.");

                await onDynamicDelay(token);

                if (array[j] < pivot)
                {
                    i++;

                    onHighlight(i, j);

                    onLog($"Перестановка: {array[i]} и {array[j]}.");
                    onExplain?.Invoke($"Элемент {array[j]} меньше опорного. Перемещаем на позицию {i}.");

                    (array[i], array[j]) = (array[j], array[i]);

                    await onStep();
                }
                else
                {
                    onExplain?.Invoke($"Элемент {array[j]} больше или равен опорному. Оставляем на месте.");
                }

                await onDynamicDelay(token);
            }

            onHighlight(i + 1, high);

            onLog($"Установка опорного элемента {pivot} на место.");
            onExplain?.Invoke($"Меняем местами опорный элемент {array[high]} с элементом {array[i + 1]}.");

            (array[i + 1], array[high]) = (array[high], array[i + 1]);

            await onStep();

            await onDynamicDelay(token);

            return i + 1;
        }
    }
}
