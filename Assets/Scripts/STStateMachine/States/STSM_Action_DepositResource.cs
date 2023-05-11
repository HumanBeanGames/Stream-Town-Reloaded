using Behaviours;
using Character;
using Scriptables;
using UnityEngine;
using Utils;

namespace STStateMachine.States
{
	/// <summary>
	/// A state that allows units to deposit resources into town storage.
	/// </summary>
	public class STSM_Action_DepositResource : STStateBase
	{
		[SerializeField]
		private float _depositTime = 2.5f;
		private float _depositTimer = 0;

		private PlayerInventory _playerInventory;
		private DepositResources _depositResources;
		private RoleDataScriptable _roleData;
		private RoleHandler _roleHandler;

		public override void OnEnter()
		{
			base.OnEnter();
			_roleData = _roleHandler.RoleData_SO;
			_depositTimer = 0;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			_depositTimer += Time.deltaTime;

			// If enough time has elapsed, deposit resources, this is so that it isn't instant.
			if (_depositTimer >= _depositTime)
			{
				Utils.Resource resource = _roleData.Resource;
				int amount = _playerInventory.ResourceCount(resource);
				_stateMachine.SetInterruptable(true);

				_depositResources.Deposit(resource, amount);
				_playerInventory.RemoveResource(resource, amount);
				_roleHandler.EquipmentHandler.DisableCarry(_roleHandler.CurrentRole);
				_stateMachine.RequestStateChange("Idle");
			}
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		protected override void OnInit()
		{
			base.OnInit();
			_playerInventory = GetComponent<PlayerInventory>();
			_depositResources = GetComponent<DepositResources>();
			_roleHandler = GetComponent<RoleHandler>();
		}
	}
}