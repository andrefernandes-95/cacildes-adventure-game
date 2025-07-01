namespace AF
{
    using System;
    using System.Collections.Generic;
    using AF.UI.EquipmentMenu;
    using UnityEngine;

    public class AccessoryTooltip : MonoBehaviour
    {
        [SerializeField] ItemTooltip itemTooltip;
        [SerializeField] GUIIconsDatabase GUIIconsDatabase;

        public void DrawAccessory(Accessory accessory)
        {
            if (accessory.GetShortDescription() != null && accessory.GetShortDescription().Length > 0)
            {
                itemTooltip.CreateTooltip(GUIIconsDatabase.statusEffects, Color.white, accessory.GetShortDescription());
            }

            CreateAttributeBonusTooltip(accessory);

            if (accessory.physicalAttackBonus != 0)
            {
                itemTooltip.CreateTooltip(
                    GUIIconsDatabase.physicalAttack,
                    Color.white,
                    $"+{accessory.physicalAttackBonus} {(Utils.IsPortuguese() ? "Ataque Físico" : "Physical Attack")}");
            }
            if (accessory.jumpAttackBonus != 0)
            {
                itemTooltip.CreateTooltip(
                    GUIIconsDatabase.physicalAttack,
                    Color.white,
                    $"+{accessory.jumpAttackBonus} {(Utils.IsPortuguese() ? "Dano em ataques aéreos" : "Damage while jump attacking")}");
            }
            if (accessory.increaseAttackPowerWithLowerHealth)
            {
                itemTooltip.CreateTooltip(
                    GUIIconsDatabase.physicalAttack,
                    Color.white,
                    $"{(Utils.IsPortuguese() ? "Aumenta o poder de ataque quanto menor for a tua vida" : "Increases attack power when your health is low")}");
            }
            if (accessory.increaseAttackPowerTheLowerTheReputation)
            {
                itemTooltip.CreateTooltip(
                    GUIIconsDatabase.physicalAttack,
                    Color.white,
                    $"{(Utils.IsPortuguese() ? "Aumenta o poder de ataque quanto menor for a tua reputação" : "Increases attack power with low reputation")}");
            }
            if (accessory.postureDamagePerParry != 0)
            {
                itemTooltip.CreateTooltip(
                    GUIIconsDatabase.posture,
                    Color.white,
                    $"+{accessory.postureDamagePerParry} {(Utils.IsPortuguese() ? "Dano de postura ao ripostar" : "Posture damage when parrying")}");
            }
            if (accessory.postureDecreaseRateBonus != 0)
            {
                string label = $"+{accessory.postureDecreaseRateBonus} ";

                if (Utils.IsPortuguese())
                {
                    label += "Velocidade de recuperação da postura";
                }
                else
                {
                    label += "Posture recovery speed";
                }

                itemTooltip.CreateTooltip(
                    GUIIconsDatabase.posture,
                    Color.white,
                    label);
            }
            if (accessory.spellDamageBonusMultiplier != 0)
            {
                itemTooltip.CreateTooltip(
                    GUIIconsDatabase.spell,
                    Color.white,
                    $"+{accessory.spellDamageBonusMultiplier} {(Utils.IsPortuguese() ? "Dano em todas as magias" : "Damage for all spells")}");
            }
            if (accessory.chanceToDoubleCoinsFromFallenEnemies)
            {
                string label = Utils.IsPortuguese()
                    ? "Chance de duplicar a quantidade de ouro obtida de inimigos derrotados"
                    : "Chance to double the amount of gold gained from defeated enemies";

                itemTooltip.CreateTooltip(
                    GUIIconsDatabase.gold,
                    Color.white,
                    label);
            }
            if (accessory.backStabAngleBonus != 0)
            {
                string label = $"+{accessory.backStabAngleBonus} ";

                if (Utils.IsPortuguese())
                {
                    label += "Ângulo de apunhalamento pelas costas";
                }
                else
                {
                    label += "Backstab angle";
                }

                itemTooltip.CreateTooltip(
                    GUIIconsDatabase.posture,
                    Color.white,
                    label);
            }

            CreateAttackMultipliers(accessory);
        }

        void CreateAttributeBonusTooltip(Accessory accessory)
        {
            if (accessory.healthBonus <= 0 && accessory.magicBonus <= 0 && accessory.staminaBonus <= 0)
            {
                return;
            }

            string label = "";

            Dictionary<string, (int value, string enText, string ptText, Color color)> damageTypes = new()
            {
                { "healthBonus",  (accessory.healthBonus,  "Health Points", "Pontos de Vida", GUIIconsDatabase.healthColor) },
                { "magicBonus",  (accessory.magicBonus,  "Mana Points", "Pontos de Mana", GUIIconsDatabase.manaColor) },
                { "staminaBonus",  (accessory.staminaBonus,  "Stamina Points", "Pontos de Stamina", GUIIconsDatabase.staminaColor) },
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

                    label += line + "\n";
                }
            }

            // Finally create the tooltip
            itemTooltip.CreateTooltip(
                GUIIconsDatabase.bonusStats,
                Color.white,
                label);
        }

        void CreateAttackMultipliers(Accessory accessory)
        {
            var bonusMultipliers = new Dictionary<string, (float value, string engLabel, string ptLabel, Texture2D icon)>
            {
                {
                    nameof(accessory.twoHandAttackBonusMultiplier),
                    (
                        accessory.twoHandAttackBonusMultiplier,
                        "Increases damage dealt when wielding weapons with both hands.",
                        "Aumenta o dano causado ao empunhar armas com as duas mãos.",
                        GUIIconsDatabase.heavyAttack
                    )
                },
                {
                    nameof(accessory.heavyAttackBonusMultiplier),
                    (
                        accessory.heavyAttackBonusMultiplier,
                        "Boosts damage of heavy attacks.",
                        "Aumenta o dano de ataques pesados.",
                        GUIIconsDatabase.heavyAttack
                    )
                },
                {
                    nameof(accessory.slashDamageMultiplier),
                    (
                        accessory.slashDamageMultiplier,
                        "Enhances damage from slashing attacks.",
                        "Aumenta o dano de ataques cortantes.",
                        GUIIconsDatabase.slash
                    )
                },
                {
                    nameof(accessory.bluntDamageMultiplier),
                    (
                        accessory.bluntDamageMultiplier,
                        "Increases damage from blunt attacks.",
                        "Aumenta o dano de ataques contundentes.",
                        GUIIconsDatabase.blunt
                    )
                },
                {
                    nameof(accessory.pierceDamageMultiplier),
                    (
                        accessory.pierceDamageMultiplier,
                        "Improves damage dealt by piercing attacks.",
                        "Aumenta o dano causado por ataques perfurantes.",
                        GUIIconsDatabase.pierce
                    )
                },
                {
                    nameof(accessory.projectileMultiplierBonus),
                    (
                        accessory.projectileMultiplierBonus,
                        "Raises damage of ranged projectile attacks.",
                        "Aumenta o dano de ataques à distância com projéteis.",
                        GUIIconsDatabase.range
                    )
                },
                {
                    nameof(accessory.footDamageMultiplier),
                    (
                        accessory.footDamageMultiplier,
                        "Increases damage dealt by kicking attacks.",
                        "Aumenta o dano causado por ataques com os pés.",
                        GUIIconsDatabase.feetAttack
                    )
                }
            };

            foreach (var kvp in bonusMultipliers)
            {
                var (value, engLabel, ptLabel, icon) = kvp.Value;
                if (value > 0f)
                {
                    string label = $"{value * 100}% " + (Utils.IsPortuguese() ? ptLabel : engLabel);
                    itemTooltip.CreateTooltip(icon, Color.white, label);
                }
            }
        }
    }
}
