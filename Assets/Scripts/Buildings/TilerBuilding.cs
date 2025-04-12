
using System;
using UnityEngine;
using Utils;

namespace Buildings
{
	/// <summary>
	/// Handles Tiling for buildings and is inherited from Tiler.
	/// </summary>
	public class TilerBuilding : Tiler
	{
		private BuildingBase _buildingBase;

		/// <summary>
		/// Initializes components.
		/// </summary>
		protected override void Init()
		{
			_buildingBase = GetComponent<BuildingBase>();
			base.Init();

			GameManager.Instance.TechTreeManager.OnBuildingAgedUp += OnBuildingAged;
		}

		private void OnBuildingAged(BuildingType type)
		{
			if (type != _buildingBase.BuildingType)
				return;

			TileHelper._tilesToUpdate.Enqueue(this);
		}

		/// <summary>
		/// Called when the Tile Value has changed and applies the appropriate rotation and model.
		/// </summary>
		protected override void OnTileValueChanged()
		{
			int rotationAngle = 0;
			int modelIndex = 0;

			if (_tileValue == 0 || _tileValue == 2 || TileValue == 8 || TileValue == 10)
			{
				rotationAngle = 90;
			}
			else if (_tileValue == 4 || _tileValue == 16 || _tileValue == 20)
			{
				rotationAngle = 0;
			}
			else if (_tileValue == 18)
			{
				modelIndex = 1;
			}
			else if (_tileValue == 6)
			{
				modelIndex = 1;
				rotationAngle = 90;
			}
			else if (_tileValue == 12)
			{
				modelIndex = 1;
				rotationAngle = 180;
			}
			else if (_tileValue == 24)
			{
				modelIndex = 1;
				rotationAngle = 270;
			}
			else if (_tileValue == 22)
			{
				modelIndex = 3;
			}
			else if (_tileValue == 14)
			{
				modelIndex = 3;
				rotationAngle = 90;
			}
			else if (_tileValue == 28)
			{
				modelIndex = 3;
				rotationAngle = 180;
			}
			else if (_tileValue == 26)
			{
				modelIndex = 3;
				rotationAngle = 270;
			}
			else
			{
				modelIndex = 2;
			}

			_buildingBase.SetModelHandlerIndex(modelIndex);

			_buildingBase.GetComponent<UpdateGraphBounds>().SetGraphBounds();

			transform.eulerAngles = new Vector3(0, rotationAngle, 0);
		}
	}
}