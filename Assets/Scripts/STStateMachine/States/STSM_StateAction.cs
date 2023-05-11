using Pathfinding;
using UnityEngine;
using System;
using Sensors;
using Target;
using Behaviours;
using Animation;
using Utils;

namespace STStateMachine.States
{
	/// <summary>
	/// Base class for all Character action states.
	/// </summary>
	public class STSM_StateAction : STStateBase
	{
		[SerializeField, Header("Can be set, for players it is automatically set.")]
		protected float _actionRate;
		[SerializeField, Header("Can be set, for players it is automatically set.")]
		protected int _actionAmount;
		[SerializeField]
		protected float _actionRange;
		[SerializeField]
		protected bool _useAnimation = true;
		[SerializeField]
		protected AnimationName _actionAnimation;
		[SerializeField]
		protected int _actionVariants;
		protected float _actionTimer;
		protected AIPath _aiPath;
		protected Targetable _target;
		protected TargetSensor _targetSensor;
		protected RotationHandler _targetLookAt;
		protected AnimationHandler _animationHandler;

		public float Range => _actionRange;

		public override void OnEnter()
		{
			if (_targetSensor.CurrentTarget == null)
			{
				((STSM_Idle)_stateMachine.GetStateByName("Idle")).NewPositionOnEnter = true;
				_stateMachine.RequestStateChange("Idle");
				return;
			}

			_targetSensor.CurrentTarget.TryGetComponent(out _target);

			if (_target == null)
				_stateMachine.RequestStateChange("Idle");

			_aiPath.enabled = false;
			_aiPath.canMove = false;
			_actionTimer = 0;

			if (_useAnimation)
			{
				_animationHandler.SetBool(AnimationName.Action, true);
				_animationHandler.SetTrigger(_actionAnimation);
				_animationHandler.SetAttackAnimationIndex(UnityEngine.Random.Range(0, _actionVariants));
			}
			_targetLookAt.LookAtTarget = true;
		}

		public override void OnUpdate()
		{
			_actionTimer += Time.deltaTime;

			if (_actionTimer >= _actionRate)
			{
				_actionTimer -= _actionRate;
				if (_useAnimation)
				{
					_animationHandler.SetAttackAnimationIndex(UnityEngine.Random.Range(0, _actionVariants));
				}

				if (DoAction())
					OnActionSuccess();
			}
		}

		public override void OnExit()
		{
			_aiPath.enabled = true;
			_aiPath.canMove = true;
			if (_useAnimation)
				_animationHandler.SetBool(AnimationName.Action, false);
			_targetLookAt.LookAtTarget = false;
		}

		protected override void OnInit()
		{
			base.OnInit();
			_aiPath = GetComponent<AIPath>();
			_targetSensor = GetComponent<TargetSensor>();
			_targetLookAt = GetComponent<RotationHandler>();
			_animationHandler = GetComponentInChildren<AnimationHandler>();
		}

		/// <summary>
		/// Called when an Action was successfully run.
		/// </summary>
		protected virtual void OnActionSuccess()
		{

		}

		/// <summary>
		/// Calls the desired action to run.
		/// </summary>
		/// <returns></returns>
		protected virtual bool DoAction()
		{
			throw new NotImplementedException();
		}
	}
}