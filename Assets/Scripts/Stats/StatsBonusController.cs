using System.Collections.Generic;
using System.Linq;
using AF.Health;
using AF.StatusEffects;
using UnityEngine;
using UnityEngine.Localization.Settings;
using static AF.ArmorBase;

namespace AF.Stats
{
    public class StatsBonusController : MonoBehaviour
    {
        [Header("Attribute Bonus")]
        public float healthBonusMultiplier = 0f;
        public float manaBonusMultiplier = 0f;
        public float staminaBonusMultiplier = 0f;
        public int healthBonus = 0;
        public int magicBonus = 0;
        public int staminaBonus = 0;
        public float staminaRegenerationBonus = 0f;
        public bool shouldRegenerateMana = false;

        [Header("Stats Bonus")]
        public int vitalityBonus = 0;
        public int enduranceBonus = 0;
        public int strengthBonus = 0;
        public int dexterityBonus = 0;
        public int intelligenceBonus = 0;
        public int vitalityBonusFromConsumable = 0;
        public int enduranceBonusFromConsumable = 0;
        public int strengthBonusFromConsumable = 0;
        public int dexterityBonusFromConsumable = 0;
        public int intelligenceBonusFromConsumable = 0;

        [Header("Elemental Defenses Bonus")]
        public int equipmentPhysicalDefenseBonus = 0;
        public int equipmentFireDefenseBonus = 0;
        public int equipmentFrostDefenseBonus = 0;
        public int equipmentLightningDefenseBonus = 0;
        public int equipmentMagicDefenseBonus = 0;
        public int equipmentDarkDefenseBonus = 0;
        public int equipmentWaterDefenseBonus = 0;

        [Header("Equipment Modifiers")]
        public float weightPenalty = 0f;
        public int equipmentPoise = 0;
        public bool ignoreWeaponRequirements = false;

        [Header("Gold & Experience")]
        public float additionalCoinMultiplier = 0f;

        [Header("Block & Parry")]
        public int parryPostureDamageBonus = 0;
        public float parryPostureWindowBonus = 0;

        [Header("Posture")]
        public int postureBonus = 0;
        public float postureDecreaseRateBonus = 0f;

        [Header("Shop Discounts")]
        public int reputationBonus = 0;
        public float discountPercentage = 0f;

        [Header("Skills & Spells")]
        public float spellDamageBonusMultiplier = 0f;

        [Header("Locomotion")]
        public int movementSpeedBonus = 0;

        [Header("Chances")]
        public bool chanceToRestoreHealthUponDeath = false;
        public bool chanceToNotLoseItemUponConsumption = false;

        [Header("Combat")]
        public float projectileMultiplierBonus = 0f;
        public bool canRage = false;
        public float backStabAngleBonus = 0f;
        public bool increaseAttackPowerWhenUnarmed = false;
        public bool increaseAttackPowerTheLowerTheReputation = false;
        public bool increaseAttackPowerWithLowerHealth = false;
        public float twoHandAttackBonusMultiplier = 0f;
        public float heavyAttackBonusMultiplier = 0f;
        public float jumpAttackBonusMultiplier = 0f;
        public float slashDamageMultiplier = 0f;
        public float pierceDamageMultiplier = 0f;
        public float bluntDamageMultiplier = 0f;
        public float footDamageMultiplier = 0f;
        public float physicalAttackBonus = 0f;

        [Header("Increase Next Attack Damage?")]
        public bool increaseNextAttackDamage = false;
        public float nextAttackMultiplierFactor = 1.3f;

        [Header("Status Controller")]
        public CharacterBaseManager character;
        public StatusController statusController;

        [Header("Databases")]
        public UIDocumentPlayerGold uIDocumentPlayerGold;
        public NotificationManager notificationManager;

        [Header("Status Effect Resistances")]
        public Dictionary<StatusEffect, float> statusEffectResistances = new();
        public Dictionary<StatusEffect, float> statusEffectDelayRates = new();


        private void Awake()
        {
            // TODO: This needs to be a character event, not global, otherwise it will run every time the player changes his equipment!

            /*
            EventManager.StartListening(EventMessages.ON_SHIELD_EQUIPMENT_CHANGED, () =>
            {
                RecalculateEquipmentBonus();
            });*/
        }

