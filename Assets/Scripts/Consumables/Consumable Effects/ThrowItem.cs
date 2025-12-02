using System.Linq;
using AF.Shooting;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Consumable Effect / Throw Item")]
    public class ThrowItem : ConsumableEffect
    {
        [Header("Animation")]
        [SerializeField] string startAnimation = "Throw";

        [Header("Throwable Item")]
        [SerializeField] GameObject itemToThrow;

        [Header("Item Graphics")]
        [SerializeField] GameObject itemToThrow_GraphicOnly;
        [SerializeField] Vector3 consumableLocalPosition;
        [SerializeField] Vector3 consumableLocalRotation;
        [SerializeField] Vector3 consumableLocalScale;

        GameObject itemToThrowGraphicInstance;

        [Header("Throw Options")]
        [SerializeField] float throwVelocity = 500;
        [SerializeField] ForceMode forceMode = ForceMode.Force;

        [Header("Achievements")]
        [SerializeField] Achievement throwItemAchievement;

        public override void OnStart(CharacterBaseManager characterBaseManager)
        {
            characterBaseManager.characterBaseWeaponsManager.HideEquipment();
            characterBaseManager.PlayBusyAnimationWithRootMotion(startAnimation);

            itemToThrowGraphicInstance = Instantiate(itemToThrow_GraphicOnly, characterBaseManager.characterTransformHelper.rightHand);
            itemToThrowGraphicInstance.transform.localPosition = consumableLocalPosition;
            itemToThrowGraphicInstance.transform.localRotation = Quaternion.Euler(consumableLocalRotation);
            itemToThrowGraphicInstance.transform.localScale = consumableLocalScale;
        }

        public override void OnUse(CharacterBaseManager characterBaseManager)
        {
            DestroyPlaceholderGraphic();

            GameObject instance = Instantiate(itemToThrow, characterBaseManager.characterTransformHelper.rightHand);
            instance.transform.parent = null;

            if (instance.TryGetComponent(out Projectile projectile))
            {
                Vector3 direction = characterBaseManager.transform.forward;

                if (characterBaseManager.GetTarget() != null)
                {
                    direction = characterBaseManager.GetTarget().transform.position - characterBaseManager.transform.position;
                }

                direction = direction.normalized * throwVelocity;
                direction.y = 0;

                projectile.Shoot(characterBaseManager, direction, forceMode);
            }

            if (throwItemAchievement != null)
            {
                throwItemAchievement.AwardAchievement();
            }
        }

        public override void OnEnd(CharacterBaseManager characterBaseManager)
        {
            DestroyPlaceholderGraphic();

            characterBaseManager.characterBaseWeaponsManager.ShowEquipment();
        }

        void DestroyPlaceholderGraphic()
        {
            if (itemToThrowGraphicInstance != null)
            {
                Destroy(itemToThrowGraphicInstance);
                itemToThrowGraphicInstance = null;
            }
        }
    }
}
