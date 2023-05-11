using UnityEditor;
using UnityEngine;
// System may be obsolete.
namespace GridSystem
{
	[CustomEditor(typeof(GridManager))]
	public class GridSystemEditor : Editor
	{
		GridManager _manager;

		private void OnEnable()
		{
			_manager = (GridManager)target;
		}


		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Generate Grid", GUILayout.Width(250)))
				{
					_manager.GenerateGrid();
					EditorUtility.SetDirty(_manager);
				}
				GUILayout.FlexibleSpace();
			}
			GUILayout.EndHorizontal();
		}
	}
}