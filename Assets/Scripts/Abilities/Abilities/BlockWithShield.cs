using AF.Health;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(fileName = "BlockWithShield", menuName = "Abilities / Reactions / Block With Shield", order = 0)]
    public class UseShield : Ability
    {
        Weapon previouslyEquippedLeftHandWeapon;
        bool hasOverridenLeftHandEquipment = false;

        public override void OnPrepare(CharacterManager characterManager)
        {
            previouslyEquippedLeftHandWeapon = characterManager.characterWeaponsManager.GetCurrentLeftWeapon();

            characterManager.characterAbilityManager.SetCurrentAbility(this);
            characterManager.RotateTowardsTarget(characterManager.rotationSpeed * 10f);

            // Search ranged weapon 
            Weapon currentLeftWeapon = characterManager.characterWeaponsManager.GetCurrentLeftWeapon();

            if (currentLeftWeapon is not Shield)
            {
                // Try to find a shield
                Shield potentialShield = characterManager.characterWeaponsManager.FindPotentialShield();
                if (potentialShield != null)
                {
                    previouslyEquippedLeftHandWeapon = currentLeftWeapon;
                    characterManager.characterWeaponsManager.EquipWeapon(potentialShield, 0, false);
                    hasOverridenLeftHandEquipment = true;
                }
            }

            characterManager.characterBlockController.StartBlocking();
        }

        public override void OnPrepare(PlayerManager playerManager)
        {
        }

        public override void OnUse(PlayerManager playerManager)
        {
        }

        public override void OnUse(CharacterManager characterManager)
        {
        }

        public override bool CanUseAbility(CharacterBaseManager character)
        {
            return true;
        }

        public override Damage GetDamage(CharacterBaseManager attacker)
        {
            return damage;
        }

        public override void OnFinished(CharacterManager characterManager)
        {
            if (hasOverridenLeftHandEquipment)
            {
                characterManager.characterWeaponsManager.EquipWeapon(previouslyEquippedLeftHandWeapon, 0, false);
            }
        }

        public override void OnFinished(PlayerManager playerManager)
        {
        }
    }
}
