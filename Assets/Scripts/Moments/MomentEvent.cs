namespace AF
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;

    [JsonConverter(typeof(MomentEventConverter))]
    public abstract class MomentEvent
    {
        public string type;
    }

    public class DialogueEvent : MomentEvent
    {
        public DialoguePayload payload;
    }

    public class WaitEvent : MomentEvent
    {
        public WaitPayload payload;
    }

    [Serializable]
    public class DialoguePayload
    {
        public string message;
    }

    [Serializable]
    public class WaitPayload
    {
        public float duration;
    }

}
