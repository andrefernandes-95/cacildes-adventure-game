using AF.Combat;
using Cinemachine;
using UnityEngine;

namespace AF
{
    public class CharacterWeaponHitbox : Hitbox
    {
        [Header("Weapon")]
        [HideInInspector] public Weapon weapon;

        public override AudioClip GetSwingSFX()
        {
            if (weapon.weaponSound == null)
            {
                return null;
            }

            return weapon.weaponSound.GetSwing();
        }

        public override AudioClip GetImpactSFX()
        {
            if (weapon.weaponSound == null)
            {
                return null;
            }

            return weapon.weaponSound.GetImpact();
        }

        protected override void HandleCharacterAttack(IDamageable damageable)
        {
            if (character is PlayerManager playerManager)
            {
                playerManager.playerCombatController.HandlePlayerAttack(damageable, weapon);
            }
        }

        public override float GetWeaponImpactImpulse()
        {
            if (weapon.weaponSize == Weapon.WeaponSize.SMALL)
            {
                return 0.2f;
            }

            if (weapon.weaponSize == Weapon.WeaponSize.LARGE)
            {
                return 0.5f;
            }

            return 1f;
        }
    }
}
