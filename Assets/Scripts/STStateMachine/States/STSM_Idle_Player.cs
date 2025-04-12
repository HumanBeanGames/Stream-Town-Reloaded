using Character;

using STStateMachine.Helpers;
using UnityEngine;
using Utils;

namespace STStateMachine.States
{
	/// <summary>
	/// Idle state logic for Player characters.
	/// </summary>
	public class STSM_Idle_Player : STSM_Idle
	{
		private RoleHandler _roleHandler;
		private PlayerInventory _inventory;

		// State References.
		private STSM_Action_GatherResource _gatherResourceAction;
		private STSM_Action_PlayerAttack _attackAction;
		private STSM_Action_Heal _healAction;
		private STSM_HelperDeposit _helperDeposit;

		/// <summary>
		/// Initialize all required data.
		/// </summary>
		protected override void OnInit()
		{
			base.OnInit();
			_roleHandler = GetComponent<RoleHandler>();
			_inventory = GetComponent<PlayerInventory>();
			_gatherResourceAction = (STSM_Action_GatherResource)_stateMachine.GetStateByName("GatherResource");
			_attackAction = (STSM_Action_PlayerAttack)_stateMachine.GetStateByName("Attack");
			_healAction = (STSM_Action_Heal)_stateMachine.GetStateByName("Heal");
			_helperDeposit = (STSM_HelperDeposit)_stateMachine.GetHelperByName("Deposit");
		}

		/// <summary>
		/// Called periodically when the player has a target. 
		/// </summary>
		protected override void OnHasTarget()
		{
			base.OnHasTarget();

			bool success = false;

			// Set next state based on role type.
			switch (_roleHandler.RoleData_SO.RoleFlags)
			{
				case PlayerRoleType.Resource:
					success = ResourceRole();
					break;
				case PlayerRoleType.Damage:
					success = DamageRole();
					break;
				case PlayerRoleType.Healer:
					success = HealerRole();
					break;
				case PlayerRoleType.Other:
					success = OtherRole();
					break;
			}

			if (success)
			{
				_goToState.UsePosition = false;
				_goToState.SetTarget(_targetSensor.CurrentTarget.transform);
				_goToState.SetDistanceSatisfaction(_targetSensor.CurrentTarget.SizeSqr + (_roleHandler.PlayerRoleData.ActionRange * _roleHandler.PlayerRoleData.ActionRange));
				_stateMachine.RequestStateChange(_goToState);
			}
		}

		/// <summary>
		/// Returns true if player can gather resources.
		/// </summary>
		/// <returns></returns>
		protected bool ResourceRole()
		{
			//Check inventory is not full and town resource isnt full
			Utils.Resource resourceType = _roleHandler.RoleData_SO.Resource;

			if (GameManager.Instance.TownResourceManager.ResourceFull(resourceType))
				return false;

			if (_inventory.ResourceFull(resourceType))
			{
				_stateMachine.InvokeHelper(_helperDeposit);
				return false;
			}

			_goToState.SetNextState(_gatherResourceAction);
			return true;
		}

		/// <summary>
		/// Returns true after setting next state to attack action.
		/// </summary>
		/// <returns></returns>
		protected bool DamageRole()
		{
			_goToState.SetNextState(_attackAction);
			return true;
		}

		/// <summary>
		/// Returns true after setting next state to heal action.
		/// </summary>
		/// <returns></returns>
		protected bool HealerRole()
		{
			_goToState.SetNextState(_healAction);
			return true;
		}

		/// <summary>
		/// Returns false, used for unhandled roles.
		/// </summary>
		/// <returns></returns>
		protected bool OtherRole()
		{
			Debug.LogWarning($"Role type behaviour not handlded.");
			return false;
		}
	}
}