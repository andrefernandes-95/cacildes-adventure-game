namespace AF
{
    using System;
    using Newtonsoft.Json;
    using UnityEngine;

    public class MomentReader : MonoBehaviour
    {
        [Serializable]
        public class MomentData
        {
            public MomentEvent[] events;
        }

        public void LoadMoment(TextAsset moment)
        {
            MomentData momentData = JsonConvert.DeserializeObject<MomentData>(moment.text);

            foreach (var evt in momentData.events)
            {
                switch (evt)
                {
                    case DialogueEvent dialogue:
                        Debug.Log($"Dialogue: {dialogue.payload.message}");
                        break;

                    case WaitEvent wait:
                        Debug.Log($"Wait: {wait.payload.duration} seconds");
                        break;

                    default:
                        Debug.LogWarning("Unhandled event type.");
                        break;
                }
            }
        }

    }
}
