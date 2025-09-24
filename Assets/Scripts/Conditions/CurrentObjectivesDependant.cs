using System;
using System.Linq;
using AF.Events;
using EditorAttributes;
using TigerForge;
using UnityEngine;

namespace AF.Conditions
{
    public class CurrentObjectivesDependant : MonoBehaviour
    {

        [SerializeField] QuestParent questParent;
        [HelpBox("Children are only true if quest is in the given current objectives")]
        [SerializeField] QuestObjective[] currentObjetives;

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

            if (questParent == null || questParent.IsCompleted() || currentObjetives.Length == 0)
            {
                Utils.UpdateTransformChildren(transform, isActive);
                return;
            }

            var current = questParent.GetCurrentObjective();
            isActive = currentObjetives.Contains(current);

            Utils.UpdateTransformChildren(transform, isActive);
        }
    }
}
