using Buildings;
using Utils;

namespace Target
{
	/// <summary>
	/// Handles buildings that are targetable.
	/// </summary>
	public class TargetableBuilding : TargetableHealth
	{
		protected BuildingBase _building;

		/// <summary>
		/// Initalizes required data.
		/// </summary>
		protected override void Init()
		{
			base.Init();
			_building = GetComponent<BuildingBase>();
		}

		/// <summary>
		/// Called when the Building's health is full.
		/// </summary>
		protected override void OnHealthPercentageFull()
		{
			if (_building.BuildingState == BuildingState.Construction)
			{
				_building.OnHealthConstruction(1.0f);
				_building.BuildingState = BuildingState.Building;
			}

			SetTargetType(_building.FinishedTargetType);

		}

		/// <summary>
		/// Called when the Building's health is neither zero or full.
		/// </summary>
		/// <param name="value"></param>
		protected override void OnHealthPercentageOther(float value)
		{
			if (_building.BuildingState == BuildingState.Building)
				SetTargetType(TargetMask.DamagedBuilding);
			else
				_building.OnHealthConstruction(value);

		}

		/// <summary>
		/// Called when the Building's health is zero.
		/// </summary>
		protected override void OnHealthPercentageZero()
		{
			if (_building.BuildingState == BuildingState.Building)
				SetTargetType(TargetMask.Nothing);
		}
	}
}