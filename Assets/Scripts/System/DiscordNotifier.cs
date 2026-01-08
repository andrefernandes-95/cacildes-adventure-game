namespace AF
{
    using System.Collections;
    using System.Text;
    using UnityEngine;
    using UnityEngine.Networking;

    public class DiscordNotifier : MonoBehaviour
    {
        [SerializeField] private string webhookUrl = "YOUR_DISCORD_WEBHOOK_URL";

        public void SendToDiscord(string message)
        {
            StartCoroutine(SendMessage(message));
        }

        private new IEnumerator SendMessage(string message)
        {
            var json = JsonUtility.ToJson(new DiscordMessage { content = message });
            byte[] body = Encoding.UTF8.GetBytes(json);

            using (UnityWebRequest www = new UnityWebRequest(webhookUrl, "POST"))
            {
                www.uploadHandler = new UploadHandlerRaw(body);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                    Debug.LogWarning("Discord webhook failed: " + www.error);
            }
        }

        [System.Serializable]
        private class DiscordMessage
        {
            public string content;
        }
    }
}