        (Weapon, Weapon) GetCurrentWeapons()
        {
            Weapon currentRightWeapon = character.characterBaseWeaponsManager.GetCurrentRightWeapon();
            Weapon currentLeftWeapon = character.characterBaseWeaponsManager.GetCurrentLeftWeapon();

            return (currentRightWeapon, currentLeftWeapon);
        }

        (Shield, Shield) GetCurrentShield()
        {
            Shield currentRightShield = character.characterBaseWeaponsManager.GetCurrentRightWeapon() as Shield;
            Shield currentLeftShield = character.characterBaseWeaponsManager.GetCurrentLeftWeapon() as Shield;

            return (currentRightShield, currentLeftShield);
        }

        Helmet GetCurrentHelmet() => character.characterBaseEquipment.GetEquippedHelmet();

        Armor GetCurrentArmor() => character.characterBaseEquipment.GetEquippedArmor();

        Gauntlet GetCurrentGauntlets() => character.characterBaseEquipment.GetEquippedGauntlet();

        Legwear GetCurrentLegwears() => character.characterBaseEquipment.GetEquippedLegwear();

        public List<Accessory> GetCurrentAccessories() => character.characterBaseEquipment.GetEquippedAccessories().ToList();

        public void RecalculateEquipmentBonus()
        {
            (Weapon currentRightWeapon, Weapon currentLeftWeapon) = GetCurrentWeapons();
            (Shield currentRightShield, Shield currentLeftShield) = GetCurrentShield();
            Helmet currentHelmet = GetCurrentHelmet();
            Armor currentArmor = GetCurrentArmor();
            Gauntlet currentGauntlet = GetCurrentGauntlets();
            Legwear currentLegwear = GetCurrentLegwears();
            List<Accessory> currentAccessories = GetCurrentAccessories();

            UpdateStatusEffectCancellationRates();
            UpdateWeightPenalty(currentRightWeapon, currentLeftWeapon, currentRightShield, currentLeftShield,
            currentHelmet, currentArmor, currentGauntlet, currentLegwear, currentAccessories);

            UpdateArmorPoise(currentHelmet, currentArmor, currentGauntlet, currentLegwear, currentAccessories);

            UpdateEquipmentPhysicalDefense(currentHelmet, currentArmor, currentGauntlet, currentLegwear, currentAccessories);
            UpdateStatusEffectResistances(currentHelmet, currentArmor, currentGauntlet, currentLegwear, currentAccessories);
            UpdateAttributes(currentRightWeapon, currentLeftWeapon, currentHelmet, currentArmor, currentGauntlet, currentLegwear, currentAccessories, currentRightShield, currentLeftShield);
            UpdateAdditionalCoinMultiplier(currentHelmet, currentArmor, currentGauntlet, currentLegwear, currentAccessories);
        }

        void UpdateStatusEffectCancellationRates()
        {
            statusEffectDelayRates.Clear();

            List<ArmorBase> equippedArmorBases = new() {
                GetCurrentHelmet(),
                GetCurrentArmor(),
                GetCurrentLegwears(),
                GetCurrentLegwears()
            };

            equippedArmorBases.AddRange(GetCurrentAccessories());

            foreach (var equippedArmor in equippedArmorBases)
            {
                if (equippedArmor == null)
                {
                    continue;
                }

                StatusEffectCancellationRate[] statusEffectCancellationRates = equippedArmor.statusEffectDelayRates;
                if (statusEffectCancellationRates.Length > 0)
                {
                    EvaluateItemResistance(statusEffectCancellationRates);
                }
            }

            (Shield rightShield, Shield leftShield) = GetCurrentShield();

            foreach (Shield shield in new List<Shield>() { leftShield, rightShield })
            {
                if (shield == null)
                {
                    continue;
                }

                StatusEffectCancellationRate[] statusEffectCancellationRates = shield.statusEffectDelayRates;
                if (statusEffectCancellationRates != null && statusEffectCancellationRates.Length > 0)
                {
                    EvaluateItemResistance(statusEffectCancellationRates);
                }
            }
        }

