namespace AF
{
    using AF.Events;
    using UnityEngine;

    public class DialogueMoment : Moment
    {
        [SerializeField] CharacterManager dialogueOwner;

        [SerializeField] IdleState idleState;

        GenericTrigger genericTrigger => GetComponent<GenericTrigger>();

        private void Awake()
        {
            onMoment_Start.AddListener(OnMomentStart);
            onMoment_End.AddListener(OnMomentEnd);

            dialogueOwner.targetManager.onTargetSet_Event.AddListener(() =>
            {
                if (genericTrigger != null)
                {
                    genericTrigger.DisableCapturable();
                }
            });

            dialogueOwner.targetManager.onClearTarget_Event.AddListener(() =>
            {
                if (genericTrigger != null)
                {
                    genericTrigger.TurnCapturable();
                }
            });
        }

        void OnMomentStart()
        {
            dialogueOwner.stateManager.ScheduleState(idleState);
            dialogueOwner.FacePlayer();
        }

        void OnMomentEnd()
        {
            dialogueOwner.stateManager.ResetDefaultState();
        }

    }
}
