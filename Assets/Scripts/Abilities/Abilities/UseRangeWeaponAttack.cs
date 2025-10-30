using AF.Health;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(fileName = "Use Range Weapon Attack", menuName = "Abilities / Weapons / New Use Range Weapon Attack", order = 0)]
    public class UseRangeWeaponAttack : Ability
    {
        int previouslyEquippedLeftWeaponIndex = 0;
        Weapon previouslyEquippedLeftHandWeapon;

        bool hasSuccessfullyEquippedRangeWeapon = false;

        [Header("Optional - Temporary Range Weapon")]
        [SerializeField] Weapon rangeWeaponToInstantiateTemporarily;

        [Header("Animations")]
        [SerializeField] string rangeAnimation = "Aim Idle";

        public override void OnPrepare(CharacterManager characterManager)
        {
            if (characterManager.characterWeaponsManager.IsTwoHanding())
            {
                characterManager.characterWeaponsManager.SetIsTwoHanding(false);
            }

            characterManager.characterAbilityManager.SetCurrentAbility(this);
            characterManager.RotateTowardsTarget(characterManager.rotationSpeed * 10f);

            // Cache the previous left weapon equipment information
            previouslyEquippedLeftHandWeapon = characterManager.characterWeaponsManager.GetCurrentLeftWeapon();
            previouslyEquippedLeftWeaponIndex = characterManager.characterWeaponsManager.GetCurrentLeftWeaponIndex();

            // Search ranged weapon
            Weapon potentialWeapon = rangeWeaponToInstantiateTemporarily != null
                ? Instantiate(rangeWeaponToInstantiateTemporarily)
                : characterManager.characterWeaponsManager.GetRangeWeapon();

            if (potentialWeapon != null)
            {
                // Always switch to the first index
                characterManager.characterWeaponsManager.SwitchWeapon(0, false);

                characterManager.characterWeaponsManager.EquipWeapon(potentialWeapon, 0, false);
                hasSuccessfullyEquippedRangeWeapon = true;

                characterManager.PlayCrossFadeBusyAnimationWithRootMotion(rangeAnimation, 0.1f);
                characterManager.characterBaseShooter.ShowArrowPlaceholder();
                characterManager.characterWeaponsManager.HideRightWeapon();
            }
        }

        public override void OnPrepare(PlayerManager playerManager)
        {
        }

        public override void OnUse(PlayerManager playerManager)
        {
        }

        public override void OnUse(CharacterManager characterManager)
        {
            ApplyDamageScaling(characterManager);

            characterManager.characterBaseShooter.FireArrow();
        }

        public override bool CanUseAbility(CharacterBaseManager character)
        {
            if (!character.characterBaseWeaponsManager.HasRangeWeapon())
            {
                return false;
            }

            return true;
        }

        public override Damage GetDamage(CharacterBaseManager attacker)
        {
            Weapon leftWeapon = attacker.characterBaseWeaponsManager.GetCurrentLeftWeapon();
            if (leftWeapon != null)
            {
                Damage weaponDamage = leftWeapon.damage.Clone();
                weaponDamage.Combine(damage);
                return weaponDamage;
            }

            return damage;
        }

        public override void OnFinished(CharacterManager characterManager)
        {
            if (hasSuccessfullyEquippedRangeWeapon)
            {
                characterManager.characterWeaponsManager.EquipWeapon(previouslyEquippedLeftHandWeapon, previouslyEquippedLeftWeaponIndex, false);
            }

            characterManager.characterWeaponsManager.ShowRightWeapon();
            characterManager.characterBaseShooter.DestroyArrowPlaceholder();
        }

        public override void OnFinished(PlayerManager playerManager)
        {
        }
    }
}
