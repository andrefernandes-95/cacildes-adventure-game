using AF.Stats;
using EditorAttributes;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Spell / New Spell")]
    public class Spell : UpgradableItem
    {
        public SpellType spellType;

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

        public bool AreRequirementsMet(CharacterBaseManager characterBaseManager)
        {
            if (spellType != null && spellType.intelligenceRequired != 0 && characterBaseManager.characterBaseStats.GetIntelligence() < spellType.intelligenceRequired)
            {
                return false;
            }
            else if (positiveReputationRequired != 0 && characterBaseManager.characterBaseStats.GetReputation() < positiveReputationRequired)
            {
                return false;
            }
            else if (negativeReputationRequired != 0 && characterBaseManager.characterBaseStats.GetReputation() > -negativeReputationRequired)
            {
                return false;
            }

            return true;
        }

        public bool HasRequirements()
        {
            if (spellType == null)
            {
                return false;
            }

            return spellType.intelligenceRequired != 0 || positiveReputationRequired != 0 || negativeReputationRequired != 0;
        }

        public string DrawRequirements(CharacterBaseManager characterBaseManager)
        {
            string text = AreRequirementsMet(characterBaseManager)
                ? LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Requirements met: ")
                : LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Requirements not met: ");

            if (spellType.intelligenceRequired != 0)
            {
                text += $"  {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Intelligence Required:")} {spellType.intelligenceRequired}   {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Current:")} {characterBaseManager.characterBaseStats.GetIntelligence()}\n";
            }
            if (positiveReputationRequired != 0)
            {
                text += $"  {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Reputation Required:")} {spellType.intelligenceRequired}   {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Current:")} {characterBaseManager.characterBaseStats.GetReputation()}\n";
            }

            if (negativeReputationRequired != 0)
            {
                text += $"  {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Reputation Required:")} -{negativeReputationRequired}   {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Current:")} {characterBaseManager.characterBaseStats.GetReputation()}\n";
            }
            return text.TrimEnd();
        }

        public bool HasAbility() => ability != null;


    }
}
