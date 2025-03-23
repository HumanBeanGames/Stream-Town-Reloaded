using System.IO;
using UnityEngine;

namespace Twitch
{
    [System.Serializable]
    public class TLSecretsData
    {
        public string ClientID;
        public string ClientSecret;
        public string BotAccessToken;
        public string BotRefreshToken;
        public string BotName;
    }

    public static class TL_Secrets
    {
        private static TLSecretsData _data;
        private static readonly string FilePath = Path.Combine(Application.streamingAssetsPath, "twitch_secrets.json");

        public static string ClientID => _data?.ClientID;
        public static string ClientSecret => _data?.ClientSecret;
        public static string BotAccessToken => _data?.BotAccessToken;
        public static string BotRefreshToken => _data?.BotRefreshToken;
        public static string BotName => _data?.BotName;

        public static bool IsLoaded => _data != null;

        public static void LoadSecrets()
        {
            if (!File.Exists(FilePath))
            {
                Debug.LogWarning($"Twitch secrets file not found at: {FilePath}");
                return;
            }

            Debug.Log("[TL_Secrets] Attempting to load Secrets!");
            string json = File.ReadAllText(FilePath);
            _data = JsonUtility.FromJson<TLSecretsData>(json);
            Debug.Log("[TL_Secrets] Secrets loaded successfully.");
        }

        public static void SaveSecrets()
        {
            string json = JsonUtility.ToJson(_data, true);
            File.WriteAllText(FilePath, json);
            Debug.Log("[TL_Secrets] Secrets saved to disk.");
        }

        public static void SetSecrets(TLSecretsData newData)
        {
            _data = newData;
            SaveSecrets();
        }

        public static TLSecretsData GetCurrentSecrets()
        {
            return _data;
        }
    }
}