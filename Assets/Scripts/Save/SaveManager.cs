using UnityEngine;
using Input = UnityEngine.Input;
using CI.QuickSave;
using UnityEngine.SceneManagement;
using AYellowpaper.SerializedCollections;
using UnityEditor;
using AF.Inventory;
using System.Linq;
using AF.Companions;
using AF.Flags;
using AF.Bonfires;
using TigerForge;
using AF.Events;
using AF.Pickups;
using System;
using System.IO;
using AF.Loading;
using UnityEngine.Localization.Settings;
using System.Collections.Generic;

namespace AF
{
    public class SaveManager : MonoBehaviour
    {

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;
        public InventoryDatabase inventoryDatabase;
        public PickupDatabase pickupDatabase;
        public QuestsDatabase questsDatabase;
        public CompanionsDatabase companionsDatabase;
        public BonfiresDatabase bonfiresDatabase;
        public GameSession gameSession;
        public FlagsDatabase flagsDatabase;
        public RecipesDatabase recipesDatabase;

        [Header("Components")]
        public FadeManager fadeManager;
        public PlayerManager playerManager;
        public NotificationManager notificationManager;
        public GameSettings gameSettings;
        public MomentManager momentManager;
        public QuestManager questManager;

        // Flags that allow us to save the game
        bool hasBossFightOnGoing = false;

        public bool canSave = true;

        public string SAVE_FILES_FOLDER = "QuickSave";

        [Header("Bow Debug Tools")]
        public bool useBowDebugTools = false; // For position the bow and arrows during editor, it shouldnt be in this script but just reusing Update methods

        private void Awake()
        {
            EventManager.StartListening(EventMessages.ON_BOSS_BATTLE_BEGINS, () => { hasBossFightOnGoing = true; });
            EventManager.StartListening(EventMessages.ON_BOSS_BATTLE_ENDS, () => { hasBossFightOnGoing = false; });

            SaveUtils.CheckAndMigrateOldSaveFiles(SAVE_FILES_FOLDER);
        }

        public bool CanSave()
        {
            if (momentManager.HasMomentOnGoing)
            {
                return false;
            }

            if (hasBossFightOnGoing)
            {
                return false;
            }

            if (playerManager.thirdPersonController.Grounded == false)
            {
                return false;
            }

            if (gameSession.gameState != GameSession.GameState.INITIALIZED_AND_SHOWN_TITLE_SCREEN)
            {
                return false;
            }

            return canSave;
        }

        public void ResetGameState(bool isFromGameOver)
        {
            playerStatsDatabase.Clear(isFromGameOver);
            equipmentDatabase.Clear();
            inventoryDatabase.SetDefaultItems(playerManager);
            pickupDatabase.Clear();
            questsDatabase.Clear();
            companionsDatabase.Clear();
            bonfiresDatabase.Clear();
            flagsDatabase.Clear();
            recipesDatabase.Clear();
        }

        void SaveRecipes(QuickSaveWriter quickSaveWriter)
        {
            quickSaveWriter.Write("craftingRecipes", recipesDatabase.craftingRecipes.Select(craftingRecipe => craftingRecipe.name));
        }
        void SavePlayerStats(QuickSaveWriter quickSaveWriter)
        {
            quickSaveWriter.Write("currentHealth", playerStatsDatabase.currentHealth);
            quickSaveWriter.Write("currentStamina", playerStatsDatabase.currentStamina);
            quickSaveWriter.Write("currentMana", playerStatsDatabase.currentMana);
            quickSaveWriter.Write("reputation", playerStatsDatabase.reputation);
            quickSaveWriter.Write("vitality", playerStatsDatabase.vitality);
            quickSaveWriter.Write("endurance", playerStatsDatabase.endurance);
            quickSaveWriter.Write("intelligence", playerStatsDatabase.intelligence);
            quickSaveWriter.Write("strength", playerStatsDatabase.strength);
            quickSaveWriter.Write("dexterity", playerStatsDatabase.dexterity);
            quickSaveWriter.Write("gold", playerStatsDatabase.gold);
            quickSaveWriter.Write("lostGold", playerStatsDatabase.lostGold);
            quickSaveWriter.Write("sceneWhereGoldWasLost", playerStatsDatabase.sceneWhereGoldWasLost);
            quickSaveWriter.Write("positionWhereGoldWasLost", playerStatsDatabase.positionWhereGoldWasLost);
        }

