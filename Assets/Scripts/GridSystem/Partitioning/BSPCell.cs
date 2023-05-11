using System.Collections.Generic;
using Target;
using UnityEngine;
using Utils;

namespace GridSystem.Partitioning
{
	/// <summary>
	/// A cell used in the Cell Space Partitioning System.
	/// </summary>
	[System.Serializable]
	public class BSPCell
	{
		public List<Targetable>[] _targetArray;

		public Vector2 TopLeft;
		public Vector2 BottomRight;
		public Vector2 Center;

		public bool Searched;

		public float Top => TopLeft.y;
		public float Left => TopLeft.x;
		public float Bottom => BottomRight.y;
		public float Right => BottomRight.x;

		// Constructor.
		public BSPCell(Vector2 topLeft, Vector2 bottomRight)
		{
			TopLeft = topLeft;
			BottomRight = bottomRight;
			Center = (topLeft + bottomRight) / 2;
			_targetArray = new List<Targetable>[TargetFlagHelper.TargetFlagCount - 1];
			Searched = false;
		}

		/// <summary>
		/// Returns true if overlapping.
		/// </summary>
		/// <param name="topLeft"></param>
		/// <param name="bottomRight"></param>
		/// <returns></returns>
		public bool IsOverlapping(Vector2 topLeft, Vector2 bottomRight)
		{
			return !(topLeft.x > Right
				|| bottomRight.x < Left
				|| topLeft.y > Bottom
				|| bottomRight.y < Top
				);
		}

		/// <summary>
		/// Returns true if two BSPCells are overlapping.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool IsOverlapping(BSPCell other)
		{
			return IsOverlapping(other.TopLeft, other.BottomRight);
		}

		/// <summary>
		/// Adds a target to the Cell
		/// </summary>
		/// <param name="target"></param>
		public void AddTarget(Targetable target)
		{
			for (int i = 0; i < TargetFlagHelper.TargetFlagCount - 1; i++)
			{
				if (target.TargetType.HasFlag(TargetFlagHelper.TargetFlags[i + 1]))
				{
					AddTarget(i, target);
				}
			}
		}

		/// <summary>
		/// Removes a target from the Cell
		/// </summary>
		/// <param name="target"></param>
		public void RemoveTarget(Targetable target)
		{
			for (int i = 0; i < TargetFlagHelper.TargetFlagCount - 1; i++)
			{
				if (target.TargetType.HasFlag(TargetFlagHelper.TargetFlags[i + 1]))
				{
					RemoveTarget(i, target);
				}
			}
		}

		/// <summary>
		/// Gets all targets defined by the Target mask into one list.
		/// </summary>
		/// <param name="targetMask"></param>
		/// <returns></returns>
		public void GetTargetsByFlag(TargetMask targetMask, ref List<Targetable> targets)
		{
			Searched = true;
			for (int i = 0; i < _targetArray.Length; i++)
			{
				if ((targetMask & TargetFlagHelper.TargetFlags[i + 1]) != 0)
				{
					if (_targetArray[i] != null)
						targets.AddRange(_targetArray[i]);
				}
			}

		}

		/// <summary>
		/// Adds a target to the cell.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="target"></param>
		private void AddTarget(int index, Targetable target)
		{
			if (_targetArray[index] == null)
				_targetArray[index] = new List<Targetable>();

			else if (_targetArray[index].Contains(target))
				return;

			_targetArray[index].Add(target);

		}

		/// <summary>
		/// Removes a target from the cell.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="target"></param>
		private void RemoveTarget(int index, Targetable target)
		{
			if (_targetArray[index] == null)
				return;
			if (!_targetArray[index].Contains(target))
				return;

			_targetArray[index].Remove(target);
		}
	}
}