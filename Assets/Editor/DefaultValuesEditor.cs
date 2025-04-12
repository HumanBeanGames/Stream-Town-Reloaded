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

        selectedTab = GUILayout.Toolbar(selectedTab, managerTabs);

        GUILayout.Space(10);

        managerActions[managerTabs[selectedTab]].Invoke();

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
                object newValue = currentValue;

                if (field.FieldType == typeof(float))
                {
                    newValue = EditorGUILayout.FloatField(field.Name, currentValue != null ? Convert.ToSingle(currentValue) : 0f);
                }
                else if (field.FieldType == typeof(int))
                {
                    newValue = EditorGUILayout.IntField(field.Name, currentValue != null ? Convert.ToInt32(currentValue) : 0);
                }
                else if (field.FieldType == typeof(string))
                {
                    newValue = EditorGUILayout.TextField(field.Name, currentValue != null ? Convert.ToString(currentValue) : "");
                }
                // Add more types as needed

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
