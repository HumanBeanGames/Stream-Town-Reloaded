using UnityEngine;
using TwitchLib.Unity;

namespace Twitch
{
    /// <summary>
    /// Initializes the TwitchLib Unity REST API client using credentials from TL_Secrets.
    /// </summary>
    public class TL_API : MonoBehaviour
    {
        public static Api API;

        private void Awake()
        {
            // Ensure secrets are loaded before anything else
            TL_Secrets.LoadSecrets();
        }

        private void Start()
        {
            InitApi();
        }

        /// <summary>
        /// Initializes the TwitchLib API client with credentials.
        /// </summary>
        public void InitApi()
        {
            Application.runInBackground = true;
            API = new Api();

            if (!TL_Secrets.IsLoaded)
            {
                Debug.LogWarning("[TL_API] Twitch secrets were not loaded. API setup skipped.");
                return;
            }

            API.Settings.AccessToken = TL_Secrets.BotAccessToken;
            API.Settings.ClientId = TL_Secrets.ClientID;

            Debug.Log("[TL_API] Twitch API initialized successfully.");
            Debug.Log($"[TL_API] AccessToken: {TL_Secrets.BotAccessToken}");
            Debug.Log($"[TL_API] ClientId: {TL_Secrets.ClientID}");
        }
    }
}
