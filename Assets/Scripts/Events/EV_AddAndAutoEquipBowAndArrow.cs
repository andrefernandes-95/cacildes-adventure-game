using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class EV_AddAndAutoEquipBowAndArrow : EventBase
    {
        [SerializeField] PlayerManager playerManager;

        [SerializeField] Weapon bow;
        [SerializeField] Arrow arrow;
        [SerializeField] int arrowAmount = 15;

        UIDocumentReceivedItemPrompt _uIDocumentReceivedItemPrompt;
        Soundbank _soundbank;

        public override IEnumerator Dispatch()
        {
            // Add items
            Weapon addedBow = playerManager.playerInventory.AddWeapon(bow);
            playerManager.characterBaseEquipment.EquipWeapon(Instantiate(addedBow), 0, false);

            // Added arrows
            for (int i = 0; i < arrowAmount; i++)
            {
                playerManager.playerInventory.AddArrow(arrow);
            }

            playerManager.equipmentDatabase.EquipArrow(arrow, 0);

            ShowNotification();

            yield return null;
        }

        void ShowNotification()
        {
            GetUIDocumentReceivedItemPrompt().gameObject.SetActive(true);
            var combinedList = new List<UIDocumentReceivedItemPrompt.ItemsReceived>();
            UIDocumentReceivedItemPrompt.ItemsReceived itemReceived = new()
            {
                itemName = bow.GetName(),
                quantity = 1,
                sprite = bow.sprite
            };
            combinedList.Add(itemReceived);
            GetUIDocumentReceivedItemPrompt().DisplayItemsReceived(combinedList);
            GetSoundbank().PlaySound(GetSoundbank().uiItemReceived);
        }

        UIDocumentReceivedItemPrompt GetUIDocumentReceivedItemPrompt()
        {
            if (_uIDocumentReceivedItemPrompt == null)
            {
                _uIDocumentReceivedItemPrompt = FindAnyObjectByType<UIDocumentReceivedItemPrompt>(FindObjectsInactive.Include);
            }

            return _uIDocumentReceivedItemPrompt;
        }
        Soundbank GetSoundbank()
        {
            if (_soundbank == null)
            {
                _soundbank = FindAnyObjectByType<Soundbank>(FindObjectsInactive.Include);
            }

            return _soundbank;
        }

    }

}
