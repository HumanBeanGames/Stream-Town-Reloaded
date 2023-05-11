using UnityEngine;

namespace Utils
{
	/// <summary>
	/// Visualises the bounds of a collidable object.
	/// </summary>
	public class BoundsVisualizer : MonoBehaviour
	{
		[SerializeField]
		private GameObject _visualizer;
		private Renderer[] _renderers;

		/// <summary>
		/// Sets the size of the visualizer.
		/// </summary>
		/// <param name="size"></param>
		public void SetSize(Vector2 size)
		{
			_visualizer.transform.localScale = new Vector3(size.x, 0.01f, size.y);

		}

		/// <summary>
		/// Called when collision state has changed and colours the visualizer accordingly.
		/// </summary>
		/// <param name="collision"></param>
		/// <param name="failColor"></param>
		/// <param name="successColor"></param>
		public void OnCollisionChange(bool collision, Color failColor, Color successColor)
		{
			for (int i = 0; i < _renderers.Length; i++)
			{
				_renderers[i].material.SetColor("_boundsVisColor", collision ? failColor : successColor);
			}
		}

		private void Awake()
		{
			_renderers = GetComponentsInChildren<Renderer>();
		}
	}
}