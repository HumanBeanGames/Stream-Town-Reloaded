using Managers;
using SavingAndLoading.SavableObjects;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.Pooling;

namespace GUIDSystem
{
	/// <summary>
	/// Class that manages GUIDs 
	/// </summary>
	[GameManager]
	public static class GUIDManager
    {
        [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
        private static GuidConfig Config = GuidConfig.Instance;

        [HideInInspector]
		private static Dictionary<string, Dictionary<uint, PoolableObject>> _worldObjects;
		[HideInInspector]
		private static bool _initialized = false;

		public static bool IsInitialized => _initialized;

		/// <summary>
		/// Initializes the dictionary
		/// </summary>
		public static void Initialize()
		{
			if (!_initialized)
			{
				_worldObjects = new Dictionary<string, Dictionary<uint, PoolableObject>>();
				for (int i = 0; i < (int)PoolType.Count; i++)
				{
					Dictionary<uint, PoolableObject> dic = new Dictionary<uint, PoolableObject>();

					_worldObjects.Add(((PoolType)i).ToString(), dic);
				}
			}
			else
				Debug.Log("GUIDManager: Is already initialized" + typeof(GUIDManager));

			_initialized = true;
		}

		/// <summary>
		/// Creates a new GUID and adds to dictionary
		/// </summary>
		/// <param name="comp"></param>
		/// <returns></returns>
		public static uint CreateGUIDandAddToDictionary(PoolableObject comp)
		{
			uint gUID = GenerateNewGUID(comp.PoolType.ToString());
			((SaveableObject)comp.SaveableObject).GUIDComponent.SetGUID(gUID);
			if (_worldObjects.ContainsKey((comp.PoolType).ToString()) && _worldObjects[comp.PoolType.ToString()].ContainsKey(gUID))
				_worldObjects[comp.PoolType.ToString()].Add(((SaveableObject)comp.SaveableObject).GUIDComponent.GUID, comp);

			return gUID;
		}

		public static void RemoveFromGUID(PoolType type, uint gUID)
		{
			if (_worldObjects.ContainsKey(type.ToString()))
			{
				if (_worldObjects[type.ToString()].ContainsKey(gUID))
					_worldObjects[type.ToString()].Remove(gUID);
			}
		}

		/// <summary>
		/// Generates a new GUID
		/// </summary>
		/// <returns>The new GUID</returns>
		private static uint GenerateNewGUID(string type)
		{
			uint gUID = 0;
			bool newGUIDFound = false;

			while (!newGUIDFound)
			{
				gUID = (uint)Random.Range(uint.MinValue, uint.MaxValue);

				// make code to create a GUID
				if (!_worldObjects[type].ContainsKey(gUID) && gUID != 0)
					newGUIDFound = true;
			}
			return gUID;
		}

		/// <summary>
		/// Adds A GUIDComponent to the dictionary,
		/// </summary>
		/// <param name="comp">The component being added to the dictionary</param>
		public static void AddToDictionary(PoolableObject comp)
		{
			List<uint> keys = _worldObjects[comp.PoolType.ToString()].Keys.ToList();
			if (!_worldObjects[comp.PoolType.ToString()].ContainsKey(((SaveableObject)comp.SaveableObject).GUIDComponent.GUID))
				_worldObjects[comp.PoolType.ToString()].Add(((SaveableObject)comp.SaveableObject).GUIDComponent.GUID, comp);
			else if (((SaveableObject)comp.SaveableObject).GUIDComponent.GUID == 0)
				Debug.Log("GUID == 0");
			else
				Debug.Log("Duplicate GUID detected");
		}

		/// <summary>
		/// Search for component using a GUID,
		/// </summary>
		/// <param name="gUID">A GUID as a key</param>
		/// <returns>The GUIDComponent value from the key</returns>
		public static PoolableObject GetComponentFromID(uint gUID, string type)
		{
			if (_worldObjects[type].TryGetValue(gUID, out PoolableObject comp))
				return comp;
			else
				return null;
		}
	}
}