using Pets.Enumerations;
using UnityEngine;

namespace Pets 
{
    public class PetModel : MonoBehaviour
	{
		[SerializeField]
		private PetType _petType;

        private Animator _animator;
		private static int _moveSpeedHash = Animator.StringToHash("MoveSpeed");

		public PetType PetType => _petType;
		public bool HasAnimator => _animator != null;

		private void Awake()
		{
			TryGetComponent(out _animator);
		}

		public void SetAnimationTrigger(string trigger)
		{
			_animator.SetTrigger(trigger);
		}

		public void SetMovementSpeed(float speed)
		{
			if (!HasAnimator)
				return;

			_animator.SetFloat(_moveSpeedHash, speed);
		}
	}
}