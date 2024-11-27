using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AlgorithmsVisualisation.SortingAlgorithms
{
    public interface IExternalSorting
    {
        Task Sort(
            string inputFilePath,
            int key,
            CancellationToken token,
            Action<string, string> addToColumn,
            Action<string, string> removeFromColumn,
            Func<CancellationToken, Task> onDynamicDelay,
            Action<string> onLog,
            Action<string> onExplain
            //<int, int> onHighlight
        );
    }
}
