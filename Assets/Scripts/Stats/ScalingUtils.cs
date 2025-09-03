using AF.Health;
using UnityEngine;

namespace AF
{
    public static class ScalingUtils
    {
        public enum StatType
        {
            STRENGTH,
            DEXTERITY,
            INTELLIGENCE,
        }

        public static int GetBonusAttackPerLevel(int level, StatType statType, Scaling grade)
        {
            if (level <= 0)
                return 0;

            float total = 0f;

            for (int i = 1; i <= level; i++)
            {
                total += GetBonusStep(i, statType);
            }

            // Apply scaling grade coefficient
            float scaledTotal = total * GetScalingCoefficient(grade);

            return Mathf.RoundToInt(scaledTotal);
        }

        /// <summary>
        /// Returns the bonus per stat level (Dark Souls diminishing returns).
        /// </summary>
        private static float GetBonusStep(int level, StatType statType)
        {
            switch (statType)
            {
                case StatType.STRENGTH:
                case StatType.DEXTERITY:
                    if (level <= 20) return 2.0f;
                    if (level <= 40) return 1.0f;
                    if (level <= 60) return 0.5f;
                    return 0.25f;

                case StatType.INTELLIGENCE:
                    if (level <= 20) return 3.0f;
                    if (level <= 40) return 1.5f;
                    if (level <= 60) return 0.75f;
                    return 0.25f;

                default:
                    return 0f;
            }
        }

        /// <summary>
        /// Converts scaling grade into a coefficient.
        /// </summary>
        private static float GetScalingCoefficient(Scaling grade)
        {
            switch (grade)
            {
                case Scaling.S: return 2.0f;
                case Scaling.A: return 1.75f;
                case Scaling.B: return 1.45f;
                case Scaling.C: return 1f;
                case Scaling.D: return 0.5f;
                case Scaling.E: return 0f;
                default: return 0f;
            }
        }

        public static Damage GetAbilityDamageForPlayerSpell(Damage abilityDamage, PlayerManager playerManager, Spell spell)
        {
            Damage scaledDamageForStats = abilityDamage.ScaleWithStats(
                playerManager.characterBaseStats.GetStrength(),
                playerManager.characterBaseStats.GetDexterity(),
                playerManager.characterBaseStats.GetIntelligence());

            if (spell == null)
            {
                return scaledDamageForStats;
            }

            if (!spell.AreRequirementsMet(playerManager))
            {
                scaledDamageForStats.Multiply(0.1f);
            }

            Damage scaledDamageFromSpellLevel = spell.GetSpellDamageForCurrentLevel(scaledDamageForStats);
            return scaledDamageFromSpellLevel;
        }
    }
}
