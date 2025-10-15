using AF.Health;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Equipment Effects/Increase Posture")]
    public class IncreasePosture : EquipmentEffect
    {
        public int bonusPosture = 30;

        #region EQUIP / UNEQUIP

        public override void OnEquip(CharacterManager characterManager)
        {
            characterManager.characterPosture.AddPostureEffect(this);
        }

        public override void OnEquip(PlayerManager playerManager)
        {
            playerManager.characterPosture.AddPostureEffect(this);
        }

        public override void OnUnequip(CharacterManager characterManager)
        {
            characterManager.characterPosture.RemovePostureEffect(this);
        }

        public override void OnUnequip(PlayerManager playerManager)
        {
            playerManager.characterPosture.RemovePostureEffect(this);
        }

        #endregion

        #region TOOLTIP

        public override string GetEquipmentEffectTooltip()
        {
            string text = "";
            if (Utils.IsPortuguese())
            {
                text += $"+{bonusPosture} pontos de postura";
            }
            else
            {
                text += $"+{bonusPosture} posture points";
            }
            return text.TrimEnd('\n');
        }

        #endregion
    }
}
