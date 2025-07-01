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
            return weaponSound.swing;
        }

        public override AudioClip GetImpactSFX()
        {
            return weaponSound.impact;
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
