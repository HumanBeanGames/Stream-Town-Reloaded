using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
	/// <summary>
	/// Holds a list of ChanceObjects with a given data type and chance value.
	/// Handles getting a random item from the list based on a dice roll.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[System.Serializable]
	public class ChanceObjectList<T>
	{
		[SerializeField]
		private List<ChanceObject<T>> _list = new List<ChanceObject<T>>();

		private float _totalChance;

		/// <summary>
		/// Returns the total value of all chances added together.
		/// </summary>
		public float TotalChance => _totalChance;

		/// <summary>
		/// Gets a random item from the list using chance. Empty Chance will increase the likelihood of getting nothing.
		/// Example: EmptyChance = 2 will result in a 50% of getting nothing.
		/// </summary>
		/// <param name="_objects"></param>
		/// <param name="emptyChance"></param>
		/// <returns></returns>
		public T GetRandomObject(float emptyChance = 0)
		{
			float roll = Random.Range(0.0f, _totalChance * (emptyChance + 1));
			T selected;

			float cumulative = 0;

			for (int i = 0; i < _list.Count; i++)
			{
				cumulative += _list[i].Chance;
				if (roll < cumulative)
				{
					selected = _list[i].Object;
					return selected;
				}
			}

			return default;
		}

		/// <summary>
		/// Adds an ChanceObject to the list.
		/// </summary>
		/// <param name="obj"></param>
		public void AddObject(ChanceObject<T> obj)
		{
			_list.Add(obj);
			CalculateTotalChance();
		}

		/// <summary>
		/// Removes a ChanceObject from the list.
		/// </summary>
		/// <param name="obj"></param>
		public void RemoveObject(ChanceObject<T> obj)
		{
			_list.Remove(obj);
			CalculateTotalChance();
		}

		/// <summary>
		/// Calculates the TotalChance of all list items.
		/// </summary>
		public void CalculateTotalChance()
		{
			_totalChance = 0;

			for (int i = 0; i < _list.Count; i++)
			{
				_totalChance += _list[i].Chance;
			}
		}
	}
}