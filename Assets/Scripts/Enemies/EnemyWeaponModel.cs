using Animation;
using UnityEngine;
using Utils;

namespace Enemies
{
	/// <summary>
	/// Handles an enemy character's weapon model.
	/// </summary>
	[System.Serializable]
	public class EnemyWeaponModel
	{
		[SerializeField]
		private GameObject _mainWeaponModel;

		[SerializeField]
		private GameObject[] _offHandModels;

		[SerializeField]
		private AnimationName _weaponAnimationName = AnimationName.GenericAction;

		[SerializeField]
		private int _animationVariants = 1;

		[SerializeField]
		private RunAnimation _runAnimation = RunAnimation.Generic;

		private AnimationHandler _animationHandler;

		public RunAnimation RunAnimation => _runAnimation;
		public int AnimationVariants => _animationVariants;
		public AnimationName WeaponAnimation => _weaponAnimationName;

		/// <summary>
		/// Sets up the reference for the Animation Handler.
		/// </summary>
		/// <param name="animationHandler"></param>
		public void SetAnimationHandler(AnimationHandler animationHandler)
		{
			_animationHandler = animationHandler;
		}

		/// <summary>
		/// Activates or deactivates the weapon models.
		/// </summary>
		/// <param name="value"></param>
		public void SetActive(bool value)
		{
			if (value)
				Activate();
			else
				Deactivate();
		}

		/// <summary>
		/// Enables the weapon models.
		/// </summary>
		private void Activate()
		{
			_mainWeaponModel.SetActive(true);
			if (_offHandModels != null)
			{
				for (int i = 0; i < _offHandModels.Length; i++)
				{
					_offHandModels[i].SetActive(true);
				}
			}
			_animationHandler.SetRunAnimationIndex((int)_runAnimation);
		}

		/// <summary>
		/// Disables the weapon models.
		/// </summary>
		private void Deactivate()
		{
			_mainWeaponModel.SetActive(false);
			if (_offHandModels != null)
			{
				for (int i = 0; i < _offHandModels.Length; i++)
				{
					_offHandModels[i].SetActive(false);
				}
			}
		}
	}
}