using Scriptables;
using UnityEditor;
using UnityEngine;
using Utils;

namespace ScriptablesEditor
{
    public class BuildingScriptablesEditor
    {
        public static void DrawBuildingScriptableData(object data)
        {
            BuildingDataScriptable building = (BuildingDataScriptable)data;

            if (building == null)
                return;

            GUILayout.Box(building.BuildingSprite.texture, GUILayout.Width(192), GUILayout.Height(192));

            GUILayout.Label("Building Type:");
            building.BuildingType = (BuildingType)EditorGUILayout.EnumPopup("", building.BuildingType, GUILayout.MaxWidth(100));

            GUILayout.Space(32);
            Utils.DrawDataFieldAndLabel("Can Level:", ref building.CanLevel);
            Utils.DrawDataFieldAndLabel("Placeable:", ref building.Placeable);

        }
    }
}