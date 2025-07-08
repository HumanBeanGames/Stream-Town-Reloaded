using System.Collections.Generic;
using Reflex.Core;

public class Autosave : PSAccess
{
    public readonly List<int> Intervals;

    public Autosave(Container container, List<int> intervals) : base(container)
    {
        Intervals = intervals;
    }
}
