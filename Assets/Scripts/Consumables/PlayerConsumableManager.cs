namespace AF
{
    using System.Collections.Generic;
    using System.Linq;
    using AF.Ladders;
    using UnityEngine;

    public class PlayerConsumableManager : CharacterBaseConsumableManager
    {
        [SerializeField] PlayerManager playerManager;
        [SerializeField] NotificationManager notificationManager;

        void Awake()
        {
            playerManager.starterAssetsInputs.onConsumeFavoriteItem.AddListener(ConsumeItem);
        }

        void ConsumeItem()
        {
            // Get the first available consumable in inventory that matches the current equipped consumable
            Consumable consumable = playerManager.playerInventory.GetAvailableConsumables(
                playerManager.equipmentDatabase.GetCurrentConsumable()
            ).FirstOrDefault();

            if (!CanConsume(consumable))
            {
                return;
            }


            if (playerManager.playerInventory.GetAvailableConsumables(consumable).Count <= 1 && !consumable.isRenewable)
            {
                playerManager.equipmentDatabase.UnequipConsumable(playerManager.equipmentDatabase.currentConsumableIndex);
            }

            playerManager.playerInventory.RemoveConsumable(consumable);

            //playerManager.playerInventory.PrepareItemForConsuming(consumableItem);

            SetCurrentConsumable(consumable);

            playerManager.uIDocumentPlayerHUDV2.UpdateEquipment();
        }

        bool CanConsume(Consumable consumable)
        {
            if (consumable == null)
            {
                return false;
            }

            if (playerManager.health.GetCurrentHealth() <= 0)
            {
                return false;
            }

            if (playerManager.playerInventory.GetAvailableConsumables(consumable).Count <= 0)
            {
                if (consumable.isRenewable)
                {
                    notificationManager.ShowNotification(
                        Utils.IsPortuguese() ? "Consumível esgotado" : "Consumable depleted",
                        notificationManager.notEnoughSpells);
                }

                return false;
            }

            if (
                playerManager.isBusy
                || playerManager.playerCombatController.isCombatting
                || playerManager.thirdPersonController.isSwimming
                || playerManager.characterPosture.isStunned
                || playerManager.playerDodgeController.isDodging
                || !playerManager.thirdPersonController.Grounded
                || playerManager.climbController.climbState != ClimbState.NONE
                || playerManager.playerInventory.disableAshesUsage && consumable.consumableEffect is ReturnToBonfire
                )
            {
                notificationManager.ShowNotification(
                    Utils.IsPortuguese() ? "Não é possível usar o item de momento" : "Can't use item at this time",
                    notificationManager.systemError);

                return false;
            }

            return true;
        }

        public override CharacterBaseManager GetCharacter()
        {
            return playerManager;
        }
    }
}
