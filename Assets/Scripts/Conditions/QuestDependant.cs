using System;
using System.Linq;
using AF.Events;
using EditorAttributes;
using TigerForge;
using UnityEngine;

namespace AF.Conditions
{
    public class QuestDependant : MonoBehaviour
    {
        public QuestParent questParent;

        [Header("Progress Requirements")]
        public bool questMustNotHaveStarted = false;
        public bool questMustHaveStarted = false;
        public QuestObjective[] requiredObjectives;
        public bool questMustBeCompleted = false;

        [Header("Settings")]
        public bool listenForQuestChanges = true;

        private void Awake()
        {
            Utils.UpdateTransformChildren(transform, false);
        }

        private void Start()
        {
            Evaluate();

            if (listenForQuestChanges)
            {
                EventManager.StartListening(EventMessages.ON_QUEST_ADDED, Evaluate);
                EventManager.StartListening(EventMessages.ON_QUESTS_PROGRESS_CHANGED, Evaluate);
            }
        }

        public void Evaluate()
        {
            bool isActive = false;

            if (questParent != null)
            {
                if (questMustBeCompleted)
                {
                    isActive = questParent.IsCompleted();
                }
                // Quest must not have been completed and requires objectives
                else if (questParent.IsCompleted() == false && requiredObjectives != null && requiredObjectives.Length > 0)
                {
                    isActive = requiredObjectives.All(obj => questParent.IsObjectiveCompleted(obj));
                }
                // Quest has started and no objectives completed
                else if (questMustHaveStarted && questParent.completedObjectives.Count <= 0)
                {
                    isActive = questParent.hasStarted;
                }
                // Quest not started
                else if (questMustNotHaveStarted)
                {
                    isActive = !questParent.hasStarted;
                }
            }

            Utils.UpdateTransformChildren(transform, isActive);
        }
    }
}
