using UnityEngine;

namespace Behaviours
{
	public class HealthModifier : MonoBehaviour
	{
		//Can unserialize these later
		[SerializeField]
		private float _range;
		[SerializeField]
		private float _value;
		[SerializeField]
		private bool _projectile;
		[SerializeField]
		private GameObject _projectilePrefab;
	}
}