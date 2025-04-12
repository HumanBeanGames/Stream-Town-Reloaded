using Character;
using GameEventSystem;

using Pets;
using Pets.Enumerations;
using UnityEngine;
using Utils;

namespace Twitch.Utils
{
	/// <summary>
	/// Holds all Color data for different types of Twitch users.
	/// </summary>
	public static class UserColours
	{
		public static Color GameMaster = new Color(255, 57, 0, 255);
		public static Color Broadcaster = Color.red;
		public static Color Moderator = Color.green;
		public static Color Subscriber = new Color(100, 65, 165, 255);
		public static Color Normal = Color.white;

		/// <summary>
		/// Returns a color based on the User's type in Twitch chat.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Color GetColourByUserType(GameUserType type)
		{
			switch (type)
			{
				case GameUserType.GameMaster:
					return GameMaster;
				case GameUserType.Broadcaster:
					return Broadcaster;
				case GameUserType.Moderator:
					return Moderator;
				case GameUserType.Subscriber:
					return Subscriber;
				case GameUserType.Normal:
					return Normal;
				default:
					return Normal;
			}
		}
	}

	public static class TwitchUtils
	{
		public static bool TryGetPlayer(string nameArg, out Player player)
		{
			player = null;
			if (GameManager.Instance.PlayerManager.PlayerExistsByNameToLower(nameArg.ToLower(), out int index))
			{
				player = GameManager.Instance.PlayerManager.GetPlayer(index);
				return true;
			}

			return false;
		}

		public static PetType GetPetTypeFromString(string arg)
		{
			arg = arg.ToLower();
			for(int i = 0; i < (int)PetType.Count;i++)
			{
				if (arg == ((PetType)i).ToString().ToLower())
					return (PetType)i;
			}

			return PetType.Count;
		}

		public static string StringToEnumString(string arg)
		{
			return char.ToUpper(arg[0]) + arg.Substring(1);
		}

		public static GameEvent.EventType StringToEventEnum(string arg)
		{
			for(int i = 0; i < (int) GameEvent.EventType.Count;i++)
			{
				if(arg == ((GameEvent.EventType)i).ToString().ToLower())
				{
					return (GameEvent.EventType)i;
				}
			}

			return GameEvent.EventType.Count;
		}

		public static Resource GetResourceFromString(string arg)
		{
			arg = arg.ToLower();

			for (int i = 1; i < (int)Resource.Count; i++)
			{
				if (arg == ((Resource)i).ToString().ToLower())
					return (Resource)i;
			}

			return Resource.None;
		}
	}
	/// <summary>
	/// Dictates what type of user this player is in Twitch chat.
	/// </summary>
	public enum GameUserType
	{
		GameMaster,
		Broadcaster,
		Moderator,
		Subscriber,
		Normal
	}
}