
using UnityEngine;

namespace Buildings
{
	/// <summary>
	/// A Generic Tiler class that is used for any objects that need to be tiled.
	/// </summary>
	public class Tiler : MonoBehaviour
	{
		/// <summary>
		/// The unit size of the object in the world.
		/// </summary>
		[SerializeField]
		protected int _size = 2;

		/// <summary>
		/// Comparison tag used to tile the same objects.
		/// </summary>
		[SerializeField]
		protected string _tag;

		protected int _tileValue = -1;

		public int TileValue => _tileValue;

		/// <summary>
		/// Updates the value of this tile based on it's neighbours.
		/// </summary>
		/// <param name="currentValue"></param>
		/// <param name="enqueueNeighbours"></param>
		public virtual void UpdateTileValue(int currentValue, bool enqueueNeighbours = false)
		{
			_tileValue = TileHelper.CalculateTileValue(transform.position, _tag, _size, enqueueNeighbours);

			OnTileValueChanged();
		}

		protected virtual void Init() { }

		protected virtual void OnTileValueChanged() { }

		// Unity Events.
		private void Awake()
		{
			Init();
		}

		private void OnEnable()
		{
			_tileValue = -1;
			UpdateTileValue(_tileValue, true);
		}
	}
}