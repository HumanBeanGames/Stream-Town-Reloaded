using UnityEngine;

namespace Utils
{
	/// <summary>
	/// A simple component that hides the game object's renderer on Awake.
	/// </summary>
	public class SimpleHideRendererOnAwake : MonoBehaviour
	{
		private void Awake()
		{
			var renderers = GetComponents<Renderer>();

			for (int i = 0; i < renderers.Length; i++)
			{
				renderers[i].enabled = false;
			}
		}
	}
}