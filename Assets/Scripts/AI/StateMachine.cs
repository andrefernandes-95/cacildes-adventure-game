using System.Collections;
using UnityEngine;

namespace AF
{

    public class StateManager : MonoBehaviour
    {
        public State currentState;
        State scheduledState;

        State defaultState;

        [SerializeField] CharacterManager characterManager;

        private void Awake()
        {
            this.defaultState = currentState;
        }

        private void Start()
        {
            if (currentState != null)
            {
                currentState.OnStateEnter(this);
            }
        }

        void FixedUpdate()
        {
            if (scheduledState != null)
            {
                currentState = scheduledState;
                currentState.OnStateEnter(this);
                scheduledState = null;
            }
            else if (currentState != null)
            {
                State nextState = currentState.Tick(this);

                if (nextState != currentState)
                {
                    ScheduleState(nextState);
                }
            }

            ResyncNavmeshAgent();
        }

        public void ScheduleState(State state)
        {
            if (scheduledState == null)
            {
                currentState?.OnStateExit(this);
                currentState = null;

                scheduledState = state;
            }
        }

        public void ResetDefaultState()
        {
            currentState = null;
            scheduledState = null;
            ScheduleState(defaultState);
        }

        public State GetDefaultState() => defaultState;

        void ResyncNavmeshAgent()
        {
            if (characterManager.agent.enabled)
            {
                characterManager.agent.transform.localPosition = Vector3.zero;
                characterManager.agent.transform.localRotation = Quaternion.identity;
            }
        }

    }
}
