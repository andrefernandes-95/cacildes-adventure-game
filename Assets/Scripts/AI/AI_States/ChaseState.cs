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

        [Header("Events")]
        public UnityEvent onStateEnter;
        public UnityEvent onTargetReached;
        public UnityEvent onTargetLost;

        [Header("Chase Actions Settings")]
        public float maxIntervalBetweenDecidingChaseActions = 5f;
        float currentIntervalBetweenChaseActions = 0f;

        [Header("Jump Actions")]
        [SerializeField] bool canJumpToReachTarget = false;
        [SerializeField] float minimumDistanceToJump = 4f;

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
        }

        public override void OnStateExit(StateManager stateManager)
        {
        }

        public override State Tick(StateManager stateManager)
        {
            if (characterManager.IsBusy())
            {
                return this;
            }

            if (ShouldJumpTowardsTarget())
            {
                PerformJumpTowardsTarget();
                return this;
            }

            if (characterManager.targetManager.currentTarget != null)
            {
                // If Target Is Dead, Stop Chasing
                if (characterManager.targetManager.currentTarget.health.GetCurrentHealth() <= 0)
                {
                    characterManager.targetManager.ClearTarget();
                    return patrolOrIdleState;
                }


                characterManager.SetAgentDestination(characterManager.targetManager.currentTarget.transform.position);

                PivotTowardsTarget();

                characterManager.SetSpeed(1f);

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
            characterManager.SetAgentDestination(playerManager.transform.position);
            characterManager.SetSpeed(1f);

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

        bool ShouldJumpTowardsTarget()
        {
            if (!canJumpToReachTarget || characterManager.characterGravity == null)
            {
                return false;
            }

            if (characterManager == null || characterManager.targetManager.currentTarget == null)
            {
                return false;
            }

            float verticalDifference = characterManager.targetManager.currentTarget.transform.position.y - characterManager.transform.position.y;

            if (Vector3.Distance(characterManager.targetManager.currentTarget.transform.position, characterManager.transform.position) > maxChaseDistance)
            {
                return false;
            }

            return Mathf.Abs(verticalDifference) > 1.5f && characterManager.characterController.isGrounded;
        }

        void PerformJumpTowardsTarget()
        {
            characterManager.characterGravity.shouldJumpToTarget = true;
        }

        void PivotTowardsTarget()
        {
            if (characterManager.isBusy)
            {
                return;
            }

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

            characterManager.RotateTowardsTargetAgentDestination();
        }

    }
}
