using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_PlayStory : EventBase
    {
        [SerializeField] TextAsset englishStory;
        [SerializeField] TextAsset portugueseStory;

        DialogueMaker _dialogueMaker;

        [Header("Options")]
        CharacterManager dialogueOwner;
        [SerializeField] bool shouldStopCharacter = true;
        [SerializeField] bool shouldFacePlayer = true;

        void Start()
        {
            dialogueOwner = GetComponentInParent<CharacterManager>();
        }

        public override IEnumerator Dispatch()
        {
            OnDialogueStart();

            if (Utils.IsPortuguese())
            {
                yield return StartCoroutine(GetDialogueMaker().PlayStory(new Ink.Runtime.Story(portugueseStory.text)));
            }
            else
            {
                yield return StartCoroutine(GetDialogueMaker().PlayStory(new Ink.Runtime.Story(englishStory.text)));
            }

            OnDialogueEnd();
        }

        void OnDialogueStart()
        {
            if (dialogueOwner != null)
            {
                if (shouldStopCharacter)
                {
                    dialogueOwner.agent.enabled = false;
                    dialogueOwner.stateManager.gameObject.SetActive(false);
                }

                if (shouldFacePlayer)
                {
                    dialogueOwner.FacePlayer();
                }
            }
        }

        void OnDialogueEnd()
        {
            if (dialogueOwner != null && shouldStopCharacter)
            {
                dialogueOwner.stateManager.gameObject.SetActive(true);
            }
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
