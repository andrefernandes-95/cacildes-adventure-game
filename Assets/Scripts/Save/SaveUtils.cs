using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AF.Inventory;
using CI.QuickSave;
using UnityEngine;

namespace AF
{
    public static class SaveUtils
    {
        public static bool HasSaveFiles(string saveFilesLocation)
        {
            try
            {
                string saveFolderPath = Path.Combine(Application.persistentDataPath, saveFilesLocation);
                bool filesExist = Directory.Exists(saveFolderPath) && Directory
                    .EnumerateFiles(saveFolderPath)
                    .Where(fileName => !fileName.Contains("steam_autocloud.vdf"))
                    .Any();
                return filesExist;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while checking for save files: {e.Message}");
                return false;
            }
        }

        public static string GetLastSaveFile(string saveFilesLocation)
        {
            string saveFolderPath = Path.Combine(Application.persistentDataPath, saveFilesLocation);

            if (!Directory.Exists(saveFolderPath))
            {
                return ""; // Directory doesn't exist, return empty string
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(saveFolderPath);
            FileInfo[] files = directoryInfo.GetFiles();

            if (files.Length == 0)
            {
                return ""; // No files found in the directory, return empty string
            }

            // Sort files by creation time in descending order
            var fileList = files
             .Where(x =>
                 x.Name.Contains(".json") && !x.Name.Contains("steam_autocloud.vdf"));

            if (fileList.Count() <= 0)
            {
                return "";
            }

            var lastFile = fileList.OrderByDescending(f => f.CreationTime)?.FirstOrDefault();

            if (lastFile == null)
            {
                return "";
            }

            return lastFile.Name.Replace(".json", ""); // Return the full path of the last file
        }

        public static string[] GetSaveFileNames(string saveFilesLocation)
        {
            string saveFolderPath = Path.Combine(Application.persistentDataPath, saveFilesLocation);

            if (!Directory.Exists(saveFolderPath))
            {
                return new string[] { }; // Directory doesn't exist, return empty string
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(saveFolderPath);
            FileInfo[] files = directoryInfo.GetFiles();

            if (files.Length == 0)
            {
                return new string[] { }; // Directory doesn't exist, return empty string
            }

            return files
                .OrderByDescending(f => f.CreationTime)
                .Where(x => x.Name.Contains(".json"))
                .Select(x => x.Name.Replace(".json", ""))
                .ToArray();
        }

        public static Texture2D GetScreenshotFilePath(string saveFilesLocation, string fileName)
        {
            string saveFolderPath = Path.Combine(Application.persistentDataPath, saveFilesLocation);

            if (!Directory.Exists(saveFolderPath))
            {

                return null;
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(saveFolderPath);
            FileInfo[] files = directoryInfo.GetFiles();

            if (files.Length == 0)
            {
                return null;
            }

            string targetFilePath = files.FirstOrDefault(file => file.Name.Replace(".jpg", "") == fileName)?.FullName;

            if (string.IsNullOrEmpty(targetFilePath))
            {
                return null;
            }

            var targetTexture = new Texture2D(2, 2);
            targetTexture.LoadImage(File.ReadAllBytes(targetFilePath));
            return targetTexture;
        }

        public static void SaveItems(QuickSaveWriter quickSaveWriter, InventoryDatabase inventoryDatabase)
        {
            SerializeAndWriteForUpgradeableItem(quickSaveWriter, "ownedWeapons", inventoryDatabase.ownedWeapons);
            SerializeAndWriteForUpgradeableItem(quickSaveWriter, "ownedSpells", inventoryDatabase.ownedSpells);
            SerializeAndWriteForUpgradeableItem(quickSaveWriter, "ownedHelmets", inventoryDatabase.ownedHelmets);
            SerializeAndWriteForUpgradeableItem(quickSaveWriter, "ownedArmors", inventoryDatabase.ownedArmors);
            SerializeAndWriteForUpgradeableItem(quickSaveWriter, "ownedLegwears", inventoryDatabase.ownedLegwears);
            SerializeAndWriteForUpgradeableItem(quickSaveWriter, "ownedGauntlets", inventoryDatabase.ownedGauntlets);

            SerializeAndWrite(quickSaveWriter, "ownedArrows", inventoryDatabase.ownedArrows);
            SerializeAndWrite(quickSaveWriter, "ownedAccessories", inventoryDatabase.ownedAccessories);
            SerializeAndWrite(quickSaveWriter, "ownedConsumables", inventoryDatabase.ownedConsumables);
            SerializeAndWrite(quickSaveWriter, "ownedKeyItems", inventoryDatabase.ownedKeyItems);
            SerializeAndWrite(quickSaveWriter, "ownedCraftingMaterials", inventoryDatabase.ownedCraftingMaterials);
            SerializeAndWrite(quickSaveWriter, "ownedUpgradeMaterials", inventoryDatabase.ownedUpgradeMaterials);
        }

        static void SerializeAndWrite<T>(QuickSaveWriter quickSaveWriter, string key, List<T> items) where T : Item
        {
            List<SerializedItem> serializedItems = new();
            foreach (T item in items)
            {
                string path = Utils.GetItemPath(item).Replace("(Clone)", "");
                serializedItems.Add(new SerializedItem
                {
                    itemID = item.itemID,
                    resourcePath = path
                });
            }
            quickSaveWriter.Write(key, serializedItems);
        }

        static void SerializeAndWriteForUpgradeableItem<T>(QuickSaveWriter quickSaveWriter, string key, List<T> items) where T : UpgradableItem
        {
            List<SerializedUpgradeableItem> serializedItems = new();
            foreach (T item in items)
            {
                string path = Utils.GetItemPath(item).Replace("(Clone)", "");
                serializedItems.Add(new SerializedUpgradeableItem
                {
                    itemID = item.itemID,
                    resourcePath = path,
                    level = item.level
                });
            }
            quickSaveWriter.Write(key, serializedItems);
        }

        public static void LoadItems(QuickSaveReader quickSaveReader, PlayerManager playerManager)
        {
            // Loading Upgradable Items
            List<Weapon> weaponsToLoad = LoadUpgradeableItems<Weapon>(quickSaveReader, "ownedWeapons");
            foreach (Weapon weapon in weaponsToLoad)
            {
                // we use inventory database and access the lists directly because we already prepared the item's id and level previously,
                // and because we dont want to generate ids or resetting the level by using inventory.AddWeapon() methods
                playerManager.playerInventory.inventoryDatabase.ownedWeapons.Add(weapon);
            }

            List<Spell> spellsToLoad = LoadUpgradeableItems<Spell>(quickSaveReader, "ownedSpells");
            foreach (Spell spell in spellsToLoad)
            {
                playerManager.playerInventory.inventoryDatabase.ownedSpells.Add(spell);
            }

            List<Helmet> helmetsToLoad = LoadUpgradeableItems<Helmet>(quickSaveReader, "ownedHelmets");
            foreach (Helmet helmet in helmetsToLoad)
            {
                playerManager.playerInventory.inventoryDatabase.ownedHelmets.Add(helmet);
            }

            List<Armor> armorsToLoad = LoadUpgradeableItems<Armor>(quickSaveReader, "ownedArmors");
            foreach (Armor armor in armorsToLoad)
            {
                playerManager.playerInventory.inventoryDatabase.ownedArmors.Add(armor);
            }

            List<Legwear> legwearsToLoad = LoadUpgradeableItems<Legwear>(quickSaveReader, "ownedLegwears");
            foreach (Legwear legwear in legwearsToLoad)
            {
                playerManager.playerInventory.inventoryDatabase.ownedLegwears.Add(legwear);
            }

            List<Gauntlet> gauntletsToLoad = LoadUpgradeableItems<Gauntlet>(quickSaveReader, "ownedGauntlets");
            foreach (Gauntlet gauntlet in gauntletsToLoad)
            {
                playerManager.playerInventory.inventoryDatabase.ownedGauntlets.Add(gauntlet);
            }

            List<Accessory> accessoriesToLoad = LoadUpgradeableItems<Accessory>(quickSaveReader, "ownedAccessories");
            foreach (Accessory accessory in accessoriesToLoad)
            {
                playerManager.playerInventory.inventoryDatabase.ownedAccessories.Add(accessory);
            }

            List<Arrow> arrowsToLoad = LoadSerializedItem<Arrow>(quickSaveReader, "ownedArrows");
            foreach (Arrow arrow in arrowsToLoad)
            {
                playerManager.playerInventory.inventoryDatabase.ownedArrows.Add(arrow);
            }

            List<Consumable> consumablesToLoad = LoadSerializedItem<Consumable>(quickSaveReader, "ownedConsumables");
            foreach (Consumable consumable in consumablesToLoad)
            {
                playerManager.playerInventory.inventoryDatabase.ownedConsumables.Add(consumable);
            }

            List<KeyItem> keyItemsToLoad = LoadSerializedItem<KeyItem>(quickSaveReader, "ownedKeyItems");
            foreach (KeyItem keyItem in keyItemsToLoad)
            {
                playerManager.playerInventory.inventoryDatabase.ownedKeyItems.Add(keyItem);
            }

            List<CraftingMaterial> craftingMaterialsToLoad = LoadSerializedItem<CraftingMaterial>(quickSaveReader, "ownedCraftingMaterials");
            foreach (CraftingMaterial material in craftingMaterialsToLoad)
            {
                playerManager.playerInventory.inventoryDatabase.ownedCraftingMaterials.Add(material);
            }

            List<UpgradeMaterial> upgradeMaterialsToLoad = LoadSerializedItem<UpgradeMaterial>(quickSaveReader, "ownedUpgradeMaterials");
            foreach (UpgradeMaterial material in upgradeMaterialsToLoad)
            {
                playerManager.playerInventory.inventoryDatabase.ownedUpgradeMaterials.Add(material);
            }
        }

        static List<T> LoadUpgradeableItems<T>(QuickSaveReader quickSaveReader, string key) where T : UpgradableItem
        {
            quickSaveReader.TryRead(key, out List<SerializedUpgradeableItem> ownedUpgradeableItems);

            List<T> itemsToAdd = new();

            if (ownedUpgradeableItems != null && ownedUpgradeableItems.Count > 0)
            {
                for (int idx = 0; idx < ownedUpgradeableItems.Count; idx++)
                {
                    SerializedUpgradeableItem serializedItem = ownedUpgradeableItems.ElementAt(idx);

                    if (serializedItem != null)
                    {
                        T itemFile = Resources.Load<T>(serializedItem.resourcePath);

                        if (itemFile != null)
                        {
                            T clone = ScriptableObject.Instantiate(itemFile);
                            clone.itemID = serializedItem.itemID;
                            clone.level = serializedItem.level;

                            itemsToAdd.Add(clone);
                        }
                    }
                }
            }

            return itemsToAdd;
        }

        static List<T> LoadSerializedItem<T>(QuickSaveReader quickSaveReader, string key) where T : Item
        {
            quickSaveReader.TryRead(key, out List<SerializedItem> ownedSerializedItems);

            List<T> itemsToAdd = new();

            if (ownedSerializedItems != null && ownedSerializedItems.Count > 0)
            {
                for (int idx = 0; idx < ownedSerializedItems.Count; idx++)
                {
                    SerializedItem serializedItem = ownedSerializedItems.ElementAt(idx);

                    if (serializedItem != null)
                    {
                        T itemFile = Resources.Load<T>(serializedItem.resourcePath);

                        if (itemFile != null)
                        {
                            T clone = ScriptableObject.Instantiate(itemFile);
                            clone.itemID = serializedItem.itemID;
                            itemsToAdd.Add(clone);
                        }
                    }
                }
            }

            return itemsToAdd;
        }

        public static void LoadEquipment(QuickSaveReader quickSaveReader, CharacterBaseInventory characterBaseInventory, EquipmentDatabase equipmentDatabase)
        {
            quickSaveReader.TryRead("weapons", out string[] serializedWeapons);
            if (serializedWeapons != null && serializedWeapons.Length > 0)
            {
                for (int idx = 0; idx < serializedWeapons.Length; idx++)
                {
                    LoadSerializedWeapon(serializedWeapons[idx], idx, true, characterBaseInventory, equipmentDatabase);
                }
            }
            quickSaveReader.TryRead("shields", out string[] serializedLeftWeapons);
            if (serializedLeftWeapons != null && serializedLeftWeapons.Length > 0)
            {
                for (int idx = 0; idx < serializedLeftWeapons.Length; idx++)
                {
                    LoadSerializedWeapon(serializedLeftWeapons[idx], idx, false, characterBaseInventory, equipmentDatabase);
                }
            }

            // Try to read arrows
            quickSaveReader.TryRead("arrows", out string[] arrows);
            if (arrows != null && arrows.Length > 0)
            {
                for (int idx = 0; idx < arrows.Length; idx++)
                {
                    string arrowId = arrows[idx];

                    if (!string.IsNullOrEmpty(arrowId))
                    {
                        Arrow match = characterBaseInventory.GetArrows().FirstOrDefault(
                            ownedArrow => ownedArrow != null && ownedArrow.itemID == arrowId);

                        if (match != null)
                        {
                            equipmentDatabase.arrows[idx] = ScriptableObject.Instantiate(match);
                        }
                    }
                }
            }

            // Try to read spells
            quickSaveReader.TryRead<string[]>("spells", out string[] spells);
            if (spells != null && spells.Length > 0)
            {
                for (int idx = 0; idx < spells.Length; idx++)
                {
                    string spellId = spells[idx];

                    if (!string.IsNullOrEmpty(spellId))
                    {
                        Spell match = characterBaseInventory.GetSpells().FirstOrDefault(
                            item => item?.itemID == spellId);

                        if (match != null)
                        {
                            equipmentDatabase.spells[idx] = ScriptableObject.Instantiate(match);
                        }
                    }
                }
            }

            // Try to read accessories
            quickSaveReader.TryRead<string[]>("accessories", out string[] accessories);
            if (accessories != null && accessories.Length > 0)
            {
                for (int idx = 0; idx < accessories.Length; idx++)
                {
                    string accessoryId = accessories[idx];

                    if (!string.IsNullOrEmpty(accessoryId))
                    {
                        Accessory match = characterBaseInventory.GetAccessories().FirstOrDefault(
                            item => item?.itemID == accessoryId);

                        if (match != null)
                        {
                            equipmentDatabase.accessories[idx] = ScriptableObject.Instantiate(match);
                        }
                    }
                }
            }

            // Try to read consumables
            quickSaveReader.TryRead<string[]>("consumables", out string[] consumables);
            if (consumables != null && consumables.Length > 0)
            {
                for (int idx = 0; idx < consumables.Length; idx++)
                {
                    string consumableId = consumables[idx];

                    if (!string.IsNullOrEmpty(consumableId))
                    {
                        Consumable match = characterBaseInventory.GetConsumables().FirstOrDefault(
                            item => item?.itemID == consumableId);

                        if (match != null)
                        {
                            equipmentDatabase.consumables[idx] = ScriptableObject.Instantiate(match);
                        }
                    }
                }
            }

            // Try to read helmet
            quickSaveReader.TryRead<string>("helmet", out string helmetId);
            if (!string.IsNullOrEmpty(helmetId))
            {
                Helmet match = characterBaseInventory.GetHelmets().FirstOrDefault(
                    item => item?.itemID == helmetId);

                if (match != null)
                {
                    equipmentDatabase.helmet = ScriptableObject.Instantiate(match);
                }
            }
            else
            {
                equipmentDatabase.UnequipHelmet();
            }

            // Try to read armor
            quickSaveReader.TryRead<string>("armor", out string armorId);
            if (!string.IsNullOrEmpty(armorId))
            {
                Armor match = characterBaseInventory.GetArmors().FirstOrDefault(
                    item => item?.itemID == armorId);

                if (match != null)
                {
                    equipmentDatabase.armor = ScriptableObject.Instantiate(match);
                }
            }
            else
            {
                equipmentDatabase.UnequipArmor();
            }

            // Try to read gauntlet
            quickSaveReader.TryRead<string>("gauntlet", out string gauntletId);
            if (!string.IsNullOrEmpty(gauntletId))
            {
                Gauntlet match = characterBaseInventory.GetGauntlets().FirstOrDefault(
                    item => item?.itemID == gauntletId);

                if (match != null)
                {
                    equipmentDatabase.gauntlet = ScriptableObject.Instantiate(match);
                }
            }
            else
            {
                equipmentDatabase.UnequipGauntlet();
            }

            // Try to read legwear
            quickSaveReader.TryRead<string>("legwear", out string legwearId);
            if (!string.IsNullOrEmpty(legwearId))
            {
                Legwear match = characterBaseInventory.GetLegwears().FirstOrDefault(
                    item => item?.itemID == legwearId);

                if (match != null)
                {
                    equipmentDatabase.legwear = ScriptableObject.Instantiate(match);
                }
            }
            else
            {
                equipmentDatabase.UnequipLegwear();
            }
        }

        static void LoadSerializedWeapon(string serializedWeaponId, int slotIndex, bool isRightHandWeapon, CharacterBaseInventory characterBaseInventory, EquipmentDatabase equipmentDatabase)
        {
            if (!string.IsNullOrEmpty(serializedWeaponId))
            {
                Weapon match = characterBaseInventory.GetWeapons().FirstOrDefault(
                    ownedWeapon => ownedWeapon != null && ownedWeapon.itemID == serializedWeaponId);

                if (match != null)
                {
                    if (isRightHandWeapon)
                    {
                        equipmentDatabase.weapons[slotIndex] = ScriptableObject.Instantiate(match);
                    }
                    else
                    {
                        equipmentDatabase.shields[slotIndex] = ScriptableObject.Instantiate(match);
                    }
                }
            }
        }
    }
}
