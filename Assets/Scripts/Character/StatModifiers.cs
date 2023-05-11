using System.Collections.Generic;
using Utils;

namespace Character 
{
    public class StatModifiers 
	{
		private Dictionary<StatType, int> _modifiers;

		public StatModifiers()
		{
			_modifiers = new Dictionary<StatType, int>();

			for(int i = 0; i < (int)StatType.Count;i++)
			{
				_modifiers.Add((StatType)i, 0);
			}
		}

		public int GetModifier(StatType stat)
		{
			return _modifiers[stat];
		}

		public void AddToModifier(StatType stat, int amount)
		{
			_modifiers[stat] += amount;
		}

		public void RemoveFromModifier(StatType stat, int amount)
		{
			_modifiers[stat] -= amount;
		}

		public void SetModifier(StatType stat, int value)
		{
			_modifiers[stat] = value;
		}
    }
}