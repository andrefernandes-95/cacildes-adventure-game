using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string EN_Description;
    public string PT_Description;
    public List<string> Items;
}


namespace AF.Inventory
{
    [CreateAssetMenu(fileName = "Inventory Database", menuName = "System/New Inventory Database", order = 0)]
    public class InventoryDatabase : ScriptableObject
    {

        [Header("Inventory")]
        [SerializedDictionary("Item", "Quantity")]
        public SerializedDictionary<Item, ItemAmount> ownedItems = new();
        public SerializedDictionary<Item, ItemAmount> defaultItems = new();

        public List<Weapon> ownedWeapons = new();
        public List<Spell> ownedSpells = new();
        public List<Arrow> ownedArrows = new();
        public List<Helmet> ownedHelmets = new();
        public List<Armor> ownedArmors = new();
        public List<Legwear> ownedLegwears = new();
        public List<Gauntlet> ownedGauntlets = new();
        public List<Accessory> ownedAccessories = new();
        public List<Consumable> ownedConsumables = new();
        public List<KeyItem> ownedKeyItems = new();
        public List<CraftingMaterial> ownedCraftingMaterials = new();
        public List<UpgradeMaterial> ownedUpgradeMaterials = new();

        public List<string> idsOfUsedConsumables = new();

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;

        [Header("Item Descriptions")]
        [SerializeField] TextAsset file;
        public SerializedDictionary<string, ItemData> itemDictionary = new SerializedDictionary<string, ItemData>();


#if UNITY_EDITOR
        private void OnEnable()
        {
            // No need to populate the list; it's serialized directly
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        public void LoadDescriptionsData()
        {
            return;
        }

        public string GetItemDescription(Item item)
        {
            return item.GetDescription();
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                // Clear the list when exiting play mode
                Clear();
            }
        }
#endif
        public void Clear()
        {
            ownedItems.Clear();
            ownedWeapons.Clear();
            ownedSpells.Clear();
            ownedArrows.Clear();
            ownedHelmets.Clear();
            ownedArmors.Clear();
            ownedLegwears.Clear();
            ownedGauntlets.Clear();
            ownedAccessories.Clear();
            ownedConsumables.Clear();
            ownedKeyItems.Clear();
            ownedCraftingMaterials.Clear();
            ownedUpgradeMaterials.Clear();
            idsOfUsedConsumables.Clear();
        }

        public void SetDefaultItems(PlayerManager playerManager)
        {
            Clear();

            Game currentGame = playerManager.gameSettings.GetCurrentGame();

            if (currentGame != null)
            {
                if (currentGame.defaultArmor != null)
                {
                    Armor addedArmor = playerManager.playerInventory.AddArmor(currentGame.defaultArmor);
                    playerManager.equipmentDatabase.EquipArmor(addedArmor, false);
                }

                if (currentGame.defaultLegwear != null)
                {
                    Legwear addedLegwear = playerManager.playerInventory.AddLegwear(currentGame.defaultLegwear);
                    playerManager.equipmentDatabase.EquipLegwear(addedLegwear, false);
                }
                if (currentGame.defaultGauntlet != null)
                {
                    Gauntlet addedGauntlet = playerManager.playerInventory.AddGauntlet(currentGame.defaultGauntlet);
                    playerManager.equipmentDatabase.EquipGauntlet(addedGauntlet, false);
                }

                if (currentGame.defaultSpell != null)
                {
                    Spell addedSpell = playerManager.playerInventory.AddSpell(currentGame.defaultSpell);
                    equipmentDatabase.EquipSpell(addedSpell, 0, false);
                }

                if (currentGame.defaultConsumable != null)
                {
                    Consumable addedConsumable = playerManager.playerInventory.AddConsumable(currentGame.defaultConsumable);
                    playerManager.equipmentDatabase.EquipConsumable(addedConsumable, 0);
                }
            }
        }

        public void ReplenishItems()
        {
            idsOfUsedConsumables.Clear();
        }

        public void AddItem(Item itemToAdd)
        {
            AddItem(itemToAdd, 1);
        }

        public void AddItem(Item itemToAdd, int quantity)
        {
            if (HasItem(itemToAdd))
            {
                ownedItems[itemToAdd].amount += quantity;
            }
            else
            {
                ownedItems.Add(itemToAdd, new ItemAmount() { amount = quantity, usages = 0 });
            }
        }

