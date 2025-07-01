namespace AF
{
    using System.Collections.Generic;
    using AF.UI.EquipmentMenu;
    using UnityEngine;

    public class WeaponTooltip : MonoBehaviour
    {
        [SerializeField] ItemTooltip itemTooltip;
        [SerializeField] GUIIconsDatabase GUIIconsDatabase;
        [SerializeField] PlayerManager playerManager;

        void DrawWeaponEffects(Weapon weapon)
        {
            Dictionary<string, (int value, Texture2D icon, Color color, string enText, string ptText)> elementalDamages = new()
            {
                { "Physical", (playerManager.characterBaseAttackManager.GetWeaponAttack(weapon), GUIIconsDatabase.physicalAttack, Color.white, "Physical Attack Power", "Poder de Ataque Físico") },
                { "Fire", (weapon.GetWeaponFireAttack(playerManager.characterBaseAttackManager), GUIIconsDatabase.fire, GUIIconsDatabase.fireColor, "Fire Attack", "Ataque de Fogo") },
                { "Frost", ((int)(weapon.GetWeaponFrostAttack(playerManager.characterBaseAttackManager) + playerManager.characterBaseAttackManager.GetIntelligenceBonusFromWeapon(weapon)), GUIIconsDatabase.frost, GUIIconsDatabase.frostColor, "Frost Attack", "Ataque de Gelo") },
                { "Lightning", (weapon.GetWeaponLightningAttack(playerManager.playerStatsDatabase.GetCurrentReputation(), playerManager.characterBaseAttackManager), GUIIconsDatabase.lightning, GUIIconsDatabase.lightningColor, "Lightning Attack", "Ataque Elétrico") },
                { "Magic", (weapon.GetWeaponMagicAttack(playerManager.characterBaseAttackManager), GUIIconsDatabase.magic, GUIIconsDatabase.magicColor, "Magic Attack", "Ataque Mágico") },
                { "Darkness", (weapon.GetWeaponDarknessAttack(playerManager.playerStatsDatabase.GetCurrentReputation(), playerManager.characterBaseAttackManager), GUIIconsDatabase.darkness, GUIIconsDatabase.darknessColor, "Darkness Attack", "Ataque de Trevas") }
            };

            foreach (var entry in elementalDamages)
            {
                if (entry.Value.value > 0)
                {
                    string label = Utils.IsPortuguese() ? $"{entry.Value.value} {entry.Value.ptText}" : $"{entry.Value.value} {entry.Value.enText}";
                    itemTooltip.CreateTooltip(entry.Value.icon, entry.Value.color, label);
                }
            }

            // === Attack Type (Slash, Blunt, Pierce) ===
            Dictionary<WeaponAttackType, (Texture2D icon, string enText, string ptText)> attackTypes = new()
            {
                { WeaponAttackType.Blunt, (GUIIconsDatabase.blunt, "Blunt Damage Type", "Tipo de Dano Contundente") },
                { WeaponAttackType.Pierce, (GUIIconsDatabase.pierce, "Piercing Damage Type", "Tipo de Dano Perfurante") },
                { WeaponAttackType.Slash, (GUIIconsDatabase.slash, "Slashing Damage Type", "Tipo de Dano Cortante") }
            };

            if (attackTypes.ContainsKey(weapon.damage.weaponAttackType))
            {
                var typeInfo = attackTypes[weapon.damage.weaponAttackType];
                string label = Utils.IsPortuguese() ? typeInfo.ptText : typeInfo.enText;
                itemTooltip.CreateTooltip(typeInfo.icon, Color.white, label);
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
                itemTooltip.CreateTooltip(GUIIconsDatabase.upgradeItem, Color.white, weapon.GetMaterialCostForNextLevel());
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

            if (weapon.blockAbsorption != 1f)
            {

                float absorptionPercentage = weapon.blockAbsorption * 100f;
                float damageTaken = 100f - absorptionPercentage;

                string label = Utils.IsPortuguese()
                    ? $"{absorptionPercentage:0}% Absorção ao bloquear com a arma\n<i><size=80%>(Ao bloquear com esta arma, {damageTaken:0}% do dano ainda será sofrido)</i>"
                    : $"{absorptionPercentage:0}% Absorption when blocking with this weapon\n<i><size=80%>({damageTaken:0}% of the damage will still go through when blocking with this weapon)</i>";

                itemTooltip.CreateTooltip(GUIIconsDatabase.physicalAbsorption, Color.white, label);
            }

            // === Life Steal ===
            if (weapon.healthRestoredWithEachHit > 0)
            {
                string label = $"+{weapon.healthRestoredWithEachHit} ";

                label += Utils.IsPortuguese()
                    ? "Vida recuperada a cada golpe certeiro"
                    : "HP restored with each successful hit";

                itemTooltip.CreateTooltip(GUIIconsDatabase.vitality, Color.white, label);
            }
        }
    }
}
