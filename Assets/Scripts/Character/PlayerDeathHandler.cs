using UnityEngine;
using STStateMachine;
using Pathfinding;
using Animation;
using Utils;
using Units;

namespace Character
{
	/// <summary>
	/// Used to handle what should happen when a player dies and revives.
	/// </summary>
	public class PlayerDeathHandler : MonoBehaviour
	{
		[SerializeField]
		private float _reviveTime = (60 * 1);
		private float _reviveCounter = 0;
		private bool _reviveActive = false;

		private StateMachine _stateMachine;
		private AIPath _aIPath;
		private Vector3 _spawnPosition;
		private AnimationHandler _animationHandler;
		private HealthHandler _healthHandler;

		/// <summary>
		/// Called when the player dies.
		/// </summary>
		public void OnDeath()
		{
			if (_stateMachine == null || _aIPath == null || _reviveActive)
				return;

			_stateMachine.enabled = false;
			_aIPath.enabled = false;
			_animationHandler.SetTrigger(AnimationName.Death);
			_reviveCounter = 0;
			_reviveActive = true;
		}

		/// <summary>
		/// Called when the player revives.
		/// </summary>
		public void OnRevive()
		{
			_stateMachine.enabled = true;
			_aIPath.enabled = true;
			_stateMachine.RequestStateChange(_stateMachine.GetStateByName("Idle"));
			_animationHandler.SetTrigger(AnimationName.Revive);
			_reviveActive = false;
			_reviveCounter = 0;
			transform.position = _spawnPosition;
		}

		// Unity Events.
		private void Awake()
		{
			_stateMachine = GetComponent<StateMachine>();
			_aIPath = GetComponent<AIPath>();
			_animationHandler = GetComponentInChildren<AnimationHandler>();
			_healthHandler = GetComponent<HealthHandler>();
		}

		private void Start()
		{
			_spawnPosition = transform.position;
		}

		private void Update()
		{
			if (!_reviveActive)
				return;

			_reviveCounter += Time.deltaTime;
			if (_reviveCounter >= _reviveTime)
			{
				_reviveCounter = 0;
				_healthHandler.Revive();
			}
		}
	}
}