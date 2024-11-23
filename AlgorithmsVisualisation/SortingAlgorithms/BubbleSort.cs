using System.Windows.Controls;

namespace AlgorithmsVisualisation.SortingAlgorithms
{
    public class BubbleSort : ISorting
    {
        public async Task Sort(Canvas canvas, List<int> array, CancellationToken token, Func<Task> onStep)
        {
            for (int i = 0; i < array.Count; i++)
            {
                for (int j = 0; j < array.Count - 1 - i; j++)
                {
                    if (token.IsCancellationRequested) return;

                    if (array[j] > array[j + 1])
                    {
                        (array[j], array[j + 1]) = (array[j + 1], array[j]);

                        await onStep();
                    }
                }
            }
        }
    }
}
