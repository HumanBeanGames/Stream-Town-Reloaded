using Animation;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Enemies
{
	/// <summary>
	/// Handles the models for an enemy character, allowing for models to be randomized on spawn.
	/// </summary>
	public class EnemyModelHandler : MonoBehaviour
	{
		[SerializeField]
		private List<GameObject> _baseModels;

		[SerializeField]
		private List<GameObject> _permanentModels;

		[SerializeField]
		private List<GameObject> _optionalModels;

		[SerializeField]
		private List<EnemyWeaponModel> _linkedWeaponModels;

		[SerializeField, Tooltip("For use when enemies dont have a weapon")]
		private int _baseAnimationVariants = 1;

		[SerializeField]
		private bool _randomize;

		private int _baseModelIndex = 0;
		private int _weaponModelIndex = 0;

		/// <summary>
		/// Randomizes the active models for the enemy.
		/// </summary>
		public void RandomizeModel()
		{
			_baseModelIndex = Random.Range(0, _baseModels.Count);
			_weaponModelIndex = Random.Range(0, _linkedWeaponModels.Count);

			// Base Models - usually bodies.
			for (int i = 0; i < _baseModels.Count; i++)
			{
				_baseModels[i].SetActive(_baseModelIndex == i ? true : false);
			}

			// Optional models - pieces of armor or clothing.
			for (int i = 0; i < _optionalModels.Count; i++)
			{
				_optionalModels[i].SetActive(Random.Range(0, 2) == 0 ? false : true);
			}

			// Any weapons, including linked weapons such as a sword and shield.
			for (int i = 0; i < _linkedWeaponModels.Count; i++)
			{
				_linkedWeaponModels[i].SetActive(_weaponModelIndex == i ? true : false);
			}

			// All models that should be permanently active.
			for (int i = 0; i < _permanentModels.Count; i++)
			{
				_permanentModels[i].SetActive(true);
			}
		}

		/// <summary>
		/// Returns the name of the animation used for Attacking.
		/// </summary>
		/// <returns></returns>
		public AnimationName GetAttackAnimation()
		{
			if (_linkedWeaponModels == null || _linkedWeaponModels.Count == 0)
			{
				return AnimationName.GenericAction;
			}

			return _linkedWeaponModels[_weaponModelIndex].WeaponAnimation;
		}

		/// <summary>
		/// Returns a random index based on the number of attack animation variants.
		/// </summary>
		/// <returns></returns>
		public int GetAttackIndex()
		{
			if (_linkedWeaponModels == null || _linkedWeaponModels.Count == 0)
			{
				return Random.Range(0, _baseAnimationVariants);
			}

			return Random.Range(0, _linkedWeaponModels[_weaponModelIndex].AnimationVariants);
		}

		/// <summary>
		/// Returns the number of Attack Animation variants.
		/// </summary>
		/// <returns></returns>
		public int GetAttackVariantCount()
		{
			if (_linkedWeaponModels == null || _linkedWeaponModels.Count == 0)
			{
				return _baseAnimationVariants;
			}

			return _linkedWeaponModels[_weaponModelIndex].AnimationVariants;
		}

		/// <summary>
		/// Returns the type of Run Animation.
		/// </summary>
		/// <returns></returns>
		public RunAnimation GetRunAnimation()
		{
			if (_linkedWeaponModels == null || _linkedWeaponModels.Count == 0)
			{
				return RunAnimation.Generic;
			}

			return _linkedWeaponModels[_weaponModelIndex].RunAnimation;
		}

		// Unity Events.
		private void Awake()
		{
			AnimationHandler anim = GetComponent<AnimationHandler>();
			for (int i = 0; i < _linkedWeaponModels.Count; i++)
			{
				_linkedWeaponModels[i].SetAnimationHandler(anim);
			}
			RandomizeModel();
		}

		private void OnEnable()
		{
			if (_linkedWeaponModels.Count == 0)
				return;

			_linkedWeaponModels[_weaponModelIndex].SetActive(true);
		}
	}
}