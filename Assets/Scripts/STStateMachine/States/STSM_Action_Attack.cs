using STStateMachine.Helpers;
using UnityEngine;

namespace STStateMachine.States
{
	/// <summary>
	/// Base action state for attacks.
	/// </summary>
	public class STSM_Action_Attack : STSM_StateAction
	{
		protected int _damage;
		protected STSM_Helper_Attack _attackHelper;

		public override void OnEnter()
		{
			base.OnEnter();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
		}

		protected override void OnInit()
		{
			base.OnInit();
			_attackHelper = (STSM_Helper_Attack)_stateMachine.GetHelperByName("Attack");
		}

		protected override bool DoAction()
		{
			// If the unit doesn't have a target, swap to idle.
			if (_target != _targetSensor.CurrentTarget)
			{
				_stateMachine.RequestStateChange("Idle");
				return false;
			}

			// If the unit is not within range of their target, swap to idle.
			if (Vector3.SqrMagnitude(_target.transform.position - transform.position) > (_actionRange * 2) + _target.SizeSqr)
			{
				_stateMachine.RequestStateChange("Idle");
				return false;
			}

			_attackHelper.Target = _target;
			_attackHelper.Damage = _actionAmount;
			_attackHelper.InvokeHelper();
			return true;
		}

		public override void OnExit()
		{
			base.OnExit();
		}
	}
}