        void EvaluateItemResistance(StatusEffectCancellationRate[] itemStatusEffectCancellationRates)
        {
            foreach (var statusEffectCancellationRate in itemStatusEffectCancellationRates)
            {
                if (statusEffectDelayRates.ContainsKey(statusEffectCancellationRate.statusEffect))
                {
                    statusEffectDelayRates[statusEffectCancellationRate.statusEffect] += statusEffectCancellationRate.delayRate;
                }
                else
                {
                    statusEffectDelayRates.Add(statusEffectCancellationRate.statusEffect, statusEffectCancellationRate.delayRate);
                }
            }
        }

        void UpdateWeightPenalty(Weapon rightWeapon, Weapon leftWeapon, Shield rightShield, Shield leftShield,
        Helmet helmet, Armor armor, Gauntlet gauntlet, Legwear legwear, List<Accessory> accessories)
        {
            weightPenalty = 0f;

            if (rightWeapon != null)
            {
                weightPenalty += rightWeapon.speedPenalty;
            }
            if (leftWeapon != null)
            {
                weightPenalty += leftWeapon.speedPenalty;
            }
            if (rightShield != null)
            {
                weightPenalty += rightShield.speedPenalty;
            }
            if (leftShield != null)
            {
                weightPenalty += leftShield.speedPenalty;
            }
            if (helmet != null)
            {
                weightPenalty += helmet.speedPenalty;
            }
            if (armor != null)
            {
                weightPenalty += armor.speedPenalty;
            }
            if (gauntlet != null)
            {
                weightPenalty += gauntlet.speedPenalty;
            }
            if (legwear != null)
            {
                weightPenalty += legwear.speedPenalty;
            }

            weightPenalty += accessories.Sum(x => x == null ? 0 : x.speedPenalty);

            weightPenalty = Mathf.Max(0, weightPenalty); // Ensure weightPenalty is non-negative
        }

        void UpdateArmorPoise(Helmet helmet, Armor armor, Gauntlet gauntlet, Legwear legwear, List<Accessory> accessories)
        {
            equipmentPoise = 0;

            if (helmet != null)
            {
                equipmentPoise += helmet.GetDamageAbsorbed().poiseDamage;
            }
            if (armor != null)
            {
                equipmentPoise += armor.GetDamageAbsorbed().poiseDamage;
            }
            if (gauntlet != null)
            {
                equipmentPoise += gauntlet.GetDamageAbsorbed().poiseDamage;
            }
            if (legwear != null)
            {
                equipmentPoise += legwear.GetDamageAbsorbed().poiseDamage;
            }

            equipmentPoise += accessories.Sum(x => x == null ? 0 : x.GetDamageAbsorbed().poiseDamage);
        }

        void UpdateEquipmentPhysicalDefense(Helmet helmet, Armor armor, Gauntlet gauntlet, Legwear legwear, List<Accessory> accessories)
        {
            equipmentPhysicalDefenseBonus = 0;

            if (helmet != null)
            {
                equipmentPhysicalDefenseBonus += helmet.GetCurrentPhysicalDefenseForLevel(helmet.level);
            }

            if (armor != null)
            {
                equipmentPhysicalDefenseBonus += armor.GetCurrentPhysicalDefenseForLevel(armor.level);
            }

            if (gauntlet != null)
            {
                equipmentPhysicalDefenseBonus += gauntlet.GetCurrentPhysicalDefenseForLevel(gauntlet.level);
            }

            if (legwear != null)
            {
                equipmentPhysicalDefenseBonus += legwear.GetCurrentPhysicalDefenseForLevel(legwear.level);
            }

            equipmentPhysicalDefenseBonus += accessories.Sum(x => x == null ? 0 : x.GetDamageAbsorbedForCurrentLevel().physical);
        }

        void UpdateStatusEffectResistances(Helmet helmet, Armor armor, Gauntlet gauntlet, Legwear legwear, List<Accessory> accessories)
        {
            statusEffectResistances.Clear();

            HandleStatusEffectEntries(helmet?.GetDamageAbsorbed().statusEffects);
            HandleStatusEffectEntries(armor?.GetDamageAbsorbed().statusEffects);
            HandleStatusEffectEntries(gauntlet?.GetDamageAbsorbed().statusEffects);
            HandleStatusEffectEntries(legwear?.GetDamageAbsorbed().statusEffects);

            var accessoryResistances = accessories
                .Where(a => a != null)
                .SelectMany(a => a.GetDamageAbsorbed().statusEffects)
                .ToArray();

            HandleStatusEffectEntries(accessoryResistances);
        }

