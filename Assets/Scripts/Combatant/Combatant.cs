namespace AF
{
    using AYellowpaper.SerializedCollections;
    using UnityEngine;
    using UnityEngine.Localization;

    [CreateAssetMenu(fileName = "Combatant", menuName = "Combatant / New Combatant", order = 0)]
    public class Combatant : ScriptableObject
    {
        [Header("Info")]
        public LocalizedString combatantName;

        [Header("Attributes")]
        public int maximumHealth = 500;
        public int maximumPosture = 100;
        public int maximumPoise = 3;

        [Header("Resistances")]
        public float pierceAbsorption = 1f;
        public float bluntAbsorption = 1f;
        public float slashAbsorption = 1f;
        public float fireAbsorption = 1f;
        public float frostAbsorption = 1f;
        public float lightningAbsorption = 1f;
        public float magicAbsorption = 1f;
        public float darknessAbsorption = 1f;
        public float waterAbsorption = 1f;

        [Header("Weaknesses")]
        public float pierceBonus = 1f;
        public float bluntBonus = 1f;
        public float slashBonus = 1f;
        public float fireBonus = 1f;
        public float frostBonus = 1f;
        public float lightningBonus = 1f;
        public float magicBonus = 1f;
        public float darknessBonus = 1f;
        public float waterBonus = 1f;

        [Header("Status Effect Maximum Resistances")]
        public int poisonResistance = 25;
        public int bleedResistance = 25;
        public int frostbiteResistance = 25;
        public int burntResistance = 25;
        public int curseResistance = 25;
        public int drownResistance = 25;
        public int confusionResistance = 25;
        public int paralysisResistance = 25;
        public int slownessResistance = 25;

    }
}
