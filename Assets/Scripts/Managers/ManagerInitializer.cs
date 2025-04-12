using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

public static class ManagerInitializer
{
    /// <summary>
    /// Loads default values from a JSON file into static fields marked with ManagedFieldAttribute.
    /// </summary>
    /// <param name="managerType">The type of the manager class to load values for.</param>
    private static void LoadManagedFields(Type managerType)
    {
        // Construct the path to the JSON file based on the manager type name.
        string path = $"Assets/Editor/ManagerDefaults/{managerType.Name}.json";
        
        // Check if the JSON file exists.
        if (System.IO.File.Exists(path))
        {
            // Read the JSON file content.
            string json = System.IO.File.ReadAllText(path);
            // Deserialize the JSON content into a dictionary of field names and values.
            var fieldValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            // Iterate over each static field in the manager class.
            foreach (var field in managerType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static))
            {
                // Check if the field is marked with ManagedFieldAttribute and has a corresponding value in the JSON.
                if (field.GetCustomAttribute<ManagedFieldAttribute>() != null && fieldValues.ContainsKey(field.Name))
                {
                    // Set the field value using the value from the JSON, converting it to the correct type.
                    field.SetValue(null, Convert.ChangeType(fieldValues[field.Name], field.FieldType));
                }
            }
        }
    }

    /// <summary>
    /// Initializes all manager classes by loading default values and invoking initialization methods.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeAllManagers()
    {
        Debug.Log("Initializing Managers!");

        // Get all types in the current assembly that have the ManagerAttribute.
        var managerTypes = Assembly.GetExecutingAssembly()
                                   .GetTypes()
                                   .Where(t => t.GetCustomAttribute<ManagerAttribute>() != null);

        // Iterate over each manager type.
        foreach (var type in managerTypes)
        {
            // Load default values from JSON into managed fields.
            LoadManagedFields(type);

            // Find a method marked with ManagerInitializationAttribute to invoke.
            var initializeMethod = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                                       .FirstOrDefault(m => m.GetCustomAttribute<ManagerInitializationAttribute>() != null);
            if (initializeMethod != null)
            {
                // Invoke the initialization method.
                initializeMethod.Invoke(null, null);
                Debug.Log($"{type.Name} initialized.");
            }
        }

        Debug.Log("Manager initialization complete!");
    }
}