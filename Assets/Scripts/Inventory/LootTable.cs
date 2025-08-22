using System.Collections.Generic;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(fileName = "Loot Table", menuName = "Data / New Loot Table", order = 0)]
    public class LootTable : ScriptableObject
    {
        [System.Serializable]
        public class LootItem
        {
            public Item item;
            public int amount = 1;
            [Range(0f, 100f)] public float chanceToGet = 100f;
        }

        public List<LootItem> loot = new();

    }
}