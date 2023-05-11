using Pathfinding;
using UnityEngine;

namespace Utils
{
	/// <summary>
	/// A component used for Updating the A* Graph in the given bounds.
	/// </summary>
	public class UpdateGraphBounds : MonoBehaviour
	{
		[SerializeField]
		private int _tag = 0;
		[SerializeField]
		private bool _walkable = false;
		[SerializeField, Tooltip("Set to false if it should modify the graph upon spawn.")]
		private bool _firstEnable = true;
		[SerializeField]
		private bool _enableOnPooled = false;

		private BoxCollider _boxCollider;

		/// <summary>
		/// Sets the A* Graph bounds to reflect the settings.
		/// </summary>
		public void SetGraphBounds()
		{
			if (AstarPath.active == null || _boxCollider == null)
				return;
			var guo = new GraphUpdateObject(_boxCollider.bounds);
			guo.modifyWalkability = true;
			guo.setWalkability = _walkable;
			guo.modifyTag = true;
			guo.setTag = _tag;
			AstarPath.active.UpdateGraphs(guo);
		}

		/// <summary>
		/// Clears any penalities and modifications to the A* grid within the bounds.
		/// </summary>
		public void UnsetGraphBounds()
		{
			if (_firstEnable)
			{
				_firstEnable = false;
				return;
			}

			if (AstarPath.active == null)
				return;

			var guo = new GraphUpdateObject(_boxCollider.bounds);
			guo.modifyWalkability = true;
			guo.setWalkability = true;
			guo.modifyTag = true;
			guo.setTag = 0;
			AstarPath.active.UpdateGraphs(guo);
		}

		// Unity Methods
		private void Awake()
		{
			_boxCollider = GetComponent<BoxCollider>();
		}

		private void Start()
		{
			if (_firstEnable || !_enableOnPooled)
				return;

			SetGraphBounds();
		}

		private void OnDisable()
		{
			UnsetGraphBounds();
		}
	}
}