using UnityEngine;
using UnityEngine.Localization;

namespace AF
{
    [CreateAssetMenu(menuName = "Data / Status Effect / New Status")]
    [System.Serializable]
    public class StatusEffect : ScriptableObject
    {
        public LocalizedString displayName;
        public LocalizedString displayNameWhenApplied;

        public string builtUpName;
        public string appliedName;

        [Header("UI")]
        public Sprite icon;
        public Color barColor;

        [Header("Options")]
        public bool isAppliedImmediately = false;

        [Header("Decay Rate")]
        public float decreaseRateWithDamage = 1f;
        public float decreaseRateWithoutDamage = 5f;

        [Header("Resistances")]
        public float fallbackResistance = 25f;

        [Header("Behaviour")]
        public StatusEffectBehaviour[] statusEffectBehaviours;

        public string GetName()
        {
            if (displayName == null || displayName.IsEmpty)
            {
                return builtUpName;
            }

            return displayName.GetLocalizedString();
        }

        public string GetAppliedName()
        {
            if (displayNameWhenApplied == null || displayNameWhenApplied.IsEmpty)
            {
                return appliedName;
            }

            return displayNameWhenApplied.GetLocalizedString();
        }

    }
}