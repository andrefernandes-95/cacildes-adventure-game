namespace AF
{
    using UnityEngine;

    public class UpgradableItem : Item
    {
        [Header("Level & Upgrades")]
        public bool canBeUpgraded = true;
        public int level = 1;
        public UpgradeMaterialData upgradeMaterialData;

        public int GetBonusAttackPerLevel(int level)
        {
            if (level == 0)
            {
                return 0;
            }

            int total = 0;

            for (int i = 1; i <= level; i++)
            {
                if (i <= 3)
                    total += 10;
                else if (i <= 6)
                    total += 20;
                else if (i <= 9)
                    total += 30;
                else
                    total += 40;
            }

            return total;
        }

        public int GetBonusPoisePerLevel(int initialPoise, int level)
        {
            if (level == 0 || initialPoise == 0)
            {
                return 0;
            }

            float total = initialPoise;

            for (int i = 1; i <= level; i++)
            {
                if (i <= 3)
                    total += .25f;
                else if (i <= 6)
                    total += .5f;
                else if (i <= 9)
                    total += 0.75f;
                else
                    total += 1f;
            }

            return Mathf.CeilToInt(total);
        }

        public int GetBonusPosturePerLevel(int initialPosture, int level)
        {
            if (level == 0 || initialPosture == 0)
            {
                return 0;
            }

            float total = initialPosture;

            for (int i = 1; i <= level; i++)
            {
                if (i <= 3)
                    total += 1.5f;
                else if (i <= 6)
                    total += 2.5f;
                else if (i <= 9)
                    total += 3.5f;
                else
                    total += 4.5f;
            }

            return Mathf.CeilToInt(total);
        }

        public int GetBonusStatusEffectAmountPerHitPerLevel(float initialAmountPerHit, int level)
        {
            if (level == 0 || initialAmountPerHit <= 0)
            {
                return 0;
            }

            float total = initialAmountPerHit;

            for (int i = 1; i <= level; i++)
            {
                if (i <= 3)
                    total += 0.75f;
                else if (i <= 6)
                    total += 1.25f;
                else if (i <= 9)
                    total += 1.75f;
                else
                    total += 3f;
            }

            return Mathf.CeilToInt(total);
        }
    }
}
