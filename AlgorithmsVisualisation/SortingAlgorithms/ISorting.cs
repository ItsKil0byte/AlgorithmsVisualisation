using System.Windows.Controls;

public interface ISorting
{
    Task Sort(
        Canvas canvas,
        List<int> array,
        CancellationToken token,
        Func<Task> onStep,
        Func<CancellationToken, Task> onDynamicDelay,
        Action<string> onLog,
        Action<string> onExplain,
        Action<int, int> onHighlight,
        Action<int, int> onHighlightRange // Новый параметр
    );
}
