using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using Newtonsoft.Json;

public class DefaultValuesEditor : EditorWindow
{
    private const string JsonDirectory = "Assets/Editor/ManagerDefaults/";

    private int selectedTab = 0;
    private string[] managerTabs;
    private Dictionary<string, Action> managerActions;
    private Dictionary<string, Dictionary<string, object>> managerDefaults;
    private Vector2 scrollPosition;

    [MenuItem("Window/Default Values Editor")]
    public static void ShowWindow()
    {
        GetWindow<DefaultValuesEditor>("Default Values Editor");
    }

    private void OnEnable()
    {
        Directory.CreateDirectory(JsonDirectory);
        var managerTypes = GetTypesWithManagerAttribute();
        managerTabs = managerTypes.Select(t => t.Name).ToArray();
        managerActions = managerTypes.ToDictionary(
            type => type.Name,
            type => (Action)(() => DisplayManagerDefaults(type))
        );
        managerDefaults = new Dictionary<string, Dictionary<string, object>>();

        LoadDefaults(managerTypes);
    }

    private IEnumerable<Type> GetTypesWithManagerAttribute()
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.GetCustomAttributes(typeof(ManagerAttribute), true).Length > 0);
    }

    private void OnGUI()
    {
        GUILayout.Label("Set Default Values", EditorStyles.boldLabel);

        int maxTabsPerRow = 5;
        int rows = Mathf.CeilToInt((float)managerTabs.Length / maxTabsPerRow);
        int previousTab = selectedTab;
        for (int row = 0; row < rows; row++)
        {
            int start = row * maxTabsPerRow;
            int end = Mathf.Min(start + maxTabsPerRow, managerTabs.Length);
            string[] rowTabs = managerTabs.Skip(start).Take(end - start).ToArray();
            selectedTab = GUILayout.Toolbar(selectedTab, rowTabs);
        }

        if (selectedTab != previousTab)
        {
            GUI.FocusControl(null); // Reset focus to avoid lingering input
        }

        GUILayout.Space(10);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(position.width), GUILayout.Height(position.height - 100));
        managerActions[managerTabs[selectedTab]].Invoke();
        GUILayout.EndScrollView();

        if (GUILayout.Button("Open JSON File"))
        {
            string path = JsonDirectory + managerTabs[selectedTab] + ".json";
            if (File.Exists(path))
            {
                System.Diagnostics.Process.Start("explorer.exe", $"/select, {Path.GetFullPath(path)}");
            }
            else
            {
                Debug.LogWarning($"JSON file for {managerTabs[selectedTab]} does not exist.");
            }
        }
    }

    private object DisplayField(string fieldName, object value, Type fieldType)
    {
        if (fieldType == typeof(float))
        {
            value = EditorGUILayout.FloatField(fieldName, Convert.ToSingle(value));
        }
        else if (fieldType == typeof(int))
        {
            value = EditorGUILayout.IntField(fieldName, Convert.ToInt32(value));
        }
        else if (fieldType == typeof(string))
        {
            value = EditorGUILayout.TextField(fieldName, Convert.ToString(value));
        }
        else if (fieldType.IsEnum)
        {
            value = EditorGUILayout.EnumPopup(fieldName, (Enum)value);
        }
        else if (fieldType.IsArray)
         {
            var elementType = fieldType.GetElementType();
            var array = value as Array ?? Array.CreateInstance(elementType, 0);
            int arraySize = EditorGUILayout.IntField(fieldName + " Size", array.Length);
            if (arraySize != array.Length)
            {
                Array newArray = Array.CreateInstance(elementType, arraySize);
                Array.Copy(array, newArray, Math.Min(array.Length, arraySize));
                array = newArray;
            }
            for (int i = 0; i < array.Length; i++)
            {
                object elementValue = array.GetValue(i);
                if (elementValue == null && elementType.GetConstructor(Type.EmptyTypes) != null)
                {
                    elementValue = Activator.CreateInstance(elementType);
                    array.SetValue(elementValue, i);
                }
                array.SetValue(DisplayField(fieldName + "[" + i + "]", elementValue, elementType), i);
            }
            value = array;
        }
        else if (fieldType.IsValueType && !fieldType.IsPrimitive)
        {
            if (value == null)
            {
                value = Activator.CreateInstance(fieldType);
            }
            EditorGUILayout.LabelField(fieldName);
            var structCopy = value; // Create a copy of the struct
            foreach (var subField in fieldType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                object subValue = subField.GetValue(structCopy);
                subValue = DisplayField(subField.Name, subValue, subField.FieldType);
                subField.SetValue(structCopy, subValue); // Use SetValue instead of SetValueDirect
            }
            value = structCopy; // Assign back the modified struct
        }
        return value;
    }

    private void DisplayManagerDefaults(Type managerType)
    {
        GUILayout.Label(managerType.Name + " Defaults", EditorStyles.boldLabel);

        if (!managerDefaults.ContainsKey(managerType.Name))
        {
            managerDefaults[managerType.Name] = LoadDefaultsFromJson(managerType);
        }

        foreach (var field in managerType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
        {
            if (field.GetCustomAttribute<ManagedFieldAttribute>() != null)
            {
                object currentValue = managerDefaults[managerType.Name].ContainsKey(field.Name) ? managerDefaults[managerType.Name][field.Name] : null;
                object newValue = DisplayField(field.Name, currentValue, field.FieldType);
                managerDefaults[managerType.Name][field.Name] = newValue;
            }
        }

        if (GUILayout.Button("Save Values"))
        {
            SaveDefaultsToJson(managerType);
        }
    }

    private Dictionary<string, object> LoadDefaultsFromJson(Type managerType)
    {
        string path = JsonDirectory + managerType.Name + ".json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        }
        return new Dictionary<string, object>();
    }

    private void SaveDefaultsToJson(Type managerType)
    {
        string json = JsonConvert.SerializeObject(managerDefaults[managerType.Name], Formatting.Indented);
        File.WriteAllText(JsonDirectory + managerType.Name + ".json", json);
    }

    private void LoadDefaults(IEnumerable<Type> managerTypes)
    {
        foreach (var type in managerTypes)
        {
            string path = JsonDirectory + type.Name + ".json";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                managerDefaults[type.Name] = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            }
        }
    }

    private void SaveDefaults(Type managerType)
    {
        string json = JsonConvert.SerializeObject(managerDefaults[managerType.Name], Formatting.Indented);
        File.WriteAllText(JsonDirectory + managerType.Name + ".json", json);
    }
}
