using UnityEngine;

namespace AF
{
    [CreateAssetMenu(fileName = "Use Weapon Attack", menuName = "Abilities / Weapons / New Use Weapon Attack", order = 0)]
    public class UseWeaponAttack : Ability
    {
        [SerializeField] bool isRightHand = true;
        [SerializeField] int attackIndex = 0;

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

            bool canPowerStance = false;

            characterManager.PlayCrossFadeBusyAnimationWithRootMotion(
                CombatUtils.GetLightAttackAnimationName(attackIndex, !isRightHand, canPowerStance), .1f);
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

            playerManager.attackStatManager.damageBonus = damage;

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
            //            damage.Multiply(characterManager.characterAbilityManager.GetChargingAmountMultiplier());

            ApplyDamageScaling(characterManager);

            // characterManager.characterCombatController = damage;

            if (isRightHand && characterManager.characterWeaponsManager.currentWeaponInstance != null)
            {
                characterManager.characterWeaponsManager.currentWeaponInstance.EnableHitbox();
            }
            else if (!isRightHand && characterManager.characterWeaponsManager.currentShieldInstance != null)
            {
                characterManager.characterWeaponsManager.currentShieldInstance.EnableHitbox();
            }
        }

        public override bool CanUseAbility(CharacterBaseManager character)
        {
            return true;
        }
    }
}
