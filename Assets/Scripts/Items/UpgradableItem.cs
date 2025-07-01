namespace AF
{
    using UnityEngine;

    public class UpgradableItem : Item
    {
        [Header("Level & Upgrades")]
        public bool canBeUpgraded = true;
        public int level = 1;

        public int GetBonusPerLevel(int level)
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
    }
}
