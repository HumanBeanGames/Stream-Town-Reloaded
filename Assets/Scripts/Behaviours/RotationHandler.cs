using Managers;
using Sensors;
using UnityEngine;

namespace Behaviours
{
	/// <summary>
	/// An attachable component for automatically handling the rotation of a Unit such that it looks at a given position or target.
	/// </summary>
	public class RotationHandler : MonoBehaviour
	{
		[SerializeField]
		private float _rotationSpeed = 5;

		private TargetSensor _targetSensor;

		private Vector3 _lookPosition = Vector3.zero;

		private Quaternion _rotation;

		private Vector3 _prevPosition;

		/// <summary>
		/// Determines if the Unit should look at it's target or a set position.
		/// </summary>
		public bool LookAtTarget { get; set; }

		/// <summary>
		/// Handles the rotation of an object, looking at a target if it has one.
		/// </summary>
		private void HandleRotation()
		{
			// If we should look at our target and we currently have a target.
			if (LookAtTarget && _targetSensor.CurrentTarget != null)
				_lookPosition = _targetSensor.CurrentTarget.transform.position - transform.position;
			else
			{
				_lookPosition = transform.position - _prevPosition;
			}

			// Set Look Position's y to 0 so that we don't rotate the unit up and down.
			_lookPosition.y = 0;

			if (_lookPosition == Vector3.zero)
				return;

			// Get a look rotation and Slerp the Unit's rotation towards the desired rotation.
			_rotation = Quaternion.LookRotation(_lookPosition);
			transform.rotation = Quaternion.Slerp(transform.rotation, _rotation, Time.deltaTime * _rotationSpeed);

			_prevPosition = transform.position;
		}

		// Unity Functions.
		private void Awake()
		{
			_targetSensor = GetComponent<TargetSensor>();
			LookAtTarget = false;
			_prevPosition = transform.position;
		}

		private void Update()
		{
			HandleRotation();
		}
	}
}