using AF.Companions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace AF
{
    public class ChaseState : State
    {
        [Header("Components")]
        public CharacterManager characterManager;

        [Header("Chase Settings")]
        public float maxChaseDistance = 20f;

        [Header("States")]
        public State patrolOrIdleState;
        public CombatState combatState;
        [SerializeField] JumpState jumpState;

        [Header("Events")]
        public UnityEvent onStateEnter;
        public UnityEvent onTargetReached;
        public UnityEvent onTargetLost;

        [Header("Chase Actions Settings")]
        public float maxIntervalBetweenDecidingChaseActions = 5f;
        float currentIntervalBetweenChaseActions = 0f;

        [Header("Companion Settings")]
        PlayerManager playerManager;
        public CompanionsDatabase companionsDatabase;

        private void Awake()
        {
            if (characterManager.IsCompanion())
            {
                playerManager = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include);
            }
        }

        public override void OnStateEnter(StateManager stateManager)
        {
            currentIntervalBetweenChaseActions = 0f;
            onStateEnter?.Invoke();
            characterManager.agent.enabled = true;
        }

        public override void OnStateExit(StateManager stateManager)
        {
            characterManager.agent.enabled = false;
        }

        public override State Tick(StateManager stateManager)
        {
            if (characterManager.IsBusy())
            {
                return this;
            }

            /*
            if (jumpState != null && jumpState.ShouldJumpTowardsTarget())
            {
                return jumpState;
            } */

            if (characterManager.targetManager.currentTarget != null)
            {
                NavMeshPath navMeshPath = new();
                characterManager.agent.CalculatePath(characterManager.targetManager.currentTarget.transform.position, navMeshPath);
                characterManager.agent.SetPath(navMeshPath);

                PivotTowardsTarget();

                float distanceToTarget = Vector3.Distance(characterManager.transform.position, characterManager.targetManager.currentTarget.transform.position);

                if (distanceToTarget <= characterManager.agent.stoppingDistance)
                {
                    onTargetReached.Invoke();
                    return combatState;
                }
                else if (distanceToTarget > maxChaseDistance)
                {
                    characterManager.targetManager.currentTarget = null;
                    onTargetLost?.Invoke();
                    return patrolOrIdleState;
                }

                currentIntervalBetweenChaseActions += Time.deltaTime;
                if (currentIntervalBetweenChaseActions >= maxIntervalBetweenDecidingChaseActions)
                {
                    currentIntervalBetweenChaseActions = 0f;

                    if (characterManager.characterCombatController != null)
                    {
                        characterManager.characterCombatController.UseChaseAction();
                        return this;
                    }
                }
            }
            else if (characterManager.IsCompanion() && characterManager?.health?.GetCurrentHealth() > 0 && companionsDatabase.IsCompanionAndIsActivelyInParty(characterManager.GetCharacterID()))
            {
                return FollowPlayer();
            }

            return this;
        }

        State FollowPlayer()
        {
            NavMeshPath navMeshPath = new();
            characterManager.agent.CalculatePath(playerManager.transform.position, navMeshPath);
            characterManager.agent.SetPath(navMeshPath);

            float distanceToTarget = Vector3.Distance(characterManager.transform.position, playerManager.transform.position);

            if (distanceToTarget <= characterManager.agent.stoppingDistance + companionsDatabase.companionToPlayerStoppingDistance)
            {
                return patrolOrIdleState;
            }
            else if (distanceToTarget >= companionsDatabase.maxDistanceToPlayerBeforeTeleportingNear)
            {
                characterManager.TeleportNearPlayer();
            }

            return this;
        }

        void PivotTowardsTarget()
        {
            return;
            if (characterManager.combatant != null && characterManager.combatant.isHumanoid)
            {
                float angleOfCurrentTarget = characterManager.GetAngleOfCurrentTarget();

                if (angleOfCurrentTarget >= 146 && angleOfCurrentTarget <= 180)
                {
                    characterManager.PlayBusyAnimationWithRootMotion("Turn_Right_180");
                    return;
                }

                if (angleOfCurrentTarget <= -146 && angleOfCurrentTarget >= -180)
                {
                    characterManager.PlayBusyAnimationWithRootMotion("Turn_Left_180");
                    return;
                }
            }
        }
    }
}
