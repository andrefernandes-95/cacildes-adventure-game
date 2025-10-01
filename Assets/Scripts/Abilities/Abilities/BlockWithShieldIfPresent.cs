using AF.Health;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(fileName = "BlockWithShield", menuName = "Abilities / Reactions / Block With Shield If Present", order = 0)]
    public class BlockWithShieldIfPresent : Ability
    {
        public override void OnPrepare(CharacterManager characterManager)
        {
            characterManager.characterAbilityManager.SetCurrentAbility(this);
            characterManager.RotateTowardsTarget(characterManager.rotationSpeed * 10f);

            Weapon currentLeftWeapon = characterManager.characterWeaponsManager.GetCurrentLeftWeapon();
            if (currentLeftWeapon is Shield)
            {
                characterManager.characterBlockController.StartBlocking();
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
        }

        public override bool CanUseAbility(CharacterBaseManager character)
        {
            return character.characterBaseWeaponsManager.GetCurrentLeftWeapon() is Shield;
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
