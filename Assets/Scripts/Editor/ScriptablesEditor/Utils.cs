using UnityEditor;
using UnityEngine;
using Utils;

namespace ScriptablesEditor
{
    public static class Utils
    {
        public static void DrawDataFieldAndLabel(string label, ref int value, int fieldWidth = 100, bool vertical = true, bool formatByUpperCase = true, params GUILayoutOption[] guiLayoutOptions)
        {
            StartLayout(vertical);
            string l = formatByUpperCase ? StringUtils.FormatStringByUpperCase(label) : label;
            GUILayout.Label(l, guiLayoutOptions);
            value = EditorGUILayout.IntField(value, GUILayout.MaxWidth(fieldWidth));
            EndLayout(vertical);
        }

        public static void DrawDataFieldAndLabel(string label, ref float value, int fieldWidth = 100, bool vertical = true, bool formatByUpperCase = true, params GUILayoutOption[] guiLayoutOptions)
        {
            StartLayout(vertical);
            string l = formatByUpperCase ? StringUtils.FormatStringByUpperCase(label): label;
            GUILayout.Label(l, guiLayoutOptions);
            value = EditorGUILayout.FloatField(value, GUILayout.MaxWidth(fieldWidth));
            EndLayout(vertical);
        }

        public static void DrawDataFieldAndLabel(string label, ref string value, int fieldWidth = 100, bool vertical = true, bool formatByUpperCase = true, params GUILayoutOption[] guiLayoutOptions)
        {
            StartLayout(vertical);
            string l = formatByUpperCase ? StringUtils.FormatStringByUpperCase(label) : label;
            GUILayout.Label(l, guiLayoutOptions);
            value = EditorGUILayout.TextField(value, GUILayout.MaxWidth(fieldWidth));
            EndLayout(vertical);
        }

        public static void DrawDataFieldAndLabel(string label, ref bool value, bool vertical = true, bool formatByUpperCase = true, params GUILayoutOption[] guiLayoutOptions)
        {
            StartLayout(vertical);
            string l = formatByUpperCase ? StringUtils.FormatStringByUpperCase(label) : label;
            GUILayout.Label(l, guiLayoutOptions);
            value = EditorGUILayout.Toggle(value);
            EndLayout(vertical);
        }

        private static void StartLayout(bool vertical)
        {
            if (vertical)
                EditorGUILayout.BeginVertical();
            else
                EditorGUILayout.BeginHorizontal();
        }

        private static void EndLayout(bool vertical)
        {
            if (vertical)
                EditorGUILayout.EndVertical();
            else
                EditorGUILayout.EndHorizontal();
        }
    }
}