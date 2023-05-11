using UnityEngine;

namespace Utils
{
	/// <summary>
	/// A simple component to disable a GameObject after a given amount of time.
	/// Timer resets when the GameObject is reenabled.
	/// </summary>
	public class SimpleDisableAfterTime : MonoBehaviour
	{
		[SerializeField, Tooltip("The amount of time to be enabled for")]
		private float _lifeTime = 5f;

		[SerializeField]
		private GameObject _objectToDisable;

		private float _counter = 0f;

		/// <summary>
		/// Sets the life time of the GameObject before it becomes disabled.
		/// </summary>
		/// <param name="value"></param>
		public void SetLifeTime(float value)
		{
			_lifeTime = value;
		}

		// Unity Methods.
		private void OnEnable()
		{
			_counter = 0;
			_objectToDisable.SetActive(true);
		}

		private void OnDisable()
		{
			_objectToDisable.SetActive(false);
		}

		private void Update()
		{
			_counter += Time.deltaTime;

			if (_counter >= _lifeTime)
			{
				_counter -= _lifeTime;
				this.enabled = false;
			}
		}
	}
}