        void HandleStatusEffectEntries(StatusEffectEntry[] resistances)
        {
            if (resistances != null && resistances.Length > 0)
            {
                foreach (var resistance in resistances)
                {
                    HandleStatusEffectEntry(resistance);
                }
            }
        }

        void HandleStatusEffectEntry(StatusEffectEntry statusEffectResistance)
        {
            if (this.statusEffectResistances.ContainsKey(statusEffectResistance.statusEffect))
            {
                this.statusEffectResistances[statusEffectResistance.statusEffect]
                    += (int)statusEffectResistance.amountPerHit;
            }
            else
            {
                this.statusEffectResistances.Add(statusEffectResistance.statusEffect, (int)statusEffectResistance.amountPerHit);
            }
        }

        void UpdateAttributes(Weapon rightWeapon, Weapon leftWeapon, Helmet helmet, Armor armor, Gauntlet gauntlet, Legwear legwear,
        List<Accessory> accessories, Shield rightShield, Shield leftShield)
        {
            ResetAttributes();

            ApplyWeaponAttributes(rightWeapon);
            ApplyWeaponAttributes(leftWeapon);

            ApplyEquipmentAttributes(helmet);
            ApplyEquipmentAttributes(armor);
            ApplyEquipmentAttributes(gauntlet);
            ApplyEquipmentAttributes(legwear);

            ApplyAccessoryAttributes(accessories);

            ApplyShieldAttributes(rightShield);
            ApplyShieldAttributes(leftShield);
        }

        void ResetAttributes()
        {

            healthBonus = magicBonus = staminaBonus = vitalityBonus = enduranceBonus = strengthBonus = dexterityBonus = intelligenceBonus = 0;
            equipmentFireDefenseBonus = equipmentFrostDefenseBonus = equipmentLightningDefenseBonus = equipmentMagicDefenseBonus =
            equipmentDarkDefenseBonus = equipmentWaterDefenseBonus = 0;

            discountPercentage = spellDamageBonusMultiplier = 0;

            reputationBonus = parryPostureDamageBonus = postureBonus = movementSpeedBonus = 0;

            parryPostureWindowBonus = staminaRegenerationBonus = postureDecreaseRateBonus = projectileMultiplierBonus = backStabAngleBonus = 0f;

            shouldRegenerateMana = chanceToRestoreHealthUponDeath = canRage = chanceToNotLoseItemUponConsumption = increaseAttackPowerWhenUnarmed =
            increaseAttackPowerTheLowerTheReputation = increaseAttackPowerWithLowerHealth = false;

            twoHandAttackBonusMultiplier = heavyAttackBonusMultiplier = jumpAttackBonusMultiplier = slashDamageMultiplier =
            pierceDamageMultiplier = bluntDamageMultiplier = footDamageMultiplier = physicalAttackBonus = 0f;

            healthBonusMultiplier = manaBonusMultiplier = staminaBonusMultiplier = 0f;
        }

        void ApplyWeaponAttributes(Weapon currentWeapon)
        {
            if (currentWeapon != null)
            {
                shouldRegenerateMana = currentWeapon.shouldRegenerateMana;
            }
        }

        void ApplyEquipmentAttributes(ArmorBase equipment)
        {
            if (equipment != null)
            {
                vitalityBonus += equipment.vitalityBonus;
                enduranceBonus += equipment.enduranceBonus;
                strengthBonus += equipment.strengthBonus;
                dexterityBonus += equipment.dexterityBonus;
                intelligenceBonus += equipment.intelligenceBonus;
                equipmentFireDefenseBonus += equipment.GetDamageAbsorbedForCurrentLevel().fire;
                equipmentFrostDefenseBonus += equipment.GetDamageAbsorbedForCurrentLevel().frost;
                equipmentLightningDefenseBonus += equipment.GetDamageAbsorbedForCurrentLevel().lightning;
                equipmentMagicDefenseBonus += equipment.GetDamageAbsorbedForCurrentLevel().magic;
                equipmentDarkDefenseBonus += equipment.GetDamageAbsorbedForCurrentLevel().darkness;
                equipmentWaterDefenseBonus += equipment.GetDamageAbsorbedForCurrentLevel().water;
                reputationBonus += equipment.reputationBonus;
                discountPercentage += equipment.discountPercentage;
                postureBonus += equipment.GetDamageAbsorbed().postureDamage;
                staminaRegenerationBonus += equipment.staminaRegenBonus;
                movementSpeedBonus += equipment.movementSpeedBonus;
                projectileMultiplierBonus += equipment.projectileMultiplierBonus;

                if (equipment.canRage)
                {
                    canRage = true;
                }
            }
        }

