using Character;
using Utils;
using System.Collections.Generic;

namespace SavingAndLoading.Structs 
{
    /// <summary>
    /// Struct holding data for a player role
    /// </summary>
    [System.Serializable]
    public struct PlayerRoleSaveData 
	{
        public PlayerRole Role;
        public int Level;
        public float Experience;

        /// <summary>
        /// Overloaded constructor,
        /// Converts PlayerRoleData to PlayerRoleSaveData
        /// </summary>
        /// <param name="data"></param>
        public PlayerRoleSaveData(PlayerRoleData data)
		{
            Role = data.Role;
            Level = data.CurrentLevel;
            Experience = data.CurrentExp;
		}

        /// <summary>
        /// Converts an array of PlayerRoleData to a list of PlayerSaveData
        /// </summary>
        /// <param name="datas">Array of PlayerRoleData to be converted</param>
        /// <returns>A List of PlayerRoleSaveData</returns>
        public static List<PlayerRoleSaveData> ToPlayerRoleSaveDatas(PlayerRoleData[] datas)
		{
            List<PlayerRoleSaveData> playerRoleSaveDatas = new List<PlayerRoleSaveData>();

            for(int i = 0; i < datas.Length; i++)
			{
                PlayerRoleSaveData temp = new PlayerRoleSaveData(datas[i]);
                playerRoleSaveDatas.Add(temp);
			}

            return playerRoleSaveDatas;
		}

        public static void ToPlayerRoleData(PlayerRoleSaveData saveData, PlayerRoleData roleData)
		{
            roleData.SetRole(saveData.Role);
            roleData.SetLevel(saveData.Level);
            roleData.SetExperience(saveData.Level);
		}

        public static void ToPlayerRoleDatas(PlayerRoleSaveData[] saveDatas, PlayerRoleData[] roleDatas)
		{
            for(int i =0; i < saveDatas.Length;i++)
			{
                ToPlayerRoleData(saveDatas[i],roleDatas[i]);
			}
		}
    }
}