using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Localization;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Item / New Item")]
    public class Item : ScriptableObject
    {
        public string itemID;

        [Header("Localization")]
        public LocalizedString nameLocalized;
        public LocalizedString descriptionLocalized;
        public LocalizedString shortDescriptionLocalized;

        [Header("UI")]
        public Sprite sprite;

        [Header("Value")]
        public ItemValue itemValue;
        [System.Obsolete("Use itemValue")]
        public float value = 0;

        [Header("Weight")]
        public ItemWeight itemWeight;

        public bool isRenewable = false;
        [Tooltip("If we want to buy this item on a shop, this will override their value when trading with an NPC. E.g. Buying a boss weapon by trading a boss soul")]
        public SerializedDictionary<Item, int> tradingItemRequirements = new();
        [Range(0, 1f)] public float dropRateOnEnemies = 0.2f;

        [Header("Debug")]
        [TextAreaAttribute(minLines: 5, maxLines: 10)] public string notes;
        [TextAreaAttribute(minLines: 1, maxLines: 2)] public string location;


        public string GetName()
        {
            if (nameLocalized != null && nameLocalized.IsEmpty == false)
            {
                return nameLocalized.GetLocalizedString();
            }

            return name;
        }


        public string GetDescription()
        {
            if (descriptionLocalized != null && descriptionLocalized.IsEmpty == false)
            {
                return descriptionLocalized.GetLocalizedString();
            }

            return "";
        }

        public string GetShortDescription()
        {
            if (shortDescriptionLocalized != null && shortDescriptionLocalized.IsEmpty == false)
            {
                return shortDescriptionLocalized.GetLocalizedString();
            }

            return "";
        }

        public bool ShouldDrop() => Random.Range(0, 1f) <= dropRateOnEnemies;

        public int GetValue()
        {
            if (itemValue != null)
            {
                return itemValue.value;
            }
            return (int)value;
        }

        public float GetWeight()
        {
            if (itemWeight != null)
            {
                return itemWeight.weight;
            }
            return 0.1f;
        }

        public bool EqualsTo(Item b)
        {
            if (b == null) return false;

            return name.Replace("(Clone)", "") == b.name.Replace("(Clone)", "");
        }
    }
}
