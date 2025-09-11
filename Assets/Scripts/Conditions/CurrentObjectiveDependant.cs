using System;
using System.Linq;
using AF.Events;
using EditorAttributes;
using TigerForge;
using UnityEngine;

namespace AF.Conditions
{
    public class CurrentObjectiveDependant : MonoBehaviour
    {

        [SerializeField] QuestParent questParent;
        [HelpBox("Children are only true if quest is in the given current objective")]
        [SerializeField] QuestObjective currentObjetive;

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
                if (currentObjetive != null)
                {
                    isActive = questParent.GetCurrentObjective() == currentObjetive && questParent.IsCompleted() == false;
                }
            }

            Utils.UpdateTransformChildren(transform, isActive);
        }
    }
}
