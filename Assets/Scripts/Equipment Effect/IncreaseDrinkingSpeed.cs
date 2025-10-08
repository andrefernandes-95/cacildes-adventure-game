using AF.Health;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Equipment Effects/Increase Drinking Speed")]
    public class IncreaseDrinkingSpeed : EquipmentEffect
    {
        [SerializeField] float drinkingAnimationSpeed = 1.2f;

        #region EQUIP / UNEQUIP

        public override void OnEquip(CharacterManager characterManager)
        {
            characterManager.onPreparingToDrinkConsumable.AddListener(OnProcess);
        }

        public override void OnEquip(PlayerManager playerManager)
        {
            playerManager.onPreparingToDrinkConsumable.AddListener(OnProcess);
        }

        public override void OnUnequip(CharacterManager characterManager)
        {
            characterManager.onPreparingToDrinkConsumable.RemoveListener(OnProcess);
        }

        public override void OnUnequip(PlayerManager playerManager)
        {
            playerManager.onPreparingToDrinkConsumable.RemoveListener(OnProcess);
        }

        #endregion

        #region DAMAGE PROCESSING

        void OnProcess(CharacterBaseManager characterBaseManager)
        {
            characterBaseManager.animator.speed = drinkingAnimationSpeed;
        }

        #endregion

        #region TOOLTIP

        public override string GetEquipmentEffectTooltip()
        {
            string text = "";

            if (Utils.IsPortuguese())
            {
                text = $"+{drinkingAnimationSpeed * 10}% velocidade ao beber itens";
            }
            else
            {
                text = $"+{drinkingAnimationSpeed * 10}% speed when drinking items";
            }

            return text.TrimEnd('\n');
        }

        #endregion
    }
}