        void SavePlayerEquipment(QuickSaveWriter quickSaveWriter)
        {
            quickSaveWriter.Write("currentWeaponIndex", equipmentDatabase.currentWeaponIndex);
            quickSaveWriter.Write("currentShieldIndex", equipmentDatabase.currentShieldIndex);
            quickSaveWriter.Write("currentArrowIndex", equipmentDatabase.currentArrowIndex);
            quickSaveWriter.Write("currentSpellIndex", equipmentDatabase.currentSpellIndex);
            quickSaveWriter.Write("currentConsumableIndex", equipmentDatabase.currentConsumableIndex);
            quickSaveWriter.Write("weapons", equipmentDatabase.weapons.Select(weapon =>
            {
                SerializedUpgradeableItem serializedWeapon = null;
                if (weapon != null)
                {
                    serializedWeapon = new();
                    serializedWeapon.itemID = weapon.itemID;
                }
                return serializedWeapon;
            }));
            quickSaveWriter.Write("shields", equipmentDatabase.shields.Select(shield =>
            {
                SerializedUpgradeableItem serializedWeapon = null;
                if (shield != null)
                {
                    serializedWeapon = new()
                    {
                        itemID = shield.itemID
                    };
                }
                return serializedWeapon;
            }));
            quickSaveWriter.Write("arrows", equipmentDatabase.arrows.Select(arrow => arrow != null ? arrow.name : ""));

            quickSaveWriter.Write("spells", equipmentDatabase.spells.Select(spell =>
            {
                SerializedUpgradeableItem serializedSpell = null;
                if (spell != null)
                {
                    serializedSpell = new()
                    {
                        itemID = spell.itemID
                    };
                }
                return serializedSpell;
            }));

            quickSaveWriter.Write("accessories", equipmentDatabase.accessories.Select(accessory =>
            {
                SerializedUpgradeableItem serializedAccessory = null;
                if (serializedAccessory != null)
                {
                    serializedAccessory = new()
                    {
                        itemID = accessory.itemID
                    };
                }
                return serializedAccessory;
            }));

            quickSaveWriter.Write("consumables", equipmentDatabase.consumables.Select(consumable => consumable != null ? consumable.name : ""));

            quickSaveWriter.Write("helmet", equipmentDatabase.helmet != null ? equipmentDatabase.helmet.itemID : "");
            quickSaveWriter.Write("armor", equipmentDatabase.armor != null ? equipmentDatabase.armor.itemID : "");
            quickSaveWriter.Write("gauntlet", equipmentDatabase.gauntlet != null ? equipmentDatabase.gauntlet.itemID : "");
            quickSaveWriter.Write("legwear", equipmentDatabase.legwear != null ? equipmentDatabase.legwear.itemID : "");

            quickSaveWriter.Write("isTwoHanding", equipmentDatabase.isTwoHanding);
        }

        void SavePlayerInventory(QuickSaveWriter quickSaveWriter)
        {
            SerializedDictionary<string, ItemAmount> keyValuePairs = new();

            foreach (var ownedItem in inventoryDatabase.ownedItems)
            {
                string path = Utils.GetItemPath(ownedItem.Key);

                if (!keyValuePairs.ContainsKey(path))
                {
                    keyValuePairs.Add(path, ownedItem.Value);
                }
            }

            quickSaveWriter.Write("ownedItems", keyValuePairs);

            quickSaveWriter.Write("idsOfUsedConsumables", inventoryDatabase.idsOfUsedConsumables);

            SaveUtils.SaveItems(quickSaveWriter, playerManager.playerInventory.inventoryDatabase);
        }
        void SavePickups(QuickSaveWriter quickSaveWriter)
        {
            quickSaveWriter.Write("pickups", pickupDatabase.pickups);
            quickSaveWriter.Write("replenishables", pickupDatabase.replenishables);
        }

