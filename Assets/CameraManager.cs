using NUnit.Framework;
using PlayerControls;
using Reflex.Attributes;
using Settings;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(UniversalAdditionalCameraData))]
public class CameraManager : MonoBehaviour
{
    /*
    [Inject] private Camera _camera;
    [Inject] private UniversalAdditionalCameraData _cameraData;
    [Inject] SaveState SaveState;
    [Inject] SavePreset SavePreset;
    [Inject] Access_CameraAADropdown _cameraAADropdown;
    [Inject] Access_Preset _preset;

    public void SetUpCamera()
    {
        _camera = new Camera();
        _cameraData = new UniversalAdditionalCameraData();
        if (Camera.main)
        {
            _camera = Camera.main;
            _cameraData = _camera.GetUniversalAdditionalCameraData();
            SettingsManager _settingsManager = FindObjectsByType<SettingsManager>(FindObjectsSortMode.None)[0];
            _settingsManager.LoadSettings();
        }
    }*/
}