        public void AddWeapon(Weapon weaponToAdd, int quantity = 1)
        {
            for (int i = 0; i < quantity; i++)
            {
                Weapon weaponInstance = Instantiate(weaponToAdd);
                weaponInstance.itemID = Guid.NewGuid().ToString();

                ownedWeapons.Add(weaponInstance);
            }
        }

        public void RemoveItem(Item itemToAdd)
        {
            RemoveItem(itemToAdd, 1);
        }

        void RemoveItemEntry<T>(List<T> itemList, string itemID) where T : Item
        {
            if (itemList == null || itemList.Count == 0 || string.IsNullOrEmpty(itemID))
                return;

            int index = itemList.FindIndex(i => i.itemID == itemID);
            if (index >= 0)
            {
                itemList.RemoveAt(index);
            }
        }

        void RemoveArrow(Arrow arrow)
        {
            if (ownedArrows == null || ownedArrows.Count == 0 || arrow == null)
                return;

            Arrow arrowMatch = ownedArrows.Find(i => i.EqualsTo(arrow));
            if (arrowMatch != null)
            {
                ownedArrows.Remove(arrowMatch);
            }

            // If this was the last arrow
            if (ownedArrows.All(ownedArrow => ownedArrow != null && ownedArrow.EqualsTo(arrowMatch) == false))
            {
                int slotIndex = Array.FindIndex(equipmentDatabase.arrows, (arrow) => arrow != null && arrow.EqualsTo(arrowMatch));
                if (slotIndex != -1)
                {
                    equipmentDatabase.UnequipArrow(slotIndex);
                }
            }
        }

        public void RemoveItem(Item itemToRemove, int quantity)
        {
            if (itemToRemove is Shield) RemoveItemEntry<Weapon>(ownedWeapons, itemToRemove.itemID);
            if (itemToRemove is Weapon) RemoveItemEntry<Weapon>(ownedWeapons, itemToRemove.itemID);
            if (itemToRemove is Arrow arr) RemoveArrow(arr);
            if (itemToRemove is Spell) RemoveItemEntry<Spell>(ownedSpells, itemToRemove.itemID);
            if (itemToRemove is Helmet) RemoveItemEntry<Helmet>(ownedHelmets, itemToRemove.itemID);
            if (itemToRemove is Gauntlet) RemoveItemEntry<Gauntlet>(ownedGauntlets, itemToRemove.itemID);
            if (itemToRemove is Armor) RemoveItemEntry<Armor>(ownedArmors, itemToRemove.itemID);
            if (itemToRemove is Legwear) RemoveItemEntry<Legwear>(ownedLegwears, itemToRemove.itemID);
            if (itemToRemove is Accessory) RemoveItemEntry<Accessory>(ownedAccessories, itemToRemove.itemID);
            if (itemToRemove is Consumable) RemoveItemEntry<Consumable>(ownedConsumables, itemToRemove.itemID);
            if (itemToRemove is KeyItem) RemoveItemEntry<KeyItem>(ownedKeyItems, itemToRemove.itemID);
            if (itemToRemove is UpgradeMaterial) RemoveItemEntry<UpgradeMaterial>(ownedUpgradeMaterials, itemToRemove.itemID);
            if (itemToRemove is CraftingMaterial) RemoveItemEntry<CraftingMaterial>(ownedCraftingMaterials, itemToRemove.itemID);

            if (!ownedItems.ContainsKey(itemToRemove))
            {
                return;
            }

            if (ownedItems[itemToRemove].amount <= 1)
            {
                // If not reusable item
                if (itemToRemove.isRenewable)
                {
                    ownedItems[itemToRemove].amount = 0;
                    ownedItems[itemToRemove].usages++;
                }
                else
                {
                    UnequipItemToRemove(itemToRemove);

                    // Remove item 
                    ownedItems.Remove(itemToRemove);
                }
            }
            else
            {
                ownedItems[itemToRemove].amount -= quantity;

                if (itemToRemove.isRenewable)
                {
                    ownedItems[itemToRemove].usages++;
                }
            }
        }

        void UnequipItemToRemove(Item item)
        {
            equipmentDatabase.UnequipItem(item);
        }

        public int GetItemAmount(Item itemToFind)
        {
            if (!ownedItems.ContainsKey(itemToFind))
            {
                return -1;
            }

            return this.ownedItems[itemToFind].amount;
        }

