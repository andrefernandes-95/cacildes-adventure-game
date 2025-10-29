namespace AF
{
    using System;
    using System.Collections.Generic;
    using AF.Health;
    using AF.UI.EquipmentMenu;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class WeaponTooltip : MonoBehaviour
    {
        [SerializeField] ItemTooltip itemTooltip;
        [SerializeField] GUIIconsDatabase GUIIconsDatabase;
        [SerializeField] PlayerManager playerManager;

        public void DrawWeaponEffects(Weapon weapon)
        {

            (Damage weaponDamage, int STRBonus, int DEXBonus, int INTBonus, int TwoHandAttackBonus) = playerManager.characterBaseAttackManager.CalculateWeaponDamageForWeapon(weapon);

            int totalDamage = weaponDamage.GetTotalDamage() + TwoHandAttackBonus;

            string damageLabel = Utils.IsPortuguese()
                ? $"Poder de Ataque: {totalDamage}\n"
                : $"Attack Power: {totalDamage}\n";

            Dictionary<string, (int value, string enText, string ptText, Color color)> damageTypes = new()
            {
                { "physical",  (weaponDamage.basePhysicalDamage,  "Physical Attack", "Dano Físico",    Color.white) },
                { "fire",      (weaponDamage.fire,      "Fire Attack",     "Dano de Fogo",   GUIIconsDatabase.fireColor) },
                { "frost",     (weaponDamage.frost,     "Frost Attack",    "Dano de Gelo",   GUIIconsDatabase.frostColor) },
                { "lightning", (weaponDamage.lightning, "Lightning Attack","Dano Elétrico",  GUIIconsDatabase.lightningColor) },
                { "magic",     (weaponDamage.magic,     "Magic Attack",    "Dano Mágico",    GUIIconsDatabase.magicColor) },
                { "darkness",  (weaponDamage.darkness,  "Darkness Attack", "Dano de Trevas", GUIIconsDatabase.darknessColor) },
                { "water",     (weaponDamage.water,     "Water Attack",    "Dano Aquático",  GUIIconsDatabase.waterColor) }
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

            if (STRBonus > 0)
            {
                damageLabel += Utils.IsPortuguese()
                ? $"<size=80%>+{STRBonus} Bónus de Força ({weapon.damage.strengthScaling})\n"
                : $"<size=80%>+{STRBonus} Strength Bonus ({weapon.damage.strengthScaling})\n";
            }

            if (DEXBonus > 0)
            {
                damageLabel += Utils.IsPortuguese()
                ? $"<size=80%>+{DEXBonus} Bónus de Destreza ({weapon.damage.dexterityScaling})\n"
                : $"<size=80%>+{DEXBonus} Dexterity Bonus ({weapon.damage.dexterityScaling})\n";
            }

            if (INTBonus > 0)
            {
                damageLabel += Utils.IsPortuguese()
                ? $"<size=80%>+{INTBonus} Bónus de Inteligência ({weapon.damage.intelligenceScaling})\n"
                : $"<size=80%>+{INTBonus} Intelligence Bonus ({weapon.damage.intelligenceScaling})\n";
            }

            if (TwoHandAttackBonus > 0)
            {
                damageLabel += Utils.IsPortuguese()
                ? $"<size=80%>+{TwoHandAttackBonus} Bónus de empunhar arma com as duas mãos ({TwoHandAttackBonus})\n"
                : $"<size=80%>+{TwoHandAttackBonus} Bonus from two-handing weapon ({TwoHandAttackBonus})\n";
            }

            // Finally create the tooltip
            itemTooltip.CreateTooltip(
                GUIIconsDatabase.physicalAbsorption,
                Color.white,
                damageLabel);

            // === Attack Type (Slash, Blunt, Pierce) ===
            if (weaponDamage.weaponAttackType == WeaponAttackType.Blunt)
            {
                itemTooltip.weaponTypeLabel.text = Utils.IsPortuguese() ? "Dano Contundente" : "Blunt Damage";
                itemTooltip.weaponTypeLabel.style.color = GUIIconsDatabase.bluntColor;
                itemTooltip.weaponTypeLabel.style.display = DisplayStyle.Flex;
            }
            else if (weaponDamage.weaponAttackType == WeaponAttackType.Slash)
            {
                itemTooltip.weaponTypeLabel.text = Utils.IsPortuguese() ? "Dano Cortante" : "Slash Damage";
                itemTooltip.weaponTypeLabel.style.color = GUIIconsDatabase.slashColor;
                itemTooltip.weaponTypeLabel.style.display = DisplayStyle.Flex;
            }
            else if (weaponDamage.weaponAttackType == WeaponAttackType.Pierce)
            {
                itemTooltip.weaponTypeLabel.text = Utils.IsPortuguese() ? "Dano Perfurante" : "Pierce Damage";
                itemTooltip.weaponTypeLabel.style.color = GUIIconsDatabase.pierceColor;
                itemTooltip.weaponTypeLabel.style.display = DisplayStyle.Flex;
            }
            else if (weaponDamage.weaponAttackType == WeaponAttackType.Range)
            {
                itemTooltip.weaponTypeLabel.text = Utils.IsPortuguese() ? "Dano de Longo-Alcance" : "Ranged Damage";
                itemTooltip.weaponTypeLabel.style.color = GUIIconsDatabase.rangeColor;
                itemTooltip.weaponTypeLabel.style.display = DisplayStyle.Flex;
            }

            // === Status Effects ===
            itemTooltip.DrawStatusEffects(weapon.damage);

            // === Requirements ===
            if (weapon.HasRequirements())
            {
                itemTooltip.CreateTooltip(
                    GUIIconsDatabase.requirements,
                    weapon.AreRequirementsMet(playerManager) ? Color.white : GUIIconsDatabase.requirementsNotMetColor,
                    weapon.DrawRequirements(playerManager));
            }

            if (weapon.damage.poiseDamage > 0)
            {
                string label = $"{weapon.damage.poiseDamage} ";

                label += Utils.IsPortuguese()
                    ? "Dano de Equilíbrio\n<i><size=80%>(Reduz a resistência do inimigo a ser interrompido durante ataques)</i>"
                    : "Poise Damage\n<i><size=80%>(Reduces enemy's ability to resist being staggered when hit)</i>";

                itemTooltip.CreateTooltip(GUIIconsDatabase.pushForce, Color.white, label);
            }

            if (weapon.damage.postureDamage > 0)
            {
                string label = $"{weapon.damage.postureDamage} ";

                label += Utils.IsPortuguese()
                    ? "Dano de Postura\n<i><size=80%>(Acumula na barra amarela do inimigo. Ao encher, permite um ataque crítico)</i>"
                    : "Posture Damage\n<i><size=80%>(Builds up the enemy's yellow bar. When full, allows a critical attack)</i>";

                itemTooltip.CreateTooltip(GUIIconsDatabase.posture, Color.white, label);
            }

            if (weapon.damage.pushForce > 0)
            {
                string label = $"{weapon.damage.pushForce} ";

                label += Utils.IsPortuguese()
                    ? "Força de Impacto\n<i><size=80%>(Empurra os inimigos ao acertar)</i>"
                    : "Impact Force\n<i><size=80%>(Pushes enemies back on hit)</i>";

                itemTooltip.CreateTooltip(GUIIconsDatabase.pushForce, Color.white, label);
            }

            if (weapon.staminaCostPerAttack > 0)
            {
                string label = $"-{weapon.GetLightAttackStaminaCost()} ";

                label += Utils.IsPortuguese()
                    ? "Stamina por ataque"
                    : "Stamina per attack";

                label += "\n";
                label += $"-{weapon.GetHeavyAttackStaminaCost()} ";

                label += Utils.IsPortuguese()
                    ? "Stamina por ataque forte"
                    : "Stamina per heavy attack";

                itemTooltip.CreateTooltip(GUIIconsDatabase.staminaCost, Color.white, label);
            }

            // === Upgradeable ===
            if (weapon.canBeUpgraded && weapon.CanBeUpgradedFurther())
            {
                itemTooltip.CreateTooltip(GUIIconsDatabase.upgradeItem, Color.white, weapon.GetMaterialCostForNextLevel(playerManager));
            }

            if (weapon.damage.ignoreBlocking)
            {
                string label = Utils.IsPortuguese()
                    ? "Ignora escudos"
                    : "Ignores shields";

                itemTooltip.CreateTooltip(GUIIconsDatabase.physicalAbsorption, Color.white, label);
            }

            if (weapon.damage.canNotBeParried)
            {
                string label = Utils.IsPortuguese()
                    ? "Não pode ser ripostada"
                    : "Can not be parried";

                itemTooltip.CreateTooltip(GUIIconsDatabase.physicalAbsorption, Color.white, label);
            }

            DrawAbsorptions(weapon);

            // === Life Steal ===
            if (weapon.healthRestoredWithEachHit > 0)
            {
                string label = $"+{weapon.healthRestoredWithEachHit} ";

                label += Utils.IsPortuguese()
                    ? "Vida recuperada a cada golpe certeiro"
                    : "HP restored with each successful hit";

                itemTooltip.CreateTooltip(GUIIconsDatabase.vitality, Color.white, label);
            }


            foreach (WeaponEffect weaponEffect in weapon.weaponEffects)
            {
                // Finally create the tooltip
                if (weaponEffect != null)
                {
                    itemTooltip.CreateTooltip(
                        GUIIconsDatabase.bonusStats,
                        Color.white,
                        weaponEffect.GetWeaponEffectTooltip());
                }
            }
        }

        void DrawAbsorptions(Weapon weapon)
        {

            float physicalBlockAbsorption = 0f;
            float fireBlockAbsorption = 0f;
            float frostBlockAbsorption = 0f;
            float lightningBlockAbsorption = 0f;
            float magicBlockAbsorption = 0f;
            float darknessBlockAbsorption = 0f;
            float waterBlockAbsorption = 0f;

            if (weapon is Shield shield)
            {
                if (shield.GetCurrentAbsorption(shield.physicalAbsorption) != -1f)
                {
                    physicalBlockAbsorption = shield.GetCurrentAbsorption(shield.physicalAbsorption);
                }

                if (shield.GetCurrentAbsorption(shield.fireAbsorption) != -1f)
                {
                    fireBlockAbsorption = shield.GetCurrentAbsorption(shield.fireAbsorption);
                }

                if (shield.GetCurrentAbsorption(shield.frostAbsorption) != -1f)
                {
                    frostBlockAbsorption = shield.GetCurrentAbsorption(shield.frostAbsorption);
                }

                if (shield.GetCurrentAbsorption(shield.lightiningAbsorption) != -1f)
                {
                    lightningBlockAbsorption = shield.GetCurrentAbsorption(shield.lightiningAbsorption);
                }

                if (shield.GetCurrentAbsorption(shield.magicAbsorption) != -1f)
                {
                    magicBlockAbsorption = shield.GetCurrentAbsorption(shield.magicAbsorption);
                }

                if (shield.GetCurrentAbsorption(shield.darknessAbsorption) != -1f)
                {
                    darknessBlockAbsorption = shield.GetCurrentAbsorption(shield.darknessAbsorption);
                }

                if (shield.GetCurrentAbsorption(shield.waterAbsorption) != -1f)
                {
                    waterBlockAbsorption = shield.GetCurrentAbsorption(shield.waterAbsorption);
                }
            }
            else if (weapon.weaponBlockAbsorption != 1f)
            {
                physicalBlockAbsorption = weapon.weaponBlockAbsorption;
            }

            DrawAbsorptionTooltip(GUIIconsDatabase.physicalAbsorption, Color.white, Utils.IsPortuguese() ? "Físico" : "Physical", physicalBlockAbsorption);
            DrawAbsorptionTooltip(GUIIconsDatabase.fireAbsorption, GUIIconsDatabase.fireColor, Utils.IsPortuguese() ? "de Fogo" : "Fire", fireBlockAbsorption);
            DrawAbsorptionTooltip(GUIIconsDatabase.frostAbsorption, GUIIconsDatabase.frostColor, Utils.IsPortuguese() ? "de Gelo" : "Frost", frostBlockAbsorption);
            DrawAbsorptionTooltip(GUIIconsDatabase.lightningAbsorption, GUIIconsDatabase.lightningColor, Utils.IsPortuguese() ? "Elétrico" : "Lightning", lightningBlockAbsorption);
            DrawAbsorptionTooltip(GUIIconsDatabase.magicAbsorption, GUIIconsDatabase.magicColor, Utils.IsPortuguese() ? "Mágico" : "Magic", magicBlockAbsorption);
            DrawAbsorptionTooltip(GUIIconsDatabase.darknessAbsorption, GUIIconsDatabase.darknessColor, Utils.IsPortuguese() ? "das Trevas" : "Dark", darknessBlockAbsorption);
            DrawAbsorptionTooltip(GUIIconsDatabase.waterAbsorption, GUIIconsDatabase.waterColor, Utils.IsPortuguese() ? "Aquático" : "Water", waterBlockAbsorption);
        }

        void DrawAbsorptionTooltip(Texture2D sprite, Color color, string damageType, float absorption)
        {
            if (absorption == 0)
            {
                return;
            }

            float absorptionPercentage = absorption * 100f;
            float damageStillTaken = 100f - absorptionPercentage;

            string label = Utils.IsPortuguese()
                ? $"{absorptionPercentage:0}% Dano {damageType} Absorvido\n<i><size=80%>({damageStillTaken:0}% do dano ainda será sofrido)</i>"
                : $"{absorptionPercentage:0}% {damageType} Damage Reduction\n<i><size=80%>({damageStillTaken:0}% of the damage will still go through)</i>";

            itemTooltip.CreateTooltip(sprite, color, label);
        }
    }
}
