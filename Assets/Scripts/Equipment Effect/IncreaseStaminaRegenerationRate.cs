using AF.Health;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Equipment Effects/Increase Stamina Regeneration Rate")]
    public class IncreaseStaminaRegenerationRate : EquipmentEffect
    {
        [SerializeField] float staminaRegenerationBonusMultiplier = 1f;

        #region EQUIP / UNEQUIP

        public override void OnEquip(CharacterManager characterManager)
        {
        }

        public override void OnEquip(PlayerManager playerManager)
        {
            playerManager.staminaStatManager.SetStaminaRegenerationBonusMultiplier(staminaRegenerationBonusMultiplier);
        }

        public override void OnUnequip(CharacterManager characterManager)
        {
        }

        public override void OnUnequip(PlayerManager playerManager)
        {
            playerManager.staminaStatManager.SetStaminaRegenerationBonusMultiplier(staminaRegenerationBonusMultiplier * -1f);
        }

        #endregion

        #region DAMAGE PROCESSING

        void OnProcess(CharacterBaseManager characterBaseManager)
        {
        }

        #endregion

        #region TOOLTIP

        public override string GetEquipmentEffectTooltip()
        {
            string text = "";

            if (Utils.IsPortuguese())
            {
                text = $"+{staminaRegenerationBonusMultiplier}% velocidade de regeneração de stamina";
            }
            else
            {
                text = $"+{staminaRegenerationBonusMultiplier}% stamina regeneration speed";
            }

            return text.TrimEnd('\n');
        }

        #endregion
    }
}
