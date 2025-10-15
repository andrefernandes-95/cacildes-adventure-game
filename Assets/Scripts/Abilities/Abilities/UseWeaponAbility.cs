using AF.Health;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(fileName = "Use Weapon Ability", menuName = "Abilities / Weapons / New Use Weapon Ability", order = 0)]
    public class UseWeaponAbility : Ability
    {
        string hashIdle = "Idle";

        public override void OnPrepare(CharacterManager characterManager)
        {
            Weapon weapon = characterManager.characterBaseWeaponsManager.GetCurrentRightWeapon();
            if (weapon != null && weapon.oh_heavyAttackAbilities.Length > 0)
            {
                Ability weaponAbility = weapon.oh_heavyAttackAbilities[Random.Range(0, weapon.oh_heavyAttackAbilities.Length)];

                if (weaponAbility != null)
                {
                    characterManager.characterAbilityManager.QueueAbility(weaponAbility);
                }
            }

            characterManager.PlayBusyAnimationWithRootMotion(hashIdle);
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
            Weapon weapon = character.characterBaseWeaponsManager.GetCurrentRightWeapon();

            if (weapon == null)
            {
                return false;
            }

            return weapon.oh_heavyAttackAbilities != null && weapon.oh_heavyAttackAbilities.Length > 0;
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

    }
}
