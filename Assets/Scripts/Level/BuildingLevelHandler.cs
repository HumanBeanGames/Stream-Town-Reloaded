using Buildings;
using Managers;
using System;

namespace Level
{
	/// <summary>
	/// Handles the leveling for any building unit.
	/// </summary>
	public class BuildingLevelHandler : LevelHandler
	{
		protected BuildingBase _buildingBase;

		public event Action<LevelHandler> OnLeveledUp;

		public override bool CanLevel()
		{
			return CanLevel();
		}

		/// <summary>
		/// returns true if the building can level up.
		/// </summary>
		/// <returns></returns>
		public bool CanLevel(bool skipCostCheck = false)
		{
			if (base.CanLevel() && (skipCostCheck || BuildingManager.CanAffordToLevel(_buildingBase.BuildingType, _currentLevel)))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Called when building levels up.
		/// </summary>
		public override void OnLevelUp()
		{
			BuildingManager.OnLevelBuilding(_buildingBase.BuildingType, _currentLevel);
			base.OnLevelUp();
			OnLeveledUp?.Invoke(this);
		}

		/// <summary>
		/// Initializes all required references and data.
		/// </summary>
		protected override void Init()
		{
			base.Init();

			_buildingBase = GetComponent<BuildingBase>();
		}
	}
}