namespace AF
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using AF.Dialogue;
    using UnityEngine;
    using Ink.Runtime;

    [RequireComponent(typeof(DialogueFunctions))]
    public class DialogueMaker : MonoBehaviour
    {
        [Header("Dependencies")]
        public UIDocumentDialogueWindow dialogueWindow;
        [SerializeField] PlayerManager playerManager;
        [SerializeField] CursorManager cursorManager;


        Dictionary<string, Character> charactersLookUp = new();

        DialogueFunctions dialogueFunctions => GetComponent<DialogueFunctions>();

        private Story story;

        Coroutine RunStoryCoroutine;

        bool hasChosenDialogueOption = false;

        void Awake()
        {
            Character[] allCharacters = Resources.LoadAll<Character>("Characters");

            foreach (Character character in allCharacters)
            {
                if (!charactersLookUp.ContainsKey(character.name))
                {
                    charactersLookUp.Add(character.name, character);
                }
            }
        }

        public IEnumerator PlayStory(Story dialogueData)
        {
            this.story = dialogueData;
            dialogueFunctions.BindStoryToFunctions(story);

            while (story.canContinue)
            {
                hasChosenDialogueOption = false;

                string text = story.Continue().Trim();
                var tags = story.currentTags;

                // Optional: Handle event tags
                foreach (var tag in tags)
                {
                    if (tag.StartsWith("eventId:"))
                    {
                        string eventId = tag.Substring("eventId:".Length);
                        Debug.Log($"Event Triggered: {eventId}");
                        // Trigger UnityEvents or scripts here
                    }
                }

                string characterName = "";

                // Detect speaker prefix (e.g. "LARA: Hello!")
                if (text.Contains(":"))
                {
                    var split = text.Split(new[] { ':' }, 2);
                    characterName = split[0].Trim();
                    text = split[1].Trim();
                }

                if (!string.IsNullOrEmpty(text))
                {
                    OnMessageStart();

                    dialogueWindow.DisplayMessageV2(GetCharacter(characterName), text, GetChoices());

                    if (story.currentChoices.Count > 0)
                    {
                        OnChoicesStart();
                        yield return new WaitUntil(() => hasChosenDialogueOption == true);
                        OnChoicesEnd();
                    }
                    else
                    {
                        yield return new WaitUntil(() => playerManager.starterAssetsInputs.interact == false);
                        yield return new WaitUntil(() => playerManager.starterAssetsInputs.interact);
                    }

                    OnMessageEnd();
                }
            }
        }

        void OnMessageStart()
        {
            dialogueWindow.gameObject.SetActive(true);
            playerManager.uIDocumentPlayerHUDV2.FadeOut();
        }

        void OnMessageEnd()
        {
            dialogueWindow.gameObject.SetActive(false);
            playerManager.uIDocumentPlayerHUDV2.FadeIn();
        }

        void OnChoicesStart()
        {
            cursorManager.ShowCursor();
            playerManager.thirdPersonController.SetLockCameraPosition(true);
        }

        void OnChoicesEnd()
        {
            cursorManager.HideCursor();
            playerManager.thirdPersonController.SetLockCameraPosition(false);
        }

        Response[] GetChoices()
        {
            if (!story.currentChoices.Any())
                return new Response[0];

            var responses = new List<Response>();

            foreach (var choice in story.currentChoices)
            {
                var r = new Response
                {
                    text = choice.text,
                    onResponseSelected = new UnityEngine.Events.UnityEvent()
                };

                // When selected
                r.onResponseSelected.AddListener(() =>
                {
                    story.ChooseChoiceIndex(choice.index);
                    hasChosenDialogueOption = true;
                });

                responses.Add(r);
            }

            return responses.ToArray();
        }

        Character GetCharacter(string characterName)
        {
            return charactersLookUp.ContainsKey(characterName) ? charactersLookUp[characterName] : null;
        }
    }
}
