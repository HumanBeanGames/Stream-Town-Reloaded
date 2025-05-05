using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.Pooling;
using Utils;
using Sirenix.OdinInspector;

namespace Managers
{
	/// <summary>
	/// Manages the object pooling of all PooledObjects in the game.
	/// </summary>
	[GameManager]
	public static class ObjectPoolingManager
	{
        [InlineEditor(InlineEditorObjectFieldModes.Foldout)]
        private static ObjectPoolingConfig Config = ObjectPoolingConfig.Instance;

        private static List<PooledObjectData> _objectsToPool => Config.objectsToPool;

        public static void AddToPool(string poolName, PoolableObject go)
		{
			_pooledObjects[poolName].Enqueue(go);
		}

		[HideInInspector]
        private static Dictionary<string, Queue<PoolableObject>> _pooledObjects;
        [HideInInspector]
        private static Dictionary<string, List<PoolableObject>> _allObjects;
        [HideInInspector]
        private static Dictionary<string, GameObject> _poolParents;
        [HideInInspector]
        private static TimeSpan _poolingDuration;

        public static TimeSpan PoolingDuration => _poolingDuration;


        private class Runner : MonoBehaviour { }
        [HideInInspector]
        private static Runner runner;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeRunner()
        {
            GameObject runnerObject = new GameObject("ObjectPoolingRunner");
            Runner runner = runnerObject.AddComponent<Runner>();
            UnityEngine.Object.DontDestroyOnLoad(runnerObject);
        }

        /// <summary>
        /// Starts the pooling process.
        /// </summary>
        public static IEnumerator InitializePooling()
		{
			DateTime before = DateTime.Now;
			yield return runner.StartCoroutine(PoolObjectsCoroutine());
			//PoolObjects();
			DateTime after = DateTime.Now;
			_poolingDuration = after.Subtract(before);
			GameStateManager.NotifyObjectsPooled();
		}

        /// <summary>
        /// Returns a pooled object by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="printWarning"></param>
        /// <returns></returns>
        public static PoolableObject GetPooledObject(string name, bool printWarning = true)
		{
			if (!_pooledObjects.ContainsKey(name))
			{
				Debug.LogError($"Tried to grab a pooled object of {name} but it didnt exist. Perhaps try pooling it you dingus!");
				return null;
			}

			if (_pooledObjects[name].Count > 0)
			{
				PoolableObject go = _pooledObjects[name].Peek();

				if (!go.gameObject.activeInHierarchy)
				{
					_pooledObjects[name].Dequeue();
					return go;
				}
			}

			// If we got to this point we ran out of pooled objects, perhaps need more
			if (printWarning)
				Debug.LogWarning($"Exceeded Pool amount and Instantiating a new object of type {name}. Current Count is {_pooledObjects[name].Count + 1}");

			return InstantiateNewObjectToPool(name);
		}

        /// <summary>
        /// Returns a list of all active game objects of a given type.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static List<PoolableObject> GetAllActivePooledObjectsOfType(string name)
		{
			if (!_allObjects.ContainsKey(name))
			{
				Debug.LogError($"Tried to get active pooled objects of {name} but they don't exist.");
				return null;
			}

			List<PoolableObject> activeObjects = new List<PoolableObject>();
			for (int i = _allObjects[name].Count - 1; i >= 0; i--)
			{
				PoolableObject go = _allObjects[name][i];

				if (go.transform.gameObject.activeInHierarchy)
				{
					activeObjects.Add(go);
				}
			}
			return activeObjects;
		}

        public static List<PoolableObject> GetAllActiveObjectsOfTypeWithinBoxCollider(BoxCollider collider, Vector3 center, string type)
		{
			Vector3 startPosition = center + new Vector3(collider.size.x * 0.5f + 1, 0, collider.size.z * 0.5f + 1);
			Vector3 endPosition = center - new Vector3(collider.size.x * 0.5f + 1, 0, collider.size.z * 0.5f + 1);
			return GetAllActiveObjectsOfTypeWithinAABB(startPosition, endPosition, type);
		}

        public static List<PoolableObject> GetAllActiveObjectsOfTypeWithinAABB(Vector3 startPosition, Vector3 endPosition, string type)
		{
			List<PoolableObject> activeObjects = new List<PoolableObject>();

			List<PoolableObject> activeObjectsOfType = GetAllActivePooledObjectsOfType(type);

			for (int i = 0; i < activeObjectsOfType.Count; i++)
			{
				if (activeObjectsOfType[i].transform.position.x > startPosition.x && activeObjectsOfType[i].transform.position.x < endPosition.x || activeObjectsOfType[i].transform.position.x > endPosition.x && activeObjectsOfType[i].transform.position.x < startPosition.x)
					if (activeObjectsOfType[i].transform.position.z > startPosition.z && activeObjectsOfType[i].transform.position.z < endPosition.z || activeObjectsOfType[i].transform.position.z > endPosition.z && activeObjectsOfType[i].transform.position.z < startPosition.z)
						activeObjects.Add(activeObjectsOfType[i]);
			}

			return activeObjects;
		}


