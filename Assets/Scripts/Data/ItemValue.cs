using AF.Inventory;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Localization;

namespace AF
{
    [CreateAssetMenu(menuName = "Data / New Item Value")]
    public class ItemValue : ScriptableObject
    {
        public int value = 0;
    }
}
