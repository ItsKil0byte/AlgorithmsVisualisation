﻿using System.Windows.Controls;

namespace AlgorithmsVisualisation.SortingAlgorithms
{
    public class BubbleSort : ISorting
    {
        public async Task Sort(Canvas canvas, List<int> array, CancellationToken token, Func<Task> onStep, Func<CancellationToken, Task> onDynamicDelay, Action<string> onExplain)
        {
            for (int i = 0; i < array.Count; i++)
            {
                for (int j = 0; j < array.Count - 1 - i; j++)
                {
                    if (token.IsCancellationRequested) return;

                    onExplain?.Invoke($"Сравниваем столбец {array[j]} и столбец {array[j + 1]}.");
                    await onDynamicDelay(token);

                    if (array[j] > array[j + 1])
                    {
                        onExplain?.Invoke($"Столбец {array[j]} больше, чем столбец {array[j + 1]}, меняем их местами.");
                        await onDynamicDelay(token);

                        (array[j], array[j + 1]) = (array[j + 1], array[j]);
                        await onStep();
                    }
                    else
                    {
                        onExplain?.Invoke($"Столбец {array[j]} меньше, чем столбец {array[j + 1]}. Ничего не меняем.");
                        await onDynamicDelay(token);
                    }
                }

                onExplain?.Invoke($"Пробег {i + 1} завершен. Столбец находится на своём месте.");
                await onDynamicDelay(token);
            }

            onExplain?.Invoke("Сортировка завершена!");
            await onDynamicDelay(token);
        }
    }
}
