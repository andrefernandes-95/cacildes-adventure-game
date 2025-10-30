using UnityEngine;

namespace AF.Shooting
{
    public abstract class CharacterBaseShooter : MonoBehaviour
    {
        public GameObject rifleWeapon;

        public readonly int hashFireBow = Animator.StringToHash("Shoot");
        public readonly int hashCast = Animator.StringToHash("Cast");
        public readonly int hashIsAiming = Animator.StringToHash("IsAiming");

        [Header("Components")]
        public CharacterBaseManager characterBaseManager;

        protected GameObject arrowPlaceholder;

        public abstract bool CanShoot();

        public abstract void FireArrow();

        public void ShowRifleWeapon()
        {
            if (rifleWeapon == null)
            {
                return;
            }
            rifleWeapon.gameObject.SetActive(true);
        }

        public void HideRifleWeapon()
        {
            if (rifleWeapon == null)
            {
                return;
            }
            rifleWeapon.gameObject.SetActive(false);
        }

        public void ShowArrowPlaceholder()
        {
            DestroyArrowPlaceholder();

            CharacterWeaponHitbox currentRangeWeapon = characterBaseManager.characterBaseWeaponsManager.currentShieldInstance;
            Arrow currentArrow = GetCurrentArrow();
            if (currentArrow != null
                && currentArrow.arrowPlaceholderPrefab != null
                && currentRangeWeapon != null
                && currentRangeWeapon.weapon != null
                && currentRangeWeapon.weapon.damage.weaponAttackType == WeaponAttackType.Range
                && currentRangeWeapon.TryGetComponent<RangeWeaponProjectileRef>(out var rangeWeaponProjectileRef)
                )
            {
                arrowPlaceholder = Instantiate(currentArrow.arrowPlaceholderPrefab, rangeWeaponProjectileRef.arrowProjectileRef.transform);

                characterBaseManager.combatAudioSource.PlayOneShot(currentArrow.drawArrowSfx);
            }
        }

        public abstract Arrow GetCurrentArrow();

        public void DestroyArrowPlaceholder()
        {
            if (arrowPlaceholder != null)
            {
                Destroy(arrowPlaceholder);
                arrowPlaceholder = null;
            }
        }

        public abstract void OnAimStart();
        public abstract void OnAimEnd();
    }
}