        void SaveQuests(QuickSaveWriter quickSaveWriter)
        {
            questManager.OnSave(quickSaveWriter);
        }

        void SaveFlags(QuickSaveWriter quickSaveWriter)
        {
            quickSaveWriter.Write("flags", flagsDatabase.flags);
        }

        void SaveSceneSettings(QuickSaveWriter quickSaveWriter)
        {
            quickSaveWriter.Write("sceneIndex", SceneManager.GetActiveScene().buildIndex);
            quickSaveWriter.Write("sceneName", SceneManager.GetActiveScene().name);
            quickSaveWriter.Write("playerPosition", playerManager.transform.position);
            quickSaveWriter.Write("playerRotation", playerManager.transform.rotation);
        }
        void SaveGameSessionSettings(QuickSaveWriter quickSaveWriter)
        {
            quickSaveWriter.Write("timeOfDay", gameSession.timeOfDay);
            quickSaveWriter.Write("currentGameIteration", gameSession.currentGameIteration);
        }
        void SaveCompanions(QuickSaveWriter quickSaveWriter)
        {
            companionsDatabase.SaveCompanionStates(quickSaveWriter);
        }

        void SaveBonfires(QuickSaveWriter quickSaveWriter)
        {
            quickSaveWriter.Write("unlockedBonfires", bonfiresDatabase.unlockedBonfires);
        }

        void LoadRecipes(QuickSaveReader quickSaveReader)
        {
            quickSaveReader.TryRead("craftingRecipes", out string[] craftingRecipes);

            if (craftingRecipes != null && craftingRecipes.Count() > 0)
            {
                foreach (var recipeName in craftingRecipes)
                {
                    CraftingRecipe craftingRecipe = Resources.Load<CraftingRecipe>("Recipes/" + recipeName);
                    if (craftingRecipe != null)
                    {
                        recipesDatabase.AddCraftingRecipe(craftingRecipe);
                    }
                }
            }
        }

        void LoadPlayerStats(QuickSaveReader quickSaveReader, bool isFromGameOver)
        {
            // Try to read currentHealth using TryRead
            quickSaveReader.TryRead("currentHealth", out float currentHealth);
            playerStatsDatabase.SetCurrentHealth(currentHealth);

            // Try to read other stats
            quickSaveReader.TryRead<float>("currentStamina", out float currentStamina);
            playerStatsDatabase.SetCurrentStamina(currentStamina);

            quickSaveReader.TryRead<float>("currentMana", out float currentMana);
            playerStatsDatabase.SetCurrentMana(currentMana);

            quickSaveReader.TryRead<int>("reputation", out int reputation);
            playerStatsDatabase.reputation = reputation;

            quickSaveReader.TryRead<int>("vitality", out int vitality);
            playerStatsDatabase.vitality = vitality;

            quickSaveReader.TryRead<int>("endurance", out int endurance);
            playerStatsDatabase.endurance = endurance;

            quickSaveReader.TryRead<int>("intelligence", out int intelligence);
            playerStatsDatabase.intelligence = intelligence;

            quickSaveReader.TryRead<int>("strength", out int strength);
            playerStatsDatabase.strength = strength;

            quickSaveReader.TryRead<int>("dexterity", out int dexterity);
            playerStatsDatabase.dexterity = dexterity;

            // Read additional stats only if not from game over
            if (!isFromGameOver)
            {
                quickSaveReader.TryRead<int>("gold", out int gold);
                playerStatsDatabase.gold = gold;

                quickSaveReader.TryRead<int>("lostGold", out int lostGold);
                playerStatsDatabase.lostGold = lostGold;

                quickSaveReader.TryRead<string>("sceneWhereGoldWasLost", out string sceneWhereGoldWasLost);
                playerStatsDatabase.sceneWhereGoldWasLost = sceneWhereGoldWasLost;

                quickSaveReader.TryRead<Vector3>("positionWhereGoldWasLost", out Vector3 positionWhereGoldWasLost);
                playerStatsDatabase.positionWhereGoldWasLost = positionWhereGoldWasLost;
            }
        }

