using GameResources;
using Target;
using UnityEngine;

namespace Behaviours
{
	/// <summary>
	/// Attachable component that allows units to Collect Resources.
	/// </summary>
	public class CollectResource : MonoBehaviour
	{
		/// <summary>
		/// Collect's resources from a Resource Holder.
		/// </summary>
		/// <param name="resourceHolder"></param>
		/// <param name="amount"></param>
		/// <returns></returns>
		public int Collect(ResourceHolder resourceHolder, int amount)
		{
			return resourceHolder.TakeResource(amount);
		}

		/// <summary>
		/// Collect's resources from a targetable if it has a Resource Holder component.
		/// </summary>
		/// <param name="targetable"></param>
		/// <param name="amount"></param>
		/// <returns></returns>
		public int Collect(Targetable targetable, int amount)
		{
			if (targetable.TryGetComponent(out ResourceHolder e))
			{
				return Collect(e, amount);
			}

			return 0;
		}
	}
}