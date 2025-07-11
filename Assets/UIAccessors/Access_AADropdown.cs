using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Reflex.Attributes;

public class Access_AADropdown : Access_Dropdown
{
    [Inject] private List<GameObject> _presetButtons;
    [Inject] private Access_Preset _preset;
    [Inject] private Camera _camera;
    [Inject] private UniversalAdditionalCameraData _cameraData;
    [Inject] private SaveState SaveState;
    [Inject] private int _cameraAA; // Adjust type if necessary

    public override void OnValueChanged(int v)
    {
        base.OnValueChanged(v);

        //REVISIT
        /*
        if (!_preset.PresetChange)
        {
            for (int i = 0; i < _presetButtons.Count; i++)
            {
                // REVISIT: This assumes a specific hierarchy
                _presetButtons[i].transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            }
            _preset.Val = 5;
        }

        if (_camera && _apply)
        {
            switch (v)
            {
                case 1:
                    _cameraData.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
                    break;
                case 2:
                    _cameraData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                    break;
                default:
                    _cameraData.antialiasing = AntialiasingMode.None;
                    break;
            }
            _cameraData.antialiasingQuality = AntialiasingQuality.High;
        }
        */
        _cameraAA = v;

        SaveState.SafeSave();
    }
}

