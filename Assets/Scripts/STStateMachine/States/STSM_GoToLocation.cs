using Pathfinding;
using Sensors;
using UnityEngine;

namespace STStateMachine.States
{
	/// <summary>
	/// A simple state action that allows a unit to move to a new location or target.
	/// </summary>
	public class STSM_GoToLocation : STStateBase
	{
		/// <summary>
		/// Minimum distance a unit has to move within the check rate time to be considered NOT stuck
		/// </summary>
		private const float MINIMUM_STUCK_DISTANCE_CHECK_SQR = 1.0f;

		/// <summary>
		/// Max Number of consecutive stuck true checks until the unit is dealt with
		/// </summary>
		private const int MAX_CONSECUTIVE_COUNT = 8;

		/// <summary>
		/// How often the unit will poll a stuck check
		/// </summary>
		private const float STUCK_DISTANCE_CHECK_RATE = 1.0f;

		/// <summary>
		/// Default satisfaction distance if there is no given distance satisfaction.
		/// Stops the unit from walking inside of the target.
		/// </summary>
		private const float ZERO_VECTOR_SATISFACTION_DISTANCE = 36.0f;

		[SerializeField]
		private float _distanceSatisfaction;
		private Vector3 _targetPosition;
		private Transform _targetTransform;
		private AIPath _aiPath;
		private STStateBase _nextState;
		private Vector3 _destination;
		private TargetSensor _targetSensor;
		private float _stuckCheckTimer;
		private Vector3 _lastStuckCheckPos;
		private bool _stuck = false;
		private int _stuckConsecutiveCount = 0;
		private float _prevSlowDownDistance = 0;
		private Vector3 _onEnabledLocation = Vector3.zero;

		public bool UsePosition { get; set; }

		/// <summary>
		/// Sets the next state the unit will enter once they've reached their location.
		/// </summary>
		/// <param name="state"></param>
		public void SetNextState(STStateBase state)
		{
			_nextState = state;
		}

		/// <summary>
		/// Sets the distance in which the unit will be satisfied and consider their destination as "reached".
		/// </summary>
		/// <param name="distance"></param>
		public void SetDistanceSatisfaction(float distance)
		{
			_distanceSatisfaction = distance;
		}

		/// <summary>
		/// Sets the target position of the unit.
		/// </summary>
		/// <param name="position"></param>
		public void SetTargetPosition(Vector3 position)
		{
			//Bias position towards Unit
			Vector3 direction = (position - transform.position).normalized;

			_targetPosition = AstarPath.active.GetNearest(position + direction, NNConstraint.Default).position;
			_aiPath.destination = position;
		}

		/// <summary>
		/// Sets the target to travel to.
		/// </summary>
		/// <param name="target"></param>
		public void SetTarget(Transform target)
		{
			_targetTransform = target;
			_aiPath.destination = target.position;
		}

		protected override void OnInit()
		{
			base.OnInit();
			_aiPath = GetComponent<AIPath>();
			_prevSlowDownDistance = _aiPath.slowdownDistance;
			_targetSensor = GetComponent<TargetSensor>();
		}

		public override void OnEnter()
		{
			_aiPath.enabled = true;
			_distanceSatisfaction += (UsePosition && _targetPosition == Vector3.zero ? ZERO_VECTOR_SATISFACTION_DISTANCE : 0);
			_stuckCheckTimer = 0;
			_lastStuckCheckPos = transform.position;

			//Check path is possible to point otherwise mark it as bad
			if (!UsePosition)
			{
				GraphNode a = AstarPath.active.GetNearest(transform.position, NNConstraint.Default).node;
				GraphNode b = AstarPath.active.GetNearest(_targetTransform.position, NNConstraint.Default).node;

				if (!PathUtilities.IsPathPossible(a, b) || a == null || b == null)
				{
					Debug.Log($"Path wasn't possible from {transform.gameObject.name} to {_targetTransform.gameObject.name}");
					_targetSensor.MarkCurrentTargetBad();

					b = AstarPath.active.GetNearest(Vector3.zero, NNConstraint.Default).node;
					if (!PathUtilities.IsPathPossible(a, b) || a == null || b == null)
					{
						transform.position = _onEnabledLocation;
					}

					_stateMachine.RequestStateChange("Idle");
				}
			}
		}

		public override void OnUpdate()
		{
			base.OnUpdate();

			// If we are using the target's position and the target is null, go to the idle state.
			if (!UsePosition && _targetTransform == null)
			{
				((STSM_Idle)_stateMachine.GetStateByName("Idle")).NewPositionOnEnter = true;
				_stateMachine.RequestStateChange("Idle");
				return;
			}

			// If we are using the target's position, set destination to target, otherwise set it to specified position.
			_destination = UsePosition ? _targetPosition : _targetTransform.position;
			_aiPath.destination = _destination;

			// Check if the unit is within range of the target location, and switch to the next state.
			float sqr = Vector3.SqrMagnitude(_aiPath.destination - transform.position);
			if (sqr <= _distanceSatisfaction)
			{
				_stateMachine.RequestStateChange(_nextState, true);
				_stuck = false;
			}

			// Check if the unit is stuck.
			StuckCheck();
		}

		public override void OnExit()
		{
			base.OnExit();
			_targetTransform = null;
			_distanceSatisfaction = 1;
			_prevSlowDownDistance = _aiPath.slowdownDistance;
		}

		/// <summary>
		/// Checks if the unit is stuck and resets their position.
		/// </summary>
		// TODO:: Fix the detection and resolution. Not working correctly.
		private void StuckCheck()
		{
			// Check if the unit has been stuck through multiple checks and reset their position.
			if (_stuckConsecutiveCount > MAX_CONSECUTIVE_COUNT)
			{
#if UNITY_EDITOR
				Debug.DrawLine(transform.position, transform.position + Vector3.up * 20, Color.red);
#endif
				transform.position = _onEnabledLocation;
				_stuckConsecutiveCount = 0;
				_stuck = false;
			}

			_stuckCheckTimer += Time.deltaTime;

			// If enough time has elapsed, check if the unit might be stuck.
			if (_stuckCheckTimer >= STUCK_DISTANCE_CHECK_RATE)
			{
				// If the unit has not moved enough over the check duration, it might be stuck.
				if (Vector3.SqrMagnitude(_lastStuckCheckPos - transform.position) < MINIMUM_STUCK_DISTANCE_CHECK_SQR)
				{
					_stuck = true;
					_stuckConsecutiveCount++;
				}
				else
				{
					_stuckConsecutiveCount = 0;
					_stuck = false;
				}

				_stuckCheckTimer -= STUCK_DISTANCE_CHECK_RATE;
				_lastStuckCheckPos = transform.position;
				_stateMachine.RequestStateChange("Idle");
			}
		}

		// Unity Functions.
		private void OnDrawGizmos()
		{
			if (_stuck)
			{
				Debug.DrawLine(transform.position, transform.position + (Vector3.up * 5), Color.black);
			}
		}

		private void OnEnable()
		{
			_onEnabledLocation = transform.position;
		}
	}
}