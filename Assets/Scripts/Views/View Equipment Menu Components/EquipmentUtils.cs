namespace AF
{
    using System;
    using System.Linq;
    using AF.Health;
    using AF.Stats;
    using AF.StatusEffects;

    public static class EquipmentUtils
    {

        public enum AttributeType { VITALITY, ENDURANCE, DEXTERITY, STRENGTH, INTELLIGENCE, REPUTATION }
        public enum AccessoryAttributeType { HEALTH_BONUS, STAMINA_BONUS, MANA_BONUS }


        public static float GetEquipLoadFromItem(Item itemToEquip, float currentWeightPenalty, EquipmentDatabase equipmentDatabase)
        {
            // Define a function to retrieve the current speed penalty from an equipped item.
            Func<Item, float> GetSpeedPenalty = (item) =>
            {
                if (item == null)
                    return 0;

                if (item is Accessory accessory)
                    return accessory.speedPenalty;

                if (item is Weapon weapon)
                    return weapon.speedPenalty;

                if (item is ArmorBase armor)
                    return armor.speedPenalty;

                return 0;
            };

            // Adjust the weight penalty by the currently equipped item based on type.
            switch (itemToEquip)
            {
                case Shield shield:
                    currentWeightPenalty -= GetSpeedPenalty(equipmentDatabase.GetCurrentLeftWeapon());
                    return Math.Max(0, currentWeightPenalty) + shield.speedPenalty;

                case Weapon weapon:
                    currentWeightPenalty -= GetSpeedPenalty(equipmentDatabase.GetCurrentWeapon());
                    return Math.Max(0, currentWeightPenalty) + weapon.speedPenalty;

                case Helmet helmet:
                    currentWeightPenalty -= GetSpeedPenalty(equipmentDatabase.helmet);
                    return Math.Max(0, currentWeightPenalty) + helmet.speedPenalty;

                case Armor armor:
                    currentWeightPenalty -= GetSpeedPenalty(equipmentDatabase.armor);
                    return Math.Max(0, currentWeightPenalty) + armor.speedPenalty;

                case Gauntlet gauntlet:
                    currentWeightPenalty -= GetSpeedPenalty(equipmentDatabase.gauntlet);
                    return Math.Max(0, currentWeightPenalty) + gauntlet.speedPenalty;

                case Legwear legwear:
                    currentWeightPenalty -= GetSpeedPenalty(equipmentDatabase.legwear);
                    return Math.Max(0, currentWeightPenalty) + legwear.speedPenalty;

                case Accessory accessory:
                    // Sum speed penalties of all equipped accessories.
                    currentWeightPenalty -= equipmentDatabase.accessories.Sum(GetSpeedPenalty);
                    return Math.Max(0, currentWeightPenalty) + accessory.speedPenalty;

                default:
                    return 0f;
            }
        }

        public static int GetAttributeFromEquipment(ArmorBase armorBase, AttributeType attributeType, CharacterBaseManager characterBaseManager, EquipmentDatabase equipmentDatabase)
        {
            // Get current value based on attribute type
            int currentValue = attributeType switch
            {
                AttributeType.VITALITY => characterBaseManager.characterBaseStats.GetVitality(),
                AttributeType.ENDURANCE => characterBaseManager.characterBaseStats.GetEndurance(),
                AttributeType.STRENGTH => characterBaseManager.characterBaseStats.GetStrength(),
                AttributeType.DEXTERITY => characterBaseManager.characterBaseStats.GetDexterity(),
                AttributeType.INTELLIGENCE => characterBaseManager.characterBaseStats.GetIntelligence(),
                AttributeType.REPUTATION => characterBaseManager.characterBaseStats.GetReputation(),
                _ => 0 // Fallback for safety
            };

            // Determine bonus from the armor base and currently equipped item
            int bonusFromEquipment = 0;
            int valueFromCurrentEquipment = 0;

            // Retrieve bonus from armorBase
            if (!equipmentDatabase.IsEquipped(armorBase))
            {
                bonusFromEquipment = attributeType switch
                {
                    AttributeType.VITALITY => armorBase.vitalityBonus,
                    AttributeType.ENDURANCE => armorBase.enduranceBonus,
                    AttributeType.STRENGTH => armorBase.strengthBonus,
                    AttributeType.DEXTERITY => armorBase.dexterityBonus,
                    AttributeType.INTELLIGENCE => armorBase.intelligenceBonus,
                    AttributeType.REPUTATION => armorBase.reputationBonus,
                    _ => 0 // Fallback for safety
                };
            }

            // Check currently equipped items
            if (armorBase is Helmet && equipmentDatabase.helmet != null)
            {
                valueFromCurrentEquipment = equipmentDatabase.helmet switch
                {
                    Helmet equippedHelmet => attributeType switch
                    {
                        AttributeType.VITALITY => equippedHelmet.vitalityBonus,
                        AttributeType.ENDURANCE => equippedHelmet.enduranceBonus,
                        AttributeType.STRENGTH => equippedHelmet.strengthBonus,
                        AttributeType.DEXTERITY => equippedHelmet.dexterityBonus,
                        AttributeType.INTELLIGENCE => equippedHelmet.intelligenceBonus,
                        AttributeType.REPUTATION => equippedHelmet.reputationBonus,
                        _ => 0
                    },
                    _ => 0
                };
            }
            else if (armorBase is Armor && equipmentDatabase.armor != null)
            {
                valueFromCurrentEquipment = equipmentDatabase.armor switch
                {
                    Armor equippedArmor => attributeType switch
                    {
                        AttributeType.VITALITY => equippedArmor.vitalityBonus,
                        AttributeType.ENDURANCE => equippedArmor.enduranceBonus,
                        AttributeType.STRENGTH => equippedArmor.strengthBonus,
                        AttributeType.DEXTERITY => equippedArmor.dexterityBonus,
                        AttributeType.INTELLIGENCE => equippedArmor.intelligenceBonus,
                        AttributeType.REPUTATION => equippedArmor.reputationBonus,
                        _ => 0
                    },
                    _ => 0
                };
            }
            else if (armorBase is Gauntlet && equipmentDatabase.gauntlet != null)
            {
                valueFromCurrentEquipment = equipmentDatabase.gauntlet switch
                {
                    Gauntlet equippedGauntlet => attributeType switch
                    {
                        AttributeType.VITALITY => equippedGauntlet.vitalityBonus,
                        AttributeType.ENDURANCE => equippedGauntlet.enduranceBonus,
                        AttributeType.STRENGTH => equippedGauntlet.strengthBonus,
                        AttributeType.DEXTERITY => equippedGauntlet.dexterityBonus,
                        AttributeType.INTELLIGENCE => equippedGauntlet.intelligenceBonus,
                        AttributeType.REPUTATION => equippedGauntlet.reputationBonus,
                        _ => 0
                    },
                    _ => 0
                };
            }
            else if (armorBase is Legwear && equipmentDatabase.legwear != null)
            {
                valueFromCurrentEquipment = equipmentDatabase.legwear switch
                {
                    Legwear equippedLegwear => attributeType switch
                    {
                        AttributeType.VITALITY => equippedLegwear.vitalityBonus,
                        AttributeType.ENDURANCE => equippedLegwear.enduranceBonus,
                        AttributeType.STRENGTH => equippedLegwear.strengthBonus,
                        AttributeType.DEXTERITY => equippedLegwear.dexterityBonus,
                        AttributeType.INTELLIGENCE => equippedLegwear.intelligenceBonus,
                        AttributeType.REPUTATION => equippedLegwear.reputationBonus,
                        _ => 0
                    },
                    _ => 0
                };
            }
            else if (armorBase is Accessory)
            {
                // Loop through each accessory in the accessories collection
                foreach (var equippedAccessory in equipmentDatabase.accessories)
                {
                    // Switch based on the specific type of attribute for the accessory
                    valueFromCurrentEquipment += attributeType switch
                    {
                        AttributeType.VITALITY => equippedAccessory?.vitalityBonus ?? 0,
                        AttributeType.ENDURANCE => equippedAccessory?.enduranceBonus ?? 0,
                        AttributeType.STRENGTH => equippedAccessory?.strengthBonus ?? 0,
                        AttributeType.DEXTERITY => equippedAccessory?.dexterityBonus ?? 0,
                        AttributeType.INTELLIGENCE => equippedAccessory?.intelligenceBonus ?? 0,
                        AttributeType.REPUTATION => equippedAccessory?.reputationBonus ?? 0,
                        _ => 0
                    };
                }
            }

            // Adjust current value by the bonuses
            currentValue = Math.Max(0, currentValue - valueFromCurrentEquipment); // Ensure non-negative
            return currentValue + bonusFromEquipment;
        }

        public static int GetStatusEffectResistanceFromEquipment(
            ArmorBase itemToEquip,
            StatusEffect statusEffect,
            PlayerStatusController playerStatusController,
            EquipmentDatabase equipmentDatabase)
        {
            // Get current value based on attribute type
            int currentValue = playerStatusController.GetCurrentResistanceForStatusEffect(statusEffect);

            // Determine bonus from the armor base and currently equipped item
            int bonusFromEquipment = 0;
            int valueFromCurrentEquipment = 0;

            // Retrieve bonus from armorBase
            if (itemToEquip != null)
            {
                int equipmentResistanceToStatusEffect = GetEquipmentResistanceForStatusEffect(itemToEquip, statusEffect);

                bonusFromEquipment = equipmentResistanceToStatusEffect;
            }

            // Check currently equipped items
            if (itemToEquip is Helmet && equipmentDatabase.helmet != null)
            {
                valueFromCurrentEquipment = GetEquipmentResistanceForStatusEffect(equipmentDatabase.helmet, statusEffect);
            }
            else if (itemToEquip is Armor && equipmentDatabase.armor != null)
            {
                valueFromCurrentEquipment = GetEquipmentResistanceForStatusEffect(equipmentDatabase.armor, statusEffect);
            }
            else if (itemToEquip is Gauntlet && equipmentDatabase.gauntlet != null)
            {
                valueFromCurrentEquipment = GetEquipmentResistanceForStatusEffect(equipmentDatabase.gauntlet, statusEffect);
            }
            else if (itemToEquip is Legwear && equipmentDatabase.legwear != null)
            {
                valueFromCurrentEquipment = GetEquipmentResistanceForStatusEffect(equipmentDatabase.legwear, statusEffect);
            }

            // Adjust current value by the bonuses
            currentValue = Math.Max(0, currentValue - valueFromCurrentEquipment); // Ensure non-negative

            return currentValue + bonusFromEquipment;
        }

        static int GetEquipmentResistanceForStatusEffect(ArmorBase item, StatusEffect statusEffect)
        {
            StatusEffectEntry match = item.GetDamageAbsorbed().statusEffects
                    .FirstOrDefault(x => x.statusEffect == statusEffect);
            if (match == null)
            {
                return 0;
            }

            return (int)match.amountPerHit;
        }


        public static int GetAttributeFromAccessory(Accessory accessory, AccessoryAttributeType attributeType, PlayerManager playerManager, EquipmentDatabase equipmentDatabase)
        {
            // Get current value based on attribute type
            int currentValue = attributeType switch
            {
                AccessoryAttributeType.HEALTH_BONUS => playerManager.health.GetMaxHealth(),
                AccessoryAttributeType.STAMINA_BONUS => playerManager.staminaStatManager.GetMaxStamina(),
                AccessoryAttributeType.MANA_BONUS => playerManager.manaManager.GetMaxMana(),
                _ => 0 // Fallback for safety
            };

            // Determine bonus from the accessory and currently equipped item
            int bonusFromEquipment = 0;
            int valueFromCurrentEquipment = 0;

            // Retrieve bonus from accessory if not equipped
            if (accessory != null && !equipmentDatabase.accessories.Contains(accessory))
            {
                bonusFromEquipment = attributeType switch
                {
                    AccessoryAttributeType.HEALTH_BONUS => accessory.healthBonus,
                    AccessoryAttributeType.STAMINA_BONUS => accessory.staminaBonus,
                    AccessoryAttributeType.MANA_BONUS => accessory.magicBonus,
                    _ => 0 // Fallback for safety
                };
            }

            // Loop through each accessory in the accessories collection
            foreach (var equippedAccessory in equipmentDatabase.accessories)
            {
                // Switch based on the specific type of attribute for the accessory
                valueFromCurrentEquipment += attributeType switch
                {
                    AccessoryAttributeType.HEALTH_BONUS => equippedAccessory?.healthBonus ?? 0,
                    AccessoryAttributeType.STAMINA_BONUS => equippedAccessory?.staminaBonus ?? 0,
                    AccessoryAttributeType.MANA_BONUS => equippedAccessory?.magicBonus ?? 0,
                    _ => 0
                };
            }

            // Adjust current value by the bonuses
            currentValue = Math.Max(0, currentValue - valueFromCurrentEquipment); // Ensure non-negative
            return currentValue + bonusFromEquipment;
        }
    }
}
