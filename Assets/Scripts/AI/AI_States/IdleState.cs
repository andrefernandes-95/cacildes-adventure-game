using AF.Companions;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class IdleState : State
    {

        [Header("Components")]
        public CharacterManager characterManager;

        [Header("Events")]
        public UnityEvent onStateEnter;
        public UnityEvent onStateUpdate;
        public UnityEvent onStateExit;

        [Header("Companion Settings")]
        public CompanionsDatabase companionsDatabase;

        [Header("States")]
        public State chaseState;

        [Header("Optional States")]
        public GreetingState greetingState;

        // Refs
        PlayerManager _playerManager;

        private void Awake()
        {
        }

        public override void OnStateEnter(StateManager stateManager)
        {
            onStateEnter?.Invoke();
        }

        public override void OnStateExit(StateManager stateManager)
        {
            onStateExit?.Invoke();
        }
        public override State Tick(StateManager stateManager)
        {
            onStateUpdate?.Invoke();

            if (ShouldFollowPlayer())
            {
                return chaseState;
            }
            else if (ShouldGreetPlayer())
            {
                return greetingState;
            }

            return this;
        }

        bool ShouldFollowPlayer()
        {
            if (characterManager.IsCompanion() == false)
            {
                return false;
            }

            if (companionsDatabase.IsCompanionAndIsActivelyInParty(characterManager.GetCharacterID()))
            {
                return Vector3.Distance(characterManager.agent.transform.position, GetPlayerManager().transform.position)
                    > characterManager.agent.stoppingDistance + companionsDatabase.companionToPlayerStoppingDistance;
            }

            return false;
        }

        bool ShouldGreetPlayer()
        {
            return greetingState != null && greetingState.CanGreet();
        }

        PlayerManager GetPlayerManager()
        {
            if (_playerManager == null) { _playerManager = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include); }
            return _playerManager;
        }

    }
}
