using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Settings/Setting Preset", fileName = "NewSettingPreset")]
public class SettingPreset : ScriptableObject
{
    public int antiAliasing;
    public int shadowType;
    public int shadowResolution;
    public bool vSync;
    public bool enabledAO;
    public int cameraAA;
}