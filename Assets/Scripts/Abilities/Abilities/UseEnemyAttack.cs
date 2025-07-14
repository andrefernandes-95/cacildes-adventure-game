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
            Weapon attackingWeapon = null;
            HitboxType attackingHitbox = attacker.characterBaseAttackManager.attackingHitboxType;

            // Handle weapon-based attacks
            switch (attackingHitbox)
            {
                case HitboxType.RIGHT_HAND:
                    attackingWeapon = attacker.characterBaseWeaponsManager.GetCurrentRightWeapon();
                    break;
                case HitboxType.LEFT_HAND:
                    attackingWeapon = attacker.characterBaseWeaponsManager.GetCurrentLeftWeapon();
                    break;
            }

            if (attackingWeapon != null)
            {
                Damage weaponDamage = attackingWeapon.damage.Clone();
                weaponDamage.Combine(damage);
                return weaponDamage;
            }

            // Handle unarmed attacks
            return GetUnarmedDamage(attacker, attackingHitbox) ?? damage;
        }

        private Damage GetUnarmedDamage(CharacterBaseManager attacker, HitboxType hitboxType)
        {
            var weaponsManager = attacker.characterBaseWeaponsManager;

            UnarmedHitbox unarmedHitbox = hitboxType switch
            {
                HitboxType.LEFT_HAND => weaponsManager.leftHandHitbox as UnarmedHitbox,
                HitboxType.RIGHT_HAND => weaponsManager.rightHandHitbox as UnarmedHitbox,
                HitboxType.LEFT_FOOT => weaponsManager.leftFootHitbox as UnarmedHitbox,
                HitboxType.RIGHT_FOOT => weaponsManager.rightFootHitbox as UnarmedHitbox,
                HitboxType.HEAD => weaponsManager.headHitbox as UnarmedHitbox,
                _ => null
            };

            if (unarmedHitbox?.unarmedWeapon?.damage != null)
            {
                Damage unarmedDamage = unarmedHitbox.unarmedWeapon.damage.Clone();
                unarmedDamage.Combine(damage);
                return unarmedDamage;
            }

            // No hitbox to combine damage, just return the ability damage itself
            return damage;
        }

        public override void OnFinished(CharacterManager characterManager)
        {
        }

        public override void OnFinished(PlayerManager playerManager)
        {
        }
    }
}
