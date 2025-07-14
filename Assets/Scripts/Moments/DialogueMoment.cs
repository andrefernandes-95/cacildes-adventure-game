namespace AF
{
    using AF.Events;
    using UnityEngine;

    public class DialogueMoment : Moment
    {
        [SerializeField] CharacterManager dialogueOwner;

        [SerializeField] IdleState idleState;

        private void Awake()
        {
            onMoment_Start.AddListener(OnMomentStart);
            onMoment_End.AddListener(OnMomentEnd);
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
