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

        protected void ShowArrowPlaceholder()
        {
            DestroyArrowPlaceholder();

            Arrow currentArrow = GetCurrentArrow();
            if (currentArrow != null)
            {
                arrowPlaceholder = Instantiate(currentArrow.arrowPlaceholderPrefab, characterBaseManager.characterTransformHelper.rightHand);

                characterBaseManager.combatAudioSource.PlayOneShot(currentArrow.drawArrowSfx);
            }
        }

        public abstract Arrow GetCurrentArrow();

        protected void DestroyArrowPlaceholder()
        {
            if (arrowPlaceholder != null)
            {
                Destroy(arrowPlaceholder);
                arrowPlaceholder = null;
            }
        }
    }
}
