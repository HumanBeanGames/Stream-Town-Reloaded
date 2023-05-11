using UnityEngine;
using Utils;

namespace Character
{
	/// <summary>
	/// Used to keep track of each player role's active equipment.
	/// </summary>
	[System.Serializable]
	public class RoleEquipment
	{
		public string RoleName;
		public PlayerRole PlayerRole;
		public GameObject BodieSlim;
		public GameObject BodieBulk;
		public GameObject BodieFeminine;
		public GameObject LeftHand;
		public GameObject RightHand;
		public GameObject Helmet;
		public bool HasCarryAnimation;
		public AnimationName CarryAnimation;
		public bool LeftHandPermanent;
	}
}