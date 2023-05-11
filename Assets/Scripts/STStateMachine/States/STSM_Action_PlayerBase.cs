using Character;
using Scriptables;
using UnityEngine;
using Utils;

namespace STStateMachine.States
{
	/// <summary>
	/// Base state for any player actions.
	/// </summary>
	public class STSM_Action_PlayerBase : STSM_StateAction
	{
		protected RoleHandler _roleHandler;
		protected RoleDataScriptable _roleData;

		protected override void OnInit()
		{
			base.OnInit();

			_roleHandler = GetComponent<RoleHandler>();
		}

		public override void OnEnter()
		{
			_roleData = _roleHandler.RoleData_SO;
			_actionAnimation = _roleData.ActionAnimationName;

			base.OnEnter();

			// set the action variables to be correct on switch.
			_actionAmount = _roleHandler.PlayerRoleData.ActionAmount;
			_actionRate = _roleHandler.PlayerRoleData.ActionRate;
			_actionRange = _roleHandler.PlayerRoleData.ActionRange;
			_animationHandler.SetActionSpeed(Mathf.Max(1,MathExtended.RemapValue(_actionRate, 1, 0, 1, 3)));
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
		}

		public override void OnExit()
		{
			base.OnExit();
			_roleHandler.AnimationHandler.SetBool(AnimationName.Action, false);
			((STSM_Idle)_stateMachine.GetStateByName("Idle")).NewPositionOnEnter = true;
		}

		protected override void OnActionSuccess()
		{
			_roleHandler.PlayerRoleData.IncreaseExperience(_actionAmount);
		}
	}
}