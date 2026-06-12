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
    [Header("Player Appearance")]
    public string defaultPlayerName = "Cacildes";
    public string playerName = "Cacildes";
    public string defaultHairColor = "#4F412D";
    public string defaultSkinColor = "#FFCCAE";
    public string defaultEyeColor = "#000000";
    public string defaultTattooColor = "#874FA5";
    public string defaultHair = "Chr_Hair_23";
    public string defaultEyebrows = "";
    public string defaultBeard = "";
    public string defaultFace = "_Chr_Head_Male_00";
    public string hairColor = "#4F412D";
    public string skinColor = "#FFCCAE";
    public string eyeColor = "#000000";
    public string tattooColor = "#874FA5";
    public string hair = "";
    public string eyebrows = "";
    public string beard = "";
    public string face = "";
    public bool isMale = true;
    public int playerPortrait = 0;

    [Header("World Settings")]
    [SerializeField] bool useDayAndNightCycle = true;

    [Header("Title Screen")]
    public AudioClip titleScreenMusic;

    [Header("Starting Equipment")]
    public Armor defaultArmor;
    public Gauntlet defaultGauntlet;
    public Legwear defaultLegwear;
    public Spell defaultSpell;
    public Consumable defaultConsumable;

    [Header("Other Settings")]
    public bool hasInitializedSettings = false;

    public float minimumCameraDistanceToPlayer = 0;
    public float maximumCameraDistanceToPlayer = 15;
    public float cameraDistance = 4;

    public float minimumCameraSensitivity = 0.1f;
    public float maximumCameraSensitivity = 10f;
    public float cameraSensitivity = 1f;

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

    public readonly string HIDE_PLAYER_HUD_KEY = "HIDE_PLAYER_HUD_KEY";

    public void SetGameLanguage(string code)
    {
        gameLanguage = code;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(gameLanguage);
    }

    public string GetGameLanguage() => gameLanguage;

    public string GetDefaultPlayerName() => defaultPlayerName;
    public string GetPlayerName() => playerName;

    public string GetDefaultHairColor() => defaultHairColor;
    public string GetHairColor() => hairColor;
    public void SetHairColor(string value) => hairColor = value;

    public string GetDefaultSkinColor() => defaultSkinColor;
    public string GetSkinColor() => skinColor;
    public void SetSkinColor(string value) => skinColor = value;

    public string GetDefaultEyeColor() => defaultEyeColor;
    public string GetEyeColor() => eyeColor;
    public void SetEyeColor(string value) => eyeColor = value;

    public string GetDefaultTattooColor() => defaultTattooColor;
    public string GetTattooColor() => tattooColor;
    public void SetTattooColor(string value) => tattooColor = value;

    public string GetDefaultHair() => defaultHair;
    public string GetHair() => hair;
    public void SetHair(string value) => hair = value;

    public string GetDefaultEyebrows() => defaultEyebrows;
    public string GetEyebrows() => eyebrows;
    public void SetEyebrows(string value) => eyebrows = value;

    public string GetDefaultBeard() => defaultBeard;
    public string GetBeard() => beard;
    public void SetBeard(string value) => beard = value;

    public string GetDefaultFace() => defaultFace;
    public string GetFace() => face;
    public void SetFace(string value) => face = value;

    public bool IsMale() => isMale;
    public void SetIsMale(bool value) => isMale = value;

    public int GetPlayerPortrait() => playerPortrait;
    public void SetPlayerPortrait(int value) => playerPortrait = value;

    public bool UseDayAndNightCycling() => useDayAndNightCycle;

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
        var source = LocalizationSettings
            .StringDatabase
            .SmartFormatter
            .GetSourceExtension<PersistentVariablesSource>();
        var characterName =
            source["global"]["playerName"] as UnityEngine.Localization.SmartFormat.PersistentVariables.StringVariable;

        characterName.Value = GetPlayerName();
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
    }

    public void SetPlayerName(string newPlayerName)
    {
        playerName = newPlayerName;
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
        automaticTranslationCode = code;
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

    public bool GetInvertYAxis() => invertYAxis;

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

    public float GetCameraSensitivity() => cameraSensitivity;

    public void SetMusicVolume(float newValue)
    {
        musicVolume = newValue;
        EventManager.EmitEvent(EventMessages.ON_MUSIC_VOLUME_CHANGED);
    }

    public int GetGraphicsQuality() => graphicsQuality;

    public float GetCameraDistance() => cameraDistance;

    public float GetMusicVolume() => musicVolume;
}
