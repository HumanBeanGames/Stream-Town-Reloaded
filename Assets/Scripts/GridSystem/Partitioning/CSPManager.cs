using Managers;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Target;
using UnityEngine;
using UnityEngine.Profiling;
using Utils;

namespace GridSystem.Partitioning
{
	/// <summary>
	/// Uses cells to partition a world for more efficient lookup of objects and cells.
	/// </summary>
	[GameManager]
	public static class CSPManager
	{
        [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        private static CSPConfig Config = CSPConfig.Instance;

		private static Vector2 _originOffset => Config.originOffset;
		private static float _width => Config.width;
		private static float _length => Config.length;
		private static float _cellWidth => Config.cellWidth;
		private static float _cellLength => Config.cellLength;

		[SerializeField, HideInInspector]
		private static List<BSPCell> _cells = new List<BSPCell>();

		[HideInInspector]
		private static int _numCellsX = 0;
        [HideInInspector]
        private static int _numCellsZ = 0;
        [HideInInspector]
        private static float _offSetX = 0;
        [HideInInspector]
        private static float _offSetZ = 0;

		/// <summary>
		/// Generates the cells to partition the world.
		/// </summary>
		public static void GeneratePartitions()
		{
			_cells = new List<BSPCell>();

			// Calculate size of each cell.
			_numCellsX = (int)(_width / _cellWidth);
			_numCellsZ = (int)(_length / _cellLength);

			_offSetX = _numCellsX * _cellWidth / 2 + _originOffset.x;
			_offSetZ = _numCellsZ * _cellLength / 2 + _originOffset.y;

			// Create the Cells.
			for (int z = 0; z < _numCellsZ; z++)
			{
				for (int x = 0; x < _numCellsX; x++)
				{
					float left = (x * _cellWidth) - _offSetX;
					float right = left + _cellWidth;
					float top = (z * _cellLength) - _offSetZ;
					float bottom = top + _cellLength;

					_cells.Add(new BSPCell(new Vector2(left, top), new Vector2(right, bottom)));
				}
			}
		}

		/// <summary>
		/// Returns a cell index based on a position.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public static int PositionToIndex(Vector3 position)
		{
			Vector2 v2Pos = new Vector2(position.x + _offSetX, position.z + _offSetZ);

			return PositionToIndex(v2Pos);
		}

		/// <summary>
		/// Returns a cell index based on a position.
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public static int PositionToIndex(Vector2 position)
		{
			int index = (int)(_numCellsX * position.x / _width);
			index += (int)(_numCellsZ * position.y / _length) * _numCellsX;

			if (index > _cells.Count - 1)
				index = _cells.Count - 1;

			return index;
		}

		/// <summary>
		/// Gets all cells in a radius around a position as a reference.
		/// </summary>
		/// <param name="position"></param>
		/// <param name="radius"></param>
		/// <param name="cells"></param>
		public static void GetCellsInRange(Vector3 position, float radius, ref List<BSPCell> cells)
		{
			GetCellsInRange(new Vector2(position.x, position.z), radius, ref cells);
		}

		/// <summary>
		/// Gets all cells in a radius around a position as a reference.
		/// </summary>
		/// <param name="position"></param>
		/// <param name="radius"></param>
		/// <param name="cells"></param>
		public static void GetCellsInRange(Vector2 position, float radius, ref List<BSPCell> cells)
		{
			Profiler.BeginSample("Get Cells In Range");
			//List<BSPCell> cells = new List<BSPCell>(1500);
			Vector2 topLeft = position - new Vector2(radius, radius);
			Vector2 bottomRight = position + new Vector2(radius, radius);

			for (int i = 0; i < _cells.Count; i++)
			{
				if (_cells[i].IsOverlapping(topLeft, bottomRight))
				{
					cells.Add(_cells[i]);
				}
			}
			Profiler.EndSample();
		}

		/// <summary>
		/// Gets all Targetable objects within a radius around a position.
		/// </summary>
		/// <param name="flag"></param>
		/// <param name="position"></param>
		/// <param name="radius"></param>
		/// <param name="targetables"></param>
		public static void GetTargetablesInRange(TargetMask flag, Vector3 position, float radius, ref List<Targetable> targetables)
		{
			List<BSPCell> cells = new List<BSPCell>(1500);
			Profiler.BeginSample("Get Targetables In Range");
			GetCellsInRange(position, radius, ref cells);
			Profiler.EndSample();
			GetTargetablesInCells(flag, ref cells, ref targetables);
		}

		/// <summary>
		/// Gets all Targetable objects within a defined list of cells.
		/// </summary>
		/// <param name="flag"></param>
		/// <param name="cells"></param>
		/// <param name="targetables"></param>
		public static void GetTargetablesInCells(TargetMask flag, ref List<BSPCell> cells, ref List<Targetable> targetables)
		{
			Profiler.BeginSample("Get Targetables In Cell");

			for (int i = 0; i < cells.Count; i++)
			{
				cells[i].GetTargetsByFlag(flag, ref targetables);
			}
			Profiler.EndSample();
		}

		//TODO: Add a "Get Closest Cell"

		/// <summary>
		/// Returns the cell based on index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static BSPCell GetCellAtIndex(int index) => _cells[index];

		/// <summary>
		/// Returns the amount of cells in the world.
		/// </summary>
		/// <returns></returns>
		public static int CellCount() => _cells == null ? 0 : _cells.Count;

		/// <summary>
		/// returns all cells as an array.
		/// </summary>
		/// <returns></returns>
		public static BSPCell[] GetCells() { return _cells.ToArray(); }

		// Unity Functions.
		private static void Awake()
		{
			//Call to initialize targetflags
			if (TargetFlagHelper.TargetFlags != null) { }
			GeneratePartitions();
		}

		private static void OnDrawGizmosSelected()
		{
			if (_cells == null)
				return;

			Color prevColor = Gizmos.color;
			Gizmos.color = Color.red;

			for (int i = 0; i < _cells.Count; i++)
			{
				BSPCell cell = _cells[i];
				if (cell.Searched)
				{
					Gizmos.color = Color.blue;
					//Gizmos.DrawWireCube(new Vector3(cell.Center.x,0,cell.Center.y), new Vector3(_cellWidth, 0.5f, _cellLength));
				}
				else
					Gizmos.color = Color.red;

				Vector3 line1 = new Vector3(cell.Left, 0, cell.Top);
				Vector3 line2 = new Vector3(cell.Right, 0, cell.Top);

				Vector3 line3 = new Vector3(cell.Left, 0, cell.Bottom);
				Vector3 line4 = new Vector3(cell.Right, 0, cell.Bottom);

				Vector3 line5 = new Vector3(cell.Left, 0, cell.Top);
				Vector3 line6 = new Vector3(cell.Left, 0, cell.Bottom);

				Vector3 line7 = new Vector3(cell.Right, 0, cell.Top);
				Vector3 line8 = new Vector3(cell.Right, 0, cell.Bottom);

				Gizmos.DrawLine(line1, line2);
				Gizmos.DrawLine(line3, line4);
				Gizmos.DrawLine(line5, line6);
				Gizmos.DrawLine(line7, line8);
			}

			Gizmos.color = prevColor;
		}
	}
}