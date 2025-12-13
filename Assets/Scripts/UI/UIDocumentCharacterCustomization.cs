using System;
using System.Collections.Generic;
using AF.Events;
using GameAnalyticsSDK;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentCharacterCustomization : MonoBehaviour
    {
        UIDocument document => GetComponent<UIDocument>();

        [Header("Components")]
        public PlayerManager playerManager;
        [SerializeField] GameSettings gameSettings;
        [SerializeField] StarterAssetsInputs starterAssetsInputs;

        public Soundbank soundbank;
        public CursorManager cursorManager;
        VisualElement root;

        Button saveChangesButton, resetSettingsButton;

        public UnityEvent onEnable;
        public UnityEvent onDisable;

        readonly List<string> availableColors = new()
        {

    "#FFCCAE",   // Cacildes Color
 
    // Browns
    "#4F412D",   // Cacildes Hair
    "#8B4513",   // Brown
    "#654321",   // Dark Brown
    "#A0522D",   // Medium Brown
    "#CD853F",   // Light Brown
    "#D2691E",   // Chocolate

    // Reds
    "#FF0000",   // Red
    "#800000",   // Dark Red
    "#CC3333",   // Medium Red
    "#FF6666",   // Light Red
    "#DC143C",   // Crimson
    "#FF6347",   // Tomato
    "#FF4500",   // Orange Red

    // Oranges
    "#FF6600",   // Dark Orange
    "#FF8800",   // Orange
    "#FFAA33",   // Medium Orange
    "#FFCC66",   // Light Orange

    // Yellows
    "#FFFF00",   // Yellow
    "#CCCC00",   // Dark Yellow
    "#FFFF66",   // Medium Yellow
    "#FFFF99",   // Light Yellow
    "#FFD700",   // Gold

    // Greens
    "#00FF00",   // Green
    "#008000",   // Dark Green
    "#00CC00",   // Medium Green
    "#99FF99",   // Light Green
    "#ADFF2F",   // Green Yellow
    "#9ACD32",   // Yellow Green
    "#00FA9A",   // Medium Spring Green
    "#20B2AA",   // Light Sea Green
    "#7FFFD4",   // Aquamarine

    // Blues
    "#0000FF",   // Blue
    "#000080",   // Dark Blue
    "#3333FF",   // Medium Blue
    "#99CCFF",   // Light Blue
    "#87CEEB",   // Sky Blue
    "#4682B4",   // Steel Blue

    // Purples / Violets / Lavenders
    "#800080",   // Purple
    "#660066",   // Dark Purple
    "#9933FF",   // Medium Purple
    "#CC99FF",   // Light Purple
    "#EE82EE",   // Violet
    "#DA70D6",   // Orchid
    "#D8BFD8",   // Thistle
    "#E6E6FA",   // Lavender

    // Pinks
    "#FFC0CB",   // Pink
    "#FF69B4",   // Dark Pink
    "#FFB6C1",   // Medium Pink
    "#FFCCCC",   // Light Pink

    // Other / Special
    "#40E0D0",    // Turquoise


            // Neutrals / Greys / Whites / Blacks
            "#000000",   // Black
    "#333333",   // Dark Gray
    "#666666",   // Gray
    "#999999",   // Light Gray
    "#D3D3D3",   // Light Grey
    "#778899",   // Light Slate Gray
    "#B0C4DE",   // Light Steel Blue
    "#FFFFFF",   // White
    "#F5F5DC",   // Beige
    "#F5DEB3",   // Wheat

        };

        public Sprite[] portraits;

        public List<string> maleFaces = new();
        public List<string> femaleFaces = new();
        public List<string> hairs = new();
        public List<string> eyebrows = new();
        public List<string> beards = new();

        // Cached sliders
        SliderInt bodyTypeSlider;
        SliderInt faceTypeSlider;
        SliderInt hairTypeSlider;
        SliderInt eyebrowTypeSlider;
        SliderInt beardTypeSlider;

        SliderInt hairColorSlider;
        SliderInt bodyColorSlider;
        SliderInt eyeColorSlider;
        SliderInt tattooColorSlider;
        SliderInt portraitSlider;

        VisualElement portraitPreview;


        private void Awake()
        {
            gameObject.SetActive(false);

            starterAssetsInputs.onMenuEvent.AddListener(Close);
        }

        private void OnEnable()
        {
            root = document.rootVisualElement;

            saveChangesButton = root.Q<Button>("SaveButton");
            resetSettingsButton = root.Q<Button>("ResetToDefaultButton");

            SetupUI();

            UIUtils.SetupButton(saveChangesButton, () =>
            {
                LogAnalytic(AnalyticsUtils.OnUIButtonClick($"CustomizedCharacter:PlayerName:{gameSettings.playerName}"));

                this.gameObject.SetActive(false);
            }, soundbank);

            UIUtils.SetupButton(resetSettingsButton, () =>
            {
                ResetDefaults();
            }, soundbank);


            cursorManager.ShowCursor();

            // Delay the focus until the next frame, required as an hack for now
            Invoke(nameof(GiveFocus), 0f);

            onEnable?.Invoke();

            playerManager.playerComponentManager.DisablePlayerControl();
        }

        private void OnDisable()
        {

            playerManager.playerComponentManager.EnablePlayerControl();

            onDisable?.Invoke();
        }

        void GiveFocus()
        {
            saveChangesButton.Focus();
        }

        /// <summary>
        ///  Unity Event
        /// </summary>
        public void Close()
        {
            if (this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(false);
            }
        }

        void SetupUI()
        {
            SetupNameInput();
            SetupTypeSliders();
            SetupColorSliders();
        }

        void SetupNameInput()
        {
            TextField nameInput = root.Q<TextField>("CharacterName");
            nameInput.value = gameSettings.playerName;
            nameInput.RegisterValueChangedCallback(ev =>
            {
                gameSettings.playerName = ev.newValue;
                OnCharacterCustomized();
            });
            nameInput.SetEnabled(Gamepad.current == null);
        }

        void SetupColorSliders()
        {
            hairColorSlider = root.Q<SliderInt>("HairColorSlider");
            bodyColorSlider = root.Q<SliderInt>("BodyColorSlider");
            eyeColorSlider = root.Q<SliderInt>("EyeColorSlider");
            tattooColorSlider = root.Q<SliderInt>("TattooColorSlider");
            portraitSlider = root.Q<SliderInt>("PortraitSlider");

            portraitPreview = root.Q<VisualElement>("PortraitPreview");

            SetupColorSlider(hairColorSlider, gameSettings.hairColor, c => gameSettings.hairColor = c);
            SetupColorSlider(bodyColorSlider, gameSettings.skinColor, c => gameSettings.skinColor = c);
            SetupColorSlider(eyeColorSlider, gameSettings.eyeColor, c => gameSettings.eyeColor = c);
            SetupColorSlider(tattooColorSlider, gameSettings.tattooColor, c => gameSettings.tattooColor = c);

            portraitSlider.lowValue = 0;
            portraitSlider.highValue = portraits.Length - 1;
            portraitSlider.value = Mathf.Clamp(gameSettings.playerPortrait, 0, portraits.Length - 1);

            portraitPreview.style.backgroundImage =
                new StyleBackground(portraits[portraitSlider.value]);

            portraitSlider.RegisterValueChangedCallback(ev =>
            {
                gameSettings.playerPortrait = ev.newValue;
                portraitPreview.style.backgroundImage =
                    new StyleBackground(portraits[ev.newValue]);
                OnCharacterCustomized();
            });
        }

        void SetupColorSlider(
            SliderInt slider,
            string currentColor,
            Action<string> onChanged)
        {
            slider.lowValue = 0;
            slider.highValue = availableColors.Count - 1;

            int index = availableColors.IndexOf(currentColor);
            slider.value = index >= 0 ? index : 0;

            slider.RegisterValueChangedCallback(ev =>
            {
                onChanged.Invoke(availableColors[ev.newValue]);
                OnCharacterCustomized();
            });
        }

        void SetupTypeSliders()
        {
            bodyTypeSlider = root.Q<SliderInt>("BodyTypeSlider");
            faceTypeSlider = root.Q<SliderInt>("FaceTypeSlider");
            hairTypeSlider = root.Q<SliderInt>("HairTypeSlider");
            eyebrowTypeSlider = root.Q<SliderInt>("EyebrowTypeSlider");
            beardTypeSlider = root.Q<SliderInt>("BeardTypeSlider");

            bodyTypeSlider.lowValue = 0;
            bodyTypeSlider.highValue = 1;
            bodyTypeSlider.value = gameSettings.isMale ? 0 : 1;
            bodyTypeSlider.RegisterValueChangedCallback(ev =>
            {
                HandleBodyTypeChange(ev.newValue);
                RefreshFaceSlider();
                OnCharacterCustomized();
            });

            SetupIndexedSlider(
                faceTypeSlider,
                gameSettings.isMale ? maleFaces : femaleFaces,
                gameSettings.face,
                HandleFaceChange);

            SetupIndexedSlider(
                hairTypeSlider,
                hairs,
                gameSettings.hair,
                HandleHairChange);

            SetupIndexedSlider(
                eyebrowTypeSlider,
                eyebrows,
                gameSettings.eyebrows,
                HandleEyebrowsChange);

            SetupIndexedSlider(
                beardTypeSlider,
                beards,
                gameSettings.beard,
                HandleBeardChange);
        }

        void SetupIndexedSlider(
            SliderInt slider,
            List<string> source,
            string currentValue,
            Action<int> onChanged)
        {
            slider.lowValue = 0;
            slider.highValue = source.Count - 1;

            int index = source.IndexOf(currentValue);
            slider.value = index >= 0 ? index : 0;

            slider.RegisterValueChangedCallback(ev => onChanged(ev.newValue));
        }

        void RefreshFaceSlider()
        {
            var list = gameSettings.isMale ? maleFaces : femaleFaces;
            SetupIndexedSlider(faceTypeSlider, list, list[0], HandleFaceChange);
        }

        void HandleBodyTypeChange(int value)
        {
            gameSettings.isMale = value == 0;
            EventManager.EmitEvent(EventMessages.ON_BODY_TYPE_CHANGED);
        }

        void HandleFaceChange(int faceIndex)
        {
            if (gameSettings.isMale && faceIndex < maleFaces.Count)
            {
                gameSettings.face = maleFaces[faceIndex];
            }
            else if (gameSettings.isMale == false && faceIndex < femaleFaces.Count)
            {
                gameSettings.face = femaleFaces[faceIndex];
            }
            OnCharacterCustomized();
        }

        void HandleHairChange(int hairIndex)
        {
            if (hairIndex < hairs.Count)
            {
                gameSettings.hair = hairs[hairIndex];
            }
            OnCharacterCustomized();
        }

        void HandleBeardChange(int beardIndex)
        {
            if (beardIndex < beards.Count)
            {
                gameSettings.beard = beards[beardIndex];
            }

            OnCharacterCustomized();
        }

        void HandleEyebrowsChange(int eyebrowIndex)
        {
            if (eyebrowIndex < eyebrows.Count)
            {
                gameSettings.eyebrows = eyebrows[eyebrowIndex];
            }

            OnCharacterCustomized();
        }

        void LogAnalytic(string eventName)
        {
            if (!GameAnalytics.Initialized)
            {
                GameAnalytics.Initialize();
            }

            GameAnalytics.NewDesignEvent(eventName);
        }

        void ResetDefaults()
        {
            gameSettings.playerName = gameSettings.defaultPlayerName;

            bodyTypeSlider.value = 0; // Male default
            hairTypeSlider.value = 0;
            faceTypeSlider.value = 0;
            eyebrowTypeSlider.value = 0;
            beardTypeSlider.value = 0;

            hairColorSlider.value = availableColors.IndexOf(gameSettings.defaultHairColor);
            bodyColorSlider.value = availableColors.IndexOf(gameSettings.defaultSkinColor);
            eyeColorSlider.value = availableColors.IndexOf(gameSettings.defaultEyeColor);
            tattooColorSlider.value = availableColors.IndexOf(gameSettings.defaultTattooColor);

            portraitSlider.value = 0;

            portraitPreview.style.backgroundImage =
                new StyleBackground(portraits[0]);

            OnCharacterCustomized();
        }

        void OnCharacterCustomized()
        {
            EventManager.EmitEvent(EventMessages.ON_CHARACTER_CUSTOMIZED);
        }
    }
}
