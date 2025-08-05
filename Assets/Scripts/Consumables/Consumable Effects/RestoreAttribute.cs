using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Consumable Effect / Restore Attribute")]
    public class RestoreAttribute : ConsumableEffect
    {
        [Header("Attribute Type")]
        [SerializeField] bool restoreHealth = false;
        [SerializeField] bool restoreMana = false;
        [SerializeField] bool restoreStamina = false;

        [Header("Restore By Points")]
        [SerializeField] bool useWholeValues = false;
        [SerializeField] int amountToRestore = 200;

        [Header("Restore By Percentage")]
        [SerializeField] bool usePercentage = false;
        [SerializeField][Range(0f, 1f)] float amountInPercentage = .8f;

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

            if (restoreHealth)
            {
                if (usePercentage)
                {
                    int points = (int)(amountInPercentage * characterBaseManager.health.GetMaxHealth());

                    characterBaseManager.health.RestoreHealth(points);
                }
                else if (useWholeValues)
                {
                    characterBaseManager.health.RestoreHealth(amountToRestore);
                }
            }

            if (restoreMana && characterBaseManager is PlayerManager playerManager)
            {
                if (usePercentage)
                {
                    playerManager.manaManager.RestoreManaPercentage(amountInPercentage);
                }
                else if (useWholeValues)
                {
                    playerManager.manaManager.RestoreManaPoints(amountToRestore);
                }
            }

            if (restoreStamina && characterBaseManager is PlayerManager playerManager2)
            {
                if (usePercentage)
                {
                    playerManager2.staminaStatManager.RestoreStaminaPercentage(amountInPercentage);
                }
                else if (useWholeValues)
                {
                    playerManager2.staminaStatManager.RestoreStaminaPoints(amountToRestore);
                }
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
