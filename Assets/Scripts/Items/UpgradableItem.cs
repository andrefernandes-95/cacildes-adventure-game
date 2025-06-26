namespace AF
{
    using UnityEngine;

    public class UpgradableItem : Item
    {
        [Header("Level & Upgrades")]
        public bool canBeUpgraded = true;
        public int level = 1;
    }
}
