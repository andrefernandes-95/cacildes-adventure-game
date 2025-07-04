namespace AF
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "Data / New Upgrade Material Data")]
    public class UpgradeMaterialData : ScriptableObject
    {
        [System.Serializable]
        public class UpgradeMaterialEntry
        {
            public UpgradeMaterial upgradeMaterial;
            public int amount = 1;
            public int goldCostForUpgrade = 100;
        }

        public UpgradeMaterialEntry[] upgradeMaterials;
    }
}
