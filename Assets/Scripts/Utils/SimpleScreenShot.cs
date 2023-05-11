using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using System;

namespace Utils
{
	public class SimpleScreenShot : MonoBehaviour
	{
		public const string FILE_PATH = "Screenshots";
		[SerializeField]
		private int _superSize = 4;


		private void Update()
		{
			if (Keyboard.current.f10Key.wasReleasedThisFrame)
			{
				DateTime time = DateTime.Now;
				CreateFolderIfDoesntExist();
				ScreenCapture.CaptureScreenshot($"{FILE_PATH}/screenshot_{time.ToFileTime()}.png",_superSize);
				Debug.Log($"Screenshot saved to Stream-Town-Working/{FILE_PATH}.");
            }
		}

		private void CreateFolderIfDoesntExist()
		{
			if (!Directory.Exists(FILE_PATH))
				Directory.CreateDirectory(FILE_PATH);
		}
	}
}