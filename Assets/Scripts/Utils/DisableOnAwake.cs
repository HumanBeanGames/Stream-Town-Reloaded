using UnityEngine;

namespace Utils
{
	/// <summary>
	/// A simple component that disables a GameObject on Awake.
	/// </summary>
	public class DisableOnAwake : MonoBehaviour
	{
		private void Awake()
		{
			this.gameObject.SetActive(false);
		}
	}
}