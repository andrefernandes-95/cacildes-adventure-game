
namespace AF
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "GUI Icons Database", menuName = "System/GUI Icons Database", order = 0)]
    public class GUIIconsDatabase : ScriptableObject
    {
        [Header("Attacks")]
        public Texture2D physicalAttack;
        public Texture2D weaponScaling;
        public Texture2D weightBalance;
        [Header("Elemental Damage Types")]
        public Texture2D holy;
        public Texture2D fire;
        public Texture2D frost;
        public Texture2D lightning;
        public Texture2D magic;
        public Texture2D darkness;
        public Texture2D water;
        [Header("Absorption")]
        public Texture2D physicalAbsorption;
        public Texture2D fireAbsorption;
        public Texture2D frostAbsorption;
        public Texture2D lightningAbsorption;
        public Texture2D magicAbsorption;
        public Texture2D darknessAbsorption;
        public Texture2D waterAbsorption;

        [Header("Weapon Damage Types")]
        public Texture2D blunt;
        public Texture2D pierce;
        public Texture2D slash;
        public Texture2D range;
        public Texture2D feetAttack;
        [Header("Attributes & Stats")]
        public Texture2D vitality;
        public Texture2D endurance;
        public Texture2D intelligence;
        public Texture2D strength;
        public Texture2D dexterity;
        [Header("Misc")]
        public Texture2D gold;
        public Texture2D pushForce;
        public Texture2D posture;
        public Texture2D statusEffects;
        public Texture2D bonusStats;
        public Texture2D heavyAttack;
        public Texture2D staminaCost;
        public Texture2D bossToken;
        public Texture2D replenishable;
        public Texture2D spell;
        public Texture2D upgradeItem;
        public Texture2D craftItem;
        public Texture2D projectile;
        public Texture2D requirements;
        [Header("Colors")]
        public Color fireColor;
        public Color frostColor;
        public Color lightningColor;
        public Color magicColor;
        public Color darknessColor;
        public Color waterColor;
        public Color requirementsNotMetColor;
        public Color healthColor;
        public Color manaColor;
        public Color staminaColor;
        public Color slashColor;
        public Color pierceColor;
        public Color bluntColor;
        public Color rangeColor;

    }
}
