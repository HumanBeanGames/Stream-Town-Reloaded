using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Managers
{
    public abstract class Config<T> : SerializedScriptableObject where T : SerializedScriptableObject
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
#if UNITY_EDITOR
                    // Try AssetDatabase in editor
                    string typeName = typeof(T).Name;
                    string[] guids = AssetDatabase.FindAssets($"t:{typeName}");
                    foreach (var guid in guids)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                        if (asset != null)
                        {
                            _instance = asset;
                            break;
                        }
                    }
#endif
                    //if (_instance == null)
                    {
                        // Runtime fallback
                        string type = typeof(T).ToString();
                        string obj = typeof(T).Name;
                        _instance = Resources.Load<T>(obj);
                    }
                }
                return _instance;
            }

            protected set => _instance = value; 
        }

#if UNITY_EDITOR
        protected virtual void OnEnable()
        {
            if (_instance == null)
                _instance = this as T;
        }
#endif
    }
}
