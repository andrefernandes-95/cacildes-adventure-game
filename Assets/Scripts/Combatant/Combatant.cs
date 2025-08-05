namespace AF
{
    using System.Collections.Generic;
    using AF.Health;
    using AF.Inventory;
    using AYellowpaper.SerializedCollections;
    using UnityEngine;
    using UnityEngine.Localization;
    [CreateAssetMenu(fileName = "Combatant", menuName = "Combatant / New Combatant", order = 0)]
    public class Combatant : ScriptableObject
    {
        [Header("Info")]
        public LocalizedString combatantName;
        public bool isHumanoid = true;

        [Header("Locomotion Settings")]
        public bool canJumpToReachTarget = true;

        [Header("Stats")]
        public int vitality = 1;
        public int endurance = 1;
        public int intelligence = 1;
        public int strength = 1;
        public int dexterity = 1;

        [Header("Other Stats")]
        public int reputation = 1;

        [Header("Attributes")]
        public int maximumHealth = 500;
        public int maximumPosture = 100;
        public int maximumPoise = 3;
        public int maximumCarryingWeight = 100;

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

        [SerializedDictionary("Status", "The width of the status resistance bar")]
        public SerializedDictionary<StatusEffect, float> statusEffectResistances = new();

        [SerializedDictionary("Status", "Delay Rate (Between 0 and 1)")]
        public SerializedDictionary<StatusEffect, float> statusEffectDelayRates = new();

        [Header("Sounds")]
        public AudioClip greeting;

        [Header("Loot")]
        public List<LootableItem> loot = new();
        public CharacterGold characterGold;

        public int GetCurrentLevel()
        {
            return vitality + endurance + strength + dexterity + intelligence;
        }
    }
}
