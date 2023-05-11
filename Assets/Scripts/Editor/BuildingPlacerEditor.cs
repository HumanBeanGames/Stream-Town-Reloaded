using UnityEngine;
using UnityEditor;
using Buildings;

namespace Globals
{
	[CustomEditor(typeof(BuildingPlacer))]
	public class BuildingPlacerEditor : Editor
	{

		BuildingPlacer _t;

		private void OnEnable()
		{
			_t = (BuildingPlacer)target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Init Data", GUILayout.Width(250)))
				{
					//_t.InitBuildingData();
				}
				GUILayout.FlexibleSpace();
			}
			GUILayout.EndHorizontal();
		}
	}
}
