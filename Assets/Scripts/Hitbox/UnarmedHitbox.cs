namespace AF
{
    using AF.Combat;
    using AF.Health;
    using UnityEngine;

    public class UnarmedHitbox : Hitbox
    {
        public Damage damage;

        [Header("Sounds")]
        public WeaponSound weaponSound;

        [Header("Camera Shake Impact Force")]
        public float hitboxImpactImpulse = 0.2f;

        public override AudioClip GetSwingSFX()
        {
            if (weaponSound == null)
            {
                return null;
            }

            return weaponSound.GetSwing();
        }

        public override AudioClip GetImpactSFX()
        {
            if (weaponSound == null)
            {
                return null;
            }

            return weaponSound.GetImpact();
        }

        protected override void HandleCharacterAttack(IDamageable damageable)
        {
        }

        public override float GetWeaponImpactImpulse()
        {
            return hitboxImpactImpulse;
        }
    }
}
