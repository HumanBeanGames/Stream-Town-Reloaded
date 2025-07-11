using Reflex.Core;
using UnityEngine;

[System.Serializable]
public class SavePreset : MonoBehaviour, IInstaller
{
    //VIDEO SETTINGS
    public int preset;

    public int displayMode;
    public int resoultion;

    public int shadowType;
    public int shadowResolution;//Shadow Res

    public bool enabledAO; // Bool

    public bool vSync; // Bool
    public int fpsLimiter;

    public float brightness;
    public float gamma;

    public int antiAliasing;
    public int cameraAA;

    //AUDIO SETTINGS

    public float masterVolume;
    public float musicVolume;
    public float playerVolume;
    public float environmentVolume;

    //GAMEPLAY SETTINGS

    public float panSensitivity;
    public float zoomSensitivity;
    public float wasdSensitivity;
    public float borderDetectionSensitivity;
    public int fov;

    public int autosaveTime;

    public int displayName;
    public int displayBuildings;

    public bool keyboardMovement;
    public bool mouseMovement;

    public string channelName;

    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.AddSingleton(this);
    }

}