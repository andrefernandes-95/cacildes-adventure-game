using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    [CreateAssetMenu(menuName = "Items / Armor / New Legwear")]
    public class Legwear : ArmorBase
    {

        public override void OnEquip(CharacterBaseManager character)
        {
            if (!character.IsUsingSyntyModularFantasyHeroModel())
            {
                return;
            }

            List<string> finalList = graphicsToShow;

            if (character.characterBaseAppearance.IsMale())
            {
                finalList.Add(male_GraphicsToShow);
            }
            else
            {
                finalList.Add(female_GraphicsToShow);
            }

            character.syntyCharacterModelManager.ToggleLegs(false);
            character.syntyCharacterModelManager.EnableArmorPiece(finalList, armorMaterial);
        }

        public override void OnUnequip(CharacterBaseManager character)
        {
            if (!character.IsUsingSyntyModularFantasyHeroModel())
            {
                return;
            }

            List<string> finalList = graphicsToShow;

            finalList.Add(male_GraphicsToShow);
            finalList.Add(female_GraphicsToShow);

            character.syntyCharacterModelManager.DisablePieces(finalList);
            character.syntyCharacterModelManager.ToggleLegs(true);
        }
    }

}
