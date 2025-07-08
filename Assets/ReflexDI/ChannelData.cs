using Reflex.Attributes;
using UnityEditor.Overlays;
using Reflex.Core;

public class ChannelData
{
    private readonly Container _container;
    public string name = "Test";

    public ChannelData(Container container)
    {
        _container = container;
    }

    // Property to resolve SaveState singleton on demand
    public SaveState SaveState => _container.Resolve<SaveState>();

    public void SetChannelName(string inName)
    {
        name = inName.ToLower();
        SaveState.SafeSave();
    }
}