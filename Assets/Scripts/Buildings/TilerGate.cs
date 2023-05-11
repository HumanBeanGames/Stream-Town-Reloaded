using UnityEngine;

namespace Buildings
{
	/// <summary>
	/// Inherited from the Tiler class, this class specifically handles the tiling of gates.
	/// </summary>
	public class TilerGate : Tiler
	{
		private BuildingBase _buildingBase;

		/// <summary>
		/// Sets up required components.
		/// </summary>
		protected override void Init()
		{
			_buildingBase = GetComponent<BuildingBase>();
			base.Init();
		}

		/// <summary>
		/// Called after the Tile Value has changed and rotates the gate appropriately.
		/// </summary>
		protected override void OnTileValueChanged()
		{
			int rotationAngle = 0;

			if (_tileValue == 0 || _tileValue == 2 || TileValue == 8 || TileValue == 10 || _tileValue == 6 || _tileValue == 14)
			{
				rotationAngle = 90;
			}
			else if (_tileValue == 12 || _tileValue == 28)
			{
				rotationAngle = 180;
			}
			else if (_tileValue == 24 || _tileValue == 26)
			{
				rotationAngle = 270;
			}

			transform.eulerAngles = new Vector3(0, rotationAngle, 0);
		}
	}
}