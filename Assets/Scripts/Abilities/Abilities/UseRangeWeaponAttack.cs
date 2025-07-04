using AF.Health;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(fileName = "Use Range Weapon Attack", menuName = "Abilities / Weapons / New Use Range Weapon Attack", order = 0)]
    public class UseRangeWeaponAttack : Ability
    {
        Weapon previouslyEquippedLeftHandWeapon;
        bool hasSuccessfullyEquippedRangeWeapon = false;

        public override void OnPrepare(CharacterManager characterManager)
        {
            if (!characterManager.characterAbilityManager.CanUseAbility())
            {
                return;
            }

            previouslyEquippedLeftHandWeapon = characterManager.characterWeaponsManager.GetCurrentLeftWeapon();

            characterManager.characterAbilityManager.SetCurrentAbility(this);
            characterManager.RotateTowardsTarget(characterManager.rotationSpeed * 10f);

            // Search ranged weapon
            Weapon potentialWeapon = characterManager.characterWeaponsManager.GetRangeWeapon();

            if (potentialWeapon != null)
            {
                characterManager.characterWeaponsManager.EquipWeapon(potentialWeapon, 0, false);
                hasSuccessfullyEquippedRangeWeapon = true;
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

        }

        public override bool CanUseAbility(CharacterBaseManager character)
        {
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
                characterManager.characterWeaponsManager.EquipWeapon(previouslyEquippedLeftHandWeapon, 0, false);
            }
        }

        public override void OnFinished(PlayerManager playerManager)
        {
        }
    }
}
