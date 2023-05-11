using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
	/// <summary>
	/// Handles all Tiler objects that need to be processed for tiling.
	/// </summary>
	public static class TileHelper
	{
		/// <summary>
		/// Lookup table to score a tile based on neighbouring positions.
		/// </summary>
		public static int[,] TileValues = new int[,]
		{
			{0,2,0},
			{16,0,4},
			{0,8,0}
		};

		/// <summary>
		/// A Queue of Tiles that need to be updated.
		/// </summary>
		public static Queue<Tiler> _tilesToUpdate = new Queue<Tiler>();

		/// <summary>
		/// Updates the value of the tile based on it's neighbours. Has the option to add neighbours
		/// to the update queue to be processed.
		/// </summary>
		/// <param name="position"></param>
		/// <param name="tag"></param>
		/// <param name="size"></param>
		/// <param name="enqueueNeighbours"></param>
		/// <returns></returns>
		public static int CalculateTileValue(Vector3 position, string tag, int size, bool enqueueNeighbours = false)
		{
			int newTileValue = 0;

			// Check all positions neighbouring this tile in a grid.
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					if (i == 0 && j == 0)
						continue;

					Vector3 rayPos = new Vector3(position.x - (i * size), position.y + 5, position.z - (j * size));

					// Check if there is a building next to this tile.
					// This may need to change in future depending on the type of things that might need to be tiled.
					if (Physics.Raycast(rayPos, Vector3.down, out RaycastHit hit, 10, LayerMask.GetMask("Building"), QueryTriggerInteraction.Ignore))
					{
						if (hit.transform.tag != tag)
						{
							Debug.DrawLine(rayPos, rayPos + (Vector3.up * -5), Color.red, 5);
							continue;
						}

						Debug.DrawLine(rayPos, rayPos + (Vector3.up * 5), Color.blue, 5);
						newTileValue += TileValues[i + 1, j + 1];

						if (enqueueNeighbours && hit.transform.TryGetComponent(out Tiler t))
						{
							_tilesToUpdate.Enqueue(hit.transform.GetComponent<Tiler>());
						}
					}
				}
			}

			return newTileValue;
		}

		/// <summary>
		/// Processes the queue of tiles that need to be updated.
		/// </summary>
		public static void ProcessQueue()
		{
			for (int i = 0; i < 5; i++)
				if (_tilesToUpdate.Count > 0)
				{
					Tiler t = _tilesToUpdate.Dequeue();
					t.UpdateTileValue(t.TileValue);
				}
				else
					i = 5;
		}
	}
}