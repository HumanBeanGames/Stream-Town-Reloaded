using Managers;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine;
using Utils;
namespace Animation
{
	/// <summary>
	/// A general purpose Animation Handler for controlling animations on Units.
	/// </summary>
	public class AnimationHandler : MonoBehaviour
	{
		/// <summary>
		/// Determines what speed the animation controller will start Running.
		/// </summary>
		protected const float RUN_SPEED = 5;

		/// <summary>
		/// Dictionary that holds all animations by Enum and Hash.
		/// </summary>
		protected static Dictionary<AnimationName, int> _animationDictionary;
		protected static bool _initialized;
		protected static int _moveSpeedHash = Animator.StringToHash("Move Speed");
		protected static int _actionSpeedHash = Animator.StringToHash("ActionSpeed");
		protected static int _runAnimationIndex = Animator.StringToHash("RunAnimationIndex");
		protected static int _actionHash = Animator.StringToHash("Action");
		protected static int _woodCuttingHash = Animator.StringToHash("WoodCutting");
		protected static int _buildHash = Animator.StringToHash("Build");
		protected static int _bowShootHash = Animator.StringToHash("BowShoot");
		protected static int _castingHash = Animator.StringToHash("Casting");
		protected static int _farmingHash = Animator.StringToHash("Farming");
		protected static int _fishingHash = Animator.StringToHash("Fishing");
		protected static int _gatheringHash = Animator.StringToHash("Gathering");
		protected static int _hammerAttackHash = Animator.StringToHash("HammerAttack");
		protected static int _healHash = Animator.StringToHash("Heal");
		protected static int _miningHash = Animator.StringToHash("Mining");
		protected static int _spearAttackHash = Animator.StringToHash("SpearAttack");
		protected static int _staffAttackHash = Animator.StringToHash("StaffAttack");
		protected static int _staffMagicAttackHash = Animator.StringToHash("StaffMagicAttack");
		protected static int _longSwordHash = Animator.StringToHash("LongSword");
		protected static int _carryWood = Animator.StringToHash("CarryWood");
		protected static int _carryHip = Animator.StringToHash("CarryHip");
		protected static int _genericAction = Animator.StringToHash("GenericAction");
		protected static int _animationIndex = Animator.StringToHash("AnimationIndex");
		protected static int _deathHash = Animator.StringToHash("Death");
		protected static int _reviveHash = Animator.StringToHash("Revive");

		protected Animator _animator;
		protected AIPath _aiPath;

		/// <summary>
		/// Returns int has of by Animation Name.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public int GetAnimationHash(AnimationName name)
		{
			return _animationDictionary[name];
		}

		/// <summary>
		/// Sets an Animation Controller Trigger based on a Hash Value.
		/// </summary>
		/// <param name="hash"></param>
		public void SetTrigger(int hash)
		{
			_animator.SetTrigger(hash);
		}

		/// <summary>
		/// Sets an Animation Controller Trigger based on Animation Name. 
		/// </summary>
		/// <param name="name"></param>
		public void SetTrigger(AnimationName name)
		{
			SetTrigger(GetAnimationHash(name));
		}

		/// <summary>
		/// Sets the Index of the Attack Animation in the Animation Controller.
		/// </summary>
		/// <param name="value"></param>
		public void SetAttackAnimationIndex(int value)
		{
			_animator.SetInteger(_animationIndex, value);
		}

		/// <summary>
		/// Sets the Animation Speed of the Action in the Animation Controller.
		/// </summary>
		/// <param name="value"></param>
		public void SetActionSpeed(float value)
		{
			_animator.SetFloat(_actionSpeedHash, value);
		}

		/// <summary>
		/// Sets the Index of the Run Animation in the Animation Controller.
		/// </summary>
		/// <param name="value"></param>
		public void SetRunAnimationIndex(int value)
		{
			_animator.SetInteger(_runAnimationIndex, value);
		}

		/// <summary>
		/// Sets the Movement Speed Parameter in the Animation Controller.
		/// </summary>
		/// <param name="value"></param>
		public void SetMoveSpeed(float value)
		{
			_animator.SetFloat(_moveSpeedHash, value);
		}

		/// <summary>
		/// Sets a Bool in the Animation Controller by the given Hash.
		/// </summary>
		/// <param name="hash"></param>
		/// <param name="value"></param>
		public void SetBool(int hash, bool value)
		{
			_animator.SetBool(hash, value);
		}

		/// <summary>
		/// Sets a Bool in the Animation Controller by the given Animation Name.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		public void SetBool(AnimationName name, bool value)
		{
			SetBool(GetAnimationHash(name), value);
		}

		protected virtual void Init()
		{
			_animator = GetComponent<Animator>();
			_aiPath = GetComponentInParent<AIPath>();
		}

		/// <summary>
		/// Generates the dictionary.
		/// </summary>
		protected virtual void BuildDictionary()
		{
			_animationDictionary = new Dictionary<AnimationName, int>();
			_animationDictionary.Add(AnimationName.Action, _actionHash);
			_animationDictionary.Add(AnimationName.WoodCutting, _woodCuttingHash);
			_animationDictionary.Add(AnimationName.Build, _buildHash);
			_animationDictionary.Add(AnimationName.BowShoot, _bowShootHash);
			_animationDictionary.Add(AnimationName.Casting, _castingHash);
			_animationDictionary.Add(AnimationName.Farming, _farmingHash);
			_animationDictionary.Add(AnimationName.Fishing, _fishingHash);
			_animationDictionary.Add(AnimationName.Gathering, _gatheringHash);
			_animationDictionary.Add(AnimationName.HammerAttack, _hammerAttackHash);
			_animationDictionary.Add(AnimationName.Heal, _healHash);
			_animationDictionary.Add(AnimationName.Mining, _miningHash);
			_animationDictionary.Add(AnimationName.SpearAttack, _spearAttackHash);
			_animationDictionary.Add(AnimationName.StaffAttack, _staffAttackHash);
			_animationDictionary.Add(AnimationName.StaffAttackMagic, _staffMagicAttackHash);
			_animationDictionary.Add(AnimationName.LongSword, _longSwordHash);
			_animationDictionary.Add(AnimationName.CarryWood, _carryWood);
			_animationDictionary.Add(AnimationName.CarryHip, _carryHip);
			_animationDictionary.Add(AnimationName.GenericAction, _genericAction);
			_animationDictionary.Add(AnimationName.Death, _deathHash);
			_animationDictionary.Add(AnimationName.Revive, _reviveHash);
		}

		// Unity Event.
		private void Awake()
		{
			Init();
			if (!_initialized)
				BuildDictionary();
		}

		public void Update()
		{
			//TODO: Modify float passed in
			// Set Move Speed value of animator
			if (_aiPath.canMove)
				_animator.SetFloat(_moveSpeedHash, _aiPath.velocity.magnitude / RUN_SPEED);
			else
				_animator.SetFloat(_moveSpeedHash, 0);
		}
	}
}