
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
        public EquipmentGraphicsHandler equipmentGraphicsHandler;
        public DefenseStatManager defenseStatManager;

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

        public void DrawStats(Item item, bool equippingOnRightHand)
        {
            root.Q<VisualElement>("PlayerName").Q<Label>().text = playerManager.gameSettings.playerName;

            root.Q<VisualElement>("Level").Q<Label>("Value").text = playerStatsDatabase.GetCurrentLevel().ToString();
            root.Q<VisualElement>("Gold").Q<Label>("Value").text = playerStatsDatabase.gold.ToString();
            SetGoldForNextLevelLabel();

            // Physical and Elemental Defenses
            int basePhysicalDefense = (int)defenseStatManager.GetDefenseAbsorption();
            var itemDefenses = GetItemDefenses(item);

            int basePoise = playerManager.characterPoise.GetMaxPoiseHits();
            int itemPoise = EquipmentUtils.GetPoiseChangeFromItem(basePoise, equipmentDatabase, item);

            int basePosture = playerManager.characterPosture.GetMaxPostureDamage();
            int itemPosture = EquipmentUtils.GetPostureChangeFromItem(basePosture, equipmentDatabase, item);

            float baseEquipLoad = equipmentGraphicsHandler.GetEquipLoad();
            float itemEquipLoad = EquipmentUtils.GetEquipLoadFromItem(item, baseEquipLoad, equipmentDatabase);

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

            SetStatLabel("Poise", basePoise, itemPoise);
            SetStatLabel("Posture", basePosture, itemPosture);
            SetStatLabel("Reputation", playerBaseStats.reputation, itemBonusStats.reputation);

            SetWeightLoadLabel("WeightLoad", baseEquipLoad, itemEquipLoad);

            DrawAttackStats(item as Weapon, equippingOnRightHand);

            SetStatLabel("PhysicalDefense", basePhysicalDefense, itemDefenses.physical);
            SetStatLabel("FireDefense", (int)playerManager.defenseStatManager.GetFireDefense(), itemDefenses.fire);
            SetStatLabel("FrostDefense", (int)playerManager.defenseStatManager.GetFrostDefense(), itemDefenses.frost);
            SetStatLabel("LightningDefense", (int)playerManager.defenseStatManager.GetLightningDefense(), itemDefenses.lightning);
            SetStatLabel("MagicDefense", (int)playerManager.defenseStatManager.GetMagicDefense(), itemDefenses.magic);
            SetStatLabel("DarknessDefense", (int)playerManager.defenseStatManager.GetDarknessDefense(), itemDefenses.darkness);
            SetStatLabel("WaterDefense", (int)playerManager.defenseStatManager.GetWaterDefense(), itemDefenses.water);

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
                playerStatusController.GetResistanceForStatusEffect(statusEffect),
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

        private void SetWeightLoadLabel(string elementName, float baseValue, float itemValue)
        {
            // Format baseValue and itemValue as percentages with two decimal places
            string formattedBaseValue = (baseValue * 100).ToString("F2") + "%";
            string formattedItemValue = (itemValue * 100).ToString("F2") + "%";

            string label = formattedBaseValue + $" ({equipmentGraphicsHandler.GetWeightLoadLabel(baseValue)})";

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

                changeIndicator.text = " > " + formattedItemValue + $" ({equipmentGraphicsHandler.GetWeightLoadLabel(itemValue)})";
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

        private int GetItemAttack(Item item, int baseAttack)
        {
            if (item is Weapon weapon)
            {
                return playerManager.characterBaseAttackManager.CalculateWeaponDamageForWeapon(weapon).weaponDamage.GetTotalDamage();
            }
            else if (item is Accessory accessory && equipmentDatabase.IsAccessoryEquiped(accessory))
            {
                return baseAttack + accessory.physicalAttackBonus;
            }
            return 0;
        }


        private (int physical, int fire, int frost, int lightning, int magic, int darkness, int water) GetItemDefenses(Item item)
        {
            if (item is ArmorBase armorBase && !(item is Accessory acc && equipmentDatabase.IsAccessoryEquiped(acc)))
            {
                return (
                    EquipmentUtils.GetElementalDefenseFromItem(armorBase, WeaponElementType.None, defenseStatManager, equipmentDatabase),
                    EquipmentUtils.GetElementalDefenseFromItem(armorBase, WeaponElementType.Fire, defenseStatManager, equipmentDatabase),
                    EquipmentUtils.GetElementalDefenseFromItem(armorBase, WeaponElementType.Frost, defenseStatManager, equipmentDatabase),
                    EquipmentUtils.GetElementalDefenseFromItem(armorBase, WeaponElementType.Lightning, defenseStatManager, equipmentDatabase),
                    EquipmentUtils.GetElementalDefenseFromItem(armorBase, WeaponElementType.Magic, defenseStatManager, equipmentDatabase),
                    EquipmentUtils.GetElementalDefenseFromItem(armorBase, WeaponElementType.Darkness, defenseStatManager, equipmentDatabase),
                    EquipmentUtils.GetElementalDefenseFromItem(armorBase, WeaponElementType.Water, defenseStatManager, equipmentDatabase)
                );
            }
            return (0, -1, -1, -1, -1, -1, -1);
        }
    }
}
