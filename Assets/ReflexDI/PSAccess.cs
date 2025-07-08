using Reflex.Core;

public abstract class PSAccess
{
    protected readonly Container Project;

    protected PSAccess(Container container)
    {
        var c = container;
        while (c.Parent != null)
            c = c.Parent;
        Project = c;
    }
}
