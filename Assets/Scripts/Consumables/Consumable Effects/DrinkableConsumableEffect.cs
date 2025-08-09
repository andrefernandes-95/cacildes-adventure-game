using UnityEngine;

namespace AF
{
    public class DrinkableConsumableEffect : ConsumableEffect
    {
        [Header("Animation")]
        [SerializeField] string startAnimation = "Drinking";

        [Header("Item Graphics")]
        [SerializeField] GameObject consumableGraphic;
        [SerializeField] Vector3 consumableLocalPosition;
        [SerializeField] Vector3 consumableLocalRotation;

        [Header("VFX")]
        [SerializeField] GameObject healingVfxPrefab;

        GameObject potionPrefabInstance;
        GameObject healingVfxPrefabInstance;

        public override void OnStart(CharacterBaseManager characterBaseManager)
        {
            characterBaseManager.characterBaseWeaponsManager.HideEquipment();

            potionPrefabInstance = Instantiate(consumableGraphic, characterBaseManager.characterTransformHelper.rightHand);
            potionPrefabInstance.transform.localPosition = consumableLocalPosition;
            potionPrefabInstance.transform.localRotation = Quaternion.Euler(consumableLocalRotation);

            characterBaseManager.PlayBusyAnimationWithRootMotion(startAnimation);
        }

        public override void OnUse(CharacterBaseManager characterBaseManager)
        {
            if (healingVfxPrefab != null)
            {
                healingVfxPrefabInstance = Instantiate(healingVfxPrefab, characterBaseManager.transform);
                healingVfxPrefabInstance.transform.parent = null;
            }
        }

        public override void OnEnd(CharacterBaseManager characterBaseManager)
        {
            characterBaseManager.characterBaseWeaponsManager.ShowEquipment();

            if (potionPrefabInstance != null)
            {
                Destroy(potionPrefabInstance);
            }

            if (healingVfxPrefabInstance != null)
            {
                Destroy(healingVfxPrefabInstance, 5f);
            }
        }
    }
}
