using System;
using System.Collections.Generic;
using System.Linq;
using AYellowpaper.SerializedCollections;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

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
            if (itemDictionary.Count > 0)
            {
                return;
            }

            string yamlContent = file.text;

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(NullNamingConvention.Instance)  // No naming conversion
                .IgnoreUnmatchedProperties()
                .Build();

            var result = deserializer.Deserialize<Dictionary<string, ItemData>>(yamlContent);

            if (result != null)
            {
                itemDictionary.Clear();

                // Step 2: Flatten items into individual keys
                foreach (var item in result)
                {
                    ItemData itemInfo = item.Value;

                    if (itemInfo.Items != null)
                    {
                        foreach (string itemName in itemInfo.Items)
                        {
                            if (!itemDictionary.ContainsKey(itemName))
                            {
                                itemDictionary.Add(itemName, itemInfo);
                            }
                            else
                            {
                                Debug.LogWarning($"Duplicate item key '{itemName}' found in YAML. Skipping.");
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.LogError("Failed to parse YAML armor data.");
            }
        }

        public string GetItemDescription(Item item)
        {
            if (itemDictionary.ContainsKey(item.name))
            {
                return Utils.IsPortuguese() ? itemDictionary[item.name].PT_Description : itemDictionary[item.name].EN_Description;
            }

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
        }

        public void SetDefaultItems()
        {
            Clear();

            foreach (var defaultItem in defaultItems)
            {
                ownedItems.Add(defaultItem.Key, defaultItem.Value);

                if (defaultItem.Key is Armor armor)
                {
                    equipmentDatabase.EquipArmor(armor, false);
                }
                else if (defaultItem.Key is Legwear legwear)
                {
                    equipmentDatabase.EquipLegwear(legwear, false);
                }
                else if (defaultItem.Key is Spell spell)
                {
                    equipmentDatabase.EquipSpell(spell, 0);
                }
            }
        }

        public void ReplenishItems()
        {
            foreach (var item in ownedItems)
            {
                if (item.Value.usages > 0)
                {
                    item.Value.amount += item.Value.usages;
                    item.Value.usages = 0;
                }
            }
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
                weaponInstance.weaponID = Guid.NewGuid().ToString();

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
            return this.ownedItems.ContainsKey(itemToFind);
        }

        public int GetWeaponsCount()
        {
            return ownedItems.Count(x => x.Key is Weapon);
        }

        public int GetSpellsCount()
        {
            return ownedItems.Count(x => x.Key is Spell);
        }
    }
}
