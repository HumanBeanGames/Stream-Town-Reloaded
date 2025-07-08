using System.Collections.Generic;
using Reflex.Core;

public class Autosave
{
    public readonly List<int> Intervals;

    public Autosave(List<int> intervals)
    {
        Intervals = intervals;
    }
}
