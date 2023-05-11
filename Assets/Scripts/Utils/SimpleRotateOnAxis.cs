using UnityEngine;

namespace Utils
{
	/// <summary>
	/// A simple component for rotating a gameobject on the defined axis.
	/// </summary>
	public class SimpleRotateOnAxis : MonoBehaviour
	{
		[SerializeField]
		private Vector3 _axis;

		[SerializeField]
		private float _speed;

		public void Update()
		{
			transform.Rotate(_axis * Time.deltaTime * _speed);
		}
	}
}