using System;
using System.Collections.Generic;
using System.Linq;
using AF.Animations;
using AF.Health;
using AF.Stats;
using AYellowpaper.SerializedCollections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Settings;
using EditorAttributes;

namespace AF
{
    public enum Scaling
    {
        S,
        A,
        B,
        C,
        D,
        E
    }

    public enum WeaponAttackType
    {
        Slash,
        Pierce,
        Blunt,
        Range,
        Staff,
    }

    public enum WeaponElementType
    {
        None,
        Fire,
        Frost,
        Lightning,
        Magic,
        Darkness,
        Water,
    }

    public enum PushForce
    {
        None = 1,
        Light = 2,
        Medium = 3,
        Large = 4,
        VeryLarge = 5,
        Colossal = 6,
    }

    [System.Serializable]
    public class WeaponUpgradeLevel
    {
        public int goldCostForUpgrade;
        public Damage newDamage;

        public SerializedDictionary<UpgradeMaterial, int> upgradeMaterials;
    }

    [CreateAssetMenu(menuName = "Items / Weapon / New Weapon")]
    public class Weapon : UpgradableItem
    {
        [Header("Attack")]
        public Damage damage;

        //        [Tooltip("How much block hit this weapon does on an enemy shield. Heavier weapons should do at least 2 or 3 hits.")]
        //        public int blockHitAmount = 1;

        //        [Header("Block Absorption")]
        //        [Range(0, 100)] public int blockAbsorption = 75;
        //        public float blockStaminaCost = 30f;

        [Header("Weapon Sounds")]
        public WeaponSound weaponSound;

        public enum WeaponSize
        {
            SMALL,
            LARGE,
            COLOSSAL
        }

        public WeaponSize weaponSize = WeaponSize.SMALL;

        [Header("World Instance")]
        public GameObject weaponPrefab;

        [Header("Requirements")]
        public int strengthRequired = 0;
        public int dexterityRequired = 0;
        public int intelligenceRequired = 0;
        public int positiveReputationRequired = 0;
        public int negativeReputationRequired = 0;

        [Header("Weapon Type")]
        public WeaponType weaponType;
        [Header("Stamina")]
        public int staminaCostPerAttack = 30;
        public int GetLightAttackStaminaCost() => staminaCostPerAttack;
        public int GetHeavyAttackStaminaCost() => staminaCostPerAttack * 2;

        [Header("Scaling")]
        public Scaling strengthScaling = Scaling.E;
        public Scaling dexterityScaling = Scaling.E;
        public Scaling intelligenceScaling = Scaling.E;
        [Header("Weapon Special Options")]
        public int manaCostToUseWeaponSpecialAttack = 0;

        [Header("Animation Overrides")]
        public List<AnimationOverride> animationOverrides;
        [Tooltip("Optional")] public List<AnimationOverride> twoHandOverrides;
        [Tooltip("Optional")] public List<AnimationOverride> blockOverrides;

        [Header("Optional - Animation Templates")]
        public WeaponAnimation weaponAnimationData;
        [Tooltip("Optional")] public WeaponAnimation aIWeaponAnimationData;
        public WeaponRangeAnimation weaponRangeAnimation;

        // TODO: This needs to be divided between right and left 
        [Tooltip("Optional")] public List<AnimationOverride> oh_weaponAnimationOverrides = new();

        [Tooltip("Optional")] public List<AnimationOverride> th_weaponAnimationOverrides = new();

        [Header("Combos")]
        public int lightAttackCombos = 2;
        public int heavyAttackCombos = 1;

        [Header("Two Hand Combos")]
        public int th_lightAttackCombos = 2;
        public int th_heavyAttackCombos = 1;


        [Header("Dual Wielding Options")]
        public bool halveDamage = false;

        [Header("Speed Penalty")]
        [Tooltip("Will be added as a negative speed to the animator when equipped")]
        public float speedPenalty = 0f;
        [Range(0.1f, 2f)] public float oneHandAttackSpeedPenalty = 1f;
        [Range(0.1f, 2f)] public float oh_HeavyAttackSpeedPenalty = 1f;
        [Range(0.1f, 2f)] public float twoHandAttackSpeedPenalty = 1f;
        [Range(0.1f, 2f)] public float th_HeavyAttackSpeedPenalty = 1f;

        [Header("Weapon Bonus")]
        public int amountOfGoldReceivedPerHit = 0;
        public bool doubleCoinsUponKillingEnemies = false;
        public bool doubleDamageDuringNightTime = false;
        public bool doubleDamageDuringDayTime = false;
        public int healthRestoredWithEachHit = 0;

        [Header("Jump Attack")]
        public float jumpAttackVelocity = -5f;

        [Header("Is Holy?")]
        public bool isHolyWeapon = false;
        public bool isHexWeapon = false;

        [Header("Range Category")]
        public ProjectileType projectileType;

        [Header("Block Options")]
        [Range(0, 1f)] public float weaponBlockAbsorption = .8f;

        [Header("Staff Options")]
        public bool shouldRegenerateMana = false;
        public bool ignoreSpellsAnimationClips = false;


        [Image("Assets/Synty/InterfaceFantasyWarriorHUD/Sprites/Icons_Map/ICON_FantasyWarrior_Map_ShopWeapons_01_Underlay.png", 64f, 64f)]
        [Header("Right Hand Settings")]
        public Vector3 rightHandPosition;
        public Vector3 rightHandRotation;
        [Image("Assets/Synty/InterfaceFantasyWarriorHUD/Sprites/Icons_Map/ICON_FantasyWarrior_Map_ShopWeapons_01_Underlay.png", 64f, 64f)]
        [Header("Left Hand Settings")]
        public Vector3 leftHandPosition;
        public Vector3 leftHandRotation;

        [Image("Assets/Graphics/Sprites/Icons/Two Handing Weapon.png", 64f, 64f)]
        [Header("Two Handing Settings")]
        public bool useTwoHandingTransform = true;
        public Vector3 twoHandingPosition;
        public Vector3 twoHandingRotation;

        [Image("Assets/Synty/InterfaceFantasyWarriorHUD/Sprites/Icons_Status/ICON_FantasyWarrior_Status_Armour_01_Clean.png", 64f, 64f)]
        [Header("Weapon Blocking Settings")]
        public bool useCustomTwoHandingBlockTransforms = false;
        public Vector3 th_BlockPosition;
        public Vector3 th_BlockRotation;

        [Image("Assets/Synty/InterfaceFantasyWarriorHUD/Sprites/Icons_Inventory/ICON_FantasyWarrior_Inventory_Bows_01_Clean.png", 64f, 64f)]
        [Header("Aiming Settings")]
        public Vector3 aimingPosition;
        public Vector3 aimingRotation;

#if UNITY_EDITOR
        private void OnEnable()
        {
            // No need to populate the list; it's serialized directly
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                // Clear the list when exiting play mode
                level = 1;
            }
        }
#endif

        public Damage CalculateValue(int currentLevel)
        {

            return this.damage;
        }

        public int GetWeaponBaseAttack()
        {
            return CalculateValue(this.level).physical;
        }
        public int GetWeaponAttack(CharacterBaseAttackManager attackStatManager)
        {
            return 0;
        }
        public int GetWeaponAttackForLevel(CharacterBaseAttackManager attackStatManager, int level)
        {
            return 0;
        }
        public int GetWeaponFireAttack(CharacterBaseAttackManager attackStatManager)
        {
            return CalculateValue(this.level).fire;
        }
        public int GetWeaponFireAttackForLevel(CharacterBaseAttackManager attackStatManager, int level)
        {
            return (int)CalculateValue(level).fire;
        }
        public int GetWeaponFrostAttack(CharacterBaseAttackManager attackStatManager)
        {
            return (int)CalculateValue(this.level).frost;
        }
        public int GetWeaponFrostAttackForLevel(CharacterBaseAttackManager attackStatManager, int level)
        {
            return (int)CalculateValue(level).frost;
        }
        public int GetWeaponLightningAttack(int playerReputation, CharacterBaseAttackManager attackStatManager)
        {
            int lightingDamage = CalculateValue(this.level).lightning;

            if (isHolyWeapon && playerReputation > 0)
            {
                lightingDamage += (int)Math.Min(100, Mathf.Pow(Mathf.Abs(playerReputation), 1.25f));
            }

            return (int)lightingDamage; ;
        }

        public int GetBaseWeaponLightningAttack()
        {
            return CalculateValue(this.level).lightning;
        }

        public int GetWeaponLightningAttackForLevel(int level, int playerReputation, CharacterBaseAttackManager attackStatManager)
        {
            int lightingDamage = CalculateValue(level).lightning;

            if (isHolyWeapon && playerReputation > 0)
            {
                lightingDamage += (int)Math.Min(100, Mathf.Pow(Mathf.Abs(playerReputation), 1.25f));
            }

            return (int)lightingDamage;
        }

        public int GetBaseWeaponDarknessAttack()
        {
            return CalculateValue(this.level).darkness;
        }
        public int GetWeaponDarknessAttack(int playerReputation, CharacterBaseAttackManager attackStatManager)
        {
            int darknessDamage = CalculateValue(this.level).darkness;

            if (isHexWeapon && playerReputation < 0)
            {
                darknessDamage += (int)Math.Min(100, Mathf.Pow(Mathf.Abs(playerReputation), 1.25f));
            }

            return (int)darknessDamage;
        }

        public int GetWeaponDarknessAttackForLevel(int level, int playerReputation, CharacterBaseAttackManager attackStatManager)
        {
            int darknessDamage = CalculateValue(level).darkness;

            if (isHexWeapon && playerReputation < 0)
            {
                darknessDamage += (int)Math.Min(100, Mathf.Pow(Mathf.Abs(playerReputation), 1.25f));
            }

            return (int)darknessDamage;
        }

        public int GetWeaponWaterAttack(CharacterBaseAttackManager attackStatManager)
        {
            return (int)CalculateValue(this.level).water;
        }

        public int GetWeaponWaterAttackForLevel(int level, CharacterBaseAttackManager attackStatManager)
        {
            return (int)CalculateValue(level).water;
        }

        public int GetWeaponBaseMagicAttack()
        {
            return (int)CalculateValue(this.level).magic;
        }
        public int GetWeaponMagicAttack(CharacterBaseAttackManager attackStatManager)
        {
            int baseMagicDamage = (int)CalculateValue(this.level).magic;

            return baseMagicDamage;
        }

        public int GetWeaponMagicAttackForLevel(int level, CharacterBaseAttackManager attackStatManager)
        {
            int baseMagicDamage = (int)CalculateValue(level).magic;


            return baseMagicDamage;
        }

        public Damage GetWeaponDamage(CharacterBaseAttackManager attackStatManager)
        {
            Damage baseDamage = CalculateValue(this.level);

            if (!AreRequirementsMet(attackStatManager.GetCharacter()))
            {
                baseDamage.Multiply(.1f);
            }

            return baseDamage;
        }

        public bool CanBeUpgradedFurther()
        {
            return canBeUpgraded && upgradeMaterialData != null && upgradeMaterialData.upgradeMaterials.Length > 0 && this.level <= upgradeMaterialData.upgradeMaterials.Length - 1;
        }

        public string GetMaterialCostForNextLevel(CharacterBaseManager characterBaseManager)
        {
            if (CanBeUpgradedFurther() && upgradeMaterialData != null && upgradeMaterialData.upgradeMaterials[this.level] != null)
            {
                int nextLevel = this.level + 1;
                string text = Utils.IsPortuguese() ? $"<size=80%>Itens necessários para melhorar arma para nível +{nextLevel}:" : $"<size=80%>Required items to upgrade weapon to level +{nextLevel}:";
                text += "\n";
                text += "<size=100%>";

                UpgradeMaterialData.UpgradeMaterialEntry upgradeData = upgradeMaterialData.upgradeMaterials[this.level];

                if (upgradeData != null)
                {
                    int amountOwned = characterBaseManager.characterBaseInventory.GetCraftingMaterialAmount(upgradeData.upgradeMaterial);
                    text += $"x{upgradeData.amount} {upgradeData.upgradeMaterial.GetName()} ";
                    text += Utils.IsPortuguese() ? $"(Possuis {amountOwned})" : $"(You Own {amountOwned})";
                    text += "\n";
                }

                return text;
            }

            return "";
        }

        public bool HasRequirements()
        {
            return strengthRequired != 0 || dexterityRequired != 0 || intelligenceRequired != 0 || positiveReputationRequired != 0 || negativeReputationRequired != 0;
        }

        public bool AreRequirementsMet(CharacterBaseManager characterBaseManager)
        {
            if (characterBaseManager.statsBonusController.ignoreWeaponRequirements)
            {
                return true;
            }

            if (strengthRequired != 0 && characterBaseManager.characterBaseStats.GetStrength() < strengthRequired)
            {
                return false;
            }
            else if (dexterityRequired != 0 && characterBaseManager.characterBaseStats.GetDexterity() < dexterityRequired)
            {
                return false;
            }
            else if (intelligenceRequired != 0 && characterBaseManager.characterBaseStats.GetIntelligence() < intelligenceRequired)
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

        public string DrawRequirements(CharacterBaseManager characterBaseManager)
        {
            bool areRequirementsMet = AreRequirementsMet(characterBaseManager);

            string text = "";
            if (areRequirementsMet)
            {
                if (Utils.IsPortuguese())
                {
                    text = "Requisitos Cumpridos:\n";
                }
                else
                {
                    text = "Requirements met:\n";
                }
            }
            else
            {
                if (Utils.IsPortuguese())
                {
                    text = "Requisitos em falta. Arma não causará dano eficiente!\n";
                }
                else
                {
                    text = "Requirements not met! Weapon won't deal efficient damage!\n";
                }
            }


            if (strengthRequired != 0)
            {
                text += $"  {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Strength Required:")} {strengthRequired}   {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Current:")} {characterBaseManager.characterBaseStats.GetStrength()}\n";
            }
            if (dexterityRequired != 0)
            {
                text += $"  {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Dexterity Required:")} {dexterityRequired}   {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Current:")} {characterBaseManager.characterBaseStats.GetDexterity()}\n";
            }
            if (intelligenceRequired != 0)
            {
                text += $"  {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Intelligence Required:")} {intelligenceRequired}   {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Current:")} {characterBaseManager.characterBaseStats.GetIntelligence()}\n";
            }
            if (positiveReputationRequired != 0)
            {
                text += $"  {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Reputation Required:")} {intelligenceRequired}   {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Current:")} {characterBaseManager.characterBaseStats.GetReputation()}\n";
            }

            if (negativeReputationRequired != 0)
            {
                text += $"  {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Reputation Required:")} -{negativeReputationRequired}   {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Current:")} {characterBaseManager.characterBaseStats.GetReputation()}\n";
            }

            return text.TrimEnd();
        }

        public bool IsCompatibleWithAmmo(Arrow arrow)
        {
            return arrow.projectileType == projectileType;
        }

        public List<AnimationOverride> GetOneHandAnimations()
        {
            if (weaponAnimationData != null)
            {
                return weaponAnimationData.GetOneHandAnimations();
            }

            return animationOverrides;
        }

        public List<AnimationOverride> GetLeftHandAnimations()
        {
            if (weaponAnimationData != null)
            {
                return weaponAnimationData.GetLeftHandAnimations();
            }

            return new();
        }
        public List<AnimationOverride> GetTwoHandAnimations()
        {
            if (weaponAnimationData != null)
            {
                return weaponAnimationData.GetTwoHandAnimations();
            }

            var animations = twoHandOverrides.ToList();
            animations.AddRange(blockOverrides);
            return animations;
        }

        public int GetCurrentPhysicalAttackForLevel(int level)
        {
            if (damage.physical <= 0)
            {
                return 0;
            }

            return damage.physical + GetBonusAttackPerLevel(level);
        }

        public int GetFireAttackForLevel(int level) => GetElementalAttackForLevel(damage.fire, level);
        public int GetFrostAttackForLevel(int level) => GetElementalAttackForLevel(damage.frost, level);
        public int GetLightningAttackForLevel(int level) => GetElementalAttackForLevel(damage.lightning, level);
        public int GetDarknessAttackForLevel(int level) => GetElementalAttackForLevel(damage.darkness, level);
        public int GetWaterAttackForLevel(int level) => GetElementalAttackForLevel(damage.water, level);
        public int GetMagicAttackForLevel(int level) => GetElementalAttackForLevel(damage.magic, level);

        int GetElementalAttackForLevel(int baseElementalDamage, int level)
        {
            if (baseElementalDamage <= 0)
            {
                return 0;
            }

            return baseElementalDamage + GetBonusAttackPerLevel(level);
        }
    }
}
