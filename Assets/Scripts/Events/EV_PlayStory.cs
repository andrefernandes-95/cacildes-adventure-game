using System.Collections;
using System.Linq;
using AF.Dialogue;
using UnityEngine;

namespace AF
{
    public class EV_PlayStory : EventBase
    {
        [SerializeField] TextAsset englishStory;
        [SerializeField] TextAsset portugueseStory;

        DialogueMaker _dialogueMaker;

        public override IEnumerator Dispatch()
        {
            if (Utils.IsPortuguese())
            {
                yield return StartCoroutine(GetDialogueMaker().PlayStory(new Ink.Runtime.Story(portugueseStory.text)));
            }
            else
            {
                yield return StartCoroutine(GetDialogueMaker().PlayStory(new Ink.Runtime.Story(englishStory.text)));
            }

            Debug.Log("Story has finished");
        }

        private void OnDisable()
        {
            StopAllCoroutines();
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
