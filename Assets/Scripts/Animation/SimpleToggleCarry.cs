using Character;
using UnityEngine;

namespace Animation
{
	/// <summary>
	/// A simple component that allows the Unit to toggle it's carry, assuming it has one.
	/// </summary>
	public class SimpleToggleCarry : MonoBehaviour
	{
		private CharacterModelHandler _equipmentHandler;

		/// <summary>
		/// Enables the Unit's Carry Object if it has one.
		/// </summary>
		public void ToggleOn()
		{
			_equipmentHandler.EnableCarry(false);
		}

		/// <summary>
		/// Disables the Unit's Carry Object if it has one.
		/// </summary>
		public void ToggleOff()
		{
			_equipmentHandler.DisableCarry(false);
		}

		private void Awake()
		{
			_equipmentHandler = GetComponentInParent<CharacterModelHandler>();
		}

	}
}