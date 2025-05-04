using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using SavingAndLoading.Structs;
using static Settings.SettingsManager;

namespace SavingAndLoading
{
	public static class GameIO
	{
		public const string SAVE_FILEPATH = "/Panda Belly/Stream Town/Saves/";
		public const string GAME_SAVE_FILEPATH = "GameSave.pog";
		public const string PLAYER_SAVE_FILEPATH = "PlayersSave.pog";

		public static SaveGameData LoadGameData()
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream fStream = new FileStream(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + SAVE_FILEPATH + GAME_SAVE_FILEPATH, FileMode.Open);
			SaveGameData data = formatter.Deserialize(fStream) as SaveGameData;
			fStream.Close();
			Debug.Log("GameIO: Loading from -> " + System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + SAVE_FILEPATH + GAME_SAVE_FILEPATH);
			return data;
		}

		public static void SaveGameData(SaveGameData data)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream fStream = new FileStream(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + SAVE_FILEPATH + GAME_SAVE_FILEPATH, FileMode.Create);
			formatter.Serialize(fStream, data);
			fStream.Close();
			Debug.Log("GameIO: Saving to -> " + System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + SAVE_FILEPATH + GAME_SAVE_FILEPATH);
		}

		public static SavePlayersData LoadPlayersData()
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream fStream = new FileStream(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + SAVE_FILEPATH + PLAYER_SAVE_FILEPATH, FileMode.Open);
			SavePlayersData data = formatter.Deserialize(fStream) as SavePlayersData;
			fStream.Close();
			Debug.Log("GameIO: Loading from -> " + System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + SAVE_FILEPATH + PLAYER_SAVE_FILEPATH);
			return data;
		}

		public static void SavePlayersData(SavePlayersData data)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream fStream = new FileStream(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + SAVE_FILEPATH + PLAYER_SAVE_FILEPATH, FileMode.Create);
			formatter.Serialize(fStream, data);
			fStream.Close();
			Debug.Log("GameIO: Saving to -> " + System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + SAVE_FILEPATH + PLAYER_SAVE_FILEPATH);
		}

		public static void SaveSettingsData(SavePreset savePreset)
		{
			string data = JsonUtility.ToJson(savePreset);
			File.WriteAllText(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/Panda Belly/Stream Town/SettingsData.json", data);
			Debug.Log("File location : " + System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/Panda Belly/Stream Town/SettingsData.json");
		}

		public static SavePreset LoadSettingsData()
		{
			string fileContents = File.ReadAllText(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/Panda Belly/Stream Town/SettingsData.json");
			return JsonUtility.FromJson<SavePreset>(fileContents);
		}

		public static bool DoesSaveFileExist(SaveFileType type)
		{
			string directory = " ";

			switch (type)
			{
				case SaveFileType.GameSave:
					directory = GAME_SAVE_FILEPATH;
					break;
				case SaveFileType.PlayersSave:
					directory = PLAYER_SAVE_FILEPATH;
					break;
			}

			return File.Exists(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + SAVE_FILEPATH + directory);
		}
		
		public enum SaveFileType
		{
			GameSave,
			PlayersSave
		}
	}
}