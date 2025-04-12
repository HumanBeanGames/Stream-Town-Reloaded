
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

		protected TownResourceManager _resourceManager;

		/// <summary>
		/// Increments the town resources of the specified type by the amount set.
		/// </summary>
		public void Increment()
		{
			_resourceManager.AddResource(_resource, _amount);
		}

		// Unity Events.
		private void Awake()
		{
			_resourceManager = GameManager.Instance.TownResourceManager;
		}
	}
}