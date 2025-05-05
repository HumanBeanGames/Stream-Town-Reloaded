using Managers;
using UnityEngine;
using Utils;

namespace GameResources
{
	/// <summary>
	/// An active resource incrementer that can be called by an event.
	/// </summary>
	public class ActiveResourceIncrementer : MonoBehaviour
	{
		[SerializeField]
		protected Utils.Resource _resource;
		[SerializeField]
		protected int _amount;

		/// <summary>
		/// Increments the town resources of the specified type by the amount set.
		/// </summary>
		public void Increment()
		{
			TownResourceManager.AddResource(_resource, _amount);
		}
	}
}