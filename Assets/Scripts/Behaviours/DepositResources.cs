using Managers;
using UnityEngine;
using Utils;

namespace Behaviours
{
	/// <summary>
	/// An attachable component used for Depositing Resources by a Unit
	/// </summary>
	public class DepositResources : MonoBehaviour
	{
		/// <summary>
		/// Deposits resources of the determined type to the Town's Resource Manager.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="amount"></param>
		public void Deposit(Utils.Resource type, int amount)
		{
			GameManager.Instance.TownResourceManager.AddResource(type, amount);
		}
	}
}