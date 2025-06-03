using System.Collections;
using AF.Events;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class AmbushState : State
    {

        [Header("Components")]
        public CharacterManager characterManager;

        [Header("Events")]
        public UnityEvent onStateEnter;
        public UnityEvent onStateUpdate;
        public UnityEvent onStateExit;
        public UnityEvent onAmbushBegin;

        public UnityEvent onAmbushFinish;

        [Header("Transitions")]
        public State idleState;

        bool ambushHasBegun = false;
        public bool shouldAwake = false;

        [Header("Options")]
        [SerializeField] float exitAmbushCrossFade = 0.2f;

        [SerializeField] string ambushIdle = "Ambush [Skeleton] - Idle";
        [SerializeField] string ambushExit = "Ambush [Skeleton]";

        private void Awake()
        {
            EventManager.StartListening(EventMessages.ON_LEAVING_BONFIRE, () =>
            {
                ambushHasBegun = false;
                shouldAwake = false;
            });
        }

        public override void OnStateEnter(StateManager stateManager)
        {
            onStateEnter?.Invoke();

            if (characterManager.agent.isOnNavMesh)
            {
                characterManager.agent.ResetPath();
            }

            characterManager.agent.speed = 0f;

            characterManager.PlayBusyAnimationWithRootMotion(ambushIdle);
        }

        public override void OnStateExit(StateManager stateManager)
        {
            onStateExit?.Invoke();
        }

        public override State Tick(StateManager stateManager)
        {
            onStateUpdate?.Invoke();

            if (shouldAwake)
            {
                return idleState;
            }

            return this;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void BeginAmbush()
        {
            BeginAmbush(0f);
        }

        // TODO: Remove float begin ambush once we finish changing every ambush state enemy in the game
        public void BeginAmbush(float beginAmbush)
        {
            if (ambushHasBegun)
            {
                return;
            }

            ambushHasBegun = true;
            PlayExitAmbush();
            onAmbushBegin?.Invoke();
        }

        public void FinishAmbush()
        {
            onAmbushFinish?.Invoke();
            shouldAwake = true;
        }

        public void PlayExitAmbush()
        {
            characterManager.PlayCrossFadeBusyAnimationWithRootMotion(ambushExit, exitAmbushCrossFade);
        }
    }
}
