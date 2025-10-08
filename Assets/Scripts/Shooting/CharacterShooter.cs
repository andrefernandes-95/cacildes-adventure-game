using UnityEngine;
using UnityEngine.Events;

namespace AF.Shooting
{
    public class CharacterShooter : CharacterBaseShooter
    {
        public UnityEvent onShoot;

        [Header("Available Arrows")]
        public Arrow[] arrows;

        CharacterManager GetCharacterManager()
        {
            return characterBaseManager as CharacterManager;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public override void FireArrow()
        {
            FireProjectile();
        }


        void FireProjectile()
        {
            if (arrows == null || GetCurrentArrow() == null)
            {
                return;
            }

            GameObject projectile = GetCurrentArrow().arrowProjectile.gameObject;

            CharacterWeaponHitbox currentRangeWeapon = characterBaseManager.characterBaseWeaponsManager.currentShieldInstance;

            Transform origin = characterBaseManager.characterTransformHelper.rightHand.transform;

            if (currentRangeWeapon != null
                && currentRangeWeapon.weapon != null
                && currentRangeWeapon.weapon.damage.weaponAttackType == WeaponAttackType.Range
                && currentRangeWeapon.TryGetComponent<RangeWeaponProjectileRef>(out var rangeWeaponProjectileRef)
                )
            {
                origin = rangeWeaponProjectileRef.shootingRef;
            }

            GameObject projectileInstance = Instantiate(projectile.gameObject, origin.position, Quaternion.identity);
            projectileInstance.TryGetComponent<Projectile>(out var projectileInstanceComponent);
            if (projectileInstanceComponent != null)
            {
                projectileInstanceComponent.shooter = this.characterBaseManager;
            }

            if (projectileInstance == null)
            {
                return;
            }

            projectileInstance.TryGetComponent(out IProjectile componentProjectile);
            if (componentProjectile == null)
            {
                return;
            }

            if (GetCharacterManager().targetManager.currentTarget != null)
            {
                Transform target = GetCharacterManager().targetManager.currentTarget.transform;
                var rot = target.position + target.up - origin.position;
                rot.y = 0;
                projectileInstance.transform.rotation = Quaternion.LookRotation(rot);
                characterBaseManager.transform.rotation = Quaternion.LookRotation(rot);
            }

            componentProjectile.Shoot(characterBaseManager, projectileInstance.transform.forward * componentProjectile.GetForwardVelocity(), componentProjectile.GetForceMode());

            onShoot?.Invoke();

            DestroyArrowPlaceholder();
        }

        public override bool CanShoot()
        {
            return true;
        }

        public override Arrow GetCurrentArrow()
        {
            if (arrows == null || arrows.Length <= 0)
            {
                return null;
            }

            return arrows[0];
        }
    }

}
