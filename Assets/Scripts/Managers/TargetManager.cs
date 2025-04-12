using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Target;
using UnityEngine;
using Utils;

//TODO:: Check if this is still required after BSP implementation
[Manager]
public class TargetManager
{
    private static Dictionary<TargetMask, List<Targetable>> _targetDictionary = new Dictionary<TargetMask, List<Targetable>>();
    [ManagedField]
    private static TargetableData[] _targetableData = new TargetableData[0];

    [ManagerInitialization]
    public static void Initialize()
    {
        string path = "Assets/Editor/ManagerDefaults/TargetManager.json";
        if (System.IO.File.Exists(path))
        {
            string json = System.IO.File.ReadAllText(path);
            _targetableData = JsonConvert.DeserializeObject<TargetableData[]>(json);
        }
        else
        {
            Debug.LogWarning("TargetManager default data file not found.");
        }
    }

    public static StationUpdate GetUpdateType(TargetMask type)
    {
        return _targetableData[TargetFlagHelper.GetIndexByFlag(type)].UpdateType;
    }

    public static List<Targetable> GetSingleTargetList(TargetMask type)
    {
        if (_targetDictionary.TryGetValue(type, out var list))
        {
            return list;
        }

        Debug.LogWarning($"[TargetManager] No list found for TargetMask: {type}");
        return new List<Targetable>();
    }

    /// <summary>
    /// Gets all targets defined by the flag into one list.
    /// </summary>
    /// <param name="flag"></param>
    /// <returns></returns>
    public static List<Targetable> GetTargetsByFlag(TargetMask flag)
    {
        List<Targetable> targets = new List<Targetable>();

        foreach (int i in Enum.GetValues(typeof(TargetMask)))
        {

            TargetMask t = (TargetMask)i;

            if (t == TargetMask.Nothing)
                continue;

            if (!flag.HasFlag(t) || !_targetDictionary.ContainsKey(t))
                continue;

            targets.AddRange(_targetDictionary[t]);

        }

        return targets;
    }

    /// <summary>
    /// Adds a target to the target dictionary
    /// </summary>
    /// <param name="target"></param>
    public static void AddTarget(Targetable target)
    {
        // Add to each flag type
        foreach (int i in Enum.GetValues(typeof(TargetMask)))
        {

            TargetMask t = (TargetMask)i;

            if (t == TargetMask.Nothing)
                continue;

            if (target.TargetType.HasFlag(t))
            {
                AddTarget(t, target);
            }
        }
    }

    /// <summary>
    /// Removes a target from the target dictionary
    /// </summary>
    /// <param name="target"></param>
    public static void RemoveTarget(Targetable target)
    {
        foreach (int i in Enum.GetValues(typeof(TargetMask)))
        {
            TargetMask t = (TargetMask)i;

            if (t == TargetMask.Nothing)
                continue;

            if (target.TargetType.HasFlag(t))
            {
                RemoveTarget(t, target);
            }
        }
    }

    /// <summary>
    /// Adds a Targetable object to the target dictionary.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="target"></param>
    private static void AddTarget(TargetMask type, Targetable target)
    {
        if (!_targetDictionary.ContainsKey(type))
            _targetDictionary[type] = new List<Targetable>();

        if (_targetDictionary[type].Contains(target))
            return;

        _targetDictionary[type].Add(target);
    }

    /// <summary>
    /// Removes a Targetable object from the target dictionary.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="target"></param>
    private static void RemoveTarget(TargetMask type, Targetable target)
    {
        if (!_targetDictionary.ContainsKey(type))
            return;

        if (!_targetDictionary[type].Contains(target))
            return;

        _targetDictionary[type].Remove(target);
    }
}