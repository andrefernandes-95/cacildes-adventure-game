namespace AF
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "Data / Spells / New Spell Type")]
    public class SpellType : ScriptableObject
    {
        public int manaCostPerCast = 20;
        public int staminaCostPerCast = 20;

        [Header("Requirements")]
        public int intelligenceRequired = 0;

    }

}
