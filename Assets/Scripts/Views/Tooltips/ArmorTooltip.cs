namespace AF
{
    using System;
    using System.Collections.Generic;
    using AF.UI.EquipmentMenu;
    using UnityEngine;

    public class ArmorTooltip : MonoBehaviour
    {
        [SerializeField] ItemTooltip itemTooltip;
        [SerializeField] GUIIconsDatabase GUIIconsDatabase;

        public void DrawArmorBase(ArmorBase armor)
        {
            string damageLabel = "";

            Dictionary<string, (int value, string enText, string ptText, Color color)> damageTypes = new()
            {
                { "physical",  (armor.damageAbsorbed.physical,  "Physical Damage Absorbed", "Dano Físico Absorvido",    Color.white) },
                { "fire",      (armor.damageAbsorbed.fire,      "Fire Damage Absorbed",     "Dano de Fogo Absorvido",   GUIIconsDatabase.fireColor) },
                { "frost",     (armor.damageAbsorbed.frost,     "Frost Damage Absorbed",    "Dano de Gelo Absorvido",   GUIIconsDatabase.frostColor) },
                { "lightning", (armor.damageAbsorbed.lightning, "Lightning Damage Absorbed","Dano Elétrico Absorvido",  GUIIconsDatabase.lightningColor) },
                { "magic",     (armor.damageAbsorbed.magic,     "Magic Damage Absorbed",    "Dano Mágico Absorvido",    GUIIconsDatabase.magicColor) },
                { "darkness",  (armor.damageAbsorbed.darkness,  "Darkness Damage Absorbed", "Dano de Trevas Absorvido", GUIIconsDatabase.darknessColor) },
                { "water",     (armor.damageAbsorbed.water,     "Water Damage Absorbed",    "Dano Aquático Absorvido",  GUIIconsDatabase.waterColor) }
            };

            foreach (var entry in damageTypes)
            {
                int value = entry.Value.value;
                if (value > 0)
                {
                    string line = $"+{value} {(Utils.IsPortuguese() ? entry.Value.ptText : entry.Value.enText)}";

                    // Apply color if defined

                    string hexColor = ColorUtility.ToHtmlStringRGB(entry.Value.color);
                    line = $"<color=#{hexColor}>{line}</color>";

                    damageLabel += line + "\n";
                }
            }

            // Finally create the tooltip
            itemTooltip.CreateTooltip(
                GUIIconsDatabase.physicalAbsorption,
                Color.white,
                damageLabel);

            if (armor.damageAbsorbed.poiseDamage > 0)
            {
                string label = $"+{armor.damageAbsorbed.poiseDamage} Poise";
                label += "\n";
                label += "<i><size=80%>(How many hits you can endure before you're interrupted)</i>";

                if (Utils.IsPortuguese())
                {
                    label = $"+{armor.damageAbsorbed.poiseDamage} Equilíbrio";
                    label += "\n";
                    label += "<i><size=80%>(Quantos golpes consegues suportar antes de seres interrompido(a))</i>";
                }

                // Finally create the tooltip
                itemTooltip.CreateTooltip(
                    GUIIconsDatabase.pushForce,
                    Color.white,
                    label);
            }

            if (armor.damageAbsorbed.postureDamage > 0)
            {
                string label = $"+{armor.damageAbsorbed.postureDamage} Posture";
                label += "\n";
                label += "<i><size=80%>(Your resistance to critical attacks, represented by a yellow bar)</i>";

                if (Utils.IsPortuguese())
                {
                    label = $"+{armor.damageAbsorbed.postureDamage} Postura";
                    label += "\n";
                    label += "<i><size=80%>(A tua resistência contra ataques críticos, representada por uma barra amarela)</i>";
                }

                // Finally create the tooltip
                itemTooltip.CreateTooltip(
                    GUIIconsDatabase.posture,
                    Color.white,
                    label);
            }


            if (armor.damageAbsorbed.statusEffects != null && armor.damageAbsorbed.statusEffects.Length > 0)
            {
                itemTooltip.CreateTooltip(GUIIconsDatabase.statusEffects, Color.white, armor.GetFormattedStatusResistances());
            }

            if (armor.statusEffectCancellationRates != null && armor.statusEffectCancellationRates.Length > 0)
            {
                itemTooltip.CreateTooltip(GUIIconsDatabase.statusEffects, Color.white, armor.GetFormattedStatusCancellationRates());
            }

            CreateAdditionalGoldTooltip(armor.additionalCoinPercentage);

            ShowStatTooltip(
                armor.vitalityBonus,
                "Vitality",
                "Vitalidade",
                StatsUtils.GetVitalityDescription(),
                GUIIconsDatabase.vitality);

            ShowStatTooltip(
                armor.enduranceBonus,
                "Endurance",
                "Resistência",
                StatsUtils.GetEnduranceDescription(),
                GUIIconsDatabase.endurance);

            ShowStatTooltip(
                armor.intelligenceBonus,
                "Intelligence",
                "Inteligência",
                StatsUtils.GetIntelligenceDescription(),
                GUIIconsDatabase.intelligence);

            ShowStatTooltip(
                armor.strengthBonus,
                "Strength",
                "Força",
                StatsUtils.GetStrengthDescription(),
                GUIIconsDatabase.strength);

            ShowStatTooltip(
                armor.dexterityBonus,
                "Dexterity",
                "Destreza",
                StatsUtils.GetDexterityDescription(),
                GUIIconsDatabase.dexterity);

            if (armor.discountPercentage > 0)
            {
                int discountPercentage = armor.GetDiscountPercentageAtShops();
                string label = $"{discountPercentage}% discount at shops";

                if (Utils.IsPortuguese())
                {
                    label = $"{discountPercentage}% disconto nas lojas";
                }

                itemTooltip.CreateTooltip(
                    GUIIconsDatabase.gold,
                    Color.white,
                    label);
            }
        }

        private void ShowStatTooltip(int bonusValue, string statNameEnglish, string statNamePortuguese, string description, Texture2D icon)
        {
            if (bonusValue <= 0) return;

            bool isPT = Utils.IsPortuguese();

            string label = isPT ? statNamePortuguese : statNameEnglish;
            label += "\n";
            label += $"<i><size=80%>{description}</size></i>";

            CreateStatTooltip(bonusValue, label, icon);
        }

        void CreateAdditionalGoldTooltip(float additionalCoinPercentage)
        {
            if (additionalCoinPercentage <= 0)
            {
                return;
            }

            string label = $"+{additionalCoinPercentage}% Gold found on enemies";

            if (Utils.IsPortuguese())
            {
                label = $"+{additionalCoinPercentage}% Ouro encontrado em inimigos";
            }

            itemTooltip.CreateTooltip(
                GUIIconsDatabase.gold,
                Color.white,
                String.Format(label, additionalCoinPercentage));
        }

        void CreateStatTooltip(int statBonus, string label, Texture2D statSprite)
        {
            string finalLabel = $"+{statBonus} {label}";

            itemTooltip.CreateTooltip(statSprite, Color.white, finalLabel);
        }
    }
}