        void ApplyAccessoryAttributes(List<Accessory> accessories)
        {
            foreach (var accessory in accessories)
            {
                vitalityBonus += accessory?.vitalityBonus ?? 0;
                enduranceBonus += accessory?.enduranceBonus ?? 0;
                strengthBonus += accessory?.strengthBonus ?? 0;
                dexterityBonus += accessory?.dexterityBonus ?? 0;
                intelligenceBonus += accessory?.intelligenceBonus ?? 0;
                equipmentFireDefenseBonus += accessory?.GetDamageAbsorbed().fire ?? 0;
                equipmentFrostDefenseBonus += accessory?.GetDamageAbsorbed().frost ?? 0;
                equipmentLightningDefenseBonus += accessory?.GetDamageAbsorbed().lightning ?? 0;
                equipmentMagicDefenseBonus += accessory?.GetDamageAbsorbed().magic ?? 0;
                equipmentDarkDefenseBonus += accessory?.GetDamageAbsorbed().darkness ?? 0;
                equipmentWaterDefenseBonus += accessory?.GetDamageAbsorbed().water ?? 0;
                reputationBonus += accessory?.reputationBonus ?? 0;
                parryPostureDamageBonus += accessory?.postureDamagePerParry ?? 0;

                backStabAngleBonus += accessory?.backStabAngleBonus ?? 0;

                healthBonus += accessory?.healthBonus ?? 0;
                magicBonus += accessory?.magicBonus ?? 0;
                staminaBonus += accessory?.staminaBonus ?? 0;
                spellDamageBonusMultiplier += accessory?.spellDamageBonusMultiplier ?? 0;
                postureBonus += accessory?.GetDamageAbsorbed().postureDamage ?? 0;
                staminaRegenerationBonus += accessory?.staminaRegenBonus ?? 0;

                postureDecreaseRateBonus += accessory?.postureDecreaseRateBonus ?? 0;

                healthBonusMultiplier += accessory?.healthBonusMultiplier ?? 0;
                staminaBonusMultiplier += accessory?.staminaBonusMultiplier ?? 0;
                manaBonusMultiplier += accessory?.manaBonusMultiplier ?? 0;


                if (accessory != null)
                {
                    if (accessory.chanceToRestoreHealthUponDeath)
                    {
                        chanceToRestoreHealthUponDeath = true;
                    }

                    if (accessory.chanceToNotLoseItemUponConsumption)
                    {
                        chanceToNotLoseItemUponConsumption = true;
                    }

                    if (accessory.increaseAttackPowerWhenUnarmed)
                    {
                        increaseAttackPowerWhenUnarmed = true;
                    }

                    if (accessory.increaseAttackPowerTheLowerTheReputation)
                    {
                        increaseAttackPowerTheLowerTheReputation = true;
                    }

                    if (accessory.increaseAttackPowerWithLowerHealth)
                    {
                        increaseAttackPowerWithLowerHealth = true;
                    }

                    if (accessory.twoHandAttackBonusMultiplier > 0)
                    {
                        twoHandAttackBonusMultiplier += accessory.twoHandAttackBonusMultiplier;
                    }

                    if (accessory.heavyAttackBonusMultiplier > 0)
                    {
                        heavyAttackBonusMultiplier += accessory.heavyAttackBonusMultiplier;
                    }

                    if (accessory.jumpAttackBonus > 0)
                    {
                        jumpAttackBonusMultiplier += accessory.jumpAttackBonus;
                    }
                    if (accessory.footDamageMultiplier > 0)
                    {
                        footDamageMultiplier += accessory.footDamageMultiplier;
                    }

                    if (accessory.slashDamageMultiplier > 0)
                    {
                        slashDamageMultiplier += accessory.slashDamageMultiplier;
                    }

                    if (accessory.bluntDamageMultiplier > 0)
                    {
                        bluntDamageMultiplier += accessory.bluntDamageMultiplier;
                    }

                    if (accessory.pierceDamageMultiplier > 0)
                    {
                        pierceDamageMultiplier += accessory.pierceDamageMultiplier;
                    }

                    if (accessory.physicalAttackBonus > 0)
                    {
                        physicalAttackBonus += accessory.physicalAttackBonus;
                    }
                }
            }
        }

