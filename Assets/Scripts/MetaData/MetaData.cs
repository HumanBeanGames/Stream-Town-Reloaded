using UnityEngine;

namespace MetaData
{
    public enum LoadType
    {
        Generate,
        Load,
    }

    public class MetaData : MonoBehaviour
    {
        public static MetaData Instance { get; private set; }

        public LoadType LoadType;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
