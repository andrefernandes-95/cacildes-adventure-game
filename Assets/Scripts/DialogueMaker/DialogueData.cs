using System.Collections.Generic;

namespace AF
{

    [System.Serializable]
    public class DialogueData
    {
        public string dialogueId;
        public List<DialogueNode> nodes;
    }

    [System.Serializable]
    public class DialogueNode
    {
        public string id;
        public string speaker;
        public string text;
        public List<DialogueChoice> choices;
        public string eventId;
    }

    [System.Serializable]
    public class DialogueChoice
    {
        public string text;
        public string nextId;
    }

}
