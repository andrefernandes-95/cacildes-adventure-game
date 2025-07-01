using AF.Health;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(fileName = "Use Weapon Attack", menuName = "Abilities / Weapons / New Use Weapon Attack", order = 0)]
    public class UseWeaponAttack : Ability
    {
        public bool isRightHand = true;
        public bool isHeavyAttack = false;
        public int attackIndex = 0;

        [Header("AI Settings")]
        public float cooldown = 5f;

        public override void OnPrepare(CharacterManager characterManager)
        {
            if (!characterManager.characterAbilityManager.CanUseAbility())
            {
                return;
            }

            characterManager.characterAbilityManager.SetCurrentAbility(this);
            characterManager.RotateTowardsTarget(characterManager.rotationSpeed * 10f);

            string hashAttack = "";
            if (isHeavyAttack)
            {
                hashAttack = CombatUtils.GetHeavyAttackAnimationName(attackIndex, !isRightHand);
            }
            else
            {
                hashAttack = CombatUtils.GetLightAttackAnimationName(attackIndex, !isRightHand, characterManager.characterBaseWeaponsManager.CanPowerStance());
            }

            characterManager.PlayCrossFadeBusyAnimationWithRootMotion(hashAttack, .1f);
        }

        public override void OnPrepare(PlayerManager playerManager)
        {
            if (!playerManager.playerAbilityManager.CanUseAbility())
            {
                return;
            }

            playerManager.playerAbilityManager.SetCurrentAbility(this);

            bool canPowerStance = false;

            playerManager.PlayBusyAnimationWithRootMotion(
                CombatUtils.GetLightAttackAnimationName(attackIndex, !isRightHand, canPowerStance));
        }

        public override void OnUse(PlayerManager playerManager)
        {
            damage.Multiply(playerManager.playerAbilityManager.GetChargingAmountMultiplier());

            ApplyDamageScaling(playerManager);

            playerManager.characterBaseAttackManager.damageBonus = damage;
            playerManager.characterBaseAttackManager.SetIsAttackingWithLeftHand(isRightHand == false);

            if (isRightHand && playerManager.playerWeaponsManager.currentWeaponInstance != null)
            {
                playerManager.playerWeaponsManager.currentWeaponInstance.EnableHitbox();
            }
            else if (!isRightHand && playerManager.playerWeaponsManager.currentShieldInstance != null)
            {
                playerManager.playerWeaponsManager.currentShieldInstance.EnableHitbox();
            }
        }

        public override void OnUse(CharacterManager characterManager)
        {
            characterManager.characterBaseAttackManager.SetIsAttackingWithLeftHand(isRightHand == false);
        }

        public override bool CanUseAbility(CharacterBaseManager character)
        {
            return true;
        }

        public override Damage GetDamage(CharacterBaseManager attacker)
        {
            if (isRightHand)
            {
                Weapon rightWeapon = attacker.characterBaseWeaponsManager.GetCurrentRightWeapon();
                if (rightWeapon != null)
                {
                    Damage weaponDamage = rightWeapon.damage.Clone();
                    weaponDamage.Combine(damage);
                    return weaponDamage;
                }
            }

            Weapon leftWeapon = attacker.characterBaseWeaponsManager.GetCurrentLeftWeapon();
            if (leftWeapon != null)
            {
                Damage weaponDamage = leftWeapon.damage.Clone();
                weaponDamage.Combine(damage);
                return weaponDamage;
            }

            return damage;
        }
    }
}
