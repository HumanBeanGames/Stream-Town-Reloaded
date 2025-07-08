using Reflex.Attributes;
using UnityEditor.Overlays;
using Reflex.Core;

public class ChannelData : PSAccess
{
    public string name = "Test";

    public ChannelData(Container container) : base(container) { }
    public SaveState SaveState => Project.Resolve<SaveState>();

    public void SetChannelName(string inName)
    {
        name = inName.ToLower();
        SaveState.SafeSave();
    }
}