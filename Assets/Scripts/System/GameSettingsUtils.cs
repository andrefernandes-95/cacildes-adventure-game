using System;
using System.IO;
using CI.QuickSave;
using UnityEngine;

namespace AF
{
    public static class GameSettingsUtils
    {
        private static readonly string PreferencesFileName = "GamePreferences.json";
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

                // Gameplay Options
                if (gamePreferencesReader.TryRead("cameraSensitivity", out int cameraSensitivity))
                {
                    gameSettings.SetCameraSensitivity(cameraSensitivity);
                }

                if (gamePreferencesReader.TryRead("cameraDistance", out int cameraDistance))
                {
                    gameSettings.SetCameraDistance(cameraDistance);
                }

                if (gamePreferencesReader.TryRead("invertYAxis", out bool invertYAxis))
                {
                    gameSettings.SetInvertYAxis(invertYAxis);
                }

                // Audio Options
                if (gamePreferencesReader.TryRead("musicVolume", out float musicVolume))
                {
                    gameSettings.SetMusicVolume(musicVolume);
                }

                // Graphics Options
                if (gamePreferencesReader.TryRead("graphicsQuality", out int graphicsQuality))
                {
                    gameSettings.SetGraphicsQuality(graphicsQuality);
                }

                // Character Customization
                if (gamePreferencesReader.TryRead("playerName", out string playerName))
                {
                    gameSettings.SetPlayerName(playerName);
                }
                if (gamePreferencesReader.TryRead("hairColor", out string hairColor))
                {
                    gameSettings.SetHairColor(hairColor);
                }
                if (gamePreferencesReader.TryRead("skinColor", out string skinColor))
                {
                    gameSettings.SetSkinColor(skinColor);
                }
                if (gamePreferencesReader.TryRead("eyeColor", out string eyeColor))
                {
                    gameSettings.SetEyeColor(eyeColor);
                }
                if (gamePreferencesReader.TryRead("tattooColor", out string tattooColor))
                {
                    gameSettings.SetTattooColor(tattooColor);
                }
                if (gamePreferencesReader.TryRead("hair", out string hair))
                {
                    gameSettings.SetHair(hair);
                }
                if (gamePreferencesReader.TryRead("eyebrows", out string eyebrows))
                {
                    gameSettings.SetEyebrows(eyebrows);
                }
                if (gamePreferencesReader.TryRead("beard", out string beard))
                {
                    gameSettings.SetBeard(beard);
                }
                if (gamePreferencesReader.TryRead("face", out string face))
                {
                    gameSettings.SetFace(face);
                }
                if (gamePreferencesReader.TryRead("isMale", out bool isMale))
                {
                    gameSettings.SetIsMale(isMale);
                }
                if (gamePreferencesReader.TryRead("playerPortrait", out int playerPortrait))
                {
                    gameSettings.SetPlayerPortrait(playerPortrait);
<<<<<<< HEAD
=======
                }

                // Auto Translation
                if (gamePreferencesReader.TryRead("automaticTranslationCode", out string automaticTranslationCode))
                {
                    gameSettings.SetAutomaticTranslation(automaticTranslationCode);
                }

                if (gamePreferencesReader.TryRead("gameLanguage", out string gameLanguage))
                {
                    gameSettings.SetGameLanguage(gameLanguage);
>>>>>>> 09e69b8b9995dbf284b0d4a00aca13a12d2e52cb
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

                // Character Customization
                quickSaveWriter.Write("playerName", gameSettings.GetPlayerName());
                quickSaveWriter.Write("hairColor", gameSettings.GetHairColor());
                quickSaveWriter.Write("skinColor", gameSettings.GetSkinColor());
                quickSaveWriter.Write("eyeColor", gameSettings.GetEyeColor());
                quickSaveWriter.Write("tattooColor", gameSettings.GetTattooColor());
                quickSaveWriter.Write("hair", gameSettings.GetHair());
                quickSaveWriter.Write("eyebrows", gameSettings.GetEyebrows());
                quickSaveWriter.Write("beard", gameSettings.GetBeard());
                quickSaveWriter.Write("face", gameSettings.GetFace());
                quickSaveWriter.Write("isMale", gameSettings.IsMale());
                quickSaveWriter.Write("playerPortrait", gameSettings.GetPlayerPortrait());
<<<<<<< HEAD
=======

                // Gameplay Options
                quickSaveWriter.Write("cameraSensitivity", gameSettings.GetCameraSensitivity());
                quickSaveWriter.Write("cameraDistance", gameSettings.GetCameraDistance());
                quickSaveWriter.Write("invertYAxis", gameSettings.GetInvertYAxis());

                // Audio Options
                quickSaveWriter.Write("musicVolume", gameSettings.GetMusicVolume());

                // Video Quality Options
                quickSaveWriter.Write("graphicsQuality", gameSettings.GetGraphicsQuality());

                // Auto language
                quickSaveWriter.Write("automaticTranslationCode", gameSettings.automaticTranslationCode);
                quickSaveWriter.Write("gameLanguage", gameSettings.GetGameLanguage());
>>>>>>> 09e69b8b9995dbf284b0d4a00aca13a12d2e52cb

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
