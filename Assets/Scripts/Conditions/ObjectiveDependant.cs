using System;
using System.Linq;
using AF.Events;
using EditorAttributes;
using TigerForge;
using UnityEngine;

namespace AF.Conditions
{
    public class ObjectiveDependant : MonoBehaviour
    {
        [SerializeField] QuestParent questParent;
        [SerializeField] QuestObjective[] requiredObjectives;

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
                if (requiredObjectives != null && requiredObjectives.Length > 0)
                {
                    isActive = requiredObjectives.All(obj => questParent.IsObjectiveCompleted(obj));
                }
            }

            Utils.UpdateTransformChildren(transform, isActive);
        }
    }
}
