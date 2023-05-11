namespace SavingAndLoading.Structs 
{
    [System.Serializable]
    public struct PlayerCustomizationSaveData
    {
        public int ChosenEyeIndex;
        public int ChosenHairIndex;
        public int ChosenFacialHairIndex;
        public int ChosenSkinIndex;
        public int ChosenHairColourIndex;
        public int ChosenEyeColourIndex;
        public int ChosenBodyTypeIndex;

        public PlayerCustomizationSaveData(int chosenEyeIndex, int chosenHairIndex, int chosenFacialHairIndex, int chosenSkinIndex, int chosenHairColourIndex, int chosenEyeColourIndex, int chosenBodyTypeIndex)
        {
            ChosenEyeIndex = chosenEyeIndex;
            ChosenHairIndex = chosenHairIndex;
            ChosenFacialHairIndex = chosenFacialHairIndex;
            ChosenSkinIndex = chosenSkinIndex;
            ChosenHairColourIndex = chosenHairColourIndex;
            ChosenEyeColourIndex = chosenEyeColourIndex;
            ChosenBodyTypeIndex = chosenBodyTypeIndex;
        }
    }
}