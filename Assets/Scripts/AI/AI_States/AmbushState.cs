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
        public ChaseState chaseState;

        bool ambushHasBegun = false;
        public bool shouldAwake = false;

        [Header("Options")]
        [SerializeField] float exitAmbushCrossFade = 0.2f;

        [SerializeField] string ambushIdle = "Ambush - Idle";
        [SerializeField] string ambushExit = "Ambush";

        bool hasSubscribedToOnTakeDamageEvent = false;

        Coroutine PlayAmbushAnimationCoroutine;
        Coroutine BeginAmbushCoroutine;

        private void Awake()
        {
            EventManager.StartListening(EventMessages.ON_LEAVING_BONFIRE, () =>
            {
                ResetAmbushFlags();
            });
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ResetAmbushFlags()
        {
            ambushHasBegun = false;
            shouldAwake = false;
        }

        void OnTakeDamage()
        {
            if (!ambushHasBegun)
            {
                BeginAmbush();
            }
            else if (!shouldAwake)
            {
                FinishAmbush();
            }
        }

        public override void OnStateEnter(StateManager stateManager)
        {
            onStateEnter?.Invoke();

            if (PlayAmbushAnimationCoroutine != null)
            {
                StopCoroutine(PlayAmbushAnimationCoroutine);
            }

            PlayAmbushAnimationCoroutine = StartCoroutine(PlayAmbushAnimation());

            if (!hasSubscribedToOnTakeDamageEvent)
            {
                characterManager.health.onTakeDamage.AddListener(OnTakeDamage);
                hasSubscribedToOnTakeDamageEvent = true;
            }
        }

        // Delay ambush animation execution to next frame to give time for Awake and Start to run and update the
        // character animations for weapons and other stuff
        IEnumerator PlayAmbushAnimation()
        {
            yield return new WaitForEndOfFrame();
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
            if (BeginAmbushCoroutine != null)
            {
                StopCoroutine(BeginAmbushCoroutine);
            }

            BeginAmbushCoroutine = StartCoroutine(BeginAmbush_Coroutine());
        }

        // TODO: Remove float begin ambush once we finish changing every ambush state enemy in the game
        IEnumerator BeginAmbush_Coroutine()
        {
            yield return new WaitForEndOfFrame();

            if (ambushHasBegun)
            {
                // if enemy is awake, we should skip to FinishAmbush() since that will make us chase the target
                FinishAmbush();
                yield break;
            }

            ambushHasBegun = true;
            PlayExitAmbush();
            onAmbushBegin?.Invoke();
        }

        public void FinishAmbush()
        {
            onAmbushFinish?.Invoke();
            characterManager.stateManager.ScheduleState(chaseState);
            shouldAwake = true;
        }

        public void PlayExitAmbush()
        {
            characterManager.PlayCrossFadeBusyAnimationWithRootMotion(ambushExit, exitAmbushCrossFade);
        }
    }
}
