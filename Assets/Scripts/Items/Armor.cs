using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Armor / New Armor")]
    public class Armor : ArmorBase
    {
        public override float GetBonusStep(int level)
        {
            if (level <= 3)
            {
                return 1;
            }
            else if (level <= 6)
            {
                return 1.5f;
            }
            else if (level <= 9)
            {
                return 2f;
            }
            else
            {
                return 3;
            }
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

            character.syntyCharacterModelManager.ToggleTorso(false);
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
            character.syntyCharacterModelManager.ToggleTorso(true);
        }
    }
}
