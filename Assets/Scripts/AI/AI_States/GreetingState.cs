using UnityEngine;

namespace AF
{
    public class GreetingState : State
    {

        [Header("Components")]
        public CharacterManager characterManager;
        PlayerManager _playerManager;

        [Header("Greeting Animation")]
        public AnimationClip animationClip;

        [Header("Duration")]
        [SerializeField] float greetingDuration = 3f;
        float lastGreetingDuration;

        private void Awake()
        {
        }

        public override void OnStateEnter(StateManager stateManager)
        {
            lastGreetingDuration = Time.time;
        }

        public override void OnStateExit(StateManager stateManager)
        {
        }

        public override State Tick(StateManager stateManager)
        {
            if (lastGreetingDuration + greetingDuration > Time.time)
            {
                return stateManager.GetDefaultState();
            }

            return this;
        }

        PlayerManager GetPlayerManager()
        {
            if (_playerManager == null) { _playerManager = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include); }
            return _playerManager;
        }
    }
}
