using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using AF.Dialogue;
using System.Linq;
using AF.Flags;
using UnityEngine.EventSystems;

namespace AF.UIExperimental
{
    public class DialogueWindow : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private PlayerManager playerManager;
        [SerializeField] private CursorManager cursorManager;
        [SerializeField] private Soundbank soundbank;
        [SerializeField] private FlagsDatabase flagsDatabase;

        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI dialogueMessage;
        [SerializeField] private GameObject characterInfoContainer;
        [SerializeField] private TextMeshProUGUI characterName;
        [SerializeField] private Image characterImage;
        [SerializeField] private Transform choicePanelRoot;
        [SerializeField] private GameObject choiceButtonPrefab;

        [Header("Settings")]
        private const float textDelay = 0.03f;
        private const float maxCharactersPerLine = 60;
        [SerializeField] private bool playTypewriterSound = true;

        private bool hasFinishedTypewriter = false;
        private Coroutine typewriteCoroutine;
        private WaitForSeconds cachedWait;
        private bool isContinuing = false;
        Button firstSelectable;

        private void Awake()
        {
            gameObject.SetActive(false);
            cachedWait = new WaitForSeconds(textDelay);
        }

        private void OnEnable()
        {
            playerManager.starterAssetsInputs.onInteract.AddListener(OnInteractPressed);
        }

        private void OnDisable()
        {
            playerManager.starterAssetsInputs.onInteract.RemoveListener(OnInteractPressed);
        }

        private void OnInteractPressed()
        {
            isContinuing = true;
        }

        void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == null && firstSelectable != null)
            {
                firstSelectable.Select();
            }
        }

        private string InsertLineBreaksAtWhitespace(string text)
        {
            if (string.IsNullOrEmpty(text) || maxCharactersPerLine <= 0)
                return text;

            int charCount = 0;
            int lastWhitespaceIndex = -1;
            char[] chars = text.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                charCount++;

                if (char.IsWhiteSpace(chars[i]))
                {
                    lastWhitespaceIndex = i;
                }

                if (charCount >= maxCharactersPerLine && lastWhitespaceIndex != -1)
                {
                    chars[lastWhitespaceIndex] = '\n';
                    charCount = i - lastWhitespaceIndex;
                    lastWhitespaceIndex = -1;
                }
            }

            return new string(chars);
        }

        /// <summary>
        /// Main method to display dialogue
        /// </summary>
        public IEnumerator DisplayMessage(Character character, string message, Response[] responses)
        {
            firstSelectable = null;
            gameObject.SetActive(true);
            cursorManager.ShowCursor();
            playerManager.uIDocumentPlayerHUDV2.FadeOut();

            // Clear previous choices
            foreach (Transform t in choicePanelRoot)
            {
                Destroy(t.gameObject);
            }

            isContinuing = false;

            message = InsertLineBreaksAtWhitespace(message);

            ShowMessage(character, message);

            // Wait until typewriter finishes or skip is requested
            yield return new WaitUntil(() => hasFinishedTypewriter || isContinuing);

            if (!hasFinishedTypewriter)
            {
                ShowAllTextAtOnce(message);
            }

            isContinuing = false;

            yield return new WaitUntil(() => isContinuing);

            // Create a new copy to prevent mutation
            Response[] clonedResponses = responses?.ToArray();

            if (clonedResponses != null && clonedResponses.Length > 0)
            {
                yield return ShowResponses(clonedResponses);
            }

            // Wait until input has been fully processed so we dont accidentally trigger unwanted GenericTriggers
            yield return new WaitUntil(() => playerManager.starterAssetsInputs.interact == false);

            HideDialogue();
        }

        private void ShowMessage(Character character, string message)
        {
            hasFinishedTypewriter = false;

            // Play dialogue open sound
            soundbank.PlaySound(soundbank.uiDialogue);

            // Optional DOTween pop for the dialogue window
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);

            // Update character info
            if (character != null && !string.IsNullOrEmpty(character.name))
            {
                characterName.text = character.isPlayer ? playerManager.gameSettings.GetPlayerName() : character.GetCharacterName();
                if (character.avatar != null)
                {
                    characterImage.sprite = character.isPlayer ? playerManager.GetPlayerPortrait() : character.avatar;
                }
                characterInfoContainer.SetActive(true);
            }
            else
            {
                characterInfoContainer.SetActive(false);
            }

            if (typewriteCoroutine != null)
            {
                StopCoroutine(typewriteCoroutine);
            }

            typewriteCoroutine = StartCoroutine(Typewrite(message));
        }

        private IEnumerator Typewrite(string dialogueText)
        {
            /*
                       dialogueMessage.text = string.Empty;
                      for (int i = 0; i <= dialogueText.Length; i++)
                       {
                           if (isContinuing) break;

                           string currentText = dialogueText.Substring(0, i);
                           dialogueMessage.text = currentText;

                           if (playTypewriterSound && !string.IsNullOrWhiteSpace(currentText))
                           {
                               // soundbank.PlaySound(soundbank.uiTypewriter);
                           }

                           yield return cachedWait;
                       }*/
            yield return null;

            // Ensure full text is shown at the end
            dialogueMessage.text = dialogueText;
            hasFinishedTypewriter = true;
            typewriteCoroutine = null;
        }

        public void ShowAllTextAtOnce(string dialogueText)
        {
            if (typewriteCoroutine != null)
            {
                StopCoroutine(typewriteCoroutine);
                typewriteCoroutine = null;
            }

            dialogueMessage.text = dialogueText;
            hasFinishedTypewriter = true;
        }

        /// <summary>
        /// Call when dialogue is done to hide UI
        /// </summary>
        public void HideDialogue()
        {
            gameObject.SetActive(false);
            playerManager.uIDocumentPlayerHUDV2.FadeIn();
            playerManager.thirdPersonController.LockCameraPosition = false;
            cursorManager.HideCursor();
        }

        public IEnumerator ShowResponses(Response[] responses)
        {
            if (responses.Length <= 0)
            {
                yield break;
            }

            Response selectedResponse = null;
            bool hasFocused = false;
            bool hasSelectedResponse = false;

            foreach (var response in responses)
            {
                GameObject buttonInstance = Instantiate(choiceButtonPrefab, choicePanelRoot.transform);

                buttonInstance.GetComponentInChildren<TextMeshProUGUI>().text = response.text;

                buttonInstance.GetComponent<Button>().onClick.AddListener(() =>
                {
                    hasSelectedResponse = true;
                    selectedResponse = response;
                    response.onResponseSelected?.Invoke();
                    playerManager.thirdPersonController.LockCameraPosition = false;
                });

                if (!hasFocused)
                {
                    hasFocused = true;
                    buttonInstance.GetComponent<Button>().Select();
                    firstSelectable = buttonInstance.GetComponent<Button>();
                }
            }

            cursorManager.ShowCursor();
            playerManager.thirdPersonController.LockCameraPosition = true;

            yield return new WaitUntil(() => hasSelectedResponse == true);

            playerManager.thirdPersonController.LockCameraPosition = false;

            selectedResponse.AwardReputation(flagsDatabase, playerManager.playerReputation);

            // Use Sub Events Option
            if (selectedResponse.subEventPage != null)
            {
                EventBase[] choiceEvents = selectedResponse.subEventPage.GetComponents<EventBase>();

                if (choiceEvents.Length > 0)
                {
                    foreach (EventBase subEvent in choiceEvents)
                    {
                        yield return subEvent.Dispatch();
                    }
                }
            }
            else if (string.IsNullOrEmpty(selectedResponse.reply) == false)
            {
                yield return DisplayMessage(selectedResponse.replier, selectedResponse.reply, new Response[] { });
            }

            selectedResponse.onResponseFinished?.Invoke();
        }
    }
}
