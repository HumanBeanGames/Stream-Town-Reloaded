using System.Collections.Generic;

namespace SavingAndLoading.Structs 
{
    [System.Serializable]
    public struct TechTreeSaveData 
	{
        public bool TechAvailable;
        public List<bool> UnlockedTechs;
        public string CurrentTechName;

        public List<ObjectiveSaveData> CurrentTechData;

        public TechTreeSaveData(List<bool> unlockedTechs, string curreentTechName, List<ObjectiveSaveData> currentTechData, bool techAvailable = true)
		{
            TechAvailable = techAvailable;
            UnlockedTechs = unlockedTechs;
            CurrentTechName = curreentTechName;
            CurrentTechData = currentTechData;
		}
    }
}