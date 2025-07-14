using AF.Dialogue;
using UnityEngine;

namespace AF
{
    public class GreetingState : State
    {

        [Header("Settings")]
        [SerializeField] float cooldown = 35f;
        float lastTimeOfGreeting = -Mathf.Infinity;

        [Header("Components")]
        public CharacterManager characterManager;
        [SerializeField] GreetingMessageController greetingMessageController;

        // Refs
        PlayerManager _playerManager;

        private void Awake()
        {
        }

        public override void OnStateEnter(StateManager stateManager)
        {
            PickGreeting();
        }

        public override void OnStateExit(StateManager stateManager)
        {
        }

        public override State Tick(StateManager stateManager)
        {
            if (!greetingMessageController.IsGreeting())
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

        public bool CanGreet()
        {
            if (characterManager.characterActivityManager.currentActivity != null)
            {
                return false;
            }

            if (greetingMessageController.IsGreeting())
            {
                return false;
            }

            if (Time.time < lastTimeOfGreeting + cooldown)
            {
                return false;
            }

            if (Vector3.Distance(characterManager.transform.position, GetPlayerManager().transform.position)
                    > characterManager.agent.stoppingDistance)
            {
                return false;
            }

            return true;
        }

        void PickGreeting()
        {
            CharacterGreeting characterGreeting = FindCharacterGreetingInChildren();
            if (characterGreeting != null)
            {
                greetingMessageController.ShowGreeting(characterGreeting);
            }

            lastTimeOfGreeting = Time.time;
        }

        CharacterGreeting FindCharacterGreetingInChildren()
        {
            CharacterGreeting[] characterGreetings = Utils.CollectComponentsFromGameObject<CharacterGreeting>(this.gameObject);

            CharacterGreeting activeCharacterGreeting = null;
            foreach (CharacterGreeting characterGreeting in characterGreetings)
            {
                if (characterGreeting.gameObject.activeSelf)
                {
                    activeCharacterGreeting = characterGreeting;
                    break;
                }
            }

            return activeCharacterGreeting;
        }
    }
}
