using Character;
using GUIDSystem;

using Pets.Enumerations;
using SavingAndLoading.Structs;
using System.Collections.Generic;
using Target;
using Utils.Pooling;

namespace SavingAndLoading.SavableObjects
{
	public class SaveablePlayer : SaveableObject
	{
		public RoleHandler RoleHandler;

		public override void LoadData(object data)
		{
			// Done in SaveManager
		}

		public override object SaveData()
		{
			List<PlayerRoleSaveData> roleData = PlayerRoleSaveData.ToPlayerRoleSaveDatas(RoleHandler.Player.RoleHandler.PlayerRolesData);
			PetType currentPet = RoleHandler.Player.Pet.ActivePetType;
			List<PetType> unlockedPets = RoleHandler.Player.GetUnlockedPetTypes();
			return (object)new PlayerSaveData(RoleHandler.Player.TwitchUser.UserID
				, RoleHandler.Player.TwitchUser.Username
				, RoleHandler.Player.TwitchUser.TwitchUserType
				, RoleHandler.Player.TwitchUser.GameUserType
				, RoleHandler.Player.TwitchUser.IsBroadcaster
				, GameManager.Instance.GUIDManager.CreateGUIDandAddToDictionary(PoolableObject)
				, RoleHandler.Player.Pet.IsActive
				, currentPet
				, unlockedPets
				, new TransformSaveData(RoleHandler.Player.Character.transform)
				, RoleHandler.Player.RoleHandler.CurrentRole
				, RoleHandler.Player.RoleHandler.PreviousRole
				, roleData, new InventorySaveData(RoleHandler.Player.RoleHandler.Inventory.Resources)
				, RoleHandler.Player.EquipmentHandler.ToSaveData()
				, RoleHandler.Player.HealthHandler.Health
				, RoleHandler.Player.HealthHandler.RegenRequiresFood) ;
		}

		public void SetVariables(Targetable target, GUIDComponent component, string poolName, PoolableObject poolableObject, RoleHandler roleHandler )
		{
			RoleHandler = roleHandler;
			base.SetVariables(target, component, poolName, poolableObject);
		}
	}
}