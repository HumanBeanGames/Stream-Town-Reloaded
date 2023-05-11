using Buildings;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Globals
{
	[CustomEditor(typeof(BuildingResourceModelHandler))]
	public class BuildingResourceModelHandlerEditor : Editor
	{

		BuildingResourceModelHandler _t;

		private void OnEnable()
		{
			_t = (BuildingResourceModelHandler)target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Setup Models", GUILayout.Width(250)))
				{
					Transform child = _t.transform;
					_t.EmptyModel = child.GetChild(0).gameObject;

					for (int i = 1; i < child.childCount; i++)
					{

						Transform t = child.GetChild(i);
						string nameToLower = t.name.ToLower();

						if (nameToLower.Contains("halffull"))
							_t.HalfFullModel = t.gameObject;
						else if (nameToLower.Contains("full"))
							_t.FullModel = t.gameObject;

					}

					EditorUtility.SetDirty(_t);
				}
				GUILayout.FlexibleSpace();
			}
			GUILayout.EndHorizontal();
		}
	}
}
