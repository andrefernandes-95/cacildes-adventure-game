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
    public bool developerModeActive = false;
    public bool hasInitializedSettings = false;

    public float minimumCameraDistanceToPlayer = 0;
    public float defaultCameraDistanceToPlayer = 4;
    public float maximumCameraDistanceToPlayer = 15;

    public float minimumMouseSensitivity = 0.1f;
    public float maximumMouseSensitivity = 10f;

    public bool invertYAxis = false;

    public enum GraphicsQuality { LOW, MEDIUM, GOOD, ULTRA };

    public readonly string GRAPHICS_QUALITY_KEY = "graphicsQuality";
    public readonly string MUSIC_VOLUME_KEY = "musicVolume";
    public readonly string MOUSE_SENSITIVITY_KEY = "mouseSensitivity";
    public readonly string CAMERA_DISTANCE_KEY = "cameraDistance";
    public readonly string INVERT_Y_AXIS_KEY = "invertYAxis";

    [Header("Custom Bindings")]
    public string jumpBinding = "";
    public string dodgeBinding = "";
    public string sprintBinding = "";
    public string toggleCombatStanceBinding = "";
    public string heavyAttackBinding = "";
    public string useAbilityBinding = "";

    public readonly string PLAYER_NAME_KEY = "PLAYER_NAME";
    public readonly string defaultPlayerName = "Cacildes";
    public readonly string HIDE_PLAYER_HUD_KEY = "HIDE_PLAYER_HUD_KEY";

    [HideInInspector] public string SAVE_FILES_FOLDER = "GamePreferences";


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

    public string GetPlayerName()
    {
        if (!PlayerPrefs.HasKey(PLAYER_NAME_KEY))
        {
            return defaultPlayerName;
        }

        return PlayerPrefs.GetString(PLAYER_NAME_KEY);
    }

    public void SetPlayerName(string playerName)
    {

        PlayerPrefs.SetString(PLAYER_NAME_KEY, playerName);
        PlayerPrefs.Save();
        UpdatePlayerNameOnLocalizedAssets();
    }

    public void ResetSettings()
    {
        SetGameQuality(2);
        SetCameraSensitivity(1f);
        SetMusicVolume(1f);
        ResetKeyBindings();
        SetInvertYAxis(false);

        EventManager.EmitEvent(EventMessages.ON_INPUT_BINDINGS_CHANGED);
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

        // Load from file
        GameSettingsUtils.LoadPreferences(this);

        hasInitializedSettings = true;
        SetGameQuality(GetGraphicsQuality());
    }

    public void SaveSettings()
    {
        GameSettingsUtils.SavePreferences(this);
    }

    public void SetGameQuality(int newValue)
    {
        PlayerPrefs.SetInt(GRAPHICS_QUALITY_KEY, Mathf.Clamp(newValue, 0, 4));

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
        else if (newValue == 3)
        {
            QualitySettings.SetQualityLevel(5);
        }

        EventManager.EmitEvent(EventMessages.ON_GRAPHICS_QUALITY_CHANGED);
    }

    public void SetInvertYAxis(bool value)
    {
        invertYAxis = value;
        PlayerPrefs.SetString(INVERT_Y_AXIS_KEY, value ? "true" : "");
        EventManager.EmitEvent(EventMessages.ON_INVERT_Y_AXIS);
    }

    public bool GetInvertYAxis()
    {
        if (!PlayerPrefs.HasKey(INVERT_Y_AXIS_KEY))
        {
            return false;
        }

        string invertYAxisValue = PlayerPrefs.GetString(INVERT_Y_AXIS_KEY);

        if (string.IsNullOrEmpty(invertYAxisValue))
        {
            return false;
        }

        return true;
    }

    public void SetCameraDistance(float newValue)
    {
        PlayerPrefs.SetFloat(CAMERA_DISTANCE_KEY, Mathf.Clamp(newValue, minimumCameraDistanceToPlayer, maximumCameraDistanceToPlayer));
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
        PlayerPrefs.SetFloat(MOUSE_SENSITIVITY_KEY, Mathf.Clamp(newValue, minimumMouseSensitivity, maximumMouseSensitivity));
        EventManager.EmitEvent(EventMessages.ON_CAMERA_SENSITIVITY_CHANGED);
    }
    public float GetCameraSensitivity()
    {
        return PlayerPrefs.HasKey(MOUSE_SENSITIVITY_KEY) ? PlayerPrefs.GetFloat(MOUSE_SENSITIVITY_KEY) : 1f;
    }

    public void SetMusicVolume(float newValue)
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, newValue);
        EventManager.EmitEvent(EventMessages.ON_MUSIC_VOLUME_CHANGED);
    }

    public int GetGraphicsQuality()
    {
        return PlayerPrefs.HasKey(GRAPHICS_QUALITY_KEY) ? PlayerPrefs.GetInt(GRAPHICS_QUALITY_KEY) : 3;
    }
    public float GetCameraDistance()
    {
        return PlayerPrefs.HasKey(CAMERA_DISTANCE_KEY) ? PlayerPrefs.GetFloat(CAMERA_DISTANCE_KEY) : defaultCameraDistanceToPlayer;
    }

    public float GetMusicVolume()
    {
        return PlayerPrefs.HasKey(MUSIC_VOLUME_KEY) ? PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY) : 1f;
    }

}
