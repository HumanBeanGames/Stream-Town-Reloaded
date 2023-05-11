using Character;
using UnityEditor;
using UnityEngine;
using Utils;
using System.Collections.Generic;
using System;

namespace Globals
{
	[CustomEditor(typeof(CharacterModelHandler))]
	public class EquipmentHandlerEditor : Editor
	{
		CharacterModelHandler _t;

		private void OnEnable()
		{
			_t = (CharacterModelHandler)target;
		}
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Setup Equipment", GUILayout.Width(250)))
				{
					_t.ResetEquipmentSet();

					Transform backModels;
					Transform bodyModels;
					Transform eyeModels;
					Transform helmetModels;
					Transform lHandModels;
					Transform rHandModels;
					List<Transform> allParentModels = new List<Transform>();

					for (int i = 0; i < _t.ModelTransform.childCount; i++)
					{
						string tToLower = _t.ModelTransform.GetChild(i).name.ToLower();

						if (tToLower.Contains("back"))
						{
							backModels = _t.ModelTransform.GetChild(i);
							allParentModels.Add(backModels);
						}
						if (tToLower.Contains("body"))
						{
							bodyModels = _t.ModelTransform.GetChild(i);
							allParentModels.Add(bodyModels);
						}
						if (tToLower.Contains("eye"))
						{
							eyeModels = _t.ModelTransform.GetChild(i);
							allParentModels.Add(eyeModels);
						}
						if (tToLower.Contains("helmet"))
						{
							helmetModels = _t.ModelTransform.GetChild(i);
							allParentModels.Add(helmetModels);
						}
						if (tToLower.Contains("lhand"))
						{
							lHandModels = _t.ModelTransform.GetChild(i);
							allParentModels.Add(lHandModels);
						}
						if (tToLower.Contains("rhand"))
						{
							rHandModels = _t.ModelTransform.GetChild(i);
							allParentModels.Add(rHandModels);
						}
					}

					List<GameObject> allChildren = new List<GameObject>();

					for(int i = 0; i < allParentModels.Count;i++)
					{
						for(int j = 0; j < allParentModels[i].childCount;j++)
						{
							allChildren.Add(allParentModels[i].GetChild(j).gameObject);
						}
					}

					foreach (int i in Enum.GetValues(typeof(PlayerRole)))
					{
						PlayerRole role = (PlayerRole)i;

						if (role == PlayerRole.Count)
							continue;

						List<GameObject> validChildren = new List<GameObject>();
						for (int j = 0; j < allChildren.Count; j++)
						{
							string childNameToLower = allChildren[j].name.ToLower();
							string roleNameToLower = (role).ToString().ToLower();

							if (childNameToLower.Contains(roleNameToLower))
							{
								validChildren.Add(allChildren[j].gameObject);
							}
						}

						RoleEquipment e = new RoleEquipment();
						e.RoleName = role.ToString();
						e.PlayerRole = (PlayerRole)i;
						e.Helmet = FindFirstObjectContainingName("Helmet", validChildren);
						e.BodieBulk = FindFirstObjectContainingName("Bulk", validChildren);
						e.BodieSlim = FindFirstObjectContainingName("Slim", validChildren);
						e.BodieFeminine = FindFirstObjectContainingName("Feminine", validChildren);
						e.LeftHand = FindFirstObjectContainingName("LHand", validChildren);
						if (e.LeftHand == null)
							e.LeftHand = FindFirstObjectContainingName("Back", validChildren);
						e.RightHand = FindFirstObjectContainingName("RHand", validChildren);
						_t.AddEquipmentSet(e);
					}
					EditorUtility.SetDirty(_t);
				}
				GUILayout.FlexibleSpace();
			}
			GUILayout.EndHorizontal();
		}

		private GameObject FindFirstObjectContainingName(string s, List<GameObject> objects)
		{
			for (int i = 0; i < objects.Count; i++)
			{
				if (objects[i].name.ToLower().Contains(s.ToLower()))
					return objects[i];
			}

			return null;
		}
	}
}
