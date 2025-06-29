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

            if (armor.vitalityBonus > 0)
            {
                string label = "Vitality";

                if (Utils.IsPortuguese())
                {
                    label = "Vitalidade";
                }
                label += "\n";
                label += $"<i><size=80%>{StatsUtils.GetVitalityDescription()}";

                CreateStatTooltip(armor.vitalityBonus, label, GUIIconsDatabase.vitality);
            }
            CreateStatTooltip(armor.enduranceBonus, Utils.IsPortuguese() ? "Resistência" : "Endurance", GUIIconsDatabase.endurance);
            CreateStatTooltip(armor.intelligenceBonus, Utils.IsPortuguese() ? "Inteligência" : "Intelligence", GUIIconsDatabase.intelligence);
            CreateStatTooltip(armor.strengthBonus, Utils.IsPortuguese() ? "Força" : "Strength", GUIIconsDatabase.strength);
            CreateStatTooltip(armor.dexterityBonus, Utils.IsPortuguese() ? "Destreza" : "Dexterity", GUIIconsDatabase.dexterity);

            /*

            CreateStatTooltip(armor.vitalityBonus, vitalityBonus.GetLocalizedString(), vitalitySprite);
            CreateStatTooltip(armor.enduranceBonus, enduranceBonus.GetLocalizedString(), enduranceSprite);
            CreateStatTooltip(armor.intelligenceBonus, intelligenceBonus.GetLocalizedString(), intelligenceSprite);
            CreateStatTooltip(armor.strengthBonus, strengthBonus.GetLocalizedString(), strengthSprite);
            CreateStatTooltip(armor.dexterityBonus, dexterityBonus.GetLocalizedString(), dexteritySprite);

            if (armor.reputationBonus > 0)
            {
                itemTooltip.CreateTooltip(
                    reputationSprite,
                    Color.white,
                    String.Format(
                        reputationBonus.GetLocalizedString(),
                        $"+{armor.reputationBonus}"));
            }
            else if (armor.reputationBonus < 0)
            {
                itemTooltip.CreateTooltip(
                    reputationSprite,
                    Color.white,
                    String.Format(
                        reputationBonus.GetLocalizedString(),
                        $"{armor.reputationBonus}"));
            }

            if (armor.discountPercentage > 0)
            {
                itemTooltip.CreateTooltip(
                    barterSprite,
                    Color.white,
                    String.Format(
                        betterPrices.GetLocalizedString(),
                        Math.Round(armor.discountPercentage * 100, 2)
                ));
            }

            if (armor.canDamageEnemiesUponAttack)
            {
                if (armor.damageDealtToEnemiesUponAttacked.physical != 0)
                {
                    itemTooltip.CreateTooltip(
                        weaponPhysicalAttackSprite,
                        Color.white,
                        String.Format(
                            physicalDamageDealtToAttackingEnemies.GetLocalizedString(),
                            armor.damageDealtToEnemiesUponAttacked.physical
                    ));
                }

                if (armor.damageDealtToEnemiesUponAttacked.fire != 0)
                {

                    itemTooltip.CreateTooltip(
                        fireSprite,
                        fire,
                        String.Format(
                            fireDamageDealtToAttackingEnemies.GetLocalizedString(),
                            armor.damageDealtToEnemiesUponAttacked.fire
                    ));

                }

                if (armor.damageDealtToEnemiesUponAttacked.frost != 0)
                {

                    itemTooltip.CreateTooltip(
                        frostSprite,
                        frost,
                        String.Format(
                            frostDamageDealtToAttackingEnemies.GetLocalizedString(),
                            armor.damageDealtToEnemiesUponAttacked.frost
                    ));

                }

                if (armor.damageDealtToEnemiesUponAttacked.lightning != 0)
                {
                    itemTooltip.CreateTooltip(
                        lightningSprite,
                        lightning,
                        String.Format(
                            lightningDamageDealtToAttackingEnemies.GetLocalizedString(),
                            armor.damageDealtToEnemiesUponAttacked.lightning
                    ));

                }

                if (armor.damageDealtToEnemiesUponAttacked.magic != 0)
                {
                    itemTooltip.CreateTooltip(
                        magicSprite,
                        magic,
                        String.Format(
                            magicDamageDealtToAttackingEnemies.GetLocalizedString(),
                            armor.damageDealtToEnemiesUponAttacked.magic
                    ));

                }

                if (armor.damageDealtToEnemiesUponAttacked.darkness != 0)
                {
                    itemTooltip.CreateTooltip(
                        darknessSprite,
                        darkness,
                        String.Format(
                            darknessDamageDealtToAttackingEnemies.GetLocalizedString(),
                            armor.damageDealtToEnemiesUponAttacked.darkness
                    ));

                }

                if (armor.damageDealtToEnemiesUponAttacked.water != 0)
                {
                    itemTooltip.CreateTooltip(
                        waterSprite,
                        water,
                        String.Format(
                            magicDamageDealtToAttackingEnemies.GetLocalizedString(),
                            armor.damageDealtToEnemiesUponAttacked.water
                    ));

                }

                if (armor.damageDealtToEnemiesUponAttacked.statusEffects != null && armor.damageDealtToEnemiesUponAttacked.statusEffects.Length > 0)
                {
                    itemTooltip.CreateTooltip(statusEffectsSprite, Color.white, armor.GetFormattedDamageDealtToEnemiesUpponAttacked());
                }
            }

            if (armor.projectileMultiplierBonus > 0)
            {
                itemTooltip.CreateTooltip(
                    projectileSprite,
                    Color.white,
                    String.Format(
                        damageOnProjectilesBonus.GetLocalizedString(),
                        armor.projectileMultiplierBonus
                ));
            }*/
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
            if (statBonus != 0)
            {
                itemTooltip.CreateTooltip(statSprite, Color.white, label);
            }
        }
    }
}
