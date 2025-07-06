using System.Collections;
using AF.Health;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class PlayerBlockController : CharacterAbstractBlockController
    {
        public PlayerManager playerManager;

        bool canCounterAttack = false;

        public const string counterAttackAnimation = "CounterAttack";

        public UnityEvent onCounterAttack;

        public bool isCounterAttacking = false;

        Coroutine parryTimerCoroutine;

        public override void ResetStates()
        {
            base.ResetStates();

            canCounterAttack = false;
            isCounterAttacking = false;
        }

        public override float GetUnarmedParryWindow()
        {
            return baseUnarmedParryWindow + playerManager.statsBonusController.parryPostureWindowBonus;
        }

        public override int GetPostureDamageFromParry()
        {
            return basePostureDamageFromParry + playerManager.statsBonusController.parryPostureDamageBonus;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnCounterAttack()
        {
            if (this.canCounterAttack)
            {
                this.canCounterAttack = false;

                playerManager.PlayBusyAnimationWithRootMotion(counterAttackAnimation);

                isCounterAttacking = true;
            }
        }

        public void SetCanCounterAttack(bool value)
        {
            this.canCounterAttack = value;
        }

        public override void BeginParrying()
        {
            if (characterManager is CharacterManager)
            {
                parryTimer = 0f;
            }

            if (parryTimerCoroutine != null)
            {
                StopCoroutine(parryTimerCoroutine);
            }

            parryTimerCoroutine = StartCoroutine(HandleParryTimer());
        }

        IEnumerator HandleParryTimer()
        {
            while (parryTimer < GetUnarmedParryWindow())
            {
                parryTimer += Time.deltaTime;
                yield return null;
            }

            parryTimer = Mathf.Infinity;
        }

        public bool IsWithinParryingWindow()
        {
            return parryTimer < GetUnarmedParryWindow();
        }

        public override bool CanUseParrying()
        {
            return true;
        }

        public override bool IsAbleToParry(Damage damage)
        {
            if (damage != null && damage.canNotBeParried)
            {
                return false;
            }

            return IsWithinParryingWindow();
        }
    }
}
