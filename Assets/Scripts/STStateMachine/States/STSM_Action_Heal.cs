using Behaviours;
using Units;
using UnityEngine;

namespace STStateMachine.States
{
	/// <summary>
	/// A state that handles units healing other units.
	/// </summary>
	public class STSM_Action_Heal : STSM_Action_PlayerBase
	{
		protected HealUnit _healUnit;
		protected HealthHandler _friendlyHealth;

		protected override void OnInit()
		{
			base.OnInit();
			_healUnit = GetComponent<HealUnit>();
		}

		protected override bool DoAction()
		{
			// Check if the unit is within range of the target.
			if (Vector3.SqrMagnitude(_target.transform.position - transform.position) > (_actionRange * 2) + _target.SizeSqr)
			{
				_stateMachine.RequestStateChange("Idle");
				return false;
			}

			//TODO: This could be merged for all do actions as its used in all/most
			// Check that the target isn't null or disabled.
			if (_target == null || !_target.gameObject.activeInHierarchy)
			{
				_stateMachine.RequestStateChange("Idle");
				return false;
			}

			// Check that the target has a health handler.
			if (_friendlyHealth == null || _friendlyHealth.gameObject != _target.gameObject)
				_friendlyHealth = _target.GetComponent<HealthHandler>();

			_friendlyHealth.ModHealth(_actionAmount);

			// Check if the target's health is full.
			if (_friendlyHealth.HealthPercentage == 1)
			{
				_targetSensor.ClearTarget();
				_stateMachine.RequestStateChange("Idle");
			}

			return true;
		}
	}
}