        void ApplyShieldAttributes(Shield shield)
        {
            if (shield != null)
            {
                parryPostureWindowBonus += shield.parryWindowBonus;
                parryPostureDamageBonus += shield.parryPostureDamageBonus;
                staminaRegenerationBonus += shield.staminaRegenBonus;
            }
        }

        void UpdateAdditionalCoinMultiplier(Helmet helmet, Armor armor, Gauntlet gauntlet, Legwear legwear, List<Accessory> accessories)
        {
            additionalCoinMultiplier = 0f;

            if (helmet?.additionalCoinMultiplier > 0)
            {
                additionalCoinMultiplier += helmet.additionalCoinMultiplier;
            }
            if (armor?.additionalCoinMultiplier > 0)
            {
                additionalCoinMultiplier += armor.additionalCoinMultiplier;
            }
            if (gauntlet?.additionalCoinMultiplier > 0)
            {
                additionalCoinMultiplier += gauntlet.additionalCoinMultiplier;
            }
            if (legwear?.additionalCoinMultiplier > 0)
            {
                additionalCoinMultiplier += legwear.additionalCoinMultiplier;
            }

            float sumFromAccessories = accessories.Sum(x => x == null ? 0 : x.additionalCoinMultiplier);
            additionalCoinMultiplier += sumFromAccessories;
        }

        public bool ShouldDoubleCoinFromFallenEnemy()
        {
            (Weapon rightWeapon, Weapon leftWeapon) = GetCurrentWeapons();
            List<Accessory> accessories = GetCurrentAccessories();

            bool hasDoublingCoinAccessoryEquipped = accessories.Any(acc => acc != null && acc.chanceToDoubleCoinsFromFallenEnemies);

            if (rightWeapon != null && rightWeapon.doubleCoinsUponKillingEnemies)
            {
                return true;
            }

            if (leftWeapon != null && leftWeapon.doubleCoinsUponKillingEnemies)
            {
                return true;
            }

            if (!hasDoublingCoinAccessoryEquipped)
            {
                return false;
            }

            return Random.Range(0, 1f) <= 0.05f;
        }

        public int GetCurrentIntelligenceBonus()
        {
            return intelligenceBonus + intelligenceBonusFromConsumable;
        }

        public int GetCurrentDexterityBonus()
        {
            return dexterityBonus + dexterityBonusFromConsumable;
        }

        public int GetCurrentStrengthBonus()
        {
            return strengthBonus + strengthBonusFromConsumable;
        }

        public int GetCurrentVitalityBonus()
        {
            return vitalityBonus + vitalityBonusFromConsumable;
        }

        public int GetCurrentEnduranceBonus()
        {
            return enduranceBonus + enduranceBonusFromConsumable;
        }

        public int GetCurrentReputationBonus()
        {
            return reputationBonus;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="value"></param>
        public void SetStatsFromConsumable(int value)
        {
            this.vitalityBonusFromConsumable = value;
            this.enduranceBonusFromConsumable = value;
            this.strengthBonusFromConsumable = value;
            this.dexterityBonusFromConsumable = value;
            this.intelligenceBonusFromConsumable = value;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="value"></param>
        public void SetIgnoreNextWeaponToEquipRequirements(bool value)
        {
            ignoreWeaponRequirements = value;
        }

        // TODO: Stuff related to rebirth, move it to its own proper class
        public void ReturnGoldAndResetStats()
        {
            int goldAmount = LevelUtils.GetRequiredExperienceForLevel(character.characterBaseStats.GetCurrentLevel());
            character.characterBaseStats.ResetStats();

            // TODO: Override in PlayerStatsBonusController

            uIDocumentPlayerGold.AddGold(goldAmount);

            bool isPortuguese = LocalizationSettings.SelectedLocale.Identifier.Code == "pt";

            notificationManager.ShowNotification(
                isPortuguese ? "Os teus atributos foram resetados" : "Your stats have been reset",
                notificationManager.systemSuccess
            );
        }
    }
}
