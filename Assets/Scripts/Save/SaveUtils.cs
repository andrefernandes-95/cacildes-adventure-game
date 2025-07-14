using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AF.Companions;
using AF.Inventory;
using AF.Pickups;
using AYellowpaper.SerializedCollections;
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

            var lastFile = fileList.OrderByDescending(f => f.CreationTime)?.First();

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

        public static void CheckAndMigrateOldSaveFiles(string saveFilesLocation)
        {
            if (!QuickSaveReader.RootExists("Scene"))
            {
                return;
            }

            string saveFileName = $"Save_{DateTime.Now:yyyyMMdd_HHmmss}_Migrated";

            QuickSaveWriter quickSaveWriter = QuickSaveWriter.Create(saveFileName);
            MigrateBonfires(quickSaveWriter);
            MigrateCompanions(quickSaveWriter);
            MigrateStats(quickSaveWriter);
            MigratePlayerEquipment(quickSaveWriter);
            MigratePlayerInventory(quickSaveWriter);
            MigratePickups(quickSaveWriter);
            MigrateFlags(quickSaveWriter);
            MigrateQuests(quickSaveWriter);
            MigrateRecipes(quickSaveWriter);
            MigrateSceneSettings(quickSaveWriter);
            MigrateGameSettings(quickSaveWriter);

            quickSaveWriter.TryCommit();

            string saveFolderPath = Path.Combine(Application.persistentDataPath, saveFilesLocation);

            string backupFolderPath = Path.Combine(Application.persistentDataPath, "Migrated_Saves_Backup");

            // Create the backup folder if it doesn't exist
            if (!Directory.Exists(backupFolderPath))
            {
                Directory.CreateDirectory(backupFolderPath);
            }

            // Define an array of file names to loop through
            string[] fileNames = { "Bonfires.json", "Companions.json", "Equipment.json", "Flags.json", "GameSession.json", "Inventory.json", "Pickups.json", "PlayerStats.json", "Quests.json", "Recipes.json", "Scene.json" };

            foreach (string fileName in fileNames)
            {
                // Check if the file exists before proceeding
                string filePath = Path.Combine(saveFolderPath, fileName);
                if (File.Exists(filePath))
                {
                    // Define the backup file path
                    string backupFilePath = Path.Combine(backupFolderPath, $"{fileName}");

                    // Create a backup of the file
                    File.Copy(filePath, backupFilePath, true);

                    // Delete the original file
                    File.Delete(filePath);
                }
            }
        }

        static void MigrateStats(QuickSaveWriter quickSaveWriter)
        {
            var playerStats = QuickSaveReader.Create("PlayerStats");

            // Try to read currentHealth using TryRead
            playerStats.TryRead("currentHealth", out float currentHealth);
            quickSaveWriter.Write("currentHealth", currentHealth);

            // Try to read other stats
            playerStats.TryRead<float>("currentStamina", out float currentStamina);
            quickSaveWriter.Write("currentStamina", currentStamina);

            playerStats.TryRead<float>("currentMana", out float currentMana);
            quickSaveWriter.Write("currentMana", currentMana);

            playerStats.TryRead<int>("reputation", out int reputation);
            quickSaveWriter.Write("reputation", reputation);

            playerStats.TryRead<int>("vitality", out int vitality);
            quickSaveWriter.Write("vitality", vitality);

            playerStats.TryRead<int>("endurance", out int endurance);
            quickSaveWriter.Write("endurance", endurance);

            playerStats.TryRead<int>("intelligence", out int intelligence);
            quickSaveWriter.Write("intelligence", intelligence);

            playerStats.TryRead<int>("strength", out int strength);
            quickSaveWriter.Write("strength", strength);

            playerStats.TryRead<int>("dexterity", out int dexterity);
            quickSaveWriter.Write("dexterity", dexterity);

            playerStats.TryRead<int>("gold", out int gold);
            quickSaveWriter.Write("gold", gold);

            playerStats.TryRead<int>("lostGold", out int lostGold);
            quickSaveWriter.Write("lostGold", lostGold);

            playerStats.TryRead<string>("sceneWhereGoldWasLost", out string sceneWhereGoldWasLost);
            quickSaveWriter.Write("sceneWhereGoldWasLost", sceneWhereGoldWasLost);

            playerStats.TryRead<Vector3>("positionWhereGoldWasLost", out Vector3 positionWhereGoldWasLost);
            quickSaveWriter.Write("positionWhereGoldWasLost", positionWhereGoldWasLost);
        }

        static void MigratePlayerEquipment(QuickSaveWriter quickSaveWriter)
        {
            var playerEquipment = QuickSaveReader.Create("Equipment");

            playerEquipment.TryRead<int>("currentWeaponIndex", out int currentWeaponIndex);
            quickSaveWriter.Write("currentWeaponIndex", currentWeaponIndex);

            playerEquipment.TryRead<int>("currentShieldIndex", out int currentShieldIndex);
            quickSaveWriter.Write("currentShieldIndex", currentShieldIndex);

            playerEquipment.TryRead<int>("currentArrowIndex", out int currentArrowIndex);
            quickSaveWriter.Write("currentArrowIndex", currentArrowIndex);

            playerEquipment.TryRead<int>("currentSpellIndex", out int currentSpellIndex);
            quickSaveWriter.Write("currentSpellIndex", currentSpellIndex);

            playerEquipment.TryRead<int>("currentConsumableIndex", out int currentConsumableIndex);
            quickSaveWriter.Write("currentConsumableIndex", currentConsumableIndex);

            playerEquipment.TryRead<string[]>("weapons", out string[] weapons);
            quickSaveWriter.Write("weapons", weapons);

            // Try to read shields
            playerEquipment.TryRead<string[]>("shields", out string[] shields);
            quickSaveWriter.Write("shields", shields);

            // Try to read arrows
            playerEquipment.TryRead<string[]>("arrows", out string[] arrows);
            quickSaveWriter.Write("arrows", arrows);

            // Try to read spells
            playerEquipment.TryRead<string[]>("spells", out string[] spells);
            quickSaveWriter.Write("spells", spells);

            // Try to read accessories
            playerEquipment.TryRead<string[]>("accessories", out string[] accessories);
            quickSaveWriter.Write("accessories", accessories);

            // Try to read consumables
            playerEquipment.TryRead<string[]>("consumables", out string[] consumables);
            quickSaveWriter.Write("consumables", consumables);

            // Try to read helmet
            playerEquipment.TryRead<string>("helmet", out string helmetName);
            quickSaveWriter.Write("helmet", helmetName);

            // Try to read armor
            playerEquipment.TryRead<string>("armor", out string armorName);
            quickSaveWriter.Write("armor", armorName);

            // Try to read gauntlet
            playerEquipment.TryRead<string>("gauntlet", out string gauntletName);
            quickSaveWriter.Write("gauntlet", gauntletName);

            // Try to read legwear
            playerEquipment.TryRead<string>("legwear", out string legwearName);
            quickSaveWriter.Write("legwear", legwearName);

            playerEquipment.TryRead<bool>("isTwoHanding", out bool isTwoHanding);
            quickSaveWriter.Write("isTwoHanding", isTwoHanding);
        }

        static void MigratePlayerInventory(QuickSaveWriter quickSaveWriter)
        {
            var inventory = QuickSaveReader.Create("Inventory");

            inventory.TryRead("ownedItems", out SerializedDictionary<string, ItemAmount> ownedItems);
            quickSaveWriter.Write("ownedItems", ownedItems);
        }

        static void MigratePickups(QuickSaveWriter quickSaveWriter)
        {
            var pickups = QuickSaveReader.Create("Pickups");
            pickups.TryRead("pickups", out SerializedDictionary<string, string> savedPickups);
            quickSaveWriter.Write("pickups", savedPickups ?? new SerializedDictionary<string, string>());
            pickups.TryRead("replenishables", out SerializedDictionary<string, ReplenishableTime> savedReplenishables);
            quickSaveWriter.Write("replenishables", savedReplenishables ?? new SerializedDictionary<string, ReplenishableTime>());
        }

        static void MigrateQuests(QuickSaveWriter quickSaveWriter)
        {
            var questsReceived = QuickSaveReader.Create("Quests");
            questsReceived.TryRead("questsReceived", out SerializedDictionary<string, int> savedQuestsReceived);
            quickSaveWriter.Write("questsReceived", savedQuestsReceived);

            questsReceived.TryRead("currentTrackedQuestIndex", out int currentTrackedQuestIndex);
            quickSaveWriter.Write("currentTrackedQuestIndex", currentTrackedQuestIndex);
        }

        static void MigrateFlags(QuickSaveWriter quickSaveWriter)
        {
            var flags = QuickSaveReader.Create("Flags");
            flags.TryRead("flags", out SerializedDictionary<string, string> savedFlags);
            quickSaveWriter.Write("flags", savedFlags);
        }

        static void MigrateSceneSettings(QuickSaveWriter quickSaveWriter)
        {
            var data = QuickSaveReader.Create("Scene");
            data.TryRead<int>("sceneIndex", out int sceneIndex);
            quickSaveWriter.Write("sceneIndex", sceneIndex);

            data.TryRead("playerPosition", out Vector3 playerPosition);
            quickSaveWriter.Write("playerPosition", playerPosition);

            data.TryRead("playerRotation", out Quaternion playerRotation);
            quickSaveWriter.Write("playerRotation", playerRotation);
        }

        static void MigrateGameSettings(QuickSaveWriter quickSaveWriter)
        {
            var data = QuickSaveReader.Create("GameSession");

            data.TryRead<float>("timeOfDay", out var timeOfDay);
            quickSaveWriter.Write("timeOfDay", timeOfDay);
        }

        static void MigrateCompanions(QuickSaveWriter quickSaveWriter)
        {
            var companions = QuickSaveReader.Create("Companions");
            companions.TryRead("companionsInParty", out SerializedDictionary<string, CompanionState> savedCompanionsInParty);
            quickSaveWriter.Write("companionsInParty", savedCompanionsInParty);
        }

        static void MigrateBonfires(QuickSaveWriter quickSaveWriter)
        {
            var bonfires = QuickSaveReader.Create("Bonfires");

            bonfires.TryRead("unlockedBonfires", out string[] unlockedBonfires);
            quickSaveWriter.Write("unlockedBonfires", unlockedBonfires);
        }

        static void MigrateRecipes(QuickSaveWriter quickSaveWriter)
        {
            var recipes = QuickSaveReader.Create("Recipes");

            recipes.TryRead("craftingRecipes", out string[] craftingRecipes);
            quickSaveWriter.Write("craftingRecipes", craftingRecipes);
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
            quickSaveReader.TryRead("weapons", out SerializedUpgradeableItem[] serializedWeapons);
            if (serializedWeapons != null && serializedWeapons.Length > 0)
            {
                for (int idx = 0; idx < serializedWeapons.Length; idx++)
                {
                    LoadSerializedWeapon(serializedWeapons[idx], idx, true, characterBaseInventory, equipmentDatabase);
                }
            }
            quickSaveReader.TryRead("shields", out SerializedUpgradeableItem[] serializedLeftWeapons);
            if (serializedLeftWeapons != null && serializedLeftWeapons.Length > 0)
            {
                for (int idx = 0; idx < serializedLeftWeapons.Length; idx++)
                {
                    LoadSerializedWeapon(serializedLeftWeapons[idx], idx, false, characterBaseInventory, equipmentDatabase);
                }
            }

            // Try to read arrows
            quickSaveReader.TryRead<string[]>("arrows", out string[] arrows);
            if (arrows != null && arrows.Length > 0)
            {
                for (int idx = 0; idx < arrows.Length; idx++)
                {
                    string arrowName = arrows[idx];

                    if (!string.IsNullOrEmpty(arrowName))
                    {
                        Arrow arrowInstance = Resources.Load<Arrow>("Items/Arrows/" + arrowName);

                        if (arrowInstance != null)
                        {
                            equipmentDatabase.arrows[idx] = arrowInstance;
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
                        Spell match = characterBaseInventory.GetSpells().First(
                            item => item.itemID == spellId);

                        if (match != null)
                        {
                            equipmentDatabase.spells[idx] = match;
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
                        Accessory match = characterBaseInventory.GetAccessories().First(
                            item => item.itemID == accessoryId);

                        if (match != null)
                        {
                            equipmentDatabase.accessories[idx] = match;
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
                        Consumable match = characterBaseInventory.GetConsumables().First(
                            item => item.itemID == consumableId);

                        if (match != null)
                        {
                            equipmentDatabase.consumables[idx] = match;
                        }
                    }
                }
            }

            // Try to read helmet
            quickSaveReader.TryRead<string>("helmet", out string helmetId);
            if (!string.IsNullOrEmpty(helmetId))
            {
                Helmet match = characterBaseInventory.GetHelmets().First(
                    item => item.itemID == helmetId);

                if (match != null)
                {
                    equipmentDatabase.helmet = match;
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
                Armor match = characterBaseInventory.GetArmors().First(
                    item => item.itemID == armorId);

                if (match != null)
                {
                    equipmentDatabase.armor = match;
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
                Gauntlet match = characterBaseInventory.GetGauntlets().First(
                    item => item.itemID == gauntletId);

                if (match != null)
                {
                    equipmentDatabase.gauntlet = match;
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
                Legwear match = characterBaseInventory.GetLegwears().First(
                    item => item.itemID == legwearId);

                if (match != null)
                {
                    equipmentDatabase.legwear = match;
                }
            }
            else
            {
                equipmentDatabase.UnequipLegwear();
            }
        }

        static void LoadSerializedWeapon(SerializedUpgradeableItem serializedWeapon, int slotIndex, bool isRightHandWeapon, CharacterBaseInventory characterBaseInventory, EquipmentDatabase equipmentDatabase)
        {
            if (serializedWeapon != null)
            {
                Weapon match = characterBaseInventory.GetWeapons().FirstOrDefault(
                    ownedWeapon => ownedWeapon != null && ownedWeapon.itemID == serializedWeapon.itemID);

                if (match != null)
                {
                    if (isRightHandWeapon)
                    {
                        equipmentDatabase.weapons[slotIndex] = match;
                    }
                    else
                    {
                        equipmentDatabase.shields[slotIndex] = match;
                    }
                }
            }
        }
    }
}
