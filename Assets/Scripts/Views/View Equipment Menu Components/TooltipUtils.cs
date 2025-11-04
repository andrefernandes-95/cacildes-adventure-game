using UnityEngine.Localization.Settings;

namespace AF
{
    public static class TooltipUtils
    {

        public static string GetLightiningDamageExplanation(int baseLightningAttack, int holyDamageScaleFromReputation, int intelligenceBonusFromWeapon)
        {
            string damageExplained = "";

            if (LocalizationSettings.SelectedLocale.Identifier.Code == "en")
            {
                damageExplained += $"Total Lightning Damage: {baseLightningAttack + holyDamageScaleFromReputation + intelligenceBonusFromWeapon}\n\n";

                damageExplained += "Explanation: \n";
                damageExplained += $"+{baseLightningAttack} from weapon's lightning damage";

                if (holyDamageScaleFromReputation > 0)
                {
                    damageExplained += $"\n+{holyDamageScaleFromReputation} from high reputation";
                }
                if (intelligenceBonusFromWeapon > 0)
                {
                    damageExplained += $"\n+{intelligenceBonusFromWeapon} from intelligence bonus";
                }
            }
            else
            {
                damageExplained += $"Dano de Trovão Total: {baseLightningAttack + holyDamageScaleFromReputation + intelligenceBonusFromWeapon}\n\n";

                damageExplained += "Explicação: \n";
                damageExplained += $"+{baseLightningAttack} dano de trovão da arma";

                if (holyDamageScaleFromReputation > 0)
                {
                    damageExplained += $"\n+{holyDamageScaleFromReputation} de reputação alta";
                }
                if (intelligenceBonusFromWeapon > 0)
                {
                    damageExplained += $"\n+{intelligenceBonusFromWeapon} de bónus de inteligência";
                }
            }

            return damageExplained;
        }

        public static string GetDarknessDamageExplanation(int baseAttack, int damageFromReputation, int intelligenceBonusFromWeapon)
        {
            string damageExplained = "";

            if (LocalizationSettings.SelectedLocale.Identifier.Code == "en")
            {
                damageExplained += $"Total Darkness Damage: {baseAttack + damageFromReputation + intelligenceBonusFromWeapon}\n\n";

                damageExplained += "Explanation: \n";
                damageExplained += $"+{baseAttack} from weapon's dark damage";

                if (damageFromReputation > 0)
                {
                    damageExplained += $"\n+{damageFromReputation} from low reputation";
                }
                if (intelligenceBonusFromWeapon > 0)
                {
                    damageExplained += $"\n+{intelligenceBonusFromWeapon} from intelligence bonus";
                }
            }
            else
            {
                damageExplained += $"Dano de Trevas Total: {baseAttack + damageFromReputation + intelligenceBonusFromWeapon}\n\n";

                damageExplained += "Explicação: \n";
                damageExplained += $"+{baseAttack} dano de trevas da arma";

                if (damageFromReputation > 0)
                {
                    damageExplained += $"\n+{damageFromReputation} de reputação baixa";
                }
                if (intelligenceBonusFromWeapon > 0)
                {
                    damageExplained += $"\n+{intelligenceBonusFromWeapon} de bónus de inteligência";
                }
            }

            return damageExplained;
        }

        public static string GetMagicDamageExplanation(PlayerManager playerManager, Weapon weapon)
        {
            CharacterBaseAttackManager attackStatManager = playerManager.characterBaseAttackManager;
            int totalMagicDamage = weapon.GetWeaponMagicAttack(attackStatManager);
            int baseMagicDamage = weapon.GetWeaponBaseMagicAttack();
            int damageFromIntelligenceScaling = (int)weapon.damage.GetIntelligenceBonus(playerManager.playerStats.GetIntelligence());
            string damageExplanation = "";

            if (LocalizationSettings.SelectedLocale.Identifier.Code == "en")
            {
                damageExplanation += $"Total Magic Damage: {totalMagicDamage}\n\n";

                damageExplanation += "Explanation: \n";
                damageExplanation += $"+{baseMagicDamage} weapon base magic damage\n";

                if (damageFromIntelligenceScaling > 0)
                {
                    damageExplanation += $"+{damageFromIntelligenceScaling} from intelligence bonus ({weapon.intelligenceScaling} Scaling)\n";
                }
            }
            else if (LocalizationSettings.SelectedLocale.Identifier.Code == "pt")
            {
                damageExplanation += $"Dano Mágico Total: {totalMagicDamage}\n\n";

                damageExplanation += "Explicação: \n";
                damageExplanation += $"+{baseMagicDamage} dano mágico da arma\n";

                if (damageFromIntelligenceScaling > 0)
                {
                    damageExplanation += $"+{damageFromIntelligenceScaling} de bónus de inteligência ({weapon.intelligenceScaling} Escala)\n";
                }
            }

            return damageExplanation;
        }

        public static string GetArrowPhysicalDamage(int damage)
        {
            string damageExplained = "";

            if (Utils.IsPortuguese())
            {
                damageExplained += $"+{damage} de Ataque Físico";
            }
            else
            {
                damageExplained += $"+{damage} Physical Attack";
            }

            return damageExplained;
        }

        public static string GetArrowLightiningDamage(int damage)
        {
            string damageExplained = "";

            if (Utils.IsPortuguese())
            {
                damageExplained += $"+{damage} de Ataque de Trovão";
            }
            else
            {
                damageExplained += $"+{damage} Lightning Attack";
            }

            return damageExplained;
        }

        public static string GetArrowDarknessDamage(int damage)
        {
            string damageExplained = "";

            if (Utils.IsPortuguese())
            {
                damageExplained += $"+{damage} de Ataque de Trevas";
            }
            else
            {
                damageExplained += $"+{damage} Darkness Attack";
            }

            return damageExplained;
        }

        public static string GetArrowMagicDamage(int damage)
        {
            string damageExplained = "";

            if (Utils.IsPortuguese())
            {
                damageExplained += $"+{damage} de Ataque Nágico";
            }
            else
            {
                damageExplained += $"+{damage} Magic Attack";
            }

            return damageExplained;
        }
    }
}
