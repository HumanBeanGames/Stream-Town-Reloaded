using Behaviours;
using Character;
using GameResources;
using Pets.Enumerations;
using STStateMachine.Helpers;
using Twitch;
using UnityEngine;
using Utils;

namespace STStateMachine.States
{
	/// <summary>
	/// An action state that allows a unit to gather resources.
	/// </summary>
	public class STSM_Action_GatherResource : STSM_Action_PlayerBase
	{
		protected CollectResource _collectResource;
		protected PlayerInventory _playerInventory;
		protected ResourceHolder _resourceHolder;
		protected STSM_HelperDeposit _helperDeposit;

		public override void OnEnter()
		{
			base.OnEnter();

			_resourceHolder = _target.GetComponent<ResourceHolder>();

			if (_resourceHolder.ResourceType == Utils.Resource.None)
				_stateMachine.RequestStateChange("Idle");
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		protected override bool DoAction()
		{
			// If the player's resource inventory is full, go to deposit.
			if (_playerInventory.ResourceFull(_resourceHolder.ResourceType))
			{
				_stateMachine.InvokeHelper(_helperDeposit);
				return false;
			}
			// if the target is null or disabled, go to deposit.
			else if (_target == null || !_target.gameObject.activeInHierarchy)
			{
				_stateMachine.InvokeHelper(_helperDeposit);
				return false;
			}
			// otherwise collect the resource and add it to the resource inventory.
			else
			{
				_playerInventory.AddResource(_resourceHolder.ResourceType, _collectResource.Collect(_target, _actionAmount));
				return true;
			}
		}

        protected override void OnActionSuccess()
        {
            base.OnActionSuccess();
            // This needs to be redone, temporary implementation
            if ( !_roleHandler.Player.IsNPC)
            {
                int rand = Random.Range(0, 5000);
                if (rand == 0)
                {
					if (_actionAnimation == AnimationName.Gathering)
					{

						if (_roleHandler.Player.PetsUnlocked[PetType.Giraffe])
							return;

						_roleHandler.Player.PetsUnlocked[PetType.Giraffe] = true;
						MessageSender.SendMessage($"{_roleHandler.Player.TwitchUser.Username} unlocked the giraffe pet!");
					}
					if(_actionAnimation == AnimationName.Fishing)
					{
						if (_roleHandler.Player.PetsUnlocked[PetType.Duck])
							return;

						_roleHandler.Player.PetsUnlocked[PetType.Duck] = true;
						MessageSender.SendMessage($"{_roleHandler.Player.TwitchUser.Username} unlocked the duck pet!");
					}
					if (_actionAnimation == AnimationName.WoodCutting)
					{
						if (_roleHandler.Player.PetsUnlocked[PetType.Butterfly])
							return;

						_roleHandler.Player.PetsUnlocked[PetType.Butterfly] = true;
						MessageSender.SendMessage($"{_roleHandler.Player.TwitchUser.Username} unlocked the butterfly pet!");
					}
				}
            }
        }

        protected override void OnInit()
		{
			base.OnInit();

			_collectResource = GetComponent<CollectResource>();
			_helperDeposit = (STSM_HelperDeposit)_stateMachine.GetHelperByName("Deposit");
			_playerInventory = GetComponent<PlayerInventory>();
		}
	}
}