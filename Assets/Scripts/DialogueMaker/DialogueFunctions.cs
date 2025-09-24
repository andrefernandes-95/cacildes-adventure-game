namespace AF
{
    using System.Collections.Generic;
    using AF.Flags;
    using Ink.Runtime;
    using UnityEngine;

    public class DialogueFunctions : MonoBehaviour
    {
        [SerializeField] QuestParent bearQuest;
        [SerializeField] QuestParent robertoQuest;
        [SerializeField] QuestParent sewersQuest;
        [SerializeField] QuestObjective robertoKilledObjective;
        [SerializeField] QuestObjective grischaKilledObjective;
        [SerializeField] PlayerManager playerManager;
        [SerializeField] FlagsDatabase flagsDatabase;

        Dictionary<string, DialogueEvent> dialogueEvents = new();
        bool hasInitializedDialogueEvents = false;

        const string DIALOGUE_EVENT_PREFIX = "dialogue_event_";

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
                return bearQuest.hasStarted && bearQuest.IsCompleted() == false;
            });

            story.BindExternalFunction("hasBegunRobertoQuest", () =>
            {
                return robertoQuest.hasStarted;
            });

            story.BindExternalFunction("hasKilledRobertoAndIsWaitingForAReward", () =>
            {
                return robertoQuest.IsObjectiveCompleted(robertoKilledObjective) && robertoQuest.IsCompleted() == false;
            });

            story.BindExternalFunction("hasCompletedRobertoQuest", () =>
            {
                return robertoQuest.IsCompleted();
            });

            story.BindExternalFunction("getReputation", () =>
            {
                return playerManager.playerStats.GetReputation();
            });

            story.BindExternalFunction("runEvent", (string eventId) =>
            {
                RunEvent(eventId);
            });

            story.BindExternalFunction("hasRunEvent", (string eventId) =>
            {
                string key = $"{DIALOGUE_EVENT_PREFIX}${eventId}";

                return flagsDatabase.ContainsFlag(key);
            });

            story.BindExternalFunction("runEventOnce", (string eventId) =>
            {
                string key = $"{DIALOGUE_EVENT_PREFIX}${eventId}";

                if (flagsDatabase.ContainsFlag(key))
                {
                    return;
                }

                flagsDatabase.AddFlag(key);
                RunEvent(eventId);
            });

            story.BindExternalFunction("hasFinishedSewersAndIsReadyForReward", () =>
            {
                return sewersQuest.IsObjectiveCompleted(grischaKilledObjective) && sewersQuest.IsCompleted() == false;
            });

            story.BindExternalFunction("hasStartedSewersQuest", () =>
            {
                return sewersQuest.hasStarted;
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
