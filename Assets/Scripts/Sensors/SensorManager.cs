using System.Collections.Generic;
using UnityEngine;

namespace Sensors
{
	/// <summary>
	/// Manages all sensors that a unit has.
	/// </summary>
	public class SensorManager : MonoBehaviour
	{
		[SerializeField]
		private float _updateRate = 0.25f;
		private float _updateTimer = 0;
		private List<SensorBase> _sensors = new List<SensorBase>();

		/// <summary>
		/// Adds a sensor to the unit.
		/// </summary>
		/// <param name="sensor"></param>
		public void AddSensor(SensorBase sensor)
		{
			if (_sensors.Contains(sensor))
				return;

			_sensors.Add(sensor);
		}

		/// <summary>
		/// Removes a sensor from a unit.
		/// </summary>
		/// <param name="sensor"></param>
		public void RemoveSensor(SensorBase sensor)
		{
			if (!_sensors.Contains(sensor))
				return;

			_sensors.Remove(sensor);
		}


		// Unity Events.
		private void Start()
		{
			_updateTimer = Random.Range(0, _updateRate);
		}

		private void Update()
		{
			_updateTimer += Time.deltaTime;

			if (_updateTimer >= _updateRate)
			{
				_updateTimer -= _updateRate;

				for (int i = 0; i < _sensors.Count; i++)
				{
					if (_sensors[i].gameObject.activeInHierarchy)
						_sensors[i].STUpdate();
				}
			}
		}
	}
}