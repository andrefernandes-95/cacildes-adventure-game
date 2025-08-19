using System;
using System.Collections.Generic;
using AF.Health;
using EditorAttributes;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace AF
{
    public enum ArmorSlot
    {
        Head,
        Chest,
        Arms,
        Legs,
    }

    public class ArmorBase : UpgradableItem
    {

        [System.Serializable]
        public class StatusEffectCancellationRate
        {
            public StatusEffect statusEffect;
            [Range(0f, 1f)] public float delayRate = 1f;
        }

        [Header("Stats")]
        [Header("Damage Absorption")]
        [SerializeField] ArmorDamageTemplate damageTemplate;

        [HelpBox("If you want custom damage, use damage absorbed, otherwise, use Damage Template for multiple pieces of armor")]
        [SerializeField] Damage damageAbsorbed = new();

        [Header("Delay of Status Effect Amount Buildups")]
        public StatusEffectCancellationRate[] statusEffectDelayRates;

        [Header("Attribute Bonus")]
        public int vitalityBonus = 0;
        public int enduranceBonus = 0;
        public int strengthBonus = 0;
        public int dexterityBonus = 0;
        public int intelligenceBonus = 0;

        [Header("Stamina")]
        public float staminaRegenBonus = 0f;

        [Header("Speed Penalties")]
        public float speedPenalty = 0;
        public int movementSpeedBonus = 0;

        [Header("Coins")]
        public float additionalCoinMultiplier = 0f;

        [Header("Reputation")]
        public int reputationBonus = 0;

        [Header("Discounts")]
        [Range(0, 1f)] public float discountPercentage = 0f;

        [Header("Damage On Enemies")]
        public bool canDamageEnemiesUponAttack = false;
        public Damage damageDealtToEnemiesUponAttacked;

        [Header("Projectile Options")]
        public float projectileMultiplierBonus = 0f;

        [Header("Rage Mode")]
        public bool canRage = false;

        [Header("Graphics")]
        public List<string> graphicsToShow = new();
        public string male_GraphicsToShow;
        public string female_GraphicsToShow;
        public Material armorMaterial;

        Damage _cachedDamageAbsorbed = null;

        public Damage GetDamageAbsorbed()
        {
            if (_cachedDamageAbsorbed == null)
            {
                Damage clonedDamage = damageTemplate != null ? Instantiate(damageTemplate).damage.Clone() : this.damageAbsorbed.Clone();

                // If using damage templates, check if the equipment is the main armor or a piece, and reduce its damage absorption accordingly
                if (damageTemplate != null)
                {
                    if (this is Legwear)
                    {
                        clonedDamage.Multiply(0.65f);
                    }
                    else if (this is Helmet)
                    {
                        clonedDamage.Multiply(0.45f);
                    }
                    else if (this is Gauntlet || this is Accessory)
                    {
                        clonedDamage.Multiply(0.25f);
                    }
                }

                _cachedDamageAbsorbed = clonedDamage;
            }

            return _cachedDamageAbsorbed;
        }

        public string GetFormattedStatusResistances()
        {
            string result = "";

            var resistenceAgainstLabel = LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "resistence against");

            foreach (var resistance in GetDamageAbsorbed().statusEffects)
            {

                if (resistance != null && resistance.statusEffect != null && resistance.statusEffect.GetName().Length > 0)
                {
                    result += $"+{resistance.amountPerHit} {resistenceAgainstLabel} {resistance.statusEffect.GetName()}\n";
                }
            }

            return result.TrimEnd();
        }


        public string GetFormattedStatusCancellationRates()
        {
            string result = "";

            foreach (var resistance in statusEffectDelayRates)
            {
                if (resistance != null)
                {
                    float buildupReductionPercent = (1f - resistance.delayRate) * 100f;

                    if (Utils.IsPortuguese())
                    {
                        result += $"{buildupReductionPercent:0.#}% de redução na taxa de acúmulo de {resistance.statusEffect.GetName()} por segundo\n";
                    }
                    else
                    {
                        result += $"{buildupReductionPercent:0.#}% reduction in buildup rate of {resistance.statusEffect.GetName()} per second\n";
                    }
                }
            }

            return result.TrimEnd();
        }


        public string GetFormattedDamageDealtToEnemiesUpponAttacked()
        {
            string result = "";

            foreach (var resistance in damageDealtToEnemiesUponAttacked.statusEffects)
            {
                if (resistance != null)
                {
                    result += $"+{resistance.amountPerHit} {resistance.statusEffect.name} inflicted on attacking enemies\n";
                }
            }

            return result.TrimEnd();
        }

        public void AttackEnemy(CharacterManager enemy)
        {
            if (!canDamageEnemiesUponAttack)
            {
                return;
            }
            enemy.characterBaseDamageReceiver.TakeDamage(damageDealtToEnemiesUponAttacked);
        }

        public virtual void OnEquip(CharacterBaseManager character)
        {
        }

        public virtual void OnUnequip(CharacterBaseManager character)
        {
        }

        public int GetDiscountPercentageAtShops()
        {
            return Mathf.Clamp((int)Math.Round(discountPercentage * 100, 2), 0, 100);
        }

        protected List<string> GetGraphicsToShow()
        {
            List<string> graphics = new();
            foreach (string s in graphicsToShow)
            {
                graphics.Add(s);
            }
            return graphics;
        }

        public int GetCurrentPhysicalDefenseForLevel(int level)
        {
            if (GetDamageAbsorbed().physical <= 0)
            {
                return 0;
            }

            return GetDamageAbsorbed().physical + GetBonusAttackPerLevel(level);
        }
        public int GetFireDefenseForLevel(int level) => GetElementalDefenseForLevel(GetDamageAbsorbed().fire, level);
        public int GetFrostDefenseForLevel(int level) => GetElementalDefenseForLevel(GetDamageAbsorbed().frost, level);
        public int GetLightningDefenseForLevel(int level) => GetElementalDefenseForLevel(GetDamageAbsorbed().lightning, level);
        public int GetDarknessDefenseForLevel(int level) => GetElementalDefenseForLevel(GetDamageAbsorbed().darkness, level);
        public int GetWaterDefenseForLevel(int level) => GetElementalDefenseForLevel(GetDamageAbsorbed().water, level);
        public int GetMagicDefenseForLevel(int level) => GetElementalDefenseForLevel(GetDamageAbsorbed().magic, level);

        int GetElementalDefenseForLevel(int baseElementalDamage, int level)
        {
            if (baseElementalDamage <= 0)
            {
                return 0;
            }

            return baseElementalDamage + GetBonusAttackPerLevel(level);
        }

        public Damage GetDamageAbsorbedForCurrentLevel()
        {
            Damage cloneDamage = this.GetDamageAbsorbed().Clone();

            cloneDamage.physical = GetCurrentPhysicalDefenseForLevel(level);
            cloneDamage.fire = GetFireDefenseForLevel(level);
            cloneDamage.frost = GetFrostDefenseForLevel(level);
            cloneDamage.lightning = GetLightningDefenseForLevel(level);
            cloneDamage.magic = GetMagicDefenseForLevel(level);
            cloneDamage.darkness = GetDarknessDefenseForLevel(level);
            cloneDamage.water = GetWaterDefenseForLevel(level);

            return cloneDamage;
        }
    }
}
