using AF.Health;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(fileName = "Throw Weapon Ability", menuName = "Abilities / Weapons / New Throw Weapon Ability", order = 0)]
    public class ThrowWeaponAbility : Ability
    {
        [Header("FX")]
        public GameObject chargingFX;

        [Header("Throw Weapon Projectile Prefab")]
        public GameObject throwWeaponProjectilePrefab;

        [Header("Override Animation Clips")]
        [SerializeField] AnimationClip spellStart;
        [SerializeField] AnimationClip spellHold;
        [SerializeField] AnimationClip spellRelease;

        public override void OnPrepare(CharacterManager characterManager)
        {
            characterManager.characterAbilityManager.SetAnimations(characterManager, spellStart, spellHold, spellRelease);
            characterManager.characterAbilityManager.hasOverridenAnimations = true;
            characterManager.characterAbilityManager.SetIsCharging(true);

            characterManager.characterAbilityManager.SetCurrentAbility(this);

            if (chargingFX != null)
            {
                GameObject chargingAbilityFXInstance = Instantiate(
                    chargingFX, characterManager.characterTransformHelper.rightHand);

                characterManager.characterAbilityManager.chargingAbilityFX = chargingAbilityFXInstance;
            }

            characterManager.PlayBusyAnimationWithRootMotion("Cast Spell");
        }

        public override void OnPrepare(PlayerManager playerManager)
        {
            playerManager.playerAbilityManager.SetAnimations(playerManager, spellStart, spellHold, spellRelease);
            playerManager.playerAbilityManager.hasOverridenAnimations = true;
            playerManager.playerAbilityManager.SetIsCharging(true);

            playerManager.playerAbilityManager.SetCurrentAbility(this);

            if (chargingFX != null)
            {
                GameObject chargingAbilityFXInstance = Instantiate(
                    chargingFX, playerManager.characterTransformHelper.rightHand);

                playerManager.playerAbilityManager.chargingAbilityFX = chargingAbilityFXInstance;
            }

            playerManager.PlayBusyAnimationWithRootMotion("Cast Spell");
        }

        public override void OnUse(PlayerManager playerManager)
        {
            damage.Multiply(playerManager.playerAbilityManager.GetChargingAmountMultiplier());
            ApplyDamageScaling(playerManager);
            playerManager.characterBaseAttackManager.damageBonus = damage;

            CombatUtils.ThrowWeapon(
                playerManager.playerWeaponsManager.currentWeaponInstance,
                throwWeaponProjectilePrefab,
                playerManager,
                playerManager.GetTarget());
        }

        public override void OnUse(CharacterManager characterManager)
        {
            damage.Multiply(characterManager.characterAbilityManager.GetChargingAmountMultiplier());
            ApplyDamageScaling(characterManager);

            characterManager.FaceTarget();
            CombatUtils.ThrowWeapon(
                characterManager.characterWeaponsManager.currentWeaponInstance,
                throwWeaponProjectilePrefab,
                characterManager,
                characterManager.targetManager.currentTarget != null ? characterManager.targetManager.currentTarget : null);
        }

        public override bool CanUseAbility(CharacterBaseManager character)
        {
            return true;
        }

        public override Damage GetDamage(CharacterBaseManager attacker)
        {
            Weapon rightWeapon = attacker.characterBaseWeaponsManager.GetCurrentRightWeapon();
            if (rightWeapon != null)
            {
                Damage weaponDamage = rightWeapon.damage.Clone();
                weaponDamage.Combine(damage);
                return weaponDamage;
            }

            return damage;
        }

        public override void OnFinished(CharacterManager characterManager)
        {
            characterManager.characterAbilityManager.ClearChargingEffects();
        }

        public override void OnFinished(PlayerManager playerManager)
        {
            playerManager.playerAbilityManager.ClearChargingEffects();
        }
    }
}
