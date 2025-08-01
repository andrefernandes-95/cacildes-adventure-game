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

        [Obsolete("Use objectives")]
        public int[] questProgresses;

        [Header("Progress Requirements")]
        public bool questMustNotHaveStarted = false;
        public bool questMustBeCompleted = false;
        public QuestObjective[] requiredObjectives;

        [Header("Quest Status Options")]
        public bool shouldBeWithinRange = true;
        public bool shouldBeOutsideRange = false;

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
                EventManager.StartListening(EventMessages.ON_QUESTS_PROGRESS_CHANGED, Evaluate);
            }
        }

        public void Evaluate()
        {
            bool isActive = false;

            if (questParent != null)
            {
                if (questMustNotHaveStarted)
                {
                    isActive = !questParent.HasStarted();
                }
                else if (questMustBeCompleted)
                {
                    isActive = questParent.IsCompleted();

                }
                else if (requiredObjectives != null && requiredObjectives.Length > 0)
                {
                    isActive = requiredObjectives.All(obj => questParent.IsObjectiveCompleted(obj));
                }
                // TODO: Legacy Code, remove
                else if (questProgresses != null)
                {
                    if (shouldBeWithinRange)
                    {
                        isActive = questProgresses.Contains(questParent.questProgress);
                    }
                    else if (shouldBeOutsideRange)
                    {
                        isActive = !questProgresses.Contains(questParent.questProgress);
                    }
                }
            }

            Utils.UpdateTransformChildren(transform, isActive);
        }
    }
}
