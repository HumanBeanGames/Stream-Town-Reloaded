using Behaviours;
using STStateMachine.Helpers;
using Units;
using UnityEngine;

namespace STStateMachine.States
{
	public class STSM_Action_PlayerAttack : STSM_Action_PlayerBase
	{
		//TODO:: Integrate AttackUnit and HealUnit properly (maybe?).
		protected AttackUnit _attackUnit;
		protected HealthHandler _targetHealth;
		protected STSM_Helper_Attack _attackHelper;

		public override void OnEnter()
		{
			base.OnEnter();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		protected override void OnInit()
		{
			base.OnInit();

			_attackUnit = GetComponent<AttackUnit>();
			_attackHelper = (STSM_Helper_Attack)_stateMachine.GetHelperByName("Attack");
		}

		protected override bool DoAction()
		{
			// Check if unit is within range to attack.
			// TODO:: may need to combine these into the base DoAction.
			if (Vector3.SqrMagnitude(_target.transform.position - transform.position) > (_actionRange * 2.5) + _target.SizeSqr)
			{
				_stateMachine.RequestStateChange("Idle");
				return false;
			}

			_attackHelper.Damage = _actionAmount;
			_attackHelper.Target = _target;
			_attackHelper.InvokeHelper();

			return true;
		}
	}
}