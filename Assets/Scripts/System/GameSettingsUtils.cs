using System;
using System.IO;
using CI.QuickSave;
using UnityEngine;

namespace AF
{
    public static class GameSettingsUtils
    {
        private static readonly string PreferencesFileName = "GamePreferences";
        private static readonly string PreferencesFolder = Path.Combine(Application.persistentDataPath, "GamePreferences");
        private static readonly string PreferencesFilePath = Path.Combine(PreferencesFolder, PreferencesFileName);

        /// <summary>
        /// Checks if the GamePreferences.json file exists.
        /// </summary>
        public static bool HasGamePreferences()
        {
            try
            {
                return File.Exists(PreferencesFilePath);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while checking GamePreferences.json: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Loads the GamePreferences.json content as a string.
        /// </summary>
        public static void LoadPreferences(GameSettings gameSettings)
        {
            try
            {
                if (!HasGamePreferences())
                {
                    Debug.LogWarning("GamePreferences.json does not exist.");
                    return;
                }

                QuickSaveReader gamePreferencesReader = QuickSaveReader.Create(PreferencesFilePath);

                if (gamePreferencesReader.TryRead("dodgeBinding", out string dodgeBinding))
                {
                    gameSettings.dodgeBinding = dodgeBinding;
                }
                if (gamePreferencesReader.TryRead("sprintBinding", out string sprintBinding))
                {
                    gameSettings.sprintBinding = sprintBinding;
                }
                if (gamePreferencesReader.TryRead("jumpBinding", out string jumpBinding))
                {
                    gameSettings.jumpBinding = jumpBinding;
                }
                if (gamePreferencesReader.TryRead("heavyAttackBinding", out string heavyAttackBinding))
                {
                    gameSettings.heavyAttackBinding = heavyAttackBinding;
                }
                if (gamePreferencesReader.TryRead("toggleCombatStanceBinding", out string toggleCombatStanceBinding))
                {
                    gameSettings.toggleCombatStanceBinding = toggleCombatStanceBinding;
                }
                if (gamePreferencesReader.TryRead("useAbilityBinding", out string useAbilityBinding))
                {
                    gameSettings.useAbilityBinding = useAbilityBinding;
                }

            }
            catch (Exception e)
            {
                Debug.LogError($"Error reading GamePreferences.json: {e.Message}");
            }
        }

        /// <summary>
        /// Saves the provided JSON string to GamePreferences.json.
        /// </summary>
        public static void SavePreferences(GameSettings gameSettings)
        {
            try
            {
                if (!Directory.Exists(PreferencesFolder))
                {
                    Directory.CreateDirectory(PreferencesFolder);
                }

                QuickSaveWriter quickSaveWriter = QuickSaveWriter.Create(PreferencesFilePath);

                quickSaveWriter.Write("dodgeBinding", gameSettings.dodgeBinding);
                quickSaveWriter.Write("sprintBinding", gameSettings.sprintBinding);
                quickSaveWriter.Write("jumpBinding", gameSettings.jumpBinding);
                quickSaveWriter.Write("heavyAttackBinding", gameSettings.heavyAttackBinding);
                quickSaveWriter.Write("toggleCombatStanceBinding", gameSettings.toggleCombatStanceBinding);
                quickSaveWriter.Write("useAbilityBinding", gameSettings.useAbilityBinding);

                if (quickSaveWriter.TryCommit())
                {
                    Debug.Log("GamePreferences.json saved successfully.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error writing GamePreferences.json: {e.Message}");
            }
        }

        /// <summary>
        /// Deletes the GamePreferences.json file if it exists.
        /// </summary>
        public static void DeletePreferences()
        {
            try
            {
                if (File.Exists(PreferencesFilePath))
                {
                    File.Delete(PreferencesFilePath);
                    Debug.Log("GamePreferences.json deleted.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error deleting GamePreferences.json: {e.Message}");
            }
        }
    }
}