        public static List<PoolableObject> GetAllActivePooledObjectsOfType(SaveItem item)
		{
			return GetAllActivePooledObjectsOfType(item.ToString());
		}

        /// <summary>
        /// Creates a new object by name and adds it to the existing pool.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static PoolableObject InstantiateNewObjectToPool(string name)
		{
			for (int i = 0; i < _objectsToPool.Count; i++)
			{
				if (_objectsToPool[i].Name == name)
				{
					GameObject obj = GameObject.Instantiate(_objectsToPool[i].Prefab);
					PoolableObject poolObj = null;
					if (!obj.TryGetComponent(out poolObj))
						poolObj = obj.AddComponent<PoolableObject>();

					poolObj.Initialize(name);
					if (obj.TryGetComponent<RectTransform>(out RectTransform rt))
					{
						rt.SetParent(_poolParents[name].transform, false);
					}
					else
						obj.transform.parent = _poolParents[name].transform;
					//_pooledObjects[name].Enqueue(poolObj);
					_allObjects[name].Add(poolObj);
					obj.name = name + _allObjects[name].Count;
					obj.SetActive(false);
					return poolObj;
				}
			}

			Debug.LogError($"Something really went wrong with trying to instantiate a new object of type {name}");
			return null;
		}

        private static IEnumerator PoolObjectsCoroutine()
		{
			_pooledObjects = new Dictionary<string, Queue<PoolableObject>>();
			_poolParents = new Dictionary<string, GameObject>();
			_allObjects = new Dictionary<string, List<PoolableObject>>();

			GameObject poolParent = new GameObject("Pooled Objects");

			for (int i = 0; i < _objectsToPool.Count; i++)
			{
				DateTime before = DateTime.Now;
				string objName = _objectsToPool[i].Name;
				GameObject parentObject = new GameObject(objName + " Pool");

				parentObject.transform.parent = poolParent.transform;
				_pooledObjects[objName] = new Queue<PoolableObject>(_objectsToPool[i].PoolAmount);
				_allObjects[objName] = new List<PoolableObject>(_objectsToPool[i].PoolAmount);
				_poolParents.Add(objName, parentObject);

				for (int j = 0; j < _objectsToPool[i].PoolAmount; j++)
				{
					GameObject obj = GameObject.Instantiate(_objectsToPool[i].Prefab, new Vector3(-500, 0, -500), Quaternion.identity, parentObject.transform);
					PoolableObject poolObj = obj.GetComponent<PoolableObject>();
					if (poolObj == null)
						poolObj = obj.AddComponent<PoolableObject>();
					obj.name = objName + j;
					//_pooledObjects[objName].Enqueue(poolObj);
					_allObjects[objName].Add(poolObj);
					poolObj.Initialize(objName);
					obj.SetActive(false);

				}
				DateTime after = DateTime.Now;
				TimeSpan duration = after.Subtract(before);
				//Debug.Log($"Pooling {objName} took {duration.TotalMilliseconds}ms");
				yield return new WaitForEndOfFrame();
			}
		}

        public static void SimplePoolObjects()
		{
			_pooledObjects = new Dictionary<string, Queue<PoolableObject>>();
			_poolParents = new Dictionary<string, GameObject>();
			_allObjects = new Dictionary<string, List<PoolableObject>>();

			GameObject poolParent = new GameObject("Pooled Objects");

			for (int i = 0; i < _objectsToPool.Count; i++)
			{
				string objName = _objectsToPool[i].Name;
				GameObject parentObject = new GameObject(objName + " Pool");

				parentObject.transform.parent = poolParent.transform;
				_pooledObjects[objName] = new Queue<PoolableObject>(_objectsToPool[i].PoolAmount);
				_allObjects[objName] = new List<PoolableObject>(_objectsToPool[i].PoolAmount);
				_poolParents.Add(objName, parentObject);

				for (int j = 0; j < _objectsToPool[i].PoolAmount; j++)
				{
					GameObject obj = GameObject.Instantiate(_objectsToPool[i].Prefab, new Vector3(-500, 0, -500), Quaternion.identity, parentObject.transform);
					PoolableObject poolObj = obj.GetComponent<PoolableObject>();
					if (poolObj == null)
						poolObj = obj.AddComponent<PoolableObject>();
					obj.name = objName + j;
					//_pooledObjects[objName].Enqueue(poolObj);
					_allObjects[objName].Add(poolObj);
					poolObj.Initialize(objName);
					obj.SetActive(false);

				}
			}
		}

        public static void DisableObjectsInPool(string name)
		{
			List<PoolableObject> objects = GetAllActivePooledObjectsOfType(name);

			for(int i =0; i < objects.Count; i++)
			{
				objects[i].gameObject.SetActive(false);
			}
		}
	}
}