using Character;
using GridSystem.Partitioning;
using GUIDSystem;
using Managers;
using System;
using System.Collections.Generic;
using Target;
using Units;
using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace Sensors
{
	/// <summary>
	/// A sensor that finds all targets based on a TargetType mask.
	/// </summary>
	public class TargetSensor : SensorBase
	{
		[SerializeField]
		private TargetMask _targetMask = TargetMask.Player;

		[SerializeField]
		private Targetable _currentTarget = null;

		[SerializeField]
		private UnityEvent _onTargetChange;

		[SerializeField]
		private bool _useStationTargets = true;

		[SerializeField]
		private bool _attackAttacker = false;

		public Targetable CurrentTarget => _currentTarget;

		private Targetable _previousTarget = null;

		private StationSensor _stationSensor = null;
		private GUIDComponent _gUIDComponent = null;

		[SerializeField]
		private float _targetSearchRange = 100f;

		public bool UpdateTarget { get; set; }
		public bool HasTarget => _currentTarget == null ? false : true;
		public float DistanceToTarget => _currentTarget == null ? float.MaxValue : Vector3.Distance(transform.position, _currentTarget.transform.position);
		public bool UseStationTargets => _useStationTargets;
		public GUIDComponent GUIDComponent => _gUIDComponent;

		/// <summary>
		/// Returns the Target Mask.
		/// </summary>
		public TargetMask TargetMask
		{
			get { return _targetMask; }
			set { SetTargetMask(value); }
		}

		public override void STUpdate()
		{
			base.STUpdate();

			// If we should be updated the target, check that we don't have a target or need a new target.
			if (HasTarget && !_currentTarget.gameObject.activeInHierarchy)
				_currentTarget = null;

			if (_useStationTargets && ((UpdateTarget && _currentTarget == null && _stationSensor.HasStation) || (_currentTarget != null && !_currentTarget.gameObject.activeInHierarchy)))
				GetTarget();

			// Check if the current target is the wrong target type and get a new target.
			else if (_currentTarget != null && !_currentTarget.TargetType.HasFlag(_targetMask))
				GetTarget();

			else if (_currentTarget == null)
				GetTarget();
		}

		/// <summary>
		/// Marks the current target as bad, informing the station to remove this from the valid target's list.
		/// </summary>
		public void MarkCurrentTargetBad()
		{
			if (_stationSensor == null || _currentTarget == null || _stationSensor.CurrentStation == null)
				return;

			_stationSensor.CurrentStation.MarkTargetInvald(_currentTarget);
			ClearTarget();
		}

		/// <summary>
		/// Clears the current target to null.
		/// </summary>
		public void ClearTarget()
		{
			UpdateTarget = true;
			_currentTarget = null;
			OnTargetChanged();
		}

		/// <summary>
		/// Attempts to set the current target.
		/// </summary>
		/// <param name="target"></param>
		/// <returns>true if successful.</returns>
		public bool TrySetTarget(Targetable target)
		{
			_currentTarget = target;
			OnTargetChanged();
			return true;
		}

		public bool TrySetTarget(Targetable target, Player player)
		{
			if(player.TargetSensor.TargetMask.HasFlag(target.TargetType))
			{
				_currentTarget = target;
				OnTargetChanged();
				Debug.Log($" Set {player.RoleHandler.CurrentRole}'s target to {target.TargetType}");

				return true;
			}

			Debug.Log($"Can't set {player.RoleHandler.CurrentRole}'s target to {target.TargetType}");
			return false;
		}

		protected override void Init()
		{
			base.Init();
			_stationSensor = GetComponent<StationSensor>();
			_gUIDComponent = GetComponent<GUIDComponent>();
		}

		/// <summary>
		/// Attempts to get a new target from the station.
		/// </summary>
		private void GetTarget()
		{
			if (!CurrentTargetValid())
			{
				if (_useStationTargets)
				{
					if (_stationSensor.CurrentStation != null)
						_stationSensor.CurrentStation.GetBestScoredTarget(transform.position, _targetMask, ref _currentTarget);
				}
				else
				{
					GetNearestTarget();
				}
			}

			OnTargetChanged();
		}

		private void GetNearestTarget()
		{
			List<Targetable> validTargets = new List<Targetable>();

			CSPManager.GetTargetablesInRange(_targetMask, transform.position, _targetSearchRange, ref validTargets);

			// Get closest target
			float closestDistSqr = float.MaxValue;
			Targetable closestTarget = null;
			float distance = 0;

			for (int i = 0; i < validTargets.Count; i++)
			{
				if (!validTargets[i].gameObject.activeInHierarchy)
					continue;

				distance = Vector3.SqrMagnitude(validTargets[i].transform.position - transform.position);

				if (distance < closestDistSqr)
				{
					closestDistSqr = distance;
					closestTarget = validTargets[i];
				}
			}

			if (closestTarget != null)
			{
				_currentTarget = closestTarget;
			}
		}

		/// <summary>
		/// Returns true if the current target is a valid target.
		/// </summary>
		/// <returns></returns>
		private bool CurrentTargetValid()
		{
			// If the target is not null, enabled and the correct flag, then the target is valid.
			if (_currentTarget != null && _currentTarget.gameObject.activeInHierarchy && _targetMask.HasFlag(_currentTarget.TargetType))
				return true;
			else
				return false;
		}

		/// <summary>
		/// Sets the current Target Mask of the unit.
		/// </summary>
		/// <param name="type"></param>
		private void SetTargetMask(TargetMask type)
		{
			_targetMask = type;

			ClearTarget();
		}

		/// <summary>
		/// Called when the current target was changed.
		/// </summary>
		private void OnTargetChanged()
		{
			if (_currentTarget != _previousTarget)
			{
				if (_previousTarget != null)
					_previousTarget.UnassignFromTarget();

				_onTargetChange.Invoke();
				_previousTarget = _currentTarget;

				if (_currentTarget != null)
					_currentTarget.AssignToTarget();
			}
		}

		private void OnAttacked(Targetable target)
		{
			if (!_attackAttacker || target == null)
				return;

			// If its a valid target for our target mask, focus it.
			if (_targetMask.HasFlag(target.TargetType))
				_currentTarget = target;
		}
		// Unity Functions.
		private void OnDisable()
		{
			ClearTarget();
		}

		private void Start()
		{
			UpdateTarget = true;

			if (TryGetComponent(out HealthHandler h))
			{
				h.OnTookDamage += OnAttacked;
			}
		}
	}
}