        void LoadPlayerEquipment(QuickSaveReader quickSaveReader)
        {
            quickSaveReader.TryRead<int>("currentWeaponIndex", out int currentWeaponIndex);
            equipmentDatabase.currentWeaponIndex = currentWeaponIndex;

            quickSaveReader.TryRead<int>("currentShieldIndex", out int currentShieldIndex);
            equipmentDatabase.currentShieldIndex = currentShieldIndex;

            quickSaveReader.TryRead<int>("currentArrowIndex", out int currentArrowIndex);
            equipmentDatabase.currentArrowIndex = currentArrowIndex;

            quickSaveReader.TryRead<int>("currentSpellIndex", out int currentSpellIndex);
            equipmentDatabase.currentSpellIndex = currentSpellIndex;

            quickSaveReader.TryRead<int>("currentConsumableIndex", out int currentConsumableIndex);
            equipmentDatabase.currentConsumableIndex = currentConsumableIndex;

            SaveUtils.LoadEquipment(quickSaveReader, playerManager.characterBaseInventory, equipmentDatabase);

            quickSaveReader.TryRead<bool>("isTwoHanding", out bool isTwoHanding);
            equipmentDatabase.isTwoHanding = isTwoHanding;
        }

        void LoadSerializedWeapon(SerializedUpgradeableItem serializedWeapon, int slotIndex, bool isRightHandWeapon)
        {
            if (serializedWeapon != null)
            {
                Weapon match = inventoryDatabase.ownedWeapons.First(ownedWeapon =>
                ownedWeapon.itemID == serializedWeapon.itemID);

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

        void LoadPlayerInventory(QuickSaveReader quickSaveReader)
        {
            inventoryDatabase.Clear();

            if (quickSaveReader.TryRead("idsOfUsedConsumables", out List<string> idsOfUsedConsumables))
            {
                inventoryDatabase.idsOfUsedConsumables = idsOfUsedConsumables;
            }

            quickSaveReader.TryRead("ownedItems", out SerializedDictionary<string, ItemAmount> ownedItems);

            if (ownedItems != null && ownedItems.Count > 0)
            {
                for (int idx = 0; idx < ownedItems.Count; idx++)
                {
                    var itemEntry = ownedItems.ElementAt(idx);

                    if (!string.IsNullOrEmpty(itemEntry.Key))
                    {
                        Item itemInstance = Resources.Load<Item>(itemEntry.Key);

                        if (itemInstance != null)
                        {
                            inventoryDatabase.ownedItems.Add(itemInstance, new()
                            {
                                amount = itemEntry.Value.amount,
                                chanceToGet = itemEntry.Value.chanceToGet,
                                usages = itemEntry.Value.usages
                            });
                        }
                    }
                }
            }

            SaveUtils.LoadItems(quickSaveReader, playerManager);
        }

        void LoadPickups(QuickSaveReader quickSaveReader)
        {
            pickupDatabase.Clear();

            quickSaveReader.TryRead("pickups", out SerializedDictionary<string, string> savedPickups);
            pickupDatabase.pickups = savedPickups;
            quickSaveReader.TryRead("replenishables", out SerializedDictionary<string, ReplenishableTime> savedReplenishables);
            pickupDatabase.replenishables = savedReplenishables;
        }

        void LoadQuests(QuickSaveReader quickSaveReader)
        {
            questManager.OnLoad(quickSaveReader);
        }

        void LoadFlags(QuickSaveReader quickSaveReader)
        {
            flagsDatabase.flags.Clear();
            quickSaveReader.TryRead("flags", out SerializedDictionary<string, string> savedFlags);

            foreach (var flag in savedFlags)
            {
                flagsDatabase.flags.Add(flag.Key, flag.Value);
            }
        }

        void LoadSceneSettings(QuickSaveReader quickSaveReader)
        {
            gameSession.currentGameIteration = 0;
            gameSession.nextMap_SpawnLocationData = null;
            gameSession.loadSavedPlayerPositionAndRotation = true;

            quickSaveReader.TryRead("playerPosition", out Vector3 playerPosition);
            gameSession.savedPlayerPosition = playerPosition;

            quickSaveReader.TryRead("playerRotation", out Quaternion playerRotation);
            gameSession.savedPlayerRotation = playerRotation;

            quickSaveReader.TryRead("currentGameIteration", out int currentGameIteration);
            if (currentGameIteration != -1)
            {
                gameSession.currentGameIteration = currentGameIteration;
            }

            quickSaveReader.TryRead<string>("sceneName", out string sceneName);
            if (!string.IsNullOrEmpty(sceneName))
            {
                LoadingManager.Instance.BeginLoading(sceneName);
            }
            else
            {
                quickSaveReader.TryRead<int>("sceneIndex", out int sceneIndex);
                LoadingManager.Instance.BeginLoading(sceneIndex);
            }
        }

        void LoadGameSessionSettings(QuickSaveReader quickSaveReader)
        {
            quickSaveReader.TryRead<float>("timeOfDay", out var timeOfDay);
            gameSession.timeOfDay = timeOfDay;
        }

        void LoadCompanions(QuickSaveReader quickSaveReader)
        {
            companionsDatabase.LoadCompanionStates(quickSaveReader);
        }

        void LoadBonfires(QuickSaveReader quickSaveReader)
        {
            bonfiresDatabase.unlockedBonfires.Clear();

            quickSaveReader.TryRead("unlockedBonfires", out string[] unlockedBonfires);

            if (unlockedBonfires != null && unlockedBonfires.Length > 0)
            {
                for (int idx = 0; idx < unlockedBonfires.Length; idx++)
                {
                    bonfiresDatabase.unlockedBonfires.Add(unlockedBonfires[idx]);
                }
            }
        }

        public bool HasSavedGame()
        {
            return SaveUtils.HasSaveFiles(SAVE_FILES_FOLDER);
        }

        public void SaveGameData(Texture2D screenshot)
        {
            if (!CanSave())
            {
                notificationManager.ShowNotification(LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Can not save at this time"), null);
                return;
            }

            string saveFileName = $"Save_{DateTime.Now:yyyyMMdd_HHmmss}";

            QuickSaveWriter quickSaveWriter = QuickSaveWriter.Create(saveFileName);
            SaveBonfires(quickSaveWriter);
            SaveCompanions(quickSaveWriter);
            SavePlayerStats(quickSaveWriter);
            SavePlayerEquipment(quickSaveWriter);
            SavePlayerInventory(quickSaveWriter);
            SavePickups(quickSaveWriter);
            SaveFlags(quickSaveWriter);
            SaveQuests(quickSaveWriter);
            SaveRecipes(quickSaveWriter);
            SaveSceneSettings(quickSaveWriter);
            SaveGameSessionSettings(quickSaveWriter);

            quickSaveWriter.Write("gameVersion", Application.version);
            quickSaveWriter.TryCommit();

            Texture2D finalScreenshot = screenshot;

            if (screenshot == null)
            {
                try
                {
                    finalScreenshot = ScreenCapture.CaptureScreenshotAsTexture();
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }
            }

            if (finalScreenshot != null)
            {
                File.WriteAllBytes(Path.Combine(Application.persistentDataPath + "/" + SAVE_FILES_FOLDER, saveFileName + ".jpg"), finalScreenshot.EncodeToJPG());
            }

            notificationManager.ShowNotification(LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Game saved"), notificationManager.systemSuccess);
        }

        public void LoadLastSavedGame(bool isFromGameOver)
        {
            string lastSave = SaveUtils.GetLastSaveFile(SAVE_FILES_FOLDER);

            LoadSaveFile(lastSave, isFromGameOver);
        }

        public void LoadSaveFile(string saveFileName)
        {
            LoadSaveFile(saveFileName, false);
        }

        void LoadSaveFile(string saveFileName, bool isFromGameOver)
        {
            if (string.IsNullOrEmpty(saveFileName) || !QuickSaveBase.RootExists(saveFileName))
            {
                // Return to title screen if no save game is available
                fadeManager.FadeIn(1f, () =>
                {
                    ResetGameStateAndReturnToTitleScreen(isFromGameOver);
                });
                return;
            }

            QuickSaveReader quickSaveReader = QuickSaveReader.Create(saveFileName);

            gameSession.gameState = GameSession.GameState.INITIALIZED_AND_SHOWN_TITLE_SCREEN;
            fadeManager.FadeIn(1f, () =>
            {
                LoadBonfires(quickSaveReader);
                LoadCompanions(quickSaveReader);
                LoadPlayerStats(quickSaveReader, isFromGameOver);
                LoadPlayerInventory(quickSaveReader);
                LoadPlayerEquipment(quickSaveReader);
                LoadPickups(quickSaveReader);
                LoadFlags(quickSaveReader);
                LoadQuests(quickSaveReader);
                LoadRecipes(quickSaveReader);
                LoadGameSessionSettings(quickSaveReader);
                LoadSceneSettings(quickSaveReader);
            });
        }

        public void LoadGameFromGameOver()
        {
            string lastSave = SaveUtils.GetLastSaveFile(SAVE_FILES_FOLDER);

            if (!string.IsNullOrEmpty(lastSave) && QuickSaveBase.RootExists(lastSave))
            {
                // Restore player attributes
                playerStatsDatabase.SetCurrentHealth(playerStatsDatabase.maxHealth);
                playerStatsDatabase.SetCurrentStamina(playerStatsDatabase.maxStamina);
                playerStatsDatabase.SetCurrentMana(playerStatsDatabase.maxMana);

                QuickSaveReader quickSaveReader = QuickSaveReader.Create(lastSave);
                LoadSceneSettings(quickSaveReader);
            }
            else
            {
                // We need to return to title screen
                ResetGameStateAndReturnToTitleScreen(isFromGameOver: true);
            }
        }

        public void ResetGameStateAndReturnToTitleScreen(bool isFromGameOver)
        {
            ResetGameState(isFromGameOver);
            gameSession.gameState = GameSession.GameState.INITIALIZED;
            SceneManager.LoadScene(0);
        }

        public void ResetGameStateForNewGamePlusAndReturnToTitleScreen()
        {
            playerStatsDatabase.ClearForNewGamePlus();
            pickupDatabase.Clear();
            questsDatabase.Clear();
            companionsDatabase.Clear();
            bonfiresDatabase.Clear();
            flagsDatabase.Clear();
            recipesDatabase.Clear();

            gameSession.gameState = GameSession.GameState.BEGINNING_NEW_GAME_PLUS;
            SceneManager.LoadScene(0);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                SaveGameData(null);
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                LoadLastSavedGame(false);
            }

            if (useBowDebugTools)
            {
                if (Input.GetKeyDown(KeyCode.F2))
                {
                    Time.timeScale = 0f;
                }
                if (Input.GetKeyDown(KeyCode.F3))
                {
                    Time.timeScale = 1f;
                }
            }
        }

        public void SetCanSave(bool canSave)
        {
            this.canSave = canSave;
        }
    }
}
