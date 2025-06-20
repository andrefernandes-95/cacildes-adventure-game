namespace AF
{
    using AF.Events;
    using UnityEngine;

    public class DialogueMoment : Moment
    {
        [SerializeField] CharacterManager dialogueOwner;

        [SerializeField] IdleState idleState;

        // Refs
        PlayerManager _playerManager;

        private void Awake()
        {
            onMoment_Start.AddListener(OnMomentStart);
            onMoment_End.AddListener(OnMomentEnd);
        }

        void OnMomentStart()
        {
            dialogueOwner.stateManager.ScheduleState(idleState);
            FacePlayer();
        }

        void OnMomentEnd()
        {
            dialogueOwner.stateManager.ResetDefaultState();
        }

        void FacePlayer()
        {
            var lookPos = GetPlayerManager().transform.position - dialogueOwner.transform.position;
            lookPos.y = 0;
            dialogueOwner.transform.rotation = Quaternion.LookRotation(lookPos);
        }

        PlayerManager GetPlayerManager()
        {
            if (_playerManager == null) { _playerManager = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include); }
            return _playerManager;
        }

    }
}
