using UnityEngine;

namespace AF
{

    [CreateAssetMenu(menuName = "Consumable Effect / Consume Item Value")]
    public class ConsumeItemValue : ConsumableEffect
    {
        [Header("Animation")]
        [SerializeField] string startAnimation = "Use Item";

        [Header("VFX")]
        [SerializeField] GameObject healingVfxPrefab;

        GameObject healingVfxPrefabInstance;

        public override void OnStart(CharacterBaseManager characterBaseManager)
        {
            characterBaseManager.characterBaseWeaponsManager.HideEquipment();
            characterBaseManager.PlayBusyAnimationWithRootMotion(startAnimation);
        }

        public override void OnUse(CharacterBaseManager characterBaseManager)
        {
            if (characterBaseManager is not PlayerManager)
            {
                return;
            }

            if (healingVfxPrefab != null)
            {
                healingVfxPrefabInstance = Instantiate(healingVfxPrefab, characterBaseManager.transform);
                healingVfxPrefabInstance.transform.parent = null;
            }

            Consumable currentConsumable = characterBaseManager.characterBaseConsumableManager.CurrentConsumable;
            if (currentConsumable != null && currentConsumable.itemValue != null)
            {
                int gold = currentConsumable.itemValue.value;

                FindAnyObjectByType<UIDocumentPlayerGold>(FindObjectsInactive.Include).AddGold(gold);
            }
        }

        public override void OnEnd(CharacterBaseManager characterBaseManager)
        {
            characterBaseManager.characterBaseWeaponsManager.ShowEquipment();

            if (healingVfxPrefabInstance != null)
            {
                Destroy(healingVfxPrefabInstance, 5f);
            }
        }
    }
}
