using UnityEngine;
using UnityEngine.Events;

namespace AF
{

    public class CombatState : State
    {
        [Header("Components")]
        public CharacterManager characterManager;


        [Header("States")]
        public State chaseState;
        public State patrolOrIdleState;

        [Header("Events")]
        public UnityEvent onAttack;


        [Header("Events")]
        public UnityEvent onStateEnter;

        bool hasChosenAttack = false;


        public override void OnStateEnter(StateManager stateManager)
        {
            onStateEnter?.Invoke();
            hasChosenAttack = false;
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

            if (HasValidTarget())
            {
                return HandleCombatWithTarget();
            }

            return patrolOrIdleState;
        }

        private bool HasValidTarget()
        {
            var target = characterManager.targetManager.currentTarget;
            if (target == null || target.health.GetCurrentHealth() <= 0)
            {
                characterManager.targetManager.ClearTarget();
                return false;
            }

            return true;
        }

        private State HandleCombatWithTarget()
        {
            var target = characterManager.targetManager.currentTarget.transform;
            float distanceToTarget = Vector3.Distance(characterManager.transform.position, target.position);

            if (!hasChosenAttack || distanceToTarget <= characterManager.agent.stoppingDistance)
            {
                onAttack?.Invoke();
                hasChosenAttack = true;
                return this;
            }
            else
            {
                return chaseState;
            }
        }

    }
}
