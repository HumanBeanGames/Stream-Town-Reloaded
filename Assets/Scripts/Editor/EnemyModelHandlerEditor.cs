using UnityEngine;
using UnityEditor;
using Enemies;

namespace Globals
{
	[CustomEditor(typeof(EnemyModelHandler))]
	public class EnemyModelHandlerEditor : Editor
	{
		EnemyModelHandler _t;

		private void OnEnable()
		{
			_t = (EnemyModelHandler)target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Randomize Model"))
				{
					_t.RandomizeModel();
					EditorUtility.SetDirty(_t);
				}
				GUILayout.FlexibleSpace();
			}
			GUILayout.EndHorizontal();
		}
	}
}
