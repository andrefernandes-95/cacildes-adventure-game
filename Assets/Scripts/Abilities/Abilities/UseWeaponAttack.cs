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

        public override void OnPrepare(CharacterManager characterManager)
        {
            characterManager.characterAbilityManager.SetCurrentAbility(this);
            characterManager.RotateTowardsTarget(characterManager.rotationSpeed * 10f);

            HandleAttackSpeed(characterManager);

            string hashAttack = "";
            if (isHeavyAttack)
            {
                hashAttack = CombatUtils.GetHeavyAttackAnimationName(attackIndex, !isRightHand);
            }
            else
            {
                hashAttack = CombatUtils.GetLightAttackAnimationName(attackIndex, !isRightHand, characterManager.characterBaseWeaponsManager.CanPowerStance());
            }

            Debug.Log($"Use Weapon Attack for character {characterManager.name}, will use: {hashAttack}");


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
        }

        public override bool CanUseAbility(CharacterBaseManager character)
        {
            if (IsTargetTooFarAway(character))
            {
                return false;
            }

            // If attempting a left hand attack, check if we are not two handing
            if (!isRightHand)
            {
                return character.characterBaseWeaponsManager.IsTwoHanding() == false;
            }

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

        void HandleAttackSpeed(CharacterManager characterManager)
        {
            Weapon currentWeapon;

            if (isRightHand)
            {
                currentWeapon = characterManager.characterWeaponsManager.GetCurrentRightWeapon();
            }
            else
            {
                currentWeapon = characterManager.characterWeaponsManager.GetCurrentLeftWeapon();
            }

            if (currentWeapon != null)
            {
                if (characterManager.characterWeaponsManager.IsTwoHanding())
                {
                    characterManager.animator.speed = isHeavyAttack ? currentWeapon.th_HeavyAttackSpeedPenalty : currentWeapon.twoHandAttackSpeedPenalty;
                }
                else
                {
                    characterManager.animator.speed = isHeavyAttack ? currentWeapon.oh_HeavyAttackSpeedPenalty : currentWeapon.oneHandAttackSpeedPenalty;
                }
            }
        }

        bool IsTargetTooFarAway(CharacterBaseManager characterBaseManager)
        {
            if (characterBaseManager.GetTarget() == null)
            {
                return true;
            }

            if (minimumDistanceToTargetToUse <= 0)
            {
                return false;
            }

            return Vector3.Distance(characterBaseManager.transform.position, characterBaseManager.GetTarget().transform.position) > minimumDistanceToTargetToUse;
        }
    }
}
