using AF.Equipment;
using AF.Health;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(fileName = "Use Generic Creature Attack", menuName = "Abilities / AI / New Use Generic Creature Attack", order = 0)]
    public class UseGenericCreatureAttack : Ability
    {
        public enum GenericCreatureAttackType
        {
            ATTACK_A,
            ATTACK_B,
            ATTACK_C,
            ATTACK_D,
        }

        [SerializeField] GenericCreatureAttackType attack = GenericCreatureAttackType.ATTACK_A;

        [SerializeField] float crossFade = 0.1f;

        public override void OnPrepare(CharacterManager characterManager)
        {
            characterManager.characterAbilityManager.SetCurrentAbility(this);
            characterManager.RotateTowardsTarget(characterManager.rotationSpeed * 10f);

            string hash = "Attack A";
            if (attack == GenericCreatureAttackType.ATTACK_B) hash = "Attack B";
            if (attack == GenericCreatureAttackType.ATTACK_C) hash = "Attack C";
            if (attack == GenericCreatureAttackType.ATTACK_D) hash = "Attack D";

            characterManager.PlayCrossFadeBusyAnimationWithRootMotion(hash, crossFade);
        }

        public override void OnPrepare(PlayerManager playerManager)
        {
            if (!playerManager.playerAbilityManager.CanUseAbility())
            {
                return;
            }

            playerManager.playerAbilityManager.SetCurrentAbility(this);

            string hash = "Attack A";
            if (attack == GenericCreatureAttackType.ATTACK_B) hash = "Attack B";
            if (attack == GenericCreatureAttackType.ATTACK_C) hash = "Attack C";
            if (attack == GenericCreatureAttackType.ATTACK_D) hash = "Attack D";

            playerManager.PlayCrossFadeBusyAnimationWithRootMotion(hash, crossFade);
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
