using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    [CreateAssetMenu(menuName = "Items / Armor / New Gauntlet")]
    public class Gauntlet : ArmorBase
    {
        public override float GetBonusStep(int level)
        {
            return .5f;
        }

        public override void OnEquip(CharacterBaseManager character)
        {
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

            character.syntyCharacterModelManager.ToggleHands(false);
            character.syntyCharacterModelManager.EnableArmorPiece(finalList, armorMaterial);
        }

        public override void OnUnequip(CharacterBaseManager character)
        {
            if (!character.IsUsingSyntyModularFantasyHeroModel())
            {
                return;
            }

            List<string> finalList = GetGraphicsToShow();

            finalList.Add(male_GraphicsToShow);
            finalList.Add(female_GraphicsToShow);

            character.syntyCharacterModelManager.DisablePieces(finalList);
            character.syntyCharacterModelManager.ToggleHands(true);
        }
    }

}
