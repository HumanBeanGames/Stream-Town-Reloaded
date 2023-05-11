using UnityEngine;

namespace Scriptables 
{
	[CreateAssetMenu(fileName = "SettingsScriptiable", menuName = "ScriptableObjects/SettingsScriptiable", order = 1)]
	public class SettingsScriptable : ScriptableObject 
	{
		[Header("Streamer Data")]
		public string channelName;

		[Header("Camera Controls")]
		public float panSensitivity;
		public float zoomSensitivity;
		public float wasdSensitivity;
		public float borderDetectionSensitivity;
		public bool mouseControls;
		public bool borderDetection;

		[Header ("Camera Data")]
		public int fov;
		public int camAA;
		
		[Header ("Save settings")]
		public int autosaveTime;
		
		[Header ("UI display")]
		public int displayName;
		public int displayBuildings;
	}
}