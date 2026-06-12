using UnityEngine;

namespace AF
{
    public static class LevelUtils
    {

        public static int GetRequiredExperienceForLevel(int level)
        {
            float baseXp = 100f;
            float exponent = 1.65f;

            // Remove 5 levels because we start out at level 5
            return Mathf.RoundToInt(baseXp * Mathf.Pow(Mathf.Clamp(level - 4, 1, Mathf.Infinity), exponent));
        }


    }
}
