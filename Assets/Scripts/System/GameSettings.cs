using AF;
using AF.Events;
using TigerForge;
using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Extensions;

[CreateAssetMenu(fileName = "GameSettings", menuName = "System/New Game Settings", order = 0)]
public class GameSettings : ScriptableObject
{
    [Header("Game")]
    [SerializeField] Game currentGame;

<<<<<<< HEAD
    [Header("Games")]
    [SerializeField] Game CacildesAdventure;
    [SerializeField] Game UnholySword;

=======
>>>>>>> 09e69b8b9995dbf284b0d4a00aca13a12d2e52cb
    [Header("Other Settings")]
    public bool hasInitializedSettings = false;

    public float minimumCameraDistanceToPlayer = 0;
    public float maximumCameraDistanceToPlayer = 15;
    public float cameraDistance = 4;

    public float minimumCameraSensitivity = 0.1f;
    public float maximumCameraSensitivity = 10f;
    public float cameraSensitivity = 1f;

    // Audio
    public float musicVolume = 1f;

    public bool invertYAxis = false;

    public int graphicsQuality = 3;

    [Header("Custom Bindings")]
    public string jumpBinding = "";
    public string dodgeBinding = "";
    public string sprintBinding = "";
    public string toggleCombatStanceBinding = "";
    public string heavyAttackBinding = "";
    public string useAbilityBinding = "";

    [Header("Automatic Translation")]
    public string automaticTranslationCode = "";

    public string gameLanguage = "en";

<<<<<<< HEAD
=======
    public void SetGameLanguage(string code)
    {
        gameLanguage = code;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(gameLanguage);
    }

    public string GetGameLanguage() => gameLanguage;

>>>>>>> 09e69b8b9995dbf284b0d4a00aca13a12d2e52cb
    public string GetDefaultPlayerName() => currentGame == null ? "" : currentGame.defaultPlayerName;
    public string GetPlayerName() => currentGame == null ? "" : currentGame.playerName;

    // Hair
    public string GetDefaultHairColor() => currentGame == null ? "" : currentGame.defaultHairColor;
    public string GetHairColor() => currentGame == null ? "" : currentGame.hairColor;
    public void SetHairColor(string value)
    {
        if (currentGame != null)
        {
            currentGame.hairColor = value;
        }
    }

    // Skin
    public string GetDefaultSkinColor() => currentGame == null ? "" : currentGame.defaultSkinColor;
    public string GetSkinColor() => currentGame == null ? "" : currentGame.skinColor;
    public void SetSkinColor(string value)
    {
        if (currentGame != null)
        {
            currentGame.skinColor = value;
        }
    }

    // Eyes
    public string GetDefaultEyeColor() => currentGame == null ? "" : currentGame.defaultEyeColor;
    public string GetEyeColor() => currentGame == null ? "" : currentGame.eyeColor;
    public void SetEyeColor(string value)
    {
        if (currentGame != null)
        {
            currentGame.eyeColor = value;
        }
    }

    // Tattoo
    public string GetDefaultTattooColor() => currentGame == null ? "" : currentGame.defaultTattooColor;
    public string GetTattooColor() => currentGame == null ? "" : currentGame.tattooColor;
    public void SetTattooColor(string value)
    {
        if (currentGame != null)
        {
            currentGame.tattooColor = value;
        }
    }

    // Hair
    public string GetDefaultHair() => currentGame == null ? "" : currentGame.defaultHair;
    public string GetHair() => currentGame == null ? "" : currentGame.hair;
    public void SetHair(string value)
    {
        if (currentGame != null)
        {
            currentGame.hair = value;
        }
    }

    // Eyebrows
    public string GetDefaultEyebrows() => currentGame == null ? "" : currentGame.defaultEyebrows;
    public string GetEyebrows() => currentGame == null ? "" : currentGame.eyebrows;
    public void SetEyebrows(string value)
    {
        if (currentGame != null)
        {
            currentGame.eyebrows = value;
        }
    }

    // Beard
    public string GetDefaultBeard() => currentGame == null ? "" : currentGame.defaultBeard;
    public string GetBeard() => currentGame == null ? "" : currentGame.beard;
    public void SetBeard(string value)
    {
        if (currentGame != null)
        {
            currentGame.beard = value;
        }
    }

    // Face
    public string GetDefaultFace() => currentGame == null ? "" : currentGame.defaultFace;
    public string GetFace() => currentGame == null ? "" : currentGame.face;
    public void SetFace(string value)
    {
        if (currentGame != null)
        {
            currentGame.face = value;
        }
    }

    // Gender
    public bool IsMale() => currentGame == null ? true : currentGame.isMale;
    public void SetIsMale(bool value)
    {
        if (currentGame != null)
        {
            currentGame.isMale = value;
        }
    }

    // Player Portrait
    public int GetPlayerPortrait() => currentGame == null ? 0 : currentGame.playerPortrait;
    public void SetPlayerPortrait(int value)
    {
        if (currentGame != null)
        {
            currentGame.playerPortrait = value;
        }
    }


#if UNITY_EDITOR
    private void OnEnable()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            Clear();
        }
    }
