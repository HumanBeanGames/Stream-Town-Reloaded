using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

namespace Scriptables
{
	[CreateAssetMenu(fileName = "AllSeasonsScriptableData", menuName = "ScriptableObjects/AllSeasonsScriptable", order = 1)]
	public class AllSeasonsScriptable : ScriptableObject
	{
        [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        public SeasonScriptable[] AllSeasons;

		public SeasonScriptable GetSeasonData(Season season)
		{
			for (int i = 0; i < AllSeasons.Length; i++)
			{
				if (AllSeasons[i].Season == season)
					return AllSeasons[i];
			}

			Debug.LogError($"Tried to return a season that hasn't been setup: {season}");
			return null;
		}
	}
}