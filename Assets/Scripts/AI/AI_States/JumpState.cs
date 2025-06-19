using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class JumpState : State
    {
        [Header("Components")]
        public CharacterManager characterManager;

        [Header("States")]
        public ChaseState chaseState;

        [Header("Events")]
        public UnityEvent onStateEnter;
        bool hasJumped = false;
        [SerializeField] float minimumHeightDistance = 0.5f;

        [SerializeField] float maxJumpCooldown = 1f;
        float lastTimeOfJump;

        public override void OnStateEnter(StateManager stateManager)
        {
            onStateEnter?.Invoke();
            hasJumped = false;
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

            if (!hasJumped)
            {
                PerformJumpTowardsTarget();
                hasJumped = true;
                lastTimeOfJump = Time.time;
            }

            if (characterManager.characterController.isGrounded)
            {
                return chaseState;
            }

            return this;
        }

        public bool ShouldJumpTowardsTarget()
        {
            if (Time.time <= lastTimeOfJump + maxJumpCooldown)
            {
                return false;
            }

            if (characterManager.combatant == null || !characterManager.combatant.canJumpToReachTarget)
            {
                return false;
            }

            if (characterManager == null
                || characterManager.characterGravity == null
                || characterManager.targetManager.currentTarget == null)
            {
                return false;
            }

            float verticalDifference = characterManager.targetManager.currentTarget.transform.position.y - characterManager.transform.position.y;

            if (Vector3.Distance(characterManager.targetManager.currentTarget.transform.position, characterManager.transform.position) > chaseState.maxChaseDistance)
            {
                return false;
            }

            if (!characterManager.characterController.isGrounded)
            {
                return false;
            }

            if (Mathf.Abs(verticalDifference) <= minimumHeightDistance)
            {
                return false;
            }

            return true;
        }

        void PerformJumpTowardsTarget()
        {
            characterManager.characterGravity.shouldJumpToTarget = true;
        }
    }
}
