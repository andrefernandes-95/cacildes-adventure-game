using System.Collections.Generic;
using AF.Events;
using TigerForge;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace AF
{

    [CreateAssetMenu(fileName = "Quests Database", menuName = "System/New Quests Database", order = 0)]
    public class QuestsDatabase : ScriptableObject
    {

        [Header("Quests")]
        public List<QuestParent> questsReceived = new();

        public List<QuestParent> trackedQuests = new();


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
                // Clear the list when exiting play mode
                Clear();
            }
        }
#endif
        public void Clear()
        {
            questsReceived.Clear();
            trackedQuests.Clear();

            foreach (var quest in Resources.LoadAll<QuestParent>("Quests"))
            {
                quest.SetProgress(-1);
            }
        }

        public bool IsQuestTracked(QuestParent questParent)
        {
            return trackedQuests.Contains(questParent);
        }

        public void SetQuestToTrack(QuestParent questParent)
        {
            // Do not track completed quests
            if (questParent.IsCompleted())
            {
                if (IsQuestTracked(questParent))
                {
                    trackedQuests.Remove(questParent);
                }

                return;
            }

            if (IsQuestTracked(questParent))
            {
                trackedQuests.Remove(questParent);
            }
            else
            {
                trackedQuests.Add(questParent);
            }

            EventManager.EmitEvent(EventMessages.ON_QUEST_TRACKED);
        }

        public void UntrackQuest(QuestParent questParent)
        {
            if (IsQuestTracked(questParent))
            {
                trackedQuests.Remove(questParent);
            }

            EventManager.EmitEvent(EventMessages.ON_QUEST_TRACKED);
        }

        public void AddQuest(QuestParent questParent)
        {
            if (questParent != null && !questsReceived.Contains(questParent))
            {
                this.questsReceived.Add(questParent);
            }
        }

        public bool ContainsQuest(QuestParent questParent)
        {
            return questsReceived.Contains(questParent);
        }
    }
}
