using System.Linq;
using AF.Health;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(fileName = "Switch Weapon", menuName = "Abilities / Weapons / Switch Weapon", order = 0)]
    public class SwitchWeapon : Ability
    {
        public int weaponIndex = 0;
        public bool isRightHand = true;

        public override void OnPrepare(CharacterManager characterManager)
        {
            characterManager.characterAbilityManager.SetCurrentAbility(this);
            characterManager.RotateTowardsTarget(characterManager.rotationSpeed * 10f);

            if (!isRightHand)
            {
                characterManager.characterWeaponsManager.SetIsTwoHanding(false);
            }

            characterManager.characterWeaponsManager.SwitchWeapon(weaponIndex, isRightHand);
            characterManager.PlaySwitchEquipmentAnimation();
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
        }

        public override void OnFinished(PlayerManager playerManager)
        {
        }
    }
}
