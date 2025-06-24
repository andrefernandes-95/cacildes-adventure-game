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

        private void Awake()
        {
            gameObject.SetActive(false);
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
            SliderInt hairColorSlider = root.Q<SliderInt>("HairColorSlider");
            hairColorSlider.value = availableColors.IndexOf(gameSettings.hairColor);
            hairColorSlider.lowValue = 0;
            hairColorSlider.highValue = availableColors.Count - 1;

            hairColorSlider.RegisterValueChangedCallback(ev =>
            {
                int indx = (int)ev.newValue;
                gameSettings.hairColor = availableColors[indx];
                OnCharacterCustomized();
            });

            SliderInt bodyColorSlider = root.Q<SliderInt>("BodyColorSlider");
            bodyColorSlider.value = availableColors.IndexOf(gameSettings.skinColor);
            bodyColorSlider.lowValue = 0;
            bodyColorSlider.highValue = availableColors.Count - 1;
            bodyColorSlider.RegisterValueChangedCallback(ev =>
            {
                int indx = (int)ev.newValue;
                gameSettings.skinColor = availableColors[indx];
                OnCharacterCustomized();
            });


            SliderInt eyeColorSlider = root.Q<SliderInt>("EyeColorSlider");
            eyeColorSlider.value = availableColors.IndexOf(gameSettings.eyeColor);
            eyeColorSlider.lowValue = 0;
            eyeColorSlider.highValue = availableColors.Count - 1;
            eyeColorSlider.RegisterValueChangedCallback(ev =>
            {
                int indx = (int)ev.newValue;
                gameSettings.eyeColor = availableColors[indx];
                OnCharacterCustomized();
            });

            SliderInt tattooColorSlider = root.Q<SliderInt>("TattooColorSlider");
            tattooColorSlider.value = availableColors.IndexOf(gameSettings.tattooColor);
            tattooColorSlider.lowValue = 0;
            tattooColorSlider.highValue = availableColors.Count - 1;
            tattooColorSlider.RegisterValueChangedCallback(ev =>
            {
                int indx = (int)ev.newValue;
                gameSettings.tattooColor = availableColors[indx];
                OnCharacterCustomized();
            });

            VisualElement portraitPreview = root.Q<VisualElement>("PortraitPreview");
            portraitPreview.style.backgroundImage = new StyleBackground(portraits[gameSettings.playerPortrait]);

            SliderInt portrairSlider = root.Q<SliderInt>("PortraitSlider");
            portrairSlider.value = gameSettings.playerPortrait;
            portrairSlider.lowValue = 0;
            portrairSlider.highValue = portraits.Length - 1;
            portrairSlider.RegisterValueChangedCallback(ev =>
            {
                gameSettings.playerPortrait = (int)ev.newValue;
                portraitPreview.style.backgroundImage = new StyleBackground(portraits[gameSettings.playerPortrait]);
                OnCharacterCustomized();
            });
        }

        void SetupTypeSliders()
        {
            SliderInt bodyTypeSlider = root.Q<SliderInt>("BodyTypeSlider");
            bodyTypeSlider.value = gameSettings.isMale ? 0 : 1;
            bodyTypeSlider.lowValue = 0;
            bodyTypeSlider.highValue = 1;
            bodyTypeSlider.RegisterValueChangedCallback(ev =>
            {
                HandleBodyTypeChange(ev.newValue);
            });

            SliderInt faceTypeSlider = root.Q<SliderInt>("FaceTypeSlider");

            if (gameSettings.isMale)
            {
                faceTypeSlider.value = Array.IndexOf(maleFaces.ToArray(), gameSettings.face);
            }
            else
            {
                faceTypeSlider.value = Array.IndexOf(femaleFaces.ToArray(), gameSettings.face);
            }

            faceTypeSlider.lowValue = 0;
            faceTypeSlider.highValue = gameSettings.isMale ? maleFaces.Count : femaleFaces.Count;
            faceTypeSlider.RegisterValueChangedCallback(ev =>
            {
                HandleFaceChange((int)ev.newValue);
            });

            SliderInt hairTypeSlider = root.Q<SliderInt>("HairTypeSlider");
            hairTypeSlider.value = Array.IndexOf(hairs.ToArray(), gameSettings.hair);
            hairTypeSlider.lowValue = 0;
            hairTypeSlider.highValue = hairs.Count;
            hairTypeSlider.RegisterValueChangedCallback(ev =>
            {
                HandleHairChange((int)ev.newValue);
            });

            SliderInt eyebrowTypeSlider = root.Q<SliderInt>("EyebrowTypeSlider");
            eyebrowTypeSlider.value = Array.IndexOf(eyebrows.ToArray(), gameSettings.eyebrows);
            eyebrowTypeSlider.lowValue = 0;
            eyebrowTypeSlider.highValue = eyebrows.Count;
            eyebrowTypeSlider.RegisterValueChangedCallback(ev =>
            {
                HandleEyebrowsChange((int)ev.newValue);
            });

            SliderInt beardTypeSlider = root.Q<SliderInt>("BeardTypeSlider");
            beardTypeSlider.value = Array.IndexOf(beards.ToArray(), gameSettings.beard);
            beardTypeSlider.lowValue = 0;
            beardTypeSlider.highValue = beards.Count;
            beardTypeSlider.RegisterValueChangedCallback(ev =>
            {
                HandleBeardChange((int)ev.newValue);
            });
        }

        void HandleBodyTypeChange(int value)
        {
            gameSettings.isMale = value == 0;
            OnCharacterCustomized();
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
            gameSettings.isMale = true;
            gameSettings.hair = hairs[0];
            gameSettings.face = maleFaces[0];
            gameSettings.beard = beards[0];
            gameSettings.eyebrows = eyebrows[0];
            gameSettings.skinColor = gameSettings.defaultSkinColor;
            gameSettings.hairColor = gameSettings.defaultHairColor;
            gameSettings.eyeColor = gameSettings.defaultEyeColor;
            gameSettings.tattooColor = gameSettings.defaultTattooColor;
            gameSettings.playerPortrait = 0;
            OnCharacterCustomized();
        }

        void OnCharacterCustomized()
        {
            EventManager.EmitEvent(EventMessages.ON_CHARACTER_CUSTOMIZED);
        }
    }
}
