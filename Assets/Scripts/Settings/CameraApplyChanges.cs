using UnityEngine;
using Managers;
using Scriptables;
using UnityEngine.Rendering.Universal;
using PlayerControls;
namespace Settings 
{
    public class CameraApplyChanges : MonoBehaviour 
	{
		//Settings data information
		private SettingsScriptable _settingsData;
		
		//Camera Components
		private Camera _camera;
		private UniversalAdditionalCameraData _cameraData;
		private CameraController _cameraController;

		public Camera Cam => _camera;
		public UniversalAdditionalCameraData CameraData => _cameraData;

		public void UpdateCameraSettings()
		{
			_camera.fieldOfView = _settingsData.fov;
			
			switch (_settingsData.camAA)
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
			
			_cameraController.PanSensitivity = _settingsData.panSensitivity;
			_cameraController.ZoomSensitivity = _settingsData.zoomSensitivity;
			_cameraController.WasdSensitivity = _settingsData.wasdSensitivity;
			_cameraController.BorderDetectionSensitivity = _settingsData.borderDetectionSensitivity;

			_cameraController.BorderDetection = _settingsData.borderDetection;
			_cameraController.MouseControls = _settingsData.mouseControls;
		}

		private void EnableCamera()
		{
			_cameraController.enabled = true;
			GameStateManager.ReadiedPlayer -= EnableCamera;
		}

		private void Awake()
		{
			_settingsData = GameManager.Instance.SettingsData;
			_camera = this.GetComponent<Camera>();
			_cameraData = this.GetComponent<UniversalAdditionalCameraData>();
			_cameraController = this.GetComponent<CameraController>();
			GameStateManager.ReadiedPlayer += EnableCamera;
			UpdateCameraSettings();
		}
	}
}