using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    [CreateAssetMenu(menuName = "Items / Accessory / New Accessory")]
    public class Accessory : ArmorBase
    {
        [Header("Attribute Bonus")]
        [Range(0, 1f)] public float healthBonusMultiplier = 0f;
        [Range(0, 1f)] public float manaBonusMultiplier = 0f;
        [Range(0, 1f)] public float staminaBonusMultiplier = 0f;

        [Header("Stat Bonuses")]
        public int healthBonus = 0;
        public int magicBonus = 0;
        public int staminaBonus = 0;
        public int physicalAttackBonus = 0;
        public int jumpAttackBonus = 0;
        public float twoHandAttackBonusMultiplier = 0f;
        public float heavyAttackBonusMultiplier = 0f;
        public float slashDamageMultiplier = 0f;
        public float pierceDamageMultiplier = 0f;
        public float bluntDamageMultiplier = 0f;
        public float footDamageMultiplier = 0f;

        [Header("Stats")]
        public bool increaseAttackPowerTheLowerTheReputation = false;
        public bool increaseAttackPowerWithLowerHealth = false;
        public bool increaseAttackPowerWhenUnarmed = false;

        [Header("Posture")]
        public int postureDamagePerParry = 0;
        public float postureDecreaseRateBonus = 0f;
        public float backStabAngleBonus = 0f;

        [Header("Spells")]
        public float spellDamageBonusMultiplier = 0f;

        [Header("Life")]
        public bool chanceToDoubleCoinsFromFallenEnemies = false;
        public bool chanceToRestoreHealthUponDeath = false;

        [Header("Inventory")]
        public bool chanceToNotLoseItemUponConsumption = false;
        public int bonusWeightLoad = 0;

        public override void OnEquip(CharacterBaseManager character)
        {
            base.OnEquip(character);

            if (!character.IsUsingSyntyModularFantasyHeroModel())
            {
                return;
            }

            List<string> finalList = GetGraphicsToShow();

            if (character.characterBaseAppearance.IsMale())
            {
                finalList.Add(male_GraphicsToShow);
            }
            else
            {
                finalList.Add(female_GraphicsToShow);
            }

            character.syntyCharacterModelManager.EnableArmorPiece(finalList, armorMaterial);
        }

        public override void OnUnequip(CharacterBaseManager character)
        {
            base.OnUnequip(character);

            if (!character.IsUsingSyntyModularFantasyHeroModel())
            {
                return;
            }

            List<string> finalList = GetGraphicsToShow();

            finalList.Add(male_GraphicsToShow);
            finalList.Add(female_GraphicsToShow);

            character.syntyCharacterModelManager.DisablePieces(finalList);
        }
    }
}
