using GridSystem.Utils;
using UnityEngine;
// System may be obsolete.
namespace GridSystem
{
	[System.Serializable]
	public class GridManager : MonoBehaviour
	{
		[SerializeField]
		private Vector2 _originOffset;
		[SerializeField]
		private int _gridLength = 100;
		[SerializeField]
		private int _gridWidth = 100;
		[SerializeField]
		private int _cellSize = 1;

		[SerializeField, HideInInspector]
		private GridNode[] _grid;

		private float _offSetX = 0;
		private float _offSetZ = 0;

		private int _cellsX;
		private int _cellsZ;

		public GridNode[] GetGrid()
		{
			return _grid;
		}

		// TODO:: Clean this up.
#if UNITY_EDITOR
		[Header("Debug Options")]
		[SerializeField]
		private bool _drawGrid = true;

		private void OnDrawGizmos()
		{
			if (!_drawGrid)
				return;

			Color prevColor = Gizmos.color;
			if (_grid != null && _grid.Length > 0)
			{
				for (int z = 0; z < _cellsZ; z++)
				{
					for (int x = 0; x < _cellsX; x++)
					{
						Color useColor = Color.white;

						switch (_grid[_cellsX * x + z].CollisionType)
						{
							case CollisionType.Walkable:
								useColor = CollisionColours.Walkable;
								break;
							case CollisionType.Unwalkable:
								useColor = CollisionColours.Unwalkable;
								break;
							case CollisionType.Water:
								useColor = CollisionColours.Water;
								break;
							case CollisionType.Friendly:
								useColor = CollisionColours.Friendly;
								break;
						}
						useColor.a = 0.5f;
						Gizmos.color = useColor;
						Gizmos.DrawCube(new Vector3(_grid[_cellsX * x + z].Position.x, 0, _grid[_cellsX * x + z].Position.y), new Vector3(_cellSize, 0.01f, _cellSize));
						Gizmos.color = new Color(1, 1, 1, 0.25f);
						Gizmos.DrawWireCube(new Vector3(_grid[_cellsX * x + z].Position.x, 0, _grid[_cellsX * x + z].Position.y), new Vector3(_cellSize, 0.01f, _cellSize));
					}
				}
			}
		}

		public void GenerateGrid()
		{
			_cellsX = (_gridWidth / _cellSize);
			_cellsZ = (_gridLength / _cellSize);
			_offSetX = -(_cellsZ * _cellSize / 2 + _originOffset.x - transform.position.x) + (_cellSize * 0.5f);
			_offSetZ = -(_cellsX * _cellSize / 2 + _originOffset.y - transform.position.z) + (_cellSize * 0.5f);

			_grid = new GridNode[_cellsZ * _cellsX];
			for (int z = 0; z < _cellsX; z++)
			{
				for (int x = 0; x < _cellsX; x++)
				{
					_grid[_cellsX * x + z] = new GridNode((CollisionType)Random.Range(0, (int)CollisionType.Friendly + 1), new Vector2(x * _cellSize + _offSetX, z * _cellSize + _offSetZ), -1);
				}
			}
		}
#endif
	}
}