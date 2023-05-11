using Pathfinding;
using UnityEngine;

namespace Behaviours
{
	[RequireComponent(typeof(AIPath))]
	public class UnitTravelToPosition : MonoBehaviour
	{
		private AIPath _aiPath;

		/// <summary>
		/// Sets the Target Position of the Unit.
		/// </summary>
		/// <param name="position"></param>
		public void SetTargetPosition(Vector3 position)
		{
			_aiPath.destination = position;
		}

		private void Awake()
		{
			_aiPath = GetComponent<AIPath>();
		}
	}
}