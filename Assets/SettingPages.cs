using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class SettingPages
{
    public List<GameObject> UIPanels;
    public List<GameObject> tabs;

    public SettingPages(List<GameObject> UIPanels, List<GameObject> tabs)
    {
        this.UIPanels = UIPanels;
        this.tabs = tabs;
    }
}