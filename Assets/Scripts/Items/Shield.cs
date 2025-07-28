using System.Linq;
using AF.Health;
using UnityEngine;
using UnityEngine.Localization.Settings;
using static AF.ArmorBase;

namespace AF
{

    [System.Serializable]
    public class StatusEffectBlockResistance
    {
        public StatusEffect statusEffect;
        [Range(0, 1f)] public float absorption = 1f;
    }

    [CreateAssetMenu(menuName = "Items / Shield / New Shield")]
    public class Shield : Weapon
    {
        [Header("Stamina Costs")]
        public float blockStaminaCost = 50;

        // Defense Absorption
        [Range(0, 1f)] public float physicalAbsorption = 1f;
        [Range(0, 1f)] public float fireAbsorption = 1f;
        [Range(0, 1f)] public float frostAbsorption = 1f;
        [Range(0, 1f)] public float lightiningAbsorption = 1f;
        [Range(0, 1f)] public float magicAbsorption = 1f;
        [Range(0, 1f)] public float darknessAbsorption = 1f;
        [Range(0, 1f)] public float waterAbsorption = 1f;
        [Range(0, 1f)] public float postureDamageAbsorption = 1f;


        [Header("Damage Types")]
        [Range(0, 1f)] public float pierceDamageAbsorption = 1f;

        [Range(0, 1f)] public float bluntDamageAbsorption = 1f;

        [Range(0, 1f)] public float slashDamageAbsorption = 1f;


        [Header("Status Effect Resistances")]
        public StatusEffectBlockResistance[] statusEffectBlockResistances;
        public StatusEffectCancellationRate[] statusEffectDelayRates;

        [Header("Stats Bonuses")]
        public int vitalityBonus = 0;
        public int enduranceBonus = 0;
        public int intelligenceBonus = 0;

        [Header("Regen Options")]
        public float staminaRegenBonus = 1f;

        [Header("Additional Stats")]
        public int postureBonus = 0;
        public int poiseBonus = 0;

        [Header("Damage Enemies On Block")]
        public bool canDamageEnemiesOnShieldAttack = false;
        public Damage damageDealtToEnemiesUponBlocking;

        [Header("Parry Bonus")]
        public float parryWindowBonus = 0f;
        public int parryPostureDamageBonus = 0;
        [Header("VFX")]
        public GameObject blockFx;

        public float GetCurrentPhysicalAbsorption(Damage incomingDamage, float baseValue)
        {
            float physicalAbsorptionBonus = 0f;

            if (slashDamageAbsorption != 1 && incomingDamage.weaponAttackType == WeaponAttackType.Slash)
            {
                physicalAbsorptionBonus += 1 - GetCurrentAbsorption(slashDamageAbsorption);
            }
            else if (bluntDamageAbsorption != 1 && incomingDamage.weaponAttackType == WeaponAttackType.Blunt)
            {
                physicalAbsorptionBonus += 1 - GetCurrentAbsorption(bluntDamageAbsorption);
            }
            else if (pierceDamageAbsorption != 1 && incomingDamage.weaponAttackType == WeaponAttackType.Pierce)
            {
                physicalAbsorptionBonus += 1 - GetCurrentAbsorption(pierceDamageAbsorption);
            }

            return GetCurrentAbsorption(baseValue) + physicalAbsorptionBonus;
        }

        public float GetCurrentAbsorption(float baseValue)
        {
            return GetAbsorptionForLevel(baseValue, level);
        }

        public float GetAbsorptionForLevel(float baseValue, int givenLevel)
        {
            float bonus = (float)(givenLevel + 0f) / 20;

            return baseValue + bonus;
        }

        public Damage FilterDamage(Damage originalDamage)
        {
            Damage incomingDamage = originalDamage.Clone();

            if (physicalAbsorption != 1)
            {
                incomingDamage.physical = (int)(incomingDamage.physical * (1 - GetCurrentPhysicalAbsorption(incomingDamage, physicalAbsorption)));
            }

            if (fireAbsorption != 1)
            {
                incomingDamage.fire = (int)(incomingDamage.fire * (1 - GetCurrentAbsorption(fireAbsorption)));
            }

            if (frostAbsorption != 1)
            {
                incomingDamage.frost = (int)(incomingDamage.frost * (1 - GetCurrentAbsorption(frostAbsorption)));
            }

            if (lightiningAbsorption != 1)
            {
                incomingDamage.lightning = (int)(incomingDamage.lightning * (1 - GetCurrentAbsorption(lightiningAbsorption)));
            }

            if (darknessAbsorption != 1)
            {
                incomingDamage.darkness = (int)(incomingDamage.darkness * (1 - GetCurrentAbsorption(darknessAbsorption)));
            }

            if (waterAbsorption != 1)
            {
                incomingDamage.water = (int)(incomingDamage.water * (1 - GetCurrentAbsorption(waterAbsorption)));
            }

            if (magicAbsorption != 1)
            {
                incomingDamage.magic = (int)(incomingDamage.magic * (1 - GetCurrentAbsorption(magicAbsorption)));
            }

            if (postureDamageAbsorption != 1)
            {
                incomingDamage.postureDamage = (int)(incomingDamage.postureDamage * (1 - GetCurrentAbsorption(postureDamageAbsorption)));
            }

            return incomingDamage;
        }

        public Damage FilterPassiveDamage(Damage incomingDamage)
        {
            if (statusEffectBlockResistances != null && statusEffectBlockResistances.Length > 0 && incomingDamage.statusEffects != null && incomingDamage.statusEffects.Length > 0)
            {
                foreach (var statusEffectBlockResistance in statusEffectBlockResistances)
                {
                    int idx = System.Array.FindIndex(incomingDamage.statusEffects, x => x.statusEffect == statusEffectBlockResistance.statusEffect);
                    if (idx != -1)
                    {
                        incomingDamage.statusEffects[idx].amountPerHit *= statusEffectBlockResistance.absorption;
                    }
                }
            }

            return incomingDamage;
        }

        public string GetFormattedStatusResistances()
        {
            string result = "";

            foreach (var resistance in statusEffectBlockResistances)
            {
                if (resistance != null)
                {
                    result += $"%{100 - (resistance.absorption * 100)} {resistance.statusEffect.GetName()} {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Absorption")}\n";
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

        public string GetFormattedStatusAttacks()
        {
            string result = "";

            foreach (var resistance in damageDealtToEnemiesUponBlocking.statusEffects)
            {
                if (resistance != null)
                {
                    result += $"+{resistance.amountPerHit} {resistance.statusEffect.name} {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "inflicted on enemy attacking shield")}\n";
                }
            }

            return result.TrimEnd();
        }

        public void AttackShieldAttacker(CharacterManager enemy)
        {
            if (!canDamageEnemiesOnShieldAttack)
            {
                return;
            }
            enemy.characterBaseDamageReceiver.TakeDamage(damageDealtToEnemiesUponBlocking);
        }

    }

}
