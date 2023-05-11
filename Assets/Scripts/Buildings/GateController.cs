using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
	/// <summary>
	/// Used to animation a gate based on player's entering and exiting it's trigger bounds.
	/// </summary>
	public class GateController : MonoBehaviour
	{
		/// <summary>
		/// True if gate should open and close.
		/// </summary>
		private bool _active = false;

		/// <summary>
		/// Current list of all colliders inside of the trigger bounds.
		/// </summary>
		private List<Collider> _collidersInBounds = new List<Collider>();

		// Animation Hashes
		private static int _openHash = Animator.StringToHash("Open");
		private static int _closeHash = Animator.StringToHash("Close");

		// Required Components.
		private Rigidbody _rigidBody;
		[SerializeField]
		private Animator[] _animators;

		/// <summary>
		/// Sets the gate to be active or not.
		/// </summary>
		/// <param name="value"></param>
		public void SetActive(bool value)
		{
			_active = value;
		}

		private void Awake()
		{
			// Get Required Components.
			_rigidBody = GetComponentInParent<Rigidbody>();

			// Set up rigidbody.
			_rigidBody.freezeRotation = true;
			_rigidBody.useGravity = false;
		}

		/// <summary>
		/// Called when a collider enters the trigger box.
		/// </summary>
		/// <param name="other"></param>
		private void OnTriggerEnter(Collider other)
		{
			// If gate is not active, do nothing and return.
			if (!_active)
				return;

			// If the collider was a player and it's not already in the list...
			if (other.tag == "Player" && !_collidersInBounds.Contains(other))
			{
				// ...Add it to the list
				_collidersInBounds.Add(other);

				// If there is only one collider in the list, trigger the open animation.
				// Only called when there is one so that it doesn't repeatedly open each time a valid collider enters.
				if (_collidersInBounds.Count == 1)
					foreach (Animator anim in _animators)
						anim.SetTrigger(_openHash);
			}

		}

		/// <summary>
		/// Called when a collider exits the trigger box.
		/// </summary>
		/// <param name="other"></param>
		private void OnTriggerExit(Collider other)
		{
			// If gate is not active, do nothing and return.
			if (!_active)
				return;

			// If the collider was a player and it's in the list...
			if (other.tag == "Player" && _collidersInBounds.Contains(other))
			{
				// ...Remove the player from the list.
				_collidersInBounds.Remove(other);

				// If there are no colliders in the trigger bounds, close the gate.
				if (_collidersInBounds.Count == 0)
					foreach (Animator anim in _animators)
						anim.SetTrigger(_closeHash);
			}
		}
	}
}