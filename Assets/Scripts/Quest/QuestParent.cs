using System.Collections.Generic;
using AF.Events;
using GameAnalyticsSDK;
using TigerForge;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization;

namespace AF
{
    [CreateAssetMenu(menuName = "Data / New Quest")]

    public class QuestParent : ScriptableObject
    {
        [Header("Quest Type")]
        public QuestType questType;

        [Header("Status")]
        public bool hasStarted = false;
        public bool isTracked = false;

        [TextArea]
        public new string name;
        public LocalizedString questName_LocalizedString;
        public Texture questIcon;

        [Header("Quest Objectives Data")]
        public List<QuestObjective> objectives = new();
        public Character questGiver;
        public Character[] relatedCharacters;

        public List<QuestObjective> completedObjectives = new();

        [Header("Quest Description")]
        public LocalizedString questDescription;

        [Header("Testing")]
        public bool useDefaultQuestProgress = false;
        public int defaultQuestProgress = 0;

#if UNITY_EDITOR

        private void OnEnable()
        {
            // No need to populate the list; it's serialized directly
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                Clear();
            }
        }
#endif

        public void Clear()
        {
            hasStarted = false;
            isTracked = false;
            completedObjectives.Clear();
        }

        public bool IsCompleted()
        {
            return hasStarted && completedObjectives.Count >= objectives.Count;
        }

        public void CompleteObjective(QuestObjective questObjective)
        {
            StartQuest();

            if (!completedObjectives.Contains(questObjective))
            {
                completedObjectives.Add(questObjective);
            }

            EventManager.EmitEvent(EventMessages.ON_QUESTS_PROGRESS_CHANGED);

            if (IsCompleted() && isTracked)
            {
                // Untrack quest
                UntrackQuest();
            }
        }

        public bool IsObjectiveCompleted(QuestObjective questObjective)
        {
            return completedObjectives.Contains(questObjective);
        }

        public void TrackQuest()
        {
            isTracked = true;
            EventManager.EmitEvent(EventMessages.ON_QUEST_TRACKED);
        }

        public void UntrackQuest()
        {
            isTracked = false;
            EventManager.EmitEvent(EventMessages.ON_QUEST_TRACKED);
        }

        public void StartQuest()
        {
            if (!hasStarted)
            {
                hasStarted = true;
                EventManager.EmitEventData(EventMessages.ON_QUEST_ADDED, this);
            }
        }

        public QuestObjective GetCurrentObjective()
        {
            if (objectives == null)
            {
                return null;
            }

            if (!hasStarted)
            {
                return null;
            }

            if (completedObjectives.Count >= objectives.Count)
            {
                return null;
            }

            if (completedObjectives.Count <= 0)
            {
                return objectives[0];
            }

            return objectives[completedObjectives.Count];
        }
    }
}
