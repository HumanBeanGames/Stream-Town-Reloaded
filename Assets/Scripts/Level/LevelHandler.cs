using UnityEngine;
using UnityEngine.Events;

namespace Level
{
	/// <summary>
	/// Base Level Handler class that handles base functionality of anything that can level up.
	/// </summary>
	public class LevelHandler : MonoBehaviour
	{
		[SerializeField]
		protected int _currentLevel = 1;
		[SerializeField]
		protected int _maxLevel = 10;

		[SerializeField]
		protected UnityEvent _onLevelUp;

		public int Level => _currentLevel;
		public int MaxLevel
		{
			get { return _maxLevel; }
			set { _maxLevel = value; }
		}

		/// <summary>
		/// Called when leveling up.
		/// </summary>
		public virtual void OnLevelUp()
		{
			if (_currentLevel >= _maxLevel)
				return;

			_currentLevel++;
			_onLevelUp.Invoke();
		}

		/// <summary>
		/// Returns true if leveling is possible.
		/// </summary>
		/// <returns></returns>
		public virtual bool CanLevel()
		{
			if (_currentLevel < _maxLevel)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Attempts to level up and returns the result.
		/// </summary>
		/// <returns></returns>
		public virtual bool TryLevel()
		{
			if (!CanLevel())
				return false;

			OnLevelUp();

			return true;
		}

		protected virtual void Init()
		{

		}

		// Unity Functions.
		private void Awake()
		{
			Init();
		}

		private void OnDisable()
		{
			_currentLevel = 1;
		}
	}
}