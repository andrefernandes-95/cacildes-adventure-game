using System.Collections.Generic;
using System.Linq;
using CI.QuickSave;
using UnityEngine;

namespace AF
{
    [System.Serializable]
    public class QuestSerializedState
    {
        public string questName;
        public bool hasStarted;
        public List<string> completedObjectives = new();
        public bool isTracked;
    }

    public class QuestManager : MonoBehaviour
    {
        public List<QuestParent> allQuests = new();

        public const string SAVE_KEY = "quests";

        public void OnSave(QuickSaveWriter quickSaveWriter)
        {
            List<QuestSerializedState> serializedStates = new();

            foreach (QuestParent questParent in allQuests)
            {
                QuestSerializedState questSerializedState = new()
                {
                    questName = questParent.name,
                    hasStarted = questParent.hasStarted,
                    completedObjectives = questParent.completedObjectives.Select(x => x.name).ToList(),
                    isTracked = questParent.isTracked
                };

                serializedStates.Add(questSerializedState);
            }

            quickSaveWriter.Write(SAVE_KEY, serializedStates);
        }

        public void OnLoad(QuickSaveReader quickSaveReader)
        {
            quickSaveReader.TryRead(SAVE_KEY, out List<QuestSerializedState> serializedQuests);

            foreach (QuestParent questParent in allQuests)
            {
                questParent.Clear();
            }

            if (serializedQuests != null && serializedQuests.Count > 0)
            {
                foreach (QuestSerializedState questSerializedState in serializedQuests)
                {
                    QuestParent target = allQuests.FirstOrDefault(quest => quest.name == questSerializedState.questName);
                    if (target != null)
                    {
                        target.hasStarted = questSerializedState.hasStarted;
                        target.isTracked = questSerializedState.isTracked;

                        target.completedObjectives.Clear();

                        foreach (string questObjectiveName in questSerializedState.completedObjectives)
                        {
                            QuestObjective targetObjective = target.objectives.FirstOrDefault(x => x.name == questObjectiveName);
                            if (targetObjective != null)
                            {
                                target.completedObjectives.Add(targetObjective);
                            }
                        }
                    }
                }
            }
        }

        public List<QuestParent> GetTrackedQuests()
        {
            return allQuests.Where(q => q.isTracked).ToList();
        }
        public List<QuestParent> GetQuestsStarted()
        {
            return allQuests.Where(q => q.hasStarted).ToList();
        }

        public void ClearQuestsForNewGamePlus()
        {
            foreach (QuestParent quest in allQuests)
            {
                quest.ResetQuest();
            }
        }
    }
}
