using Scriptables;
using UnityEngine;
using Utils;

namespace Character
{
	/// <summary>
	/// Holds all current role data for a player character's role.
	/// </summary>
	[System.Serializable]
	public class RoleData
	{
		public string RoleName;
		public PlayerRole Role;
		public PlayerRoleType RoleFlags;
		public TargetMask TargetFlags;
		public Utils.Resource Resource;
		public AnimationName ActionAnimationName;
		public int ActionAnimationVariants;
		public StationMask StationFlags;
		public int BaseActionAmount;
		public float BaseActionSpeed;
		public float BaseActionRange;
		public int BaseRoleLevel;
		public float BaseFoodUpkeep;
		public float BaseGoldUpkeep;
		public int BaseHealth;
		public int BaseHealthRegen;
		public int BaseDamageReduction;
		public int BaseMovementSpeed;
		public int BaseMaxResource;
		public float MaxResourcePerLevel;
		public float MovementSpeedPerLevel;
		public float ActionAmountPerLevel;
		public float ActionSpeedPerLevel;
		public float ActionRangePerLevel;
		public float HealthPerLevel;
		public float HealthRegenPerLevel;
		public float DamageReductionPerLevel;
		public float GlobalActionPerLevel;
		public float GlobalActionSpeedPerLevel;
		public float GlobalActionRangePerLevel;
		public float GlobalMovementSpeedPerLevel;
		public float GlobalHealthPerLevel;
		public float GlobalHealthRegenPerLevel;
		public float GlobalResourceCarryPerLevel;
		public float GlobalDamageReductionPerLevel;
		public bool HasUserLimit;
		public int BaseMaxUserLimit;
		public int CurrentRoleCount;
		public Sprite DisplayIcon;
		public AudioClip[] ActionClips;

		public RoleData(RoleDataScriptable e)
		{
			Role = e.Role;
			RoleFlags = e.RoleFlags;
			TargetFlags = e.TargetFlags;
			StationFlags = e.StationFlags;
			ActionAnimationName = e.ActionAnimationName;
			ActionAnimationVariants = e.ActionAnimationVariants;
			Resource = e.Resource;
			BaseActionAmount = e.BaseActionAmount;
			BaseActionSpeed = e.BaseActionSpeed;
			BaseActionRange = e.BaseActionRange;
			BaseRoleLevel = e.BaseRoleLevel;
			BaseFoodUpkeep = e.BaseFoodUpkeep;
			BaseGoldUpkeep = e.BaseGoldUpkeep;
			BaseMaxResource = e.BaseMaxResource;
			MaxResourcePerLevel = e.MaxResourcePerLevel;
			ActionAmountPerLevel = e.ActionAmountPerLevel;
			ActionSpeedPerLevel = e.ActionSpeedPerLevel;
			ActionRangePerLevel = e.ActionRangePerLevel;
			HealthPerLevel = e.HealthPerLevel;
			HealthRegenPerLevel = e.HealthRegenPerLevel;
			DamageReductionPerLevel = e.DamageReductionPerLevel;
			GlobalActionPerLevel = e.GlobalActionPerLevel;
			GlobalActionSpeedPerLevel = e.GlobalActionSpeedPerLevel;
			GlobalActionRangePerLevel = e.GlobalActionRangePerLevel;
			GlobalMovementSpeedPerLevel = e.GlobalMovementSpeedPerLevel;
			GlobalHealthPerLevel = e.GlobalHealthPerLevel;
			GlobalHealthRegenPerLevel = e.GlobalHealthRegenPerLevel;
			GlobalResourceCarryPerLevel = e.GlobalResourceCarryPerLevel;
			GlobalDamageReductionPerLevel = e.GlobalDamageReductionPerLevel;
			HasUserLimit = e.HasUserLimit;
			BaseMaxUserLimit = e.BaseMaxUserLimit;
			BaseHealth = e.BaseHealth;
			BaseHealthRegen = e.BaseHealthRegen;
			BaseDamageReduction = e.BaseDamageReduction;
			BaseMovementSpeed = e.BaseMovementSpeed;
			MovementSpeedPerLevel = e.MovementSpeedPerLevel;
			CurrentRoleCount = 0;
			RoleName = e.Role.ToString();
			DisplayIcon = e.DisplayIcon;
			ActionClips = e.ActionClips;
	}
	}
}