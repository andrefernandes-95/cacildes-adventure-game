namespace AF
{
    using UnityEngine;
    using UnityEngine.Networking;
    using SimpleJSON;
    using System.Collections;

    public class Translator : MonoBehaviour
    {
        [Header("Debug Options")]
        public bool isDebug = true;

        [Header("UI")]
        [SerializeField] private UIDocumentLoadingSpinner uIDocumentLoadingSpinner;

        [Header("Databases")]
        [SerializeField] GameSettings gameSettings;

        // Translate with auto-detected source language
        public void TranslateText(string sourceText, System.Action<string> callback)
        {
            if (gameSettings.IsUsingAutomaticTranslation())
            {
                StartCoroutine(Process("auto", gameSettings.automaticTranslationCode, sourceText, callback));
            }
            else
            {
                callback?.Invoke(sourceText);
            }
        }

        // Core coroutine
        private IEnumerator Process(
            string sourceLang,
            string targetLang,
            string sourceText,
            System.Action<string> callback)
        {
            if (string.IsNullOrEmpty(sourceText))
            {
                callback?.Invoke("");
                yield break;
            }

            uIDocumentLoadingSpinner?.Show();

            string url =
                $"https://translate.googleapis.com/translate_a/single" +
                $"?client=gtx&sl={sourceLang}&tl={targetLang}&dt=t&q={UnityWebRequest.EscapeURL(sourceText)}";

            using UnityWebRequest request = UnityWebRequest.Get(url);

            yield return request.SendWebRequest();

            string resultText = sourceText; // fallback default

            if (request.result == UnityWebRequest.Result.Success &&
                !string.IsNullOrEmpty(request.downloadHandler.text))
            {
                try
                {
                    resultText = ExtractFullTranslation(
                        JSONNode.Parse(request.downloadHandler.text)
                    );
                }
                catch (System.Exception e)
                {
                    Debug.LogError(
                        "[Translator] JSON parse failed:\n" +
                        e + "\nRaw:\n" + request.downloadHandler.text
                    );
                }
            }
            else
            {
                Debug.LogError(
                    "[Translator] Request failed: " +
                    request.error
                );
            }

            uIDocumentLoadingSpinner?.Hide();
            callback?.Invoke(resultText);
        }

        string ExtractFullTranslation(JSONNode root)
        {
            if (root == null || root[0] == null)
                return string.Empty;

            System.Text.StringBuilder sb = new();

            foreach (JSONNode segment in root[0].Children)
            {
                if (segment != null && segment.Count > 0)
                    sb.Append(segment[0]);
            }

            return sb.ToString();
        }
    }
}
