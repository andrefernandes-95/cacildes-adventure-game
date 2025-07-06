using System.Collections;
using AF.Health;
using UnityEngine;

namespace AF
{
    public class CharacterBlockController : CharacterAbstractBlockController
    {

        [Header("Settings")]
        public bool shouldFaceTargetWhenBlockingAttack = true;

        [SerializeField] bool isParrying = false;

        Coroutine StartBlockingCoroutine;

        [Header("Blocking Options")]
        [SerializeField] float minBlockTime = 1.5f;
        [SerializeField] float maxBlockTime = 4.5f;


        public override void ResetStates()
        {
            base.ResetStates();
            isParrying = false;
        }

        public override void BlockAttack(Damage damage)
        {
            if (shouldFaceTargetWhenBlockingAttack)
            {
                (characterManager as CharacterManager)?.FaceTarget();
            }

            base.BlockAttack(damage);
        }

        public override int GetPostureDamageFromParry()
        {
            return basePostureDamageFromParry;
        }

        public override float GetUnarmedParryWindow()
        {
            return baseUnarmedParryWindow;
        }

        public void StartBlocking()
        {
            if (StartBlockingCoroutine != null)
            {
                StopCoroutine(StartBlockingCoroutine);
            }

            StartBlockingCoroutine = StartCoroutine(HandleBlocking());
        }

        IEnumerator HandleBlocking()
        {
            SetIsBlocking(true);
            characterManager.PlayCrossFadeBusyAnimationWithRootMotion(characterManager.characterAbstractBlockController.hashBlock, .1f);

            HandleBlockStart();

            float blockTime = Random.Range(minBlockTime, maxBlockTime);

            yield return new WaitForSeconds(blockTime);

            HandleBlockEnd();

            SetIsBlocking(false);
        }

        public void HandleBlockStart()
        {
            (characterManager as CharacterManager).faceTarget = true;
        }

        public void HandleBlockEnd()
        {
            (characterManager as CharacterManager).faceTarget = false;
            characterManager.PlayAnimationWithCrossFade("Idle");
        }

        public override void BeginParrying()
        {
            isParrying = true;
            characterManager.PlayAnimationWithCrossFade(hashPrepareParry);
        }

        public override bool CanUseParrying()
        {
            if (isParrying)
            {
                return false;
            }

            if (characterManager.GetTarget() is PlayerManager playerManager)
            {
                return playerManager.playerCombatController.IsAttacking();
            }

            if (characterManager.GetTarget() is CharacterManager target)
            {
                return target.characterBaseWeaponsManager.IsAttacking();
            }

            return true;
        }

        public override bool IsAbleToParry(Damage damage)
        {
            if (!isParrying)
            {
                return false;
            }

            if (damage != null && damage.canNotBeParried)
            {
                return false;
            }

            return true;
        }
    }
}
