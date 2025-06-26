using System;
using System.Collections.Generic;
using System.Linq;
using AF.Inventory;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace AF.Pickups
{
    public class AddItemUtil : MonoBehaviour
    {
        [Header("Data")]

        [Header("Effect Instances")]
        [SerializedDictionary("Item", "Quantity")]
        public SerializedDictionary<Item, ItemAmount> itemsToAdd;

        // Scene References
        UIDocumentReceivedItemPrompt uIDocumentReceivedItemPrompt;
        PlayerInventory playerInventory;
        Soundbank soundbank;


        public void OnAddItem()
        {
            GetUIDocumentReceivedItemPrompt().gameObject.SetActive(true);

            List<UIDocumentReceivedItemPrompt.ItemsReceived> itemsToDisplay = new();

            foreach (var item in itemsToAdd)
            {
                if (item.Value.chanceToGet != 100)
                {
                    int chance = UnityEngine.Random.Range(0, 100);
                    if (chance > item.Value.chanceToGet)
                    {
                        continue;
                    }
                }

                itemsToDisplay.Add(new()
                {
                    itemName = item.Key.GetName(),
                    quantity = item.Value.amount,
                    sprite = item.Key.sprite,
                    isCard = false
                });

                switch (item.Key)
                {
                    case Shield shield:
                        GetPlayerInventory().AddShield(shield);
                        break;
                    case Weapon weapon:
                        GetPlayerInventory().AddWeapon(weapon);
                        break;
                    case Spell spell:
                        GetPlayerInventory().AddSpell(spell);
                        break;
                    case Arrow arrow:
                        GetPlayerInventory().AddArrow(arrow);
                        break;
                    case Helmet helmet:
                        GetPlayerInventory().AddHelmet(helmet);
                        break;
                    case Gauntlet gauntlet:
                        GetPlayerInventory().AddGauntlet(gauntlet);
                        break;
                    case Armor armor:
                        GetPlayerInventory().AddArmor(armor);
                        break;
                    case Legwear legwear:
                        GetPlayerInventory().AddLegwear(legwear);
                        break;
                    case Consumable consumable:
                        GetPlayerInventory().AddConsumable(consumable);
                        break;
                    case Accessory accessory:
                        GetPlayerInventory().AddAccessory(accessory);
                        break;
                    case UpgradeMaterial upgradeMaterial:
                        GetPlayerInventory().AddUpgradeMaterial(upgradeMaterial);
                        break;
                    case CraftingMaterial craftingMaterial:
                        GetPlayerInventory().AddCraftingMaterial(craftingMaterial);
                        break;
                    case KeyItem keyItem:
                        GetPlayerInventory().AddKeyItem(keyItem);
                        break;
                    default:
                        // Handle other item types if needed
                        break;
                }

            }

            GetUIDocumentReceivedItemPrompt().DisplayItemsReceived(itemsToDisplay);

            GetSoundbank().PlaySound(GetSoundbank().uiItemReceived);
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
    }
}
