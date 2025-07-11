using UnityEngine;
using UnityEngine.Rendering.Universal;
public class ProjectCamera
{
    private Camera _camera;
    private UniversalAdditionalCameraData _cameraData;

    public ProjectCamera(Camera camera = null, UniversalAdditionalCameraData cameraData = null)
    {
        _camera = camera;
        _cameraData = cameraData;
    }

    public Camera Cam
    {
        get => _camera;
        set => _camera = value;
    }

    public UniversalAdditionalCameraData Data
    {
        get => _cameraData;
        set => _cameraData = value;
    }

    public bool Exists => _camera != null;
}