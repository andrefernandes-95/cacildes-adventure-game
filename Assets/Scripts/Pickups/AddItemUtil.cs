using System;
using System.Collections.Generic;
using System.Linq;
using AF.Inventory;
using AYellowpaper.SerializedCollections;
using EditorAttributes;
using UnityEngine;

namespace AF.Pickups
{
    public class AddItemUtil : MonoBehaviour
    {
        [System.Serializable]
        public class ItemToAdd
        {
            public Item item;
            public int amount;
        }

        [Header("Data")]

        [HelpBox("Obsolete - use itemsObtained")]
        [Header("Effect Instances")]
        [Obsolete("Use itemsObtained")]
        public SerializedDictionary<Item, ItemAmount> itemsToAdd;

        [Header("Items To Receive")]
        public List<ItemToAdd> itemsObtained = new();

        // Scene References
        UIDocumentReceivedItemPrompt uIDocumentReceivedItemPrompt;
        PlayerInventory playerInventory;
        Soundbank soundbank;


        public void OnAddItem()
        {
            GetUIDocumentReceivedItemPrompt().gameObject.SetActive(true);

            List<UIDocumentReceivedItemPrompt.ItemsReceived> itemsToDisplay = new();

            List<ItemToAdd> itemsList = GetItemsToAdd();

            foreach (var item in itemsList)
            {
                itemsToDisplay.Add(new()
                {
                    itemName = item.item.GetName(),
                    quantity = item.amount,
                    sprite = item.item.sprite,
                    isCard = false
                });

                GetPlayerInventory().AddItem(item.item, item.amount);

            }

            GetUIDocumentReceivedItemPrompt().DisplayItemsReceived(itemsToDisplay);

            GetSoundbank().PlaySound(GetSoundbank().uiItemReceived);

            playerInventory.playerManager.companionsSceneManager.GiveLootToCompanions(GetItemsToAdd().Select(x => x.item).ToList());
        }

        UIDocumentReceivedItemPrompt GetUIDocumentReceivedItemPrompt()
        {
            if (uIDocumentReceivedItemPrompt == null)
            {
                uIDocumentReceivedItemPrompt = FindAnyObjectByType<UIDocumentReceivedItemPrompt>(FindObjectsInactive.Include);
            }

            return uIDocumentReceivedItemPrompt;
        }

        Soundbank GetSoundbank()
        {
            if (soundbank == null)
            {
                soundbank = FindAnyObjectByType<Soundbank>(FindObjectsInactive.Include);
            }

            return soundbank;
        }

        PlayerInventory GetPlayerInventory()
        {
            if (playerInventory == null)
            {
                playerInventory = FindAnyObjectByType<PlayerInventory>(FindObjectsInactive.Include);
            }

            return playerInventory;
        }

        List<ItemToAdd> GetItemsToAdd()
        {
            if (itemsObtained != null && itemsObtained.Count > 0)
            {
                return itemsObtained;
            }

            return itemsToAdd.Select(x =>
            {
                ItemToAdd itemToAdd = new()
                {
                    item = x.Key,
                    amount = x.Value.amount
                };

                return itemToAdd;
            }).ToList();
        }
    }
}
