using UnityEngine;

namespace AF
{
    [CreateAssetMenu(fileName = "Throw Weapon Ability", menuName = "Abilities / Weapons / New Throw Weapon Ability", order = 0)]
    public class ThrowWeaponAbility : Ability
    {
        [Header("FX")]
        public GameObject chargingFX;

        [Header("Override Animation Clips")]
        [SerializeField] AnimationClip spellStart;
        [SerializeField] AnimationClip spellHold;
        [SerializeField] AnimationClip spellRelease;

        public override void OnPrepare(CharacterManager characterManager)
        {
        }

        public override void OnPrepare(PlayerManager playerManager)
        {
            if (!playerManager.playerAbilityManager.CanUseAbility())
            {
                return;
            }

            playerManager.playerAbilityManager.SetAnimations(playerManager, spellStart, spellHold, spellRelease);
            playerManager.playerAbilityManager.hasOverridenAnimations = true;

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
            playerManager.attackStatManager.damageBonus = damage;
            playerManager.playerThrowWeaponManager.ThrowWeapon();
        }

        public override void OnUse(CharacterManager characterManager)
        {

        }

        public override bool CanUseAbility(CharacterBaseManager character)
        {
            return true;
        }

    }
}
