using UnityEngine;
using TwitchLib.Unity;

namespace Twitch
{
	public class TL_API : MonoBehaviour
	{
		public static Api API;

		public void InitApi()
		{
			Application.runInBackground = true;
			API = new Api();

			API.Settings.AccessToken = TL_Secrets.BotAccessToken;
			API.Settings.ClientId = TL_Secrets.ClientID;

		}

		private void Start()
		{
			InitApi();
		}
	}
}