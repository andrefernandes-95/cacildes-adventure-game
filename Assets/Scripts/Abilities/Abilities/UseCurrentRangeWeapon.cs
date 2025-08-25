using AF.Health;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(fileName = "Use Current Range Weapon Attack", menuName = "Abilities / Weapons / New Use Current Range Weapon Attack", order = 0)]
    public class UseCurrentRangeWeaponAttack : Ability
    {
        [Header("Animations")]
        [SerializeField] string rangeAnimation = "Aim Idle";

        public override void OnPrepare(CharacterManager characterManager)
        {
            characterManager.characterAbilityManager.SetCurrentAbility(this);
            characterManager.RotateTowardsTarget(characterManager.rotationSpeed * 10f);

            // Search ranged weapon
            Weapon potentialWeapon = characterManager.characterWeaponsManager.GetRangeWeapon();

            if (potentialWeapon != null)
            {
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
            characterManager.characterWeaponsManager.ShowRightWeapon();
            characterManager.characterBaseShooter.DestroyArrowPlaceholder();
        }

        public override void OnFinished(PlayerManager playerManager)
        {
        }
    }
}
