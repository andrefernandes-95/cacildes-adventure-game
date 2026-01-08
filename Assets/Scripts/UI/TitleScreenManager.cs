using System.Collections;
using AF.Music;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class TitleScreenManager : MonoBehaviour
    {
        [Header("Events")]
        public UnityEvent onAwake_Event;
        public UnityEvent onPlayerBeginsGame_Event;
        public UnityEvent ifPlayerHasSeenTitleScreen_Event;

        [Header("Game Session")]
        public GameSession gameSession;
        public SaveManager saveManager;
        [SerializeField] PlayerManager playerManager;
        [SerializeField] BGMManager bGMManager;
        public GameSettings gameSettings;
        [SerializeField] CursorManager cursorManager;

        private void Awake()
        {
            if (gameSession.gameState == GameSession.GameState.NOT_INITIALIZED)
            {
                saveManager.ResetGameState(false);
                gameSession.gameState = GameSession.GameState.INITIALIZED;
            }

            gameSettings.UpdatePlayerNameOnLocalizedAssets();
        }

        private void Start()
        {
            bool shouldBeginImmediately = false;

            if (gameSession.gameState == GameSession.GameState.BEGINNING_NEW_GAME_PLUS)
            {
                gameSession.gameState = GameSession.GameState.INITIALIZED_AND_SHOWN_TITLE_SCREEN;
                gameSession.currentGameIteration++;

                shouldBeginImmediately = true;
            }
            else if (gameSession.gameState == GameSession.GameState.INITIALIZED_AND_SHOWN_TITLE_SCREEN)
            {
                ifPlayerHasSeenTitleScreen_Event?.Invoke();
                gameObject.SetActive(false);
                return;
            }

            onAwake_Event?.Invoke();

            // Show cursor
            if (cursorManager != null)
            {
                cursorManager.ShowCursor();
            }

            // Hide Player HUD
            playerManager.uIDocumentPlayerHUDV2.HideHUD();

            if (shouldBeginImmediately)
            {
                StartGame();
            }

            PlayTitleScreenMusic();

            StartCoroutine(MakePlayerSleep());
        }

        void PlayTitleScreenMusic()
        {
            if (bGMManager != null && gameSettings.GetCurrentGame() != null && gameSettings.GetCurrentGame().titleScreenMusic != null)
            {
                bGMManager.PlayMusic(gameSettings.GetCurrentGame().titleScreenMusic);
            }
        }

        IEnumerator MakePlayerSleep()
        {
            yield return new WaitForEndOfFrame();

            playerManager.PlayBusyAnimationWithRootMotion("Sleeping");
        }

        public void StartGame()
        {
            gameSession.gameState = GameSession.GameState.INITIALIZED_AND_SHOWN_TITLE_SCREEN;
            onPlayerBeginsGame_Event?.Invoke();

            // Hide cursor
            if (cursorManager != null)
            {
                cursorManager.HideCursor();
            }

            // Show Player HUD
            playerManager.uIDocumentPlayerHUDV2.ShowHUD();

            gameObject.SetActive(false);
        }
    }
}
