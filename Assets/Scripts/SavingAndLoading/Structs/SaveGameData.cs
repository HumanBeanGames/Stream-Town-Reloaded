using System.Collections.Generic;

namespace SavingAndLoading.Structs
{
    /// <summary>
    /// Struct Holding information needed to load the game from a save file
    /// </summary>
    [System.Serializable]
    public class SaveGameData 
	{
        public WorldGenSaveData WorldGenData;
        public List<BuildingSaveData> BuildingSaveData;
        public List<EnemySaveData> EnemySaveData;
		public WorldSaveData WorldSaveData;

		// Do XML commenting here once done
		public SaveGameData(WorldGenSaveData worldGenSaveData, List<BuildingSaveData> buildingSaveData, List<EnemySaveData> enemySaveData, WorldSaveData worldSaveData)
		{
			WorldGenData = worldGenSaveData;
			BuildingSaveData = buildingSaveData;
			EnemySaveData = enemySaveData;
			WorldSaveData = worldSaveData;
		}
	}
}