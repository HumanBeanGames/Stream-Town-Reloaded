using Character;
using Pets.Enumerations;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Pets
{
	public class Pet : MonoBehaviour
	{
		[SerializeField]
		private float _closestDistanceToPlayer = 1.0f;
		[SerializeField]
		private float _maxDistanceFromPlayer = 5.0f;
		[SerializeField]
		private float _minMoveSpeed = 0.5f;
		[SerializeField]
		private float _maxMoveSpeed = 10.0f;
		[SerializeField]
		private float _rotationSpeed = 5.0f;

		private float _closestDistanceSqrd;
		private float _maxDistanceSqrd;

		private PetType _activePetType;
		private Dictionary<PetType, PetModel> _petModels = new Dictionary<PetType, PetModel>();
		private PetModel _activePetModel;

		[SerializeField]
		private Player _owner;
		private Transform _ownerTransform;

		public PetType ActivePetType => _activePetType;

		public PetModel ActivePet => _activePetModel;
		public bool IsActive;

		public void SetOwner(Transform owner, Player player)
		{
			_owner = player;
			_ownerTransform = owner;
			transform.position = _ownerTransform.position;
		}

		public void ActivatePet()
		{
			gameObject.SetActive(true);
			IsActive = true;
		}

		public void DeactivatePet()
		{
			gameObject.SetActive(false);
			IsActive = false;
		}

		public void TrySetActivePet(PetType petType)
		{
			if (petType == PetType.None)
			{
				if (_activePetModel != null)
					_activePetModel.gameObject.SetActive(false);
				_activePetModel = _petModels[petType];
				_activePetType = PetType.None;
			}
			else if (_petModels.ContainsKey(petType))
			{
				if (_activePetModel != null)
					_activePetModel.gameObject.SetActive(false);
				_activePetModel = _petModels[petType];
				_activePetModel.gameObject.SetActive(true);
				_activePetType = petType;
				IsActive = true;
			}
		}

		private void Update()
		{
			if (_ownerTransform == null)
				return;

			Vector3 dir = _ownerTransform.position - transform.position;

			float sqDist = dir.sqrMagnitude;
			Vector3 lookDir = dir;
			lookDir.y = 0;
			dir.Normalize();

			float scalar = MathExtended.RemapValue(sqDist, _closestDistanceSqrd, _maxDistanceSqrd, _minMoveSpeed, _maxMoveSpeed);
			if (scalar < _minMoveSpeed)
				scalar = _minMoveSpeed;

			if (scalar > _maxMoveSpeed)
				scalar = _maxMoveSpeed;
			transform.position += dir * scalar * Time.deltaTime;

			if (_activePetModel && _activePetModel.HasAnimator)
				_activePetModel.SetMovementSpeed(scalar);

			if (lookDir == Vector3.zero)
				return;

			Quaternion rotation = Quaternion.LookRotation(lookDir);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * _rotationSpeed);


		}

		private void Awake()
		{
			_closestDistanceSqrd = _closestDistanceToPlayer * _closestDistanceToPlayer;
			_maxDistanceSqrd = _maxDistanceFromPlayer * _maxDistanceFromPlayer;

			for (int i = 0; i < transform.childCount; i++)
			{
				GameObject go = transform.GetChild(i).gameObject;

				if (go.TryGetComponent(out PetModel petModel))
				{
					_petModels.Add(petModel.PetType, petModel);
				}

				go.SetActive(false);
			}
			_petModels.Add(PetType.None, null);
		}
	}
}