using UnityEngine;
using UnityEditor;
using Buildings;
using System.Collections.Generic;

namespace Globals
{
	[CustomEditor(typeof(BuildingModelHandler))]
	public class BuildingModelHandlerEditor : Editor
	{
		BuildingModelHandler _t;

		private void OnEnable()
		{
			_t = (BuildingModelHandler)target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Setup models", GUILayout.Width(250)))
				{
					Transform child = _t.transform;
					List<Transform> children = new List<Transform>();
					_t.Upgrades = new List<GameObject>();
					_t.OtherModels = new List<GameObject>();

					for (int i = 0; i < child.childCount; i++)
					{
						children.Add(child.GetChild(i));
					}

					for (int i = 0; i < children.Count; i++)
					{
						string nameToLower = children[i].name.ToLower();
						if (nameToLower.Contains("base"))
							_t.FullModel = children[i].gameObject;
						else if (nameToLower.Contains("stage_01") && !nameToLower.Contains("upgrade"))
							_t.Stage1 = children[i].gameObject;
						else if (nameToLower.Contains("stage_02") && !nameToLower.Contains("upgrade"))
							_t.Stage2 = children[i].gameObject;
						else if (nameToLower.Contains("stage_03") && !nameToLower.Contains("upgrade"))
							_t.Stage3 = children[i].gameObject;
						else if (nameToLower.Contains("upgrade"))
							_t.Upgrades.Add(children[i].gameObject);
						else
							_t.OtherModels.Add(children[i].gameObject);
					}
					EditorUtility.SetDirty(_t);
				}
				GUILayout.FlexibleSpace();
			}
			GUILayout.EndHorizontal();
		}
	}
}