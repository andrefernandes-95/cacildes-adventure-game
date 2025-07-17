namespace AF
{
    using System.Collections.Generic;
    using Ink.Runtime;
    using UnityEngine;

    public class DialogueFunctions : MonoBehaviour
    {
        [SerializeField] QuestParent bearQuest;

        Dictionary<string, DialogueEvent> dialogueEvents = new();
        bool hasInitializedDialogueEvents = false;

        void CollectDialogueEvents()
        {
            if (hasInitializedDialogueEvents)
            {
                return;
            }

            hasInitializedDialogueEvents = true;

            DialogueEvent[] sceneDialogueEvents = FindObjectsByType<DialogueEvent>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (DialogueEvent dialogueEvent in sceneDialogueEvents)
            {
                if (!dialogueEvents.ContainsKey(dialogueEvent.eventId))
                {
                    dialogueEvents.Add(dialogueEvent.eventId, dialogueEvent);
                }
            }
        }

        public void BindStoryToFunctions(Story story)
        {
            story.BindExternalFunction("isDoingChickensQuest", () =>
            {
                return bearQuest.questProgress != -1 && bearQuest.IsCompleted() == false;
            });
            story.BindExternalFunction("runEvent", (string eventId) =>
            {
                RunEvent(eventId);
            });
        }

        void RunEvent(string eventId)
        {
            CollectDialogueEvents();

            if (dialogueEvents.ContainsKey(eventId))
            {
                dialogueEvents[eventId].Execute();
            }
        }
    }
}
