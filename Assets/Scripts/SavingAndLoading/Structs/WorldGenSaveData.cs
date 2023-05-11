using UnityEngine;
using System.Collections.Generic;

namespace SavingAndLoading.Structs
{
    /// <summary>
    /// Struct Holding information needed to load the world generation from a save file
    /// </summary>
    [System.Serializable]
    public struct WorldGenSaveData
    {
        public MeshSaveData MapMesh;

        public List<ResourceSaveData> Resources;
        public List<FoliageSaveData> Foliage;
        public List<EnemyCampSaveData> EnemyCamps;

        /// <summary>
        /// Sets the values of WorldSaveData
        /// </summary>
        /// <param name="mesh">The words mesh data</param>
        /// <param name="resources">The worlds resources</param>
        /// <param name="foliage">The worlds foliage</param>
        /// <param name="camps">The worlds camps</param>
        
        public WorldGenSaveData(MeshSaveData mesh, List<ResourceSaveData> resources, List<FoliageSaveData> foliage, List<EnemyCampSaveData> camps)
        {   
            MapMesh = mesh;
            Resources = resources;
            Foliage = foliage;
            EnemyCamps = camps;
        }
    }
}