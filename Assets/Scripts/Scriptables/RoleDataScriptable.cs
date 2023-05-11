using UnityEngine;
using Utils;

namespace Scriptables
{
	/// <summary>
	/// Scriptable Object that holds all data for a Role.
	/// </summary>
	[CreateAssetMenu(fileName = "Role Data", menuName = "ScriptableObjects/RoleDataObject", order = 1)]
	public class RoleDataScriptable : ScriptableObject
	{
		public PlayerRole Role;
		public PlayerRoleType RoleFlags;
		public TargetMask TargetFlags;
		public Utils.Resource Resource = Utils.Resource.None;
		public AnimationName ActionAnimationName;
		public int ActionAnimationVariants = 1;
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
		public float ExpModifier = 1;
		public Sprite DisplayIcon;
		public AudioClip[] ActionClips;
	}
}