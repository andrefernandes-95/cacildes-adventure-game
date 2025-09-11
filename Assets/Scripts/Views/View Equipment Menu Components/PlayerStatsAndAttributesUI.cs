
namespace AF
{
    using AF.Health;
    using AF.Stats;
    using AF.StatusEffects;
    using UnityEngine;
    using UnityEngine.Localization.Settings;
    using UnityEngine.UIElements;

    public class PlayerStatsAndAttributesUI : MonoBehaviour
    {
        [Header("Components")]
        public PlayerManager playerManager;

        [Header("UI Documents")]
        public UIDocument uIDocument;
        public VisualElement root;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;

        [HideInInspector] public bool shouldRerender = true;

        public StatusEffect poison, bleed, burnt, frostbite, paralysis, fear, curse, drowning;

        private void OnEnable()
        {
            if (shouldRerender)
            {
                shouldRerender = false;

                SetupRefs();
            }
        }

        void SetupRefs()
        {
            root = uIDocument.rootVisualElement;
        }

        public void DrawStats(Item item, bool equippingOnRightHand, int slotIndex)
        {
            root.Q<VisualElement>("PlayerName").Q<Label>().text = playerManager.gameSettings.playerName;

            root.Q<VisualElement>("Level").Q<Label>("Value").text = playerStatsDatabase.GetCurrentLevel().ToString();
            root.Q<VisualElement>("Gold").Q<Label>("Value").text = playerStatsDatabase.gold.ToString();
            SetGoldForNextLevelLabel();

            float baseEquipLoad = playerManager.statsBonusController.weightPenalty;
            float itemEquipLoad = EquipmentUtils.GetEquipLoadFromItem(item, baseEquipLoad, equipmentDatabase, slotIndex);

            var playerBaseStats = GetPlayerBaseStats();
            var itemBonusStats = GetItemBonusStats(item);

            // Setting Labels for each stat
            SetStatLabel("Vitality", playerBaseStats.vitality, itemBonusStats.vitality);
            SetStatLabel("Endurance", playerBaseStats.endurance, itemBonusStats.endurance);
            SetStatLabel("Strength", playerBaseStats.strength, itemBonusStats.strength);
            SetStatLabel("Dexterity", playerBaseStats.dexterity, itemBonusStats.dexterity);
            SetStatLabel("Intelligence", playerBaseStats.intelligence, itemBonusStats.intelligence);

            SetStatLabel("Health",
                playerManager.health.GetMaxHealth(), itemBonusStats.healthBonus, "" + (int)playerManager.health.GetCurrentHealth());
            SetStatLabel("Stamina",
                playerManager.staminaStatManager.GetMaxStamina(), itemBonusStats.staminaBonus, "" + (int)playerManager.playerStatsDatabase.currentStamina);
            SetStatLabel("Mana",
                playerManager.manaManager.GetMaxMana(), itemBonusStats.magicBonus, "" + (int)playerManager.playerStatsDatabase.currentMana);

            SetStatLabel("Reputation", playerBaseStats.reputation, itemBonusStats.reputation);

            SetWeightLoadLabel("WeightLoad", baseEquipLoad, itemEquipLoad);

            DrawAttackStats(item as Weapon, equippingOnRightHand);


            // Base defenses (without equipment)
            int basePhysDEF = playerManager.characterBaseDefenseManager.BaseDamageAbsorbed.physical;
            int baseFireDEF = playerManager.characterBaseDefenseManager.BaseDamageAbsorbed.fire;
            int baseFrostDEF = playerManager.characterBaseDefenseManager.BaseDamageAbsorbed.frost;
            int baseMagicDEF = playerManager.characterBaseDefenseManager.BaseDamageAbsorbed.magic;
            int baseLightDEF = playerManager.characterBaseDefenseManager.BaseDamageAbsorbed.lightning;
            int baseDarkDEF = playerManager.characterBaseDefenseManager.BaseDamageAbsorbed.darkness;
            int baseWaterDEF = playerManager.characterBaseDefenseManager.BaseDamageAbsorbed.water;

            // Item defenses combined with base defenses
            int itemPhysDEF = -1;
            int itemFireDEF = -1;
            int itemFrostDEF = -1;
            int itemMagicDEF = -1;
            int itemLightDEF = -1;
            int itemDarkDEF = -1;
            int itemWaterDEF = -1;

            int currentPoise = playerManager.characterBaseDefenseManager.CurrentDamageAbsorbed.poiseDamage;
            int basePoise = playerManager.characterBaseDefenseManager.BaseDamageAbsorbed.poiseDamage;
            int itemPoise = -1;

            int currentPosture = playerManager.characterBaseDefenseManager.CurrentDamageAbsorbed.postureDamage;
            int basePosture = playerManager.characterBaseDefenseManager.BaseDamageAbsorbed.postureDamage;
            int itemPosture = -1;

            if (item is ArmorBase armorBase)
            {
                Damage damageFromArmor = armorBase.GetDamageAbsorbed();
                itemPhysDEF = damageFromArmor.physical + basePhysDEF;
                itemFireDEF = damageFromArmor.fire + baseFireDEF;
                itemFrostDEF = damageFromArmor.frost + baseFrostDEF;
                itemMagicDEF = damageFromArmor.magic + baseMagicDEF;
                itemLightDEF = damageFromArmor.lightning + baseLightDEF;
                itemDarkDEF = damageFromArmor.darkness + baseDarkDEF;
                itemWaterDEF = damageFromArmor.water + baseWaterDEF;

                itemPoise = damageFromArmor.poiseDamage + basePoise;
                itemPosture = damageFromArmor.postureDamage + basePosture;
            }

            // Current defenses
            int currentPhysDEF = playerManager.characterBaseDefenseManager.CurrentDamageAbsorbed.physical;
            int currentFireDEF = playerManager.characterBaseDefenseManager.CurrentDamageAbsorbed.fire;
            int currentFrostDEF = playerManager.characterBaseDefenseManager.CurrentDamageAbsorbed.frost;
            int currentMagicDEF = playerManager.characterBaseDefenseManager.CurrentDamageAbsorbed.magic;
            int currentLightDEF = playerManager.characterBaseDefenseManager.CurrentDamageAbsorbed.lightning;
            int currentDarkDEF = playerManager.characterBaseDefenseManager.CurrentDamageAbsorbed.darkness;
            int currentWaterDEF = playerManager.characterBaseDefenseManager.CurrentDamageAbsorbed.water;

            SetStatLabel("PhysicalDefense", currentPhysDEF, itemPhysDEF);
            SetStatLabel("FireDefense", currentFireDEF, itemFireDEF);
            SetStatLabel("FrostDefense", currentFrostDEF, itemFrostDEF);
            SetStatLabel("LightningDefense", currentLightDEF, itemLightDEF);
            SetStatLabel("MagicDefense", currentMagicDEF, itemMagicDEF);
            SetStatLabel("DarknessDefense", currentDarkDEF, itemDarkDEF);
            SetStatLabel("WaterDefense", currentWaterDEF, itemWaterDEF);

            SetStatLabel("Poise", currentPoise, itemPoise);
            SetStatLabel("Posture", currentPosture, itemPosture);

            DrawStatusEffectLabel("Poison", poison, item);
            DrawStatusEffectLabel("Bleed", bleed, item);
            DrawStatusEffectLabel("Burnt", burnt, item);
            DrawStatusEffectLabel("Frostbite", frostbite, item);
            DrawStatusEffectLabel("Paralysis", paralysis, item);
            DrawStatusEffectLabel("Fear", fear, item);
            DrawStatusEffectLabel("Curse", curse, item);
            DrawStatusEffectLabel("Drowning", drowning, item);
        }

        void DrawAttackStats(Weapon weapon, bool equippingOnRightHand)
        {
            var newDamage = weapon != null ? playerManager.characterBaseAttackManager.CalculateWeaponDamageForWeapon(weapon).weaponDamage : new Damage();
            var rightDamage = playerManager.characterBaseAttackManager.rightWeaponCurrentDamage;
            var leftDamage = playerManager.characterBaseAttackManager.leftWeaponCurrentDamage;

            var statNames = new[] { "PhysicalAttack", "FireAttack", "FrostAttack", "LightningAttack" };

            foreach (var stat in statNames)
            {
                int rightCurrent = GetStatValue(rightDamage, stat);
                int leftCurrent = GetStatValue(leftDamage, stat);
                int newValue = GetStatValue(newDamage, stat);

                SetStatLabel($"Right{stat}", rightCurrent, equippingOnRightHand ? newValue : rightCurrent);
                SetStatLabel($"Left{stat}", leftCurrent, !equippingOnRightHand ? newValue : leftCurrent);
            }
        }

        int GetStatValue(Damage damage, string statName)
        {
            return statName switch
            {
                "PhysicalAttack" => damage.physical,
                "FireAttack" => damage.fire,
                "FrostAttack" => damage.frost,
                "LightningAttack" => damage.lightning,
                "MagicAttack" => damage.magic,
                "DarknessAttack" => damage.darkness,
                "WaterAttack" => damage.water,
                _ => 0
            };
        }

        void DrawStatusEffectLabel(string elementName, StatusEffect statusEffect, Item item)
        {
            PlayerStatusController playerStatusController = playerManager.statusController as PlayerStatusController;

            SetStatLabel(
                elementName,
                playerStatusController.GetCurrentResistanceForStatusEffect(statusEffect),
                item != null
                    ? EquipmentUtils.GetStatusEffectResistanceFromEquipment(item as ArmorBase, statusEffect, playerStatusController, equipmentDatabase)
                    : 0);
        }

        private void SetStatLabel(string elementName, int baseValue, int itemValue, string currentValue = "")
        {
            string label = (!string.IsNullOrEmpty(currentValue) ?
                (currentValue + "/")
                : "") + baseValue.ToString();

            Label changeIndicator =
                  root.Q<VisualElement>(elementName).Q<Label>("ChangeIndicator");
            changeIndicator.style.display = DisplayStyle.None;

            if (itemValue > 0 && itemValue != baseValue)
            {
                if (itemValue > baseValue)
                {
                    changeIndicator.style.color = Color.green;
                }
                else if (itemValue < baseValue)
                {
                    changeIndicator.style.color = Color.red;
                }

                changeIndicator.text = " > " + itemValue;
                changeIndicator.style.display = DisplayStyle.Flex;
            }

            root.Q<VisualElement>(elementName).Q<Label>("Value").text = label;
        }

        void SetGoldForNextLevelLabel()
        {
            root.Q<VisualElement>("GoldForNextLevel").Q<Label>("Value").text =
                playerManager.playerLevelManager.GetRequiredExperienceForNextLevel().ToString();
        }

        float GetStrengthWeightLoadBonus()
        {
            float bonus = playerManager.characterBaseStats.GetStrength();

            bonus *= 0.0025f;

            if (bonus > 0f)
            {
                return bonus;
            }

            return 0f;
        }

        public float GetHeavyWeightThreshold()
        {

            return 0.135f + GetStrengthWeightLoadBonus();
        }
        public float GetMidWeightThreshold()
        {

            return 0.05f + GetStrengthWeightLoadBonus();
        }

        bool IsLightWeightForGivenValue(float givenWeightPenalty)
        {
            return playerManager.characterBaseWeight.WillMidroll((int)givenWeightPenalty) == false && playerManager.characterBaseWeight.WillHeavyroll((int)givenWeightPenalty) == false;
        }

        bool IsMidWeightForGivenValue(float givenWeightPenalty)
        {
            return playerManager.characterBaseWeight.WillMidroll((int)givenWeightPenalty) == true && playerManager.characterBaseWeight.WillHeavyroll((int)givenWeightPenalty) == false;
        }

        bool IsHeavyWeightForGivenValue(float givenWeightPenalty)
        {
            return playerManager.characterBaseWeight.WillHeavyroll((int)givenWeightPenalty) == true;
        }

        string GetWeightLoadLabel(float givenWeightLoad)
        {
            if (IsLightWeightForGivenValue(givenWeightLoad))
            {
                return LocalizationSettings.SelectedLocale.Identifier.Code == "en" ? "Light Load" : "Leve";
            }
            if (IsMidWeightForGivenValue(givenWeightLoad))
            {
                return LocalizationSettings.SelectedLocale.Identifier.Code == "en" ? "Medium Load" : "Médio";
            }
            if (IsHeavyWeightForGivenValue(givenWeightLoad))
            {
                return LocalizationSettings.SelectedLocale.Identifier.Code == "en" ? "Heavy Load" : "Pesado";
            }

            return "";
        }

        private void SetWeightLoadLabel(string elementName, float baseValue, float itemValue)
        {
            // Format baseValue and itemValue as percentages with two decimal places
            string formattedBaseValue = baseValue + "";
            string formattedItemValue = itemValue + "";

            string label = formattedBaseValue + $" ({GetWeightLoadLabel(baseValue)})";

            Label changeIndicator =
                  root.Q<VisualElement>(elementName).Q<Label>("ChangeIndicator");
            changeIndicator.style.display = DisplayStyle.None;

            if (itemValue > 0 && itemValue != baseValue)
            {
                if (itemValue < baseValue)
                {
                    changeIndicator.style.color = Color.green;
                }
                else if (itemValue > baseValue)
                {
                    changeIndicator.style.color = Color.red;
                }

                changeIndicator.text = " > " + formattedItemValue + $" ({GetWeightLoadLabel(itemValue)})";
                changeIndicator.style.display = DisplayStyle.Flex;
            }

            root.Q<VisualElement>(elementName).Q<Label>("Value").text = label;
        }

        private (int vitality, int endurance, int strength, int dexterity, int intelligence, int reputation) GetPlayerBaseStats()
        {
            return (
                playerManager.playerStats.GetVitality(),
                playerManager.playerStats.GetEndurance(),
                playerManager.playerStats.GetStrength(),
                playerManager.playerStats.GetDexterity(),
                playerManager.playerStats.GetIntelligence(),
                playerManager.playerStats.GetReputation()
            );
        }

        private (int vitality, int endurance, int strength, int dexterity, int intelligence, int reputation,
        int healthBonus, int staminaBonus, int magicBonus) GetItemBonusStats(Item item)
        {
            if (item is ArmorBase armor)
            {
                return (
                    EquipmentUtils.GetAttributeFromEquipment(armor, EquipmentUtils.AttributeType.VITALITY, playerManager, equipmentDatabase),
                    EquipmentUtils.GetAttributeFromEquipment(armor, EquipmentUtils.AttributeType.ENDURANCE, playerManager, equipmentDatabase),
                    EquipmentUtils.GetAttributeFromEquipment(armor, EquipmentUtils.AttributeType.STRENGTH, playerManager, equipmentDatabase),
                    EquipmentUtils.GetAttributeFromEquipment(armor, EquipmentUtils.AttributeType.DEXTERITY, playerManager, equipmentDatabase),
                    EquipmentUtils.GetAttributeFromEquipment(armor, EquipmentUtils.AttributeType.INTELLIGENCE, playerManager, equipmentDatabase),
                    EquipmentUtils.GetAttributeFromEquipment(armor, EquipmentUtils.AttributeType.REPUTATION, playerManager, equipmentDatabase),
                    EquipmentUtils.GetAttributeFromAccessory(armor as Accessory, EquipmentUtils.AccessoryAttributeType.HEALTH_BONUS, playerManager, equipmentDatabase),
                    EquipmentUtils.GetAttributeFromAccessory(armor as Accessory, EquipmentUtils.AccessoryAttributeType.STAMINA_BONUS, playerManager, equipmentDatabase),
                    EquipmentUtils.GetAttributeFromAccessory(armor as Accessory, EquipmentUtils.AccessoryAttributeType.MANA_BONUS, playerManager, equipmentDatabase)
                );
            }
            return (0, 0, 0, 0, 0, 0, 0, 0, 0);
        }

    }
}
