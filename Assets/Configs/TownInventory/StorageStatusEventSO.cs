using UnityEngine;
using UnityEngine.Events;
using Utils;

namespace Events
{
    [CreateAssetMenu(menuName = "Events/StorageStatusEvent", fileName = "NewStorageStatusEvent")]
    public class StorageStatusEventSO : ScriptableObject
    {
        [System.Serializable]
        public class StorageStatusUnityEvent : UnityEvent<StorageStatus> { }

        [SerializeField]
        private StorageStatusUnityEvent _event = new StorageStatusUnityEvent();


        public static implicit operator UnityEvent<StorageStatus>(StorageStatusEventSO so)
        {
            return so._event;
        }
    }
}
