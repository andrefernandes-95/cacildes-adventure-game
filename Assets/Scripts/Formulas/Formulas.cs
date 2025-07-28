
using UnityEngine;

namespace AF
{

    public static class Formulas
    {
        [Header("Scaling Multipliers")]
        public static float E = 0;
        public static float D = .45f;
        public static float C = .85f;
        public static float B = 1.55f;
        public static float A = 2.45f;
        public static float S = 3.25f;
        public static float levelMultiplier = 1.25f;

        public static int CalculateStatForLevel(int baseValue, int level, float multiplier)
        {
            return (int)Mathf.Sqrt(level * multiplier) * 2 + baseValue + level;
        }


    }

}