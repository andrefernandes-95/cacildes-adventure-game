using UnityEngine;
using System.Collections;
using AF.Music;
using TigerForge;
using AF.Events;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using DG.Tweening;

namespace AF
{
    public class SceneSettings : MonoBehaviour
    {
        [Header("Components")]
        public BGMManager bgmManager;
        public CursorManager cursorManager;
        public PlayerManager playerManager;
        public TeleportManager teleportManager;

        [Header("Ambient Music")]
        public AudioClip dayMusic;
        public AudioClip nightMusic;

        [Header("Playlists (Alternative to Ambient Music)")]
        public AudioClip[] playlist;
        Coroutine ChooseNextSongCoroutine;
        bool isPlayingMusicFromThePlaylist = false;

        [Header("Ambience Sounds")]
        public AudioClip dayAmbience;
        public AudioClip nightAmbience;

        [Header("Map")]
        public bool isInterior;
        public bool displaySceneName = true;
        public float displaySceneNameDelay = 3f;
        public float displaySceneNameDuration = 3f;
        public string sceneName = "";
        public UIDocument sceneNameDocument;
        public AudioClip sceneNameSfx;

        [Header("Tutorial")]
        public DestroyableParticle respawnFx;

        [Header("Systems")]
        public GameSession gameSession;
        public GameSettings gameSettings;
        public PickupDatabase pickupDatabase;
        public StarterAssetsInputs starterAssetsInputs;

        [Header("Events")]
        public UnityEvent onSceneStart;

        Coroutine DisplaySceneNameCoroutineInstance;

        void Awake()
        {
            sceneNameDocument.rootVisualElement.contentContainer.style.opacity = 0;

            onSceneStart?.Invoke();

            if (string.IsNullOrEmpty(sceneName))
            {
                sceneName = SceneManager.GetActiveScene().name;
            }

            gameSettings.LoadSettings();
        }

        private void Start()
        {
            if (displaySceneName)
            {
                DisplaySceneName();
            }

            EventManager.StartListening(EventMessages.ON_HOUR_CHANGED, OnHourChanged);
            OnHourChanged();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void DisplaySceneName()
        {
            if (DisplaySceneNameCoroutineInstance != null)
            {
                StopCoroutine(DisplaySceneNameCoroutineInstance);
            }

            DisplaySceneNameCoroutineInstance = StartCoroutine(DisplaySceneName_Coroutine());
        }

        IEnumerator DisplaySceneName_Coroutine()
        {
            yield return new WaitForSeconds(displaySceneNameDelay);

            sceneNameDocument.rootVisualElement.Q<Label>().text = sceneName;

            DOTween.To(
                  () => sceneNameDocument.rootVisualElement.contentContainer.style.opacity.value,
                  (value) => sceneNameDocument.rootVisualElement.contentContainer.style.opacity = value,
                  1,
                  1f
            );

            if (sceneNameSfx != null)
            {
                bgmManager.PlaySound(sceneNameSfx, null);
                yield return new WaitForSeconds(sceneNameSfx.length);
            }

            yield return new WaitForSeconds(displaySceneNameDuration);

            DOTween.To(
                  () => sceneNameDocument.rootVisualElement.contentContainer.style.opacity.value,
                  (value) => sceneNameDocument.rootVisualElement.contentContainer.style.opacity = value,
                  0,
                  1f
            );
        }

        /// <summary>
        /// Public Method to use for showing certain titles during in-game events
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>

        public void DisplaySceneName(string sceneName)
        {
            if (DisplaySceneNameCoroutineInstance != null)
            {
                StopCoroutine(DisplaySceneNameCoroutineInstance);
            }

            DisplaySceneNameCoroutineInstance = StartCoroutine(DisplaySceneName_Coroutine(sceneName));
        }

        public IEnumerator DisplaySceneName_Coroutine(string sceneName)
        {
            sceneNameDocument.rootVisualElement.Q<Label>().text = sceneName;

            DOTween.To(
                  () => sceneNameDocument.rootVisualElement.contentContainer.style.opacity.value,
                  (value) => sceneNameDocument.rootVisualElement.contentContainer.style.opacity = value,
                  1,
                  1f
            );

            yield return new WaitForSeconds(displaySceneNameDuration);

            DOTween.To(
                  () => sceneNameDocument.rootVisualElement.contentContainer.style.opacity.value,
                  (value) => sceneNameDocument.rootVisualElement.contentContainer.style.opacity = value,
                  0,
                  1f
            );
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="audioClip"></param>
        public void SetDayMusic(AudioClip audioClip)
        {
            this.dayMusic = audioClip;
        }

        void EvaluatePlaylist()
        {
            if (isPlayingMusicFromThePlaylist)
            {
                return;
            }

            bgmManager.StopMusic();

            AudioClip chosenAudioClip = playlist[Random.Range(0, playlist.Length)];
            bgmManager.PlayMusic(chosenAudioClip);
            isPlayingMusicFromThePlaylist = true;

            if (ChooseNextSongCoroutine != null)
            {
                StopCoroutine(ChooseNextSongCoroutine);
            }

            ChooseNextSongCoroutine = StartCoroutine(ChooseNextSong_Coroutine(chosenAudioClip.length));
        }

        IEnumerator ChooseNextSong_Coroutine(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            bgmManager.StopMusic();

            yield return new WaitForSeconds(5f);
            isPlayingMusicFromThePlaylist = false;

            EvaluatePlaylist();
        }

        void OnHourChanged()
        {
            pickupDatabase.OnHourChangedCheckForReplenishablesToClear();

            EvaluateDayNightMusic();
        }

        public void EvaluateDayNightMusic()
        {
            if (bgmManager.IsBusy())
            {
                return;
            }

            if (playlist != null && playlist.Length > 0)
            {
                EvaluatePlaylist();
            }
            else if (gameSession.IsNightTime())
            {
                EvaluateNightMusic();
            }
            else
            {
                EvaluateDayMusic();
            }
        }

        void EvaluateNightMusic()
        {
            if (nightMusic != null)
            {
                bgmManager.PlayMusic(nightMusic);
            }

            if (nightAmbience != null)
            {
                bgmManager.PlayAmbience(nightAmbience);
            }
        }

        void EvaluateDayMusic()
        {
            if (dayMusic != null)
            {
                bgmManager.PlayMusic(dayMusic);
            }

            if (dayAmbience != null)
            {
                bgmManager.PlayAmbience(dayAmbience);
            }
        }
    }
}
