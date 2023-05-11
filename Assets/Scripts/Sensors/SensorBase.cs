using UnityEngine;

namespace Sensors
{
	/// <summary>
	/// Base class for all Sensors.
	/// </summary>
	[RequireComponent(typeof(SensorManager))]
	public class SensorBase : MonoBehaviour
	{
		private SensorManager _manager;

		public virtual void STUpdate()
		{

		}

		protected virtual void Init()
		{

		}

		// Unity Functions.
		private void Awake()
		{
			_manager = GetComponent<SensorManager>();
			_manager.AddSensor(this);
			Init();
		}

		private void OnEnable()
		{
			if (_manager)
				_manager.AddSensor(this);
		}

		private void OnDisable()
		{
			if (_manager)
				_manager.RemoveSensor(this);
		}
	}
}