using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    // TODO: Remove
    public class ViewComponent_GameSettings : MonoBehaviour
    {
        VisualElement optionsContainer;

        Button controlsButton, audioButton, languageButton, graphicsButton, saveProgressButton, exitButton, newGamePlusButton, gameplayButton, resetSettingsButton;

        [SerializeField] GameSettings gameSettings;
        [SerializeField] StarterAssetsInputs inputs;

        [Header("Quest To Allow New Game Plus")]
        public QuestParent questParentToAllowNewGamePlus;

        // Sub-menus
        VisualElement controlsMenu;
        VisualElement graphicsMenu;
        VisualElement audioMenu;
        VisualElement languageMenu;
        VisualElement gameplayMenu;

        [SerializeField] SaveManager saveManager;
        [SerializeField] GameSession gameSession;


        [Header("Components")]
        UIDocumentPlayerHUDV2 uiDocumentPlayerHUDV2;
        [SerializeField] Soundbank soundbank;
        [SerializeField] FadeManager fadeManager;
        [SerializeField] MenuManager menuManager;

        VisualElement root => GetComponent<UIDocument>().rootVisualElement;

        public void SetupRefs()
        {
            if (uiDocumentPlayerHUDV2 == null)
            {
                uiDocumentPlayerHUDV2 = FindAnyObjectByType<UIDocumentPlayerHUDV2>(FindObjectsInactive.Include);
            }

            UpdateUI();
        }


        void UpdateUI()
        {
            optionsContainer = root.Q<VisualElement>("OptionsMenu");
            optionsContainer.style.display = DisplayStyle.Flex;

            controlsButton = root.Q<Button>("ControlsSettings");
            graphicsButton = root.Q<Button>("GraphicsSettings");
            gameplayButton = root.Q<Button>("GameplaySettings");
            languageButton = root.Q<Button>("LanguageSettings");
            audioButton = root.Q<Button>("AudioSettings");

            saveProgressButton = root.Q<Button>("SaveGame");
            saveProgressButton.SetEnabled(saveManager.CanSave());

            exitButton = root.Q<Button>("ExitGame");
            newGamePlusButton = root.Q<Button>("NewGamePlus");
            resetSettingsButton = root.Q<Button>("ResetSettings");

            controlsMenu = root.Q<VisualElement>("Controls");
            graphicsMenu = root.Q<VisualElement>("Graphics");
            audioMenu = root.Q<VisualElement>("Audio");
            languageMenu = root.Q<VisualElement>("Language");
            gameplayMenu = root.Q<VisualElement>("Gameplay");

            DisableSubMenus();

            SetupButtons();

            ShowDefaultSubMenu();
        }

        void ShowDefaultSubMenu()
        {
            controlsButton.Focus();
            controlsMenu.style.display = DisplayStyle.Flex;
        }

        void SetupButtons()
        {
            SetupControlsButton();
            SetupGraphicsButton();
            SetupAudioButton();
            SetupLanguageButton();
            SetupSaveProgressButton();
            SetupNewGamePlusButtons();
            SetupExitButton();
            SetupResetSettingsButton();
            SetupGameplaySettingsButton();
        }

        void DisableSubMenus()
        {
            controlsMenu.style.display = DisplayStyle.None;
            graphicsMenu.style.display = DisplayStyle.None;
            audioMenu.style.display = DisplayStyle.None;
            languageMenu.style.display = DisplayStyle.None;
            gameplayMenu.style.display = DisplayStyle.None;
        }

        void SetupControlsButton()
        {
            UIUtils.SetupButton(controlsButton, () =>
            {
                DisableSubMenus();

                controlsMenu.style.display = DisplayStyle.Flex;
            }, soundbank);
        }

        void SetupGameplaySettingsButton()
        {
            UIUtils.SetupButton(gameplayButton, () =>
            {
                DisableSubMenus();

                gameplayMenu.style.display = DisplayStyle.Flex;
            }, soundbank);
        }

        void SetupGraphicsButton()
        {
            UIUtils.SetupButton(graphicsButton, () =>
            {
                DisableSubMenus();

                graphicsMenu.style.display = DisplayStyle.Flex;
            }, soundbank);
        }

        void SetupAudioButton()
        {
            UIUtils.SetupButton(audioButton, () =>
            {
                DisableSubMenus();

                audioMenu.style.display = DisplayStyle.Flex;
            }, soundbank);
        }

        void SetupLanguageButton()
        {
            UIUtils.SetupButton(languageButton, () =>
            {
                DisableSubMenus();

                languageMenu.style.display = DisplayStyle.Flex;
            }, soundbank);
        }

        void SetupSaveProgressButton()
        {
            UIUtils.SetupButton(saveProgressButton, () =>
            {
                DisableSubMenus();

                saveManager.TrySaveGameData(menuManager.screenshotBeforeOpeningMenu);
            }, soundbank);
        }

        void SetupNewGamePlusButtons()
        {
            if (questParentToAllowNewGamePlus != null && questParentToAllowNewGamePlus.IsCompleted())
            {
                UIUtils.SetupButton(newGamePlusButton, () =>
                {
                    AnalyticsUtils.OnBeginNewGamePlus(saveManager.playerManager);

                    fadeManager.FadeIn(1f, () =>
                    {
                        saveManager.ResetGameStateForNewGamePlusAndReturnToTitleScreen();
                    });

                }, soundbank);
                newGamePlusButton.style.display = DisplayStyle.Flex;
            }
            else
            {
                newGamePlusButton.style.display = DisplayStyle.None;
            }
        }

        void SetupExitButton()
        {
            UIUtils.SetupButton(exitButton, () =>
            {
                fadeManager.FadeIn(1f, () =>
                {
                    saveManager.ResetGameStateAndReturnToTitleScreen(false);
                });

            }, soundbank);
        }

        void SetupResetSettingsButton()
        {
            UIUtils.SetupButton(resetSettingsButton, () =>
            {
                gameSettings.ResetSettings();
                inputs.RestoreDefaultKeyBindings();
            }, soundbank);
        }

    }
}