#endif

    void Clear()
    {
        hasInitializedSettings = false;
    }

    public void UpdatePlayerNameOnLocalizedAssets()
    {
        // Get our GlobalVariablesSource
        var source = LocalizationSettings
            .StringDatabase
            .SmartFormatter
            .GetSourceExtension<PersistentVariablesSource>();
        // Get the specific global variable
        var characterName =
            source["global"]["playerName"] as UnityEngine.Localization.SmartFormat.PersistentVariables.StringVariable;

        // Update the global variable
        characterName.Value = GetPlayerName();
<<<<<<< HEAD
    }

    public bool ShouldShowPlayerHUD()
    {
        if (!PlayerPrefs.HasKey(HIDE_PLAYER_HUD_KEY))
        {
            return true;
        }

        return string.IsNullOrEmpty(PlayerPrefs.GetString(HIDE_PLAYER_HUD_KEY));
    }

    public void SetShouldShowPlayerHUD(bool value)
    {
        PlayerPrefs.SetString(HIDE_PLAYER_HUD_KEY, value ? "" : "true");
        PlayerPrefs.Save();
        EventManager.EmitEvent(EventMessages.ON_PLAYER_HUD_VISIBILITY_CHANGED);
=======
>>>>>>> 09e69b8b9995dbf284b0d4a00aca13a12d2e52cb
    }

    public void SetPlayerName(string newPlayerName)
    {
        if (currentGame == null)
        {
            Debug.LogWarning("Current game is null. SetPlayerName() was aborted.");
            return;
        }

        currentGame.playerName = newPlayerName;
        UpdatePlayerNameOnLocalizedAssets();
    }

    public void ResetSettings()
    {
        ResetKeyBindings();

        SetCameraDistance(4);
        SetCameraSensitivity(1f);
        SetInvertYAxis(false);

        SetGraphicsQuality(3);

        SetMusicVolume(1f);

        SetAutomaticTranslation("");
        SetGameLanguage("en");

        EventManager.EmitEvent(EventMessages.ON_INPUT_BINDINGS_CHANGED);
    }


    public void SetAutomaticTranslation(string code)
    {
        this.automaticTranslationCode = code;
    }

    public bool IsUsingAutomaticTranslation()
    {
        return string.IsNullOrEmpty(automaticTranslationCode) == false;
    }

    void ResetKeyBindings()
    {
        jumpBinding = "";
        dodgeBinding = "";
        sprintBinding = "";
        toggleCombatStanceBinding = "";
        heavyAttackBinding = "";
        useAbilityBinding = "";
    }

    public void LoadSettings()
    {
        if (hasInitializedSettings)
        {
            return;
        }

        automaticTranslationCode = "";

        // Load from file
        GameSettingsUtils.LoadPreferences(this);

        hasInitializedSettings = true;
        SetGraphicsQuality(GetGraphicsQuality());
    }

    public void SaveSettings()
    {
        GameSettingsUtils.SavePreferences(this);
    }

    public void SetGraphicsQuality(int newValue)
    {
        graphicsQuality = Mathf.Clamp(newValue, 0, 4);

        if (newValue == 0)
        {
            QualitySettings.SetQualityLevel(0);
        }
        else if (newValue == 1)
        {
            QualitySettings.SetQualityLevel(2);
        }
        else if (newValue == 2)
        {
            QualitySettings.SetQualityLevel(4);
        }
        else if (newValue >= 3)
        {
            QualitySettings.SetQualityLevel(5);
        }

        EventManager.EmitEvent(EventMessages.ON_GRAPHICS_QUALITY_CHANGED);
    }

    public void SetInvertYAxis(bool value)
    {
        invertYAxis = value;
        EventManager.EmitEvent(EventMessages.ON_INVERT_Y_AXIS);
    }

    public bool GetInvertYAxis()
    {
        return invertYAxis;
    }

    public void SetCameraDistance(float newValue)
    {
        cameraDistance = Mathf.Clamp(newValue, minimumCameraDistanceToPlayer, maximumCameraDistanceToPlayer);
        EventManager.EmitEvent(EventMessages.ON_CAMERA_DISTANCE_CHANGED);
    }

    public void IncreaseCameraDistance(float cameraDistanceAmountPerZoom = 0.5f)
    {
        SetCameraDistance(GetCameraDistance() - cameraDistanceAmountPerZoom);
    }

    public void DecreaseCameraDistance(float cameraDistanceAmountPerZoom = 0.5f)
    {
        SetCameraDistance(GetCameraDistance() + cameraDistanceAmountPerZoom);
    }

    public void SetCameraSensitivity(float newValue)
    {
        cameraSensitivity = Mathf.Clamp(newValue, minimumCameraSensitivity, maximumCameraSensitivity);
        EventManager.EmitEvent(EventMessages.ON_CAMERA_SENSITIVITY_CHANGED);
    }

    public float GetCameraSensitivity()
    {
        return cameraSensitivity;
    }

    public void SetMusicVolume(float newValue)
    {
        musicVolume = newValue;
        EventManager.EmitEvent(EventMessages.ON_MUSIC_VOLUME_CHANGED);
    }

    public int GetGraphicsQuality()
    {
        return graphicsQuality;
    }

    public float GetCameraDistance()
    {
        return cameraDistance;
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }
    public bool UseDayAndNightCycling() => currentGame != null && currentGame.UseDayAndNightCycle();

<<<<<<< HEAD
    public bool IsCacildesAdventure() => currentGame == CacildesAdventure;
    public bool IsUnholySword() => currentGame == UnholySword;

    public bool UseDayAndNightCycling() => currentGame != null && currentGame.UseDayAndNightCycle();

=======
>>>>>>> 09e69b8b9995dbf284b0d4a00aca13a12d2e52cb
    public void SetCurrentGame(Game game) => this.currentGame = game;
    public Game GetCurrentGame() => this.currentGame;
}
