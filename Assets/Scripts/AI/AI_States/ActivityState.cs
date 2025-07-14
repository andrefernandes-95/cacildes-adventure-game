using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace AF
{
    public class ActivityState : State
    {
        [Header("Components")]
        public CharacterManager characterManager;

        [Header("Events")]
        public UnityEvent onStateEnter;
        public UnityEvent onStateUpdate;
        public UnityEvent onStateExit;

        [Header("Activities")]
        [SerializeField] NPCActivity[] activities;
        int currentActivityIndex = -1;

        [Header("Optional States")]
        public GreetingState greetingState;

        bool hasChosenNextActivity = false;

        float defaultStoppingDistance;

        void Awake()
        {
            defaultStoppingDistance = characterManager.agent.stoppingDistance;
        }

        public override void OnStateEnter(StateManager stateManager)
        {
            onStateEnter?.Invoke();

            if (!hasChosenNextActivity)
            {
                ChooseNextActivity();
            }
        }

        public override void OnStateExit(StateManager stateManager)
        {
            onStateExit?.Invoke();
            characterManager.agent.enabled = false;
            characterManager.agent.stoppingDistance = defaultStoppingDistance;
        }

        public override State Tick(StateManager stateManager)
        {
            onStateUpdate?.Invoke();

            if (ShouldGreetPlayer())
            {
                StopCurrentActivity();
                return greetingState;
            }

            if (characterManager.characterActivityManager.currentActivity != null)
            {
                return this;
            }

            if (!hasChosenNextActivity)
            {
                ChooseNextActivity();
                return this;
            }

            NPCActivity currentActivity = activities[currentActivityIndex];

            if (currentActivity != null)
            {
                if (!characterManager.agent.enabled)
                {
                    characterManager.agent.enabled = true;
                }

                characterManager.agent.stoppingDistance = currentActivity.stoppingDistance;
                characterManager.agent.SetDestination(currentActivity.GetActivityDestination().position);

                if (HasReachedActivity())
                {
                    PerformCurrentActivity();
                }
            }

            return this;
        }

        void ChooseNextActivity()
        {
            if (activities.Length == 0)
                return;

            currentActivityIndex = (currentActivityIndex + 1) % activities.Length;
            hasChosenNextActivity = true;
        }

        bool HasReachedActivity()
        {
            return activities[currentActivityIndex].HasReachedActivity(characterManager);
        }

        void PerformCurrentActivity()
        {
            if (characterManager.characterBaseActivityManager.currentActivity == null)
            {
                characterManager.agent.enabled = false;
                characterManager.characterBaseActivityManager.SetActivity(activities[currentActivityIndex]);
            }

            hasChosenNextActivity = false;
        }

        void StopCurrentActivity()
        {
            characterManager.characterBaseActivityManager.StopCurrentActivity();
            hasChosenNextActivity = false;
        }

        bool ShouldGreetPlayer()
        {
            return greetingState != null && greetingState.CanGreet();
        }
    }
}
