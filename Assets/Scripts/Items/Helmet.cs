using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    [CreateAssetMenu(menuName = "Items / Armor / New Helmet")]
    public class Helmet : ArmorBase
    {
        public bool hideBeard = true;
        public bool hideEyebrows = true;
        public bool hideHair = true;
        public bool hideFace = true;

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

            if (hideHair)
            {
                character.syntyCharacterModelManager.ToggleHair(false);
            }
            if (hideBeard)
            {
                character.syntyCharacterModelManager.ToggleBeard(false);
            }
            if (hideEyebrows)
            {
                character.syntyCharacterModelManager.ToggleEyebrows(false);
            }
            if (hideFace)
            {
                character.syntyCharacterModelManager.ToggleFace(false);
            }

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

            if (hideHair)
            {
                character.syntyCharacterModelManager.ToggleHair(true);
            }
            if (hideBeard)
            {
                character.syntyCharacterModelManager.ToggleBeard(true);
            }
            if (hideEyebrows)
            {
                character.syntyCharacterModelManager.ToggleEyebrows(true);
            }
            if (hideFace)
            {
                character.syntyCharacterModelManager.ToggleFace(true);
            }
        }
    }

}
