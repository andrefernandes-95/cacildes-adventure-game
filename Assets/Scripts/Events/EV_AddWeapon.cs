namespace AF
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EV_AddWeapon : EventBase
    {
        [SerializeField] PlayerManager playerManager;

        [SerializeField] Weapon weapon;
        [SerializeField] bool autoEquip = false;
        [SerializeField] bool isRightHand = true;

        [SerializeField] bool showNotification = false;

        UIDocumentReceivedItemPrompt _uIDocumentReceivedItemPrompt;

        public override IEnumerator Dispatch()
        {
            // Add items
            Weapon addedWeapon = playerManager.playerInventory.AddWeapon(weapon);

            if (autoEquip)
            {
                playerManager.characterBaseEquipment.EquipWeapon(addedWeapon, 0, isRightHand);
            }

            ShowNotification();

            yield return null;
        }

        void ShowNotification()
        {
            GetUIDocumentReceivedItemPrompt().gameObject.SetActive(true);
            var combinedList = new List<UIDocumentReceivedItemPrompt.ItemsReceived>();
            UIDocumentReceivedItemPrompt.ItemsReceived itemReceived = new()
            {
                itemName = weapon.GetName(),
                quantity = 1,
                sprite = weapon.sprite
            };
            combinedList.Add(itemReceived);
            GetUIDocumentReceivedItemPrompt().DisplayItemsReceived(combinedList);
        }

        UIDocumentReceivedItemPrompt GetUIDocumentReceivedItemPrompt()
        {
            if (_uIDocumentReceivedItemPrompt == null)
            {
                _uIDocumentReceivedItemPrompt = FindAnyObjectByType<UIDocumentReceivedItemPrompt>(FindObjectsInactive.Include);
            }

            return _uIDocumentReceivedItemPrompt;
        }

    }



}
