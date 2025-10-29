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

            foreach (var defaultItem in defaultItems)
            {
                if (defaultItem.Key is Armor armor)
                {
                    Armor addedArmor = playerManager.playerInventory.AddArmor(armor);
                    playerManager.equipmentDatabase.EquipArmor(addedArmor, false);
                }
                else if (defaultItem.Key is Legwear legwear)
                {
                    Legwear addedLegwear = playerManager.playerInventory.AddLegwear(legwear);
                    playerManager.equipmentDatabase.EquipLegwear(addedLegwear, false);
                }
                else if (defaultItem.Key is Spell spell)
                {
                    Spell addedSpell = playerManager.playerInventory.AddSpell(spell);
                    equipmentDatabase.EquipSpell(addedSpell, 0, false);
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

        public void RemoveItem(Item itemToRemove, int quantity)
        {
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
