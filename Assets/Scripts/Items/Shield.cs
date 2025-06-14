﻿using System.Linq;
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
        public StatusEffectCancellationRate[] statusEffectCancellationRates;

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

        public Damage FilterDamage(Damage originalDamage)
        {
            Damage incomingDamage = originalDamage.Copy();

            if (physicalAbsorption != 1)
            {
                incomingDamage.physical = (int)(incomingDamage.physical * physicalAbsorption);
            }

            if (fireAbsorption != 1)
            {
                incomingDamage.fire = (int)(incomingDamage.fire * fireAbsorption);
            }

            if (frostAbsorption != 1)
            {
                incomingDamage.frost = (int)(incomingDamage.frost * frostAbsorption);
            }

            if (lightiningAbsorption != 1)
            {
                incomingDamage.lightning = (int)(incomingDamage.lightning * lightiningAbsorption);
            }

            if (darknessAbsorption != 1)
            {
                incomingDamage.darkness = (int)(incomingDamage.darkness * darknessAbsorption);
            }

            if (waterAbsorption != 1)
            {
                incomingDamage.water = (int)(incomingDamage.water * waterAbsorption);
            }

            if (magicAbsorption != 1)
            {
                incomingDamage.magic = (int)(incomingDamage.magic * magicAbsorption);
            }

            if (postureDamageAbsorption != 1)
            {
                incomingDamage.postureDamage = (int)(incomingDamage.postureDamage * postureDamageAbsorption);
            }

            if (slashDamageAbsorption != 1 && incomingDamage.weaponAttackType == WeaponAttackType.Slash)
            {
                incomingDamage.physical = (int)(incomingDamage.physical * slashDamageAbsorption);
            }
            else if (bluntDamageAbsorption != 1 && incomingDamage.weaponAttackType == WeaponAttackType.Blunt)
            {
                incomingDamage.physical = (int)(incomingDamage.physical * bluntDamageAbsorption);
            }
            else if (pierceDamageAbsorption != 1 && incomingDamage.weaponAttackType == WeaponAttackType.Pierce)
            {
                incomingDamage.physical = (int)(incomingDamage.physical * pierceDamageAbsorption);
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

            foreach (var resistance in statusEffectCancellationRates)
            {
                if (resistance != null)
                {
                    result += $"-{resistance.amountToCancelPerSecond} {resistance.statusEffect.GetName()} {LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Inflicted Per Second")}\n";
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
            enemy.damageReceiver.TakeDamage(damageDealtToEnemiesUponBlocking);
        }

    }

}
