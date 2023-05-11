using Pathfinding;
using Sensors;
using UnityEngine;

namespace STStateMachine.States
{
	/// <summary>
	/// Generice Idle State for a simple idle action.
	/// </summary>
	public class STSM_Idle : STStateBase
	{
		[SerializeField]
		protected float _newLocationTime = 3f;
		protected float _newLocationTimer = 0f;

		[SerializeField]
		protected float _idleRange = 10f;

		[SerializeField]
		protected float _checkRate = 0.5f;
		protected float _checkTimer = 0;

		[SerializeField]
		protected bool _useWorldCenterPos = false;

		protected STSM_GoToLocation _goToState;

		protected TargetSensor _targetSensor;

		protected AIPath _aiPath;
		protected Vector3 _idleStartPosition;

		protected StationSensor _stationSensor;

		/// <summary>
		/// If true, the unit will pick a new position to go to.
		/// </summary>
		public bool NewPositionOnEnter { get; set; }

		public override void OnEnter()
		{
			base.OnEnter();
			_idleStartPosition = transform.position;

			_checkTimer = 0;

			if (NewPositionOnEnter)
			{
				_newLocationTimer = _newLocationTime;
				NewPositionOnEnter = false;
				_checkTimer = _checkRate;
			}
			else
				_newLocationTimer = 0;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			_checkTimer += Time.deltaTime;
			_newLocationTimer += Time.deltaTime;

			// If enough time has elapsed, check if the unit has a target
			if (_checkTimer >= _checkRate && _targetSensor.HasTarget)
			{
				_checkTimer -= _checkRate;
				OnHasTarget();
			}

			// If enough time has elapsed, get a new location to move to and idle.
			else if (_newLocationTimer >= _newLocationTime)
			{
				OnNewIdleLocation();
			}

		}

		protected override void OnInit()
		{
			base.OnInit();
			_targetSensor = GetComponent<TargetSensor>();
			_aiPath = GetComponent<AIPath>();
			_goToState = (STSM_GoToLocation)_stateMachine.GetStateByName("GoTo");
			_stationSensor = GetComponent<StationSensor>();
			NewPositionOnEnter = true;
		}

		protected virtual void OnHasTarget()
		{
			// Overriden in inherited classes.
		}

		/// <summary>
		/// Gets a new position within a circle sized based on the idle range.
		/// </summary>
		protected virtual void OnNewIdleLocation()
		{
			//TODO:: Implement height when terrain is added
			if (_targetSensor.UseStationTargets)
			{
				if (_stationSensor.CurrentStation != null)
					_idleStartPosition = _stationSensor.CurrentStation.transform.position;
			}
			else if (_useWorldCenterPos)
				_idleStartPosition = Vector3.zero;
			else
				_idleStartPosition = transform.position;

			Vector2 insideCircle = Random.insideUnitCircle * _idleRange;

			_goToState.UsePosition = true;
			_goToState.SetNextState(this);
			_goToState.SetTargetPosition(new Vector3(insideCircle.x + _idleStartPosition.x, 0, insideCircle.y + _idleStartPosition.z));
			_stateMachine.RequestStateChange(_goToState);
		}
	}
}