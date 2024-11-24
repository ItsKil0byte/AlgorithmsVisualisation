using System.Windows.Controls;

namespace AlgorithmsVisualisation.SortingAlgorithms
{
    public interface ISorting
    {
        Task Sort(
            Canvas canvas,
            List<int> array,
            CancellationToken token,
            Func<Task> onStep,
            Func<CancellationToken, Task> onDynamicDelay,
            Action<string> onLog,
            Action<string> onExplain
        );
    }
}
