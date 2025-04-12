using Buildings;
using Character;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace Sensors
{
	/// <summary>
	/// A sensor that finds appropriate stations for units.
	/// </summary>
	public class StationSensor : SensorBase
	{
		[SerializeField]
		private StationMask _stationMask;

		[SerializeField]
		private Station _currentStation;
		private Station _previousStation;
		private Player _player;

		[SerializeField]
		private UnityEvent _onStationChange;

		private StationManager _stationManager;

		public Station CurrentStation => _currentStation;
		public bool HasStation => _currentStation == null ? false : true;
		public bool UpdateStation { get; set; }

		public float DistanceToStation => _currentStation == null ? float.MaxValue : Vector3.Distance(transform.position, _currentStation.transform.position);

		public StationMask StationMask
		{
			get { return _stationMask; }
			set { OnSetStationMask(value); }
		}

		public Player Player
		{
			get { return _player; }
			set { _player = value; }
		}

		public override void STUpdate()
		{
			base.STUpdate();

			if (UpdateStation || _currentStation == null)
				GetNearestStation();
		}

		/// <summary>
		/// Attemmpts to set the current station.
		/// </summary>
		/// <param name="station"></param>
		public bool TrySetStation(Station station)
		{
			_currentStation = station;
			return true;
		}

		public bool TrySetStation(Station station, Player player)
		{
			if (station.Flags.HasFlag(player.StationSensor.StationMask))
			{
				_currentStation = station;
				Debug.Log($"Set {player.RoleHandler.CurrentRole}'s station to {station.gameObject.name}");
				_onStationChange.Invoke();
				return true;
			}

			Debug.Log($"Can't set {player.RoleHandler.CurrentRole}'s station to {station.gameObject.name}");
			return false;
		}

		/// <summary>
		/// Forces the current station to update to the nearest available station.
		/// </summary>
		public void ForceUpdateStation()
		{
			GetNearestStation();
		}

		/// <summary>
		/// Sets current station to the nearest station.
		/// </summary>
		private void GetNearestStation()
		{
			if (!_stationManager)
				return;

			//TODO: Implement BSP, also doesnt need to be called so often
			List<Station> stations = _stationManager.GetStationsByFlag(_stationMask);

			if (stations == null || stations.Count == 0)
			{
				_currentStation = null;
			}
			else
			{
				Station closest = null;
				float closestDistanceSqrd = float.MaxValue;

				for (int i = 0; i < stations.Count; i++)
				{
					float distanceSqrd = Vector3.SqrMagnitude(stations[i].transform.position - transform.position);

					if (distanceSqrd < closestDistanceSqrd)
					{
						closest = stations[i];
						closestDistanceSqrd = distanceSqrd;
					}
				}

				_currentStation = closest;
			}

			if (_currentStation != _previousStation)
			{
				_onStationChange.Invoke();
				_previousStation = _currentStation;
			}
		}

		/// <summary>
		/// Called when station mask has been set.
		/// </summary>
		/// <param name="flags"></param>
		private void OnSetStationMask(StationMask flags)
		{
			_stationMask = flags;

			_currentStation = null;
		}

		// Unity Functions.
		private void Start()
		{
			_stationManager = GameManager.Instance.StationManager;
			UpdateStation = true;
		}

		private void OnDisable()
		{
			_currentStation = null;
		}
	}
}