using Sirenix.OdinInspector;
using UnityEngine;
using Utils;

namespace Scriptables
{
	[CreateAssetMenu(fileName = "All Role Data", menuName = "ScriptableObjects/AllRoleDataScriptable", order = 2)]
	public class AllRoleDataScriptable : ScriptableObject
	{
        [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        public RoleDataScriptable[] RoleData;

		public RoleDataScriptable GetDataByRoleType(PlayerRole role)
		{
			for (int i = 0; i < RoleData.Length; i++)
			{
				if (RoleData[i].Role == role)
					return RoleData[i];
			}
			Debug.LogError($"Attempted to get role that didnt exist in All Role Data: {role}");
			return null;
		}
	}
}