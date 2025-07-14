namespace AF
{
    using AF.Combat;
    using UnityEngine;

    public class UnarmedHitbox : Hitbox
    {
        public UnarmedWeapon unarmedWeapon;

        public override AudioClip GetSwingSFX()
        {
            if (unarmedWeapon.weaponSound == null)
            {
                return null;
            }

            return unarmedWeapon.weaponSound.GetSwing();
        }

        public override AudioClip GetImpactSFX()
        {
            if (unarmedWeapon.weaponSound == null)
            {
                return null;
            }

            return unarmedWeapon.weaponSound.GetImpact();
        }

        protected override void HandleCharacterAttack(IDamageable damageable)
        {
        }

        public override float GetWeaponImpactImpulse()
        {
            return unarmedWeapon.hitboxImpactImpulse;
        }
    }
}
