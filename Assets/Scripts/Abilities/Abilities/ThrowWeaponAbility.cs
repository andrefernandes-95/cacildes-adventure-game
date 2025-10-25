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

        [Header("Settings")]
        [SerializeField] bool isChargeable = true;
        [SerializeField] bool shouldRotateOnUpdate = true;

        [Header("Animations")]
        [SerializeField] string chargedSpellAnimationHash = "Cast Spell";
        [SerializeField] string simpleCastAnimationHash = "Simple Cast";

        public override void OnPrepare(CharacterManager characterManager)
        {
            characterManager.characterAbilityManager.SetAnimations(characterManager, spellStart, spellHold, spellRelease);
            characterManager.characterAbilityManager.hasOverridenAnimations = true;

            characterManager.characterAbilityManager.SetCurrentAbility(this);

            if (isChargeable)
            {
                characterManager.characterAbilityManager.SetIsCharging(true);

                if (chargingFX != null)
                {
                    GameObject chargingAbilityFXInstance = Instantiate(
                        chargingFX, characterManager.characterTransformHelper.rightHand);

                    characterManager.characterAbilityManager.chargingAbilityFX = chargingAbilityFXInstance;
                }
            }


            characterManager.PlayBusyAnimationWithRootMotion(isChargeable ? chargedSpellAnimationHash : simpleCastAnimationHash);
        }

        public override void OnPrepare(PlayerManager playerManager)
        {
            playerManager.playerAbilityManager.SetAnimations(playerManager, spellStart, spellHold, spellRelease);
            playerManager.playerAbilityManager.hasOverridenAnimations = true;

            playerManager.playerAbilityManager.SetCurrentAbility(this);

            if (isChargeable)
            {
                playerManager.playerAbilityManager.SetIsCharging(true);

                if (chargingFX != null)
                {
                    GameObject chargingAbilityFXInstance = Instantiate(
                        chargingFX, playerManager.characterTransformHelper.rightHand);

                    playerManager.playerAbilityManager.chargingAbilityFX = chargingAbilityFXInstance;
                }
            }

            playerManager.PlayBusyAnimationWithRootMotion(isChargeable ? chargedSpellAnimationHash : simpleCastAnimationHash);
        }

        public override void OnUse(PlayerManager playerManager)
        {
            if (isChargeable) damage.Multiply(playerManager.playerAbilityManager.GetChargingAmountMultiplier());
            ApplyDamageScaling(playerManager);
            playerManager.characterBaseAttackManager.damageBonus = damage;

            CombatUtils.ThrowWeapon(
                playerManager.playerWeaponsManager.currentWeaponInstance,
                throwWeaponProjectilePrefab,
                playerManager,
                playerManager.GetTarget(),
                shouldRotateOnUpdate);
        }

        public override void OnUse(CharacterManager characterManager)
        {
            if (isChargeable) damage.Multiply(characterManager.characterAbilityManager.GetChargingAmountMultiplier());
            ApplyDamageScaling(characterManager);

            characterManager.FaceTarget();
            CombatUtils.ThrowWeapon(
                characterManager.characterWeaponsManager.currentWeaponInstance,
                throwWeaponProjectilePrefab,
                characterManager,
                characterManager.targetManager.currentTarget != null ? characterManager.targetManager.currentTarget : null,
                shouldRotateOnUpdate);
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
            if (isChargeable) characterManager.characterAbilityManager.ClearChargingEffects();
        }

        public override void OnFinished(PlayerManager playerManager)
        {
            if (isChargeable) playerManager.playerAbilityManager.ClearChargingEffects();
        }
    }
}