        public bool HasItem(Item itemToFind)
        {
            return itemToFind switch
            {
                Shield shield => ownedWeapons.Exists(x => x == itemToFind),
                Weapon weapon => ownedWeapons.Exists(x => x == itemToFind),
                Spell spell => ownedSpells.Exists(x => x == itemToFind),
                Arrow arrow => ownedArrows.Exists(x => x == itemToFind),
                Helmet helmet => ownedHelmets.Exists(x => x == itemToFind),
                Gauntlet gauntlet => ownedGauntlets.Exists(x => x == itemToFind),
                Armor armor => ownedArmors.Exists(x => x == itemToFind),
                Legwear legwear => ownedLegwears.Exists(x => x == itemToFind),
                Consumable consumable => ownedConsumables.Exists(x => x.EqualsTo(itemToFind)),
                Accessory accessory => ownedAccessories.Exists(x => x == itemToFind),
                UpgradeMaterial upgradeMaterial => ownedUpgradeMaterials.Exists(x => x == itemToFind),
                CraftingMaterial craftingMaterial => ownedCraftingMaterials.Exists(x => x.EqualsTo(itemToFind)),
                KeyItem keyItem => ownedKeyItems.Exists(x => x.EqualsTo(itemToFind)),
                _ => false,
            };
        }

        public Item GetFirstItem(Item itemToFind)
        {
            return itemToFind switch
            {
                Shield shield => ownedWeapons.FirstOrDefault(x => x.EqualsTo(itemToFind)),
                Weapon weapon => ownedWeapons.FirstOrDefault(x => x.EqualsTo(itemToFind)),
                Spell spell => ownedSpells.FirstOrDefault(x => x.EqualsTo(itemToFind)),
                Arrow arrow => ownedArrows.FirstOrDefault(x => x.EqualsTo(itemToFind)),
                Helmet helmet => ownedHelmets.FirstOrDefault(x => x.EqualsTo(itemToFind)),
                Gauntlet gauntlet => ownedGauntlets.FirstOrDefault(x => x.EqualsTo(itemToFind)),
                Armor armor => ownedArmors.FirstOrDefault(x => x.EqualsTo(itemToFind)),
                Legwear legwear => ownedLegwears.FirstOrDefault(x => x.EqualsTo(itemToFind)),
                Consumable consumable => ownedConsumables.FirstOrDefault(x => x.EqualsTo(itemToFind)),
                Accessory accessory => ownedAccessories.FirstOrDefault(x => x.EqualsTo(itemToFind)),
                UpgradeMaterial upgradeMaterial => ownedUpgradeMaterials.FirstOrDefault(x => x.EqualsTo(itemToFind)),
                CraftingMaterial craftingMaterial => ownedCraftingMaterials.FirstOrDefault(x => x.EqualsTo(itemToFind)),
                KeyItem keyItem => ownedKeyItems.FirstOrDefault(x => x.EqualsTo(itemToFind)),
                _ => null,
            };
        }

        public int GetWeaponsCount()
        {
            return ownedItems.Count(x => x.Key is Weapon);
        }

        public int GetSpellsCount()
        {
            return ownedItems.Count(x => x.Key is Spell);
        }

        public int GetArrowAmount(Arrow arrowToCheck)
        {
            return ownedArrows.Count(arrow => arrowToCheck.EqualsTo(arrow));
        }

        public int GetConsumableAmount(Consumable consumable)
        {
            return ownedConsumables.Count(ownedConsumable => ownedConsumable.EqualsTo(consumable));
        }

        public void RemoveConsumable(Consumable consumable)
        {
            int idx = ownedConsumables.FindIndex(x => x.EqualsTo(consumable));
            if (idx != -1)
            {
                ownedConsumables.RemoveAt(idx);
            }
        }
        public void RemoveKeyItem(KeyItem keyItem)
        {
            int idx = ownedKeyItems.FindIndex(x => x.EqualsTo(keyItem));
            if (idx != -1)
            {
                ownedKeyItems.RemoveAt(idx);
            }
        }

        public void RemoveCraftingMaterial(CraftingMaterial craftingMaterial)
        {
            int idx = ownedCraftingMaterials.FindIndex(x => x.EqualsTo(craftingMaterial));
            if (idx != -1)
            {
                ownedCraftingMaterials.RemoveAt(idx);
            }
        }
    }
}
