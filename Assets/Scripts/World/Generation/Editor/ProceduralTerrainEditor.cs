using UnityEditor;
using UnityEngine;

namespace World.Generation
{
	[CustomEditor(typeof(ProceduralWorldGenerator))]
	public class ProceduralTerrainEditor : Editor
	{
		ProceduralWorldGenerator _t;

		private void OnEnable()
		{
			_t = (ProceduralWorldGenerator)target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Generate"))
				{
					_t.GenerateTerrain();
				}
				GUILayout.FlexibleSpace();
			}
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Generate Fake Resources"))
				{
					_t.MainMenuGenerateWorld();
				}
				GUILayout.FlexibleSpace();
			}
			GUILayout.EndHorizontal();
		}
	}
}
