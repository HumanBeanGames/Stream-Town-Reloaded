using UnityEngine;
using Scriptables;
using Sirenix.OdinInspector;
using Utils;
using Target;
using System.Collections.Generic;
using Utils.Pooling;

namespace Managers
{
    [CreateAssetMenu(menuName = "Configs/Object Pooling Config")]
    public class ObjectPoolingConfig : Config<ObjectPoolingConfig>
    {
        [SerializeField]
        public List<PooledObjectData> objectsToPool;
    }
}   
