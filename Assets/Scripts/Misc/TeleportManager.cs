using System.Linq;
using AF.Bonfires;
using AF.Companions;
using AF.Loading;
using AF.Music;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;

namespace AF
{
    public class TeleportManager : MonoBehaviour
    {
        [Header("Game Session")]
        public GameSession gameSession;

        [Header("Databases")]
        public BonfiresDatabase bonfiresDatabase;
        public UnityAction onChangingScene;

        [Header("Components")]
        public PlayerManager playerManager;
        public FadeManager fadeManager;
        public BGMManager bGMManager;
        public CompanionsSceneManager companionsSceneManager;
        public NotificationManager notificationManager;

        [SerializeField] SpawnLocationData bonfireSpawnLocationData;

        SceneLocation queuedSceneLocation;
        SpawnLocationData queuedSpawnLocation;

        void Start()
        {
            SpawnPlayer();

            companionsSceneManager.SpawnCompanions();

            LoadingManager.Instance.EndLoading();
        }

        public void TeleportToLastRestedBonfire()
        {
            if (string.IsNullOrEmpty(bonfiresDatabase.lastBonfireSceneId))
            {
                notificationManager.ShowNotification(
                    LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "No bonfire to travel to. Rest at one first."),
                 null);
                return;
            }

            Teleport(bonfiresDatabase.lastBonfireSceneId, bonfireSpawnLocationData);
        }

        public void Teleport(string sceneName)
        {
            Teleport(sceneName, bonfireSpawnLocationData);
        }

        public void Teleport(SceneLocation sceneLocation, SpawnLocationData spawnLocationData)
        {
            Teleport(sceneLocation.id, spawnLocationData);
        }

        public void Teleport(string sceneName, SpawnLocationData spawnLocationData)
        {
            gameSession.nextMap_SpawnLocationData = spawnLocationData;

            bGMManager.StopMusic();

            onChangingScene?.Invoke();

            fadeManager.FadeIn(1f, () =>
            {
                LoadingManager.Instance.BeginLoading(sceneName);
                //                SceneManager.LoadScene(sceneName);
                //StartCoroutine(LoadSceneAsync(sceneName));
            });
        }

        void SpawnPlayer()
        {
            if (gameSession.loadSavedPlayerPositionAndRotation)
            {
                gameSession.loadSavedPlayerPositionAndRotation = false;

                playerManager.playerComponentManager.UpdatePosition(gameSession.savedPlayerPosition, gameSession.savedPlayerRotation);
            }
            else if (gameSession.nextMap_SpawnLocationData != null)
            {

                SpawnLocationTransform[] candidates = FindObjectsByType<SpawnLocationTransform>(FindObjectsInactive.Include, FindObjectsSortMode.None);

                GameObject spawnGameObject = candidates
                    .FirstOrDefault(candidate => candidate.spawnLocationData == gameSession.nextMap_SpawnLocationData)?.gameObject;

                gameSession.nextMap_SpawnLocationData = null;

                if (spawnGameObject != null)
                {
                    playerManager.playerComponentManager.TeleportPlayer(spawnGameObject.transform);

                    if (spawnGameObject.transform.childCount > 0)
                    {
                        var targetRot = spawnGameObject.transform.GetChild(0).transform.position - spawnGameObject.transform.position;
                        targetRot.y = 0;
                        playerManager.transform.rotation = Quaternion.LookRotation(targetRot);
                    }
                }
            }
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void QueueSpawnLocation(SpawnLocationData spawnLocationData) => queuedSpawnLocation = spawnLocationData;


        /// <summary>
        /// Unity Event
        /// </summary>
        public void QueueSceneLocation(SceneLocation sceneLocation) => queuedSceneLocation = sceneLocation;


        /// <summary>
        /// Unity Event
        /// </summary>
        public void LoadQueuedLocation()
        {
            if (queuedSceneLocation != null && queuedSpawnLocation != null)
            {
                Teleport(queuedSceneLocation, queuedSpawnLocation);
                queuedSceneLocation = null;
                queuedSpawnLocation = null;
            }
        }
    }
}
