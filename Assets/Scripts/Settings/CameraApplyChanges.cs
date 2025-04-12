using UnityEngine;

using Scriptables;
using UnityEngine.Rendering.Universal;
using PlayerControls;
namespace Settings 
{
    public class CameraApplyChanges : MonoBehaviour 
	{
		//Camera Components
		private Camera _camera;
		private UniversalAdditionalCameraData _cameraData;
		private CameraController _cameraController;

		public Camera Cam => _camera;
		public UniversalAdditionalCameraData CameraData => _cameraData;

		public void UpdateCameraSettings(Camera camera)
		{
			camera.fieldOfView = SettingsManager.CurrentSettings.fov;

			var cameraData = camera.GetComponent<UniversalAdditionalCameraData>();

			cameraData.antialiasing = SettingsManager.CurrentSettings.camAA;

			cameraData.antialiasingQuality = AntialiasingQuality.High;

			var cameraController = camera.GetComponent<CameraController>();
			cameraController.PanSensitivity = SettingsManager.CurrentSettings.panSensitivity;
			cameraController.ZoomSensitivity = SettingsManager.CurrentSettings.zoomSensitivity;
			cameraController.WasdSensitivity = SettingsManager.CurrentSettings.wasdSensitivity;
			cameraController.BorderDetectionSensitivity = SettingsManager.CurrentSettings.borderDetectionSensitivity;

			cameraController.BorderDetection = SettingsManager.CurrentSettings.borderDetection;
			cameraController.MouseControls = SettingsManager.CurrentSettings.mouseControls;
		}

		private void EnableCamera()
		{
			_cameraController.enabled = true;
			GameStateManager.ReadiedPlayer -= EnableCamera;
		}

		private void Awake()
		{
			_camera = this.GetComponent<Camera>();
			_cameraData = this.GetComponent<UniversalAdditionalCameraData>();
			_cameraController = this.GetComponent<CameraController>();
			GameStateManager.ReadiedPlayer += EnableCamera;
			UpdateCameraSettings(_camera);
		}
	}
}