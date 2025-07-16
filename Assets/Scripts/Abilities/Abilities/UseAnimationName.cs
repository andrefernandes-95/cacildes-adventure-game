using AF.Equipment;
using AF.Health;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(fileName = "Use Animation Name", menuName = "Abilities / AI / New Use Animation Name", order = 0)]
    public class UseAnimationName : Ability
    {
        [SerializeField] string animationName = "";
        [SerializeField] float crossFade = 0.1f;

        public override void OnPrepare(CharacterManager characterManager)
        {
            characterManager.characterAbilityManager.SetCurrentAbility(this);
            characterManager.RotateTowardsTarget(characterManager.rotationSpeed * 10f);
            characterManager.PlayCrossFadeBusyAnimationWithRootMotion(animationName, crossFade);
        }

        public override void OnPrepare(PlayerManager playerManager)
        {
            if (!playerManager.playerAbilityManager.CanUseAbility())
            {
                return;
            }

            playerManager.playerAbilityManager.SetCurrentAbility(this);
            playerManager.PlayCrossFadeBusyAnimationWithRootMotion(animationName, crossFade);
        }

        public override void OnUse(PlayerManager playerManager)
        {
            damage.Multiply(playerManager.playerAbilityManager.GetChargingAmountMultiplier());
            ApplyDamageScaling(playerManager);
            playerManager.characterBaseAttackManager.damageBonus = damage;
        }

        public override void OnUse(CharacterManager characterManager)
        {
            damage.Multiply(characterManager.characterAbilityManager.GetChargingAmountMultiplier());
            ApplyDamageScaling(characterManager);
            characterManager.characterBaseAttackManager.damageBonus = damage;
        }

        public override bool CanUseAbility(CharacterBaseManager character)
        {
            return true;
        }

        public override Damage GetDamage(CharacterBaseManager attacker)
        {
            return AbilityUtils.GetAbilityDamageForAIAttack(attacker, damage);
        }

        public override void OnFinished(CharacterManager characterManager)
        {
        }

        public override void OnFinished(PlayerManager playerManager)
        {
        }
    }
}
