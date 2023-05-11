using UnityEngine;

namespace Utils
{
	/// <summary>
	/// Delays the Setting of the GraphBounds
	/// </summary>
	public class SimpleDelayGraphUpdateOnce : MonoBehaviour
	{
		[SerializeField]
		private float _delay = 0.25f;
		private float _counter = 0;
		private UpdateGraphBounds _UGB;

		private void Awake()
		{
			_UGB = GetComponent<UpdateGraphBounds>();
		}

		private void Update()
		{
			_counter += Time.deltaTime;

			if (_counter >= _delay)
			{
				_UGB.SetGraphBounds();
				this.enabled = false;
			}
		}
	}
}