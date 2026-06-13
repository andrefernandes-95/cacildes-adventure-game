using System.Collections;
using GameAnalyticsSDK;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentTitleScreen : MonoBehaviour
    {
        UIDocument document => GetComponent<UIDocument>();

        [Header("Components")]
        [SerializeField] PlayerManager playerManager;
        public TitleScreenManager titleScreenManager;

        public CursorManager cursorManager;
        public UIDocumentTitleScreenCredits uIDocumentTitleScreenCredits;
        public UIDocumentChangelog uIDocumentChangelog;
        public UIDocumentTitleScreenOptions uIDocumentTitleScreenOptions;
        public UIDocumentTitleScreenSaveFiles uIDocumentTitleScreenSaveFiles;
        public Soundbank soundbank;
        public SaveManager saveManager;

        [SerializeField] SteamDLC supporterEdition;

        [Header("Game Session")]
        public GameSession gameSession;

        VisualElement root;
        VisualElement TitleScreenContainer;
        VisualElement GDPRWarning;
        Button acceptGDPRButton;
        Button rejectGDPRButton;

        private void PopIn(Button button)
        {
            // Animate scale to 1.2x size (pop-in)
            button.experimental.animation.Scale(1.2f, 400)
                .OnCompleted(() => PopOut(button));  // After pop-in, call PopOut()
        }

        private void PopOut(Button button)
        {
            // Animate scale back to original size (pop-out)
            button.experimental.animation.Scale(1f, 400)
                .OnCompleted(() => PopIn(button));  // After pop-out, call PopIn() again for looping
        }

        private void OnEnable()
        {
            root = document.rootVisualElement;

            var versionLabel = root.Q<Label>("Version");
            versionLabel.text = Application.version;

            root.Q<Label>("SupporterEdition").style.display = supporterEdition.IsOwned() ? DisplayStyle.Flex : DisplayStyle.None;

            Button newGameButton = root.Q<Button>("NewGameButton");
            Button continueButton = root.Q<Button>("ContinueButton");
            Button loadGameButton = root.Q<Button>("LoadGameButton");
            Button playTutorialButton = root.Q<Button>("PlayTutorialButton");
            Button optionsButton = root.Q<Button>("OptionsButton");
            Button controlsButton = root.Q<Button>("ControlsButton");
            Button creditsButton = root.Q<Button>("CreditsButton");
            Button changelogButton = root.Q<Button>("ChangelogButton");
            Button exitButton = root.Q<Button>("ExitButton");
            Button btnGithub = root.Q<Button>("btnGithub");
            Button joinDiscordButton = root.Q<Button>("JoinDiscord");
            Button myMusicButton = root.Q<Button>("VisitBandcamp");
            Button btnYoutube = root.Q<Button>("btnYoutube");
            Button btnBlueSky = root.Q<Button>("btnBlueSky");
            Button btnItchio = root.Q<Button>("btnItchio");
            Button btnInstagram = root.Q<Button>("btnInstagram");

            UIUtils.SetupButton(newGameButton, () =>
            {
                AnalyticsUtils.OnBeginNewGame(playerManager.discordNotifier);
                saveManager.ResetGameState(false);
                titleScreenManager.StartGame();

                gameObject.SetActive(false);
            }, soundbank);

            continueButton.SetEnabled(saveManager.HasSavedGame());

            UIUtils.SetupButton(continueButton, () =>
            {
                saveManager.LoadLastSavedGame(false);
                gameObject.SetActive(false);
            }, soundbank);

            UIUtils.SetupButton(loadGameButton, () =>
            {
                uIDocumentTitleScreenSaveFiles.gameObject.SetActive(true);
                gameObject.SetActive(false);
            }, soundbank);

            UIUtils.SetupButton(joinDiscordButton, () =>
            {
                AnalyticsUtils.OnDiscordVisit();
                Application.OpenURL("https://discord.gg/JwnZMc27D2");
                joinDiscordButton.Focus();
            }, soundbank);

            joinDiscordButton.style.scale = new Scale(Vector3.one); // Set initial scale
            joinDiscordButton.RegisterCallback<GeometryChangedEvent>(evt => PopIn(joinDiscordButton));

            UIUtils.SetupButton(creditsButton, () =>
            {
                uIDocumentTitleScreenCredits.gameObject.SetActive(true);
                gameObject.SetActive(false);
            }, soundbank);

            UIUtils.SetupButton(changelogButton, () =>
            {
                uIDocumentChangelog.gameObject.SetActive(true);
                gameObject.SetActive(false);
            }, soundbank);

            UIUtils.SetupButton(optionsButton, () =>
            {
                uIDocumentTitleScreenOptions.gameObject.SetActive(true);
                gameObject.SetActive(false);
            }, soundbank);

            UIUtils.SetupButton(myMusicButton, () =>
            {
                AnalyticsUtils.OnBandcampVisit();
                Application.OpenURL("https://polygoncity.bandcamp.com/");
                myMusicButton.Focus();
            }, soundbank);

            UIUtils.SetupButton(exitButton, () =>
            {
                Application.Quit();
            }, soundbank);

            UIUtils.SetupButton(btnGithub, () =>
            {
                AnalyticsUtils.OnGithubVisit();
                Application.OpenURL("https://github.com/andrefernandes-95/cacildes-adventure-game");
            }, soundbank);

            UIUtils.SetupButton(btnBlueSky, () =>
            {

                Application.OpenURL("https://bsky.app/profile/cacildesadventure.bsky.social");
            }, soundbank);

            UIUtils.SetupButton(btnItchio, () =>
            {

                Application.OpenURL("https://andrefcasimiro.itch.io/");
            }, soundbank);

            UIUtils.SetupButton(btnYoutube, () =>
            {
                Application.OpenURL("https://www.youtube.com/@CacildesAdventure");
            }, soundbank);

            UIUtils.SetupButton(btnInstagram, () =>
            {
                Application.OpenURL("https://www.instagram.com/cacildes_adventure/");
            }, soundbank);

            root.Q<VisualElement>("Snow").style.display = SeasonalEvents.IsChristmasTime() ? DisplayStyle.Flex : DisplayStyle.None;

            cursorManager.ShowCursor();

            SetupGDPRWarning();
        }

        void SetupGDPRWarning()
        {
            GDPRWarning = root.Q<VisualElement>("GDPRWarning");
            TitleScreenContainer = root.Q<VisualElement>("TitleScreenContainer");

            acceptGDPRButton = root.Q<Button>("AcceptGDPR");
            rejectGDPRButton = root.Q<Button>("DeclineGDPR");

            var DataCollectionToggle = root.Q<Toggle>("DataCollectionToggle");
            DataCollectionToggle.RegisterValueChangedCallback(ev =>
            {
                if (ev.newValue)
                {
                    AnalyticsConsentManager.Instance.AcceptAnalytics();
                }
                else
                {

                    AnalyticsConsentManager.Instance.DeclineAnalytics();
                }
            });

            UpdateConsentToggle();

            bool isShowingGDPRWarning = false;

            if (AnalyticsConsentManager.Instance == null || AnalyticsConsentManager.Instance.ConsentState != AnalyticsConsentState.Unknown)
            {
                Debug.LogWarning("AnalyticsConsentManager not found.");
                GDPRWarning.style.display = DisplayStyle.None;
                TitleScreenContainer.style.display = DisplayStyle.Flex;
            }
            else
            {
                isShowingGDPRWarning = true;
                GDPRWarning.style.display = DisplayStyle.Flex;
                TitleScreenContainer.style.display = DisplayStyle.None;
            }

            UIUtils.SetupButton(acceptGDPRButton, () =>
            {
                HandleGDPRClick(true);
            }, soundbank);

            UIUtils.SetupButton(rejectGDPRButton, () =>
            {
                HandleGDPRClick(false);
            }, soundbank);

            StartCoroutine(GiveFocus(isShowingGDPRWarning));
        }

        void HandleGDPRClick(bool isConsenting)
        {
            if (isConsenting)
            {
                AnalyticsConsentManager.Instance.AcceptAnalytics();
            }
            else
            {
                AnalyticsConsentManager.Instance.DeclineAnalytics();
            }

            GDPRWarning.style.display = DisplayStyle.None;
            TitleScreenContainer.style.display = DisplayStyle.Flex;

            UpdateConsentToggle();

            StartCoroutine(GiveFocus(false));
        }

        void UpdateConsentToggle()
        {
            var DataCollectionToggle = root.Q<Toggle>("DataCollectionToggle");
            DataCollectionToggle.value = AnalyticsConsentManager.Instance.ConsentState == AnalyticsConsentState.Accepted;
        }

        IEnumerator GiveFocus(bool isShowingGDPRWarning)
        {
            yield return new WaitForSeconds(.5f);

            if (isShowingGDPRWarning)
            {
                acceptGDPRButton.Focus();
            }
            else
            {
                Button newGameButton = root.Q<Button>("NewGameButton");
                newGameButton.Focus();
            }
        }
    }
}
