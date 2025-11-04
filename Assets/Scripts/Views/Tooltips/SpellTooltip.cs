namespace AF
{
    using System.Collections.Generic;
    using AF.Health;
    using AF.UI.EquipmentMenu;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class SpellTooltip : MonoBehaviour
    {
        [SerializeField] ItemTooltip itemTooltip;
        [SerializeField] GUIIconsDatabase GUIIconsDatabase;
        [SerializeField] PlayerManager playerManager;

        public void DrawSpellEffects(Spell spell)
        {
            Damage spellDamage = ScalingUtils.GetAbilityDamageForPlayerSpell(
                spell.ability.GetDamage(playerManager),
                playerManager,
                spell);

            int bonusFromStrength = ScalingUtils.GetBonusAttackPerLevel(playerManager.characterBaseStats.GetStrength(), ScalingUtils.StatType.STRENGTH, spellDamage.strengthScaling);
            int bonusFromDexterity = ScalingUtils.GetBonusAttackPerLevel(playerManager.characterBaseStats.GetDexterity(), ScalingUtils.StatType.DEXTERITY, spellDamage.dexterityScaling);
            int bonusFromIntelligence = ScalingUtils.GetBonusAttackPerLevel(playerManager.characterBaseStats.GetIntelligence(), ScalingUtils.StatType.INTELLIGENCE, spellDamage.intelligenceScaling);

            int totalDamage = spellDamage.GetTotalDamage();

            string damageLabel = Utils.IsPortuguese()
                ? $"Poder de Ataque: {totalDamage}\n"
                : $"Attack Power: {totalDamage}\n";

            Dictionary<string, (int value, string enText, string ptText, Color color)> damageTypes = new()
            {
                { "physical",  (spellDamage.physical - bonusFromStrength - bonusFromDexterity,  "Physical Attack", "Dano Físico",    Color.white) },
                { "fire",      (spellDamage.fire - bonusFromIntelligence,      "Fire Attack",     "Dano de Fogo",   GUIIconsDatabase.fireColor) },
                { "frost",     (spellDamage.frost - bonusFromIntelligence,     "Frost Attack",    "Dano de Gelo",   GUIIconsDatabase.frostColor) },
                { "lightning", (spellDamage.lightning - bonusFromIntelligence, "Lightning Attack","Dano Elétrico",  GUIIconsDatabase.lightningColor) },
                { "magic",     (spellDamage.magic - bonusFromIntelligence,     "Magic Attack",    "Dano Mágico",    GUIIconsDatabase.magicColor) },
                { "darkness",  (spellDamage.darkness - bonusFromIntelligence,  "Darkness Attack", "Dano de Trevas", GUIIconsDatabase.darknessColor) },
                { "water",     (spellDamage.water - bonusFromIntelligence,     "Water Attack",    "Dano Aquático",  GUIIconsDatabase.waterColor) }
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

            if (bonusFromStrength > 0)
            {
                damageLabel += Utils.IsPortuguese()
                ? $"<size=80%>+{bonusFromStrength} Bónus de Força ({spellDamage.strengthScaling})\n"
                : $"<size=80%>+{bonusFromStrength} Strength Bonus ({spellDamage.strengthScaling})\n";
            }

            if (bonusFromDexterity > 0)
            {
                damageLabel += Utils.IsPortuguese()
                ? $"<size=80%>+{bonusFromDexterity} Bónus de Destreza ({spellDamage.dexterityScaling})\n"
                : $"<size=80%>+{bonusFromDexterity} Dexterity Bonus ({spellDamage.dexterityScaling})\n";
            }

            if (bonusFromIntelligence > 0)
            {
                damageLabel += Utils.IsPortuguese()
                ? $"<size=80%>+{bonusFromIntelligence} Bónus de Inteligência ({spellDamage.intelligenceScaling})\n"
                : $"<size=80%>+{bonusFromIntelligence} Intelligence Bonus ({spellDamage.intelligenceScaling})\n";
            }

            // Finally create the tooltip
            itemTooltip.CreateTooltip(
                GUIIconsDatabase.physicalAbsorption,
                Color.white,
                damageLabel);

            // === Status Effects ===
            itemTooltip.DrawStatusEffects(spellDamage);

            // === Requirements ===
            if (spell.HasRequirements())
            {
                itemTooltip.CreateTooltip(
                    GUIIconsDatabase.requirements,
                    spell.AreRequirementsMet(playerManager) ? Color.white : GUIIconsDatabase.requirementsNotMetColor,
                    spell.DrawRequirements(playerManager));
            }

            if (spellDamage.poiseDamage > 0)
            {
                string label = $"{spellDamage.poiseDamage} ";

                label += Utils.IsPortuguese()
                    ? "Dano de Equilíbrio\n<i><size=80%>(Reduz a resistência do inimigo a ser interrompido durante ataques)</i>"
                    : "Poise Damage\n<i><size=80%>(Reduces enemy's ability to resist being staggered when hit)</i>";

                itemTooltip.CreateTooltip(GUIIconsDatabase.pushForce, Color.white, label);
            }

            if (spellDamage.postureDamage > 0)
            {
                string label = $"{spellDamage.postureDamage} ";

                label += Utils.IsPortuguese()
                    ? "Dano de Postura\n<i><size=80%>(Acumula na barra amarela do inimigo. Ao encher, permite um ataque crítico)</i>"
                    : "Posture Damage\n<i><size=80%>(Builds up the enemy's yellow bar. When full, allows a critical attack)</i>";

                itemTooltip.CreateTooltip(GUIIconsDatabase.posture, Color.white, label);
            }

            if (spellDamage.pushForce > 0)
            {
                string label = $"{spellDamage.pushForce} ";

                label += Utils.IsPortuguese()
                    ? "Força de Impacto\n<i><size=80%>(Empurra os inimigos ao acertar)</i>"
                    : "Impact Force\n<i><size=80%>(Pushes enemies back on hit)</i>";

                itemTooltip.CreateTooltip(GUIIconsDatabase.pushForce, Color.white, label);
            }

            if (spell.GetManaCost() > 0)
            {
                string label = $"-{spell.staminaCostPerCast} ";

                label += Utils.IsPortuguese()
                    ? "Mana para lançar"
                    : "Mana to cast";
                itemTooltip.CreateTooltip(GUIIconsDatabase.magic, Color.white, label);
            }

            if (spell.staminaCostPerCast > 0)
            {
                string label = $"-{spell.staminaCostPerCast} ";

                label += Utils.IsPortuguese()
                    ? "Stamina por ataque"
                    : "Stamina per attack";
                itemTooltip.CreateTooltip(GUIIconsDatabase.staminaCost, Color.white, label);
            }

            // === Upgradeable ===
            if (spell.canBeUpgraded && spell.CanBeUpgradedFurther())
            {
                itemTooltip.CreateTooltip(GUIIconsDatabase.upgradeItem, Color.white, spell.GetMaterialCostForNextLevel(playerManager));
            }

            if (spellDamage.ignoreBlocking)
            {
                string label = Utils.IsPortuguese()
                    ? "Ignora escudos"
                    : "Ignores shields";

                itemTooltip.CreateTooltip(GUIIconsDatabase.physicalAbsorption, Color.white, label);
            }

            if (spellDamage.canNotBeParried)
            {
                string label = Utils.IsPortuguese()
                    ? "Não pode ser ripostada"
                    : "Can not be parried";

                itemTooltip.CreateTooltip(GUIIconsDatabase.physicalAbsorption, Color.white, label);
            }

            float physicalBlockAbsorption = 0f;

            if (physicalBlockAbsorption != 0f)
            {
                float absorptionPercentage = physicalBlockAbsorption * 100f;
                float damageStillTaken = 100f - absorptionPercentage;

                string label = Utils.IsPortuguese()
                    ? $"{absorptionPercentage:0}% Absorção ao bloquear com a arma\n<i><size=80%>(Ao bloquear com esta arma, {damageStillTaken:0}% do dano ainda será sofrido)</i>"
                    : $"{absorptionPercentage:0}% Absorption when blocking with this weapon\n<i><size=80%>({damageStillTaken:0}% of the damage will still go through when blocking with this weapon)</i>";

                itemTooltip.CreateTooltip(GUIIconsDatabase.physicalAbsorption, Color.white, label);
            }

        }
    }
}
