using UnityEngine;

namespace Utils
{
	/// <summary>
	/// A simple component that rotates a gameobject to look at the main camera.
	/// </summary>
	public class SimpleLookAtCamera : MonoBehaviour
	{
		private void Update()
		{
			transform.LookAt(Camera.main.transform);
			transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
		}
	}
}