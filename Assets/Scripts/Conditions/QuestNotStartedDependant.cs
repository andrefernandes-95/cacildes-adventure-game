using AF.Events;
using TigerForge;
using UnityEngine;

namespace AF.Conditions
{
    public class QuestNotStartedDependant : MonoBehaviour
    {
        [SerializeField] QuestParent questParent;

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
                isActive = questParent.hasStarted == false;
            }

            Utils.UpdateTransformChildren(transform, isActive);
        }
    }
}
