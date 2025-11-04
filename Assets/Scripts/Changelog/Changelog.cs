using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Localization;

namespace AF
{
    [CreateAssetMenu(menuName = "Misc / Changelog / New Changelog")]
    public class Changelog : ScriptableObject
    {
        public string date = "13/08/2024";
        public Sprite changelogThumbnail;
        public LocalizedString smallDescription;

        [Obsolete] public LocalizedString[] additions;
        [Obsolete] public LocalizedString[] improvements;
        [Obsolete] public LocalizedString[] bugfixes;

        public UpdateType updateType = UpdateType.SMALL_UPDATE;

        public enum UpdateType
        {
            SMALL_UPDATE,
            BIG_UPDATE,
            EXPANSION
        }

        [Header("Changelog JSON File")]
        public TextAsset data;

        public Dictionary<string, Dictionary<string, List<string>>> GetData()
        {
            if (data == null || string.IsNullOrEmpty(data.text))
            {
                Debug.LogWarning("Changelog JSON file is missing or empty.");
                return new Dictionary<string, Dictionary<string, List<string>>>();
            }

            try
            {
                // Deserialize the JSON into the nested dictionary structure
                var parsedData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, List<string>>>>(data.text);

                if (parsedData == null)
                {
                    Debug.LogWarning("Failed to parse changelog JSON.");
                    return new Dictionary<string, Dictionary<string, List<string>>>();
                }

                return parsedData;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error reading changelog JSON: {ex.Message}");
                return new Dictionary<string, Dictionary<string, List<string>>>();
            }
        }
    }

}
