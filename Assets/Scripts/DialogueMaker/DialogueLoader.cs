using UnityEngine;

namespace AF
{
    public class DialogueLoader : MonoBehaviour
    {
        public TextAsset english;
        public TextAsset portuguese;

        DialogueMaker _dialogueMaker;

        public void LoadDialogue()
        {
            if (Utils.IsPortuguese())
            {
                GetDialogueMaker().StartDialogue(new Ink.Runtime.Story(portuguese.text));
            }
            else
            {
                GetDialogueMaker().StartDialogue(new Ink.Runtime.Story(english.text));
            }

        }

        DialogueMaker GetDialogueMaker()
        {
            if (_dialogueMaker == null)
            {
                _dialogueMaker = FindAnyObjectByType<DialogueMaker>(FindObjectsInactive.Include);
            }
            return _dialogueMaker;
        }
    }

}
