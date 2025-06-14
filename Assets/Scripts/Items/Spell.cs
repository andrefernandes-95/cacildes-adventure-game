using AF.Stats;
using EditorAttributes;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Spell / New Spell")]
    public class Spell : Item
    {
        public GameObject projectile;
        public float manaCostPerCast = 20;

        [Header("Animations")]
        public AnimationClip castAnimationOverride;
        public bool animationCanNotBeOverriden = false;

        [Header("Spell Type")]
        public bool isFaithSpell = false;
        public bool isHexSpell = false;

        [Header("Status Effects")]
        public StatusEffect[] statusEffects;
        public float effectsDurationInSeconds = 15f;

        [Header("Spawn Options")]
        public bool spawnAtPlayerFeet = false;
        public float playerFeetOffsetY = 0f;
        public bool spawnOnLockedOnEnemies = false;
        public bool ignoreSpawnFromCamera = false;
        public bool parentToPlayer = false;

        [Header("Requirements")]
        public int intelligenceRequired = 0;
        public int positiveReputationRequired = 0;
        public int negativeReputationRequired = 0;

        [Header("Actions")]
        [HelpBox("If true, will use the new action system")]
        public Ability ability;

        public string GetFormattedAppliedStatusEffects()
        {
            string result = "";

            foreach (var statusEffect in statusEffects)
            {
                if (statusEffect != null)
                {
                    result += $"{statusEffect.GetName()}\n";
                }
            }

            return result.TrimEnd();
        }

        public bool AreRequirementsMet(StatsBonusController statsBonusController)
        {
            if (intelligenceRequired != 0 && statsBonusController.GetCurrentIntelligence() < intelligenceRequired)
            {
                return false;
            }
            else if (positiveReputationRequired != 0 && statsBonusController.GetCurrentReputation() < positiveReputationRequired)
            {
                return false;
            }
            else if (negativeReputationRequired != 0 && statsBonusController.GetCurrentReputation() > -negativeReputationRequired)
            {
                return false;
            }

            return true;
        }

        public bool HasRequirements()
        {
            return intelligenceRequired != 0 || positiveReputationRequired != 0 || negativeReputationRequired != 0;
        }

        public string DrawRequirements(StatsBonusController statsBonusController)
        {
            string text = AreRequirementsMet(statsBonusController)
                ? LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Requirements met: ")
                : LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Requirements not met: ");

            if (intelligenceRequired != 0)
            {
                text += $"  {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Intelligence Required:")} {intelligenceRequired}   {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Current:")} {statsBonusController.GetCurrentIntelligence()}\n";
            }
            if (positiveReputationRequired != 0)
            {
                text += $"  {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Reputation Required:")} {intelligenceRequired}   {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Current:")} {statsBonusController.GetCurrentReputation()}\n";
            }

            if (negativeReputationRequired != 0)
            {
                text += $"  {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Reputation Required:")} -{negativeReputationRequired}   {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Current:")} {statsBonusController.GetCurrentReputation()}\n";
            }
            return text.TrimEnd();
        }

        public bool HasAbility() => ability != null;


    }
}
