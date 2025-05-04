using Character.Enumerations;
using Managers;
using Twitch.Utils;
using TwitchLib.Client.Enums;

namespace Twitch
{
	/// <summary>
	/// Holds data for a registered Twitch User.
	/// </summary>
	public class TwitchUser
	{
		public string UserID;
		public string Username;
		public UserType TwitchUserType;
		public GameUserType GameUserType;
		public ActivityStatus ActivityStatus;
		public float TimeSinceLastMessage;
		public bool IsBroadcaster = false;

		public TwitchUser(string userID, string username)
		{
			UserID = userID;
			Username = username;
		}

		public void UpdateActivity()
		{
			float time = TimeManager.WorldTimePassed - TimeSinceLastMessage;

			if (time < 300)
				ActivityStatus = ActivityStatus.Active;
			else if (time < 600)
				ActivityStatus = ActivityStatus.LastTenMinutes;
			else if (time < 3600)
				ActivityStatus = ActivityStatus.LastHour;
			else
				ActivityStatus = ActivityStatus.Inactive;
		}
	}
}