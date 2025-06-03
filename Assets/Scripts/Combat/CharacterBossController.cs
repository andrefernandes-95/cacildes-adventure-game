using System.Linq;
using AF.Events;
using AF.Flags;
using AF.Music;
using GameAnalyticsSDK;
using TigerForge;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace AF
{
#if UNITY_EDITOR

    [CustomEditor(typeof(CharacterBossController), editorForChildClasses: true)]
    public class CharacterBossControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            CharacterBossController bossController = target as CharacterBossController;

            if (GUILayout.Button("Test Boss Fight"))
            {
                bossController.TestBossFight();
            }
        }
    }
#endif
    public class CharacterBossController : MonoBehaviour
    {
        public bool isBoss = false;

        [Header("Settings")]
        public string bossName;
        public AudioClip bossMusic;

        public UIDocument bossHud;
        public IMGUIContainer bossFillBar;

        public CharacterManager characterManager;

        [Header("Events")]
        public UnityEvent onBattleBegin;
        public UnityEvent onBossDefeated;

        // Flags
        [HideInInspector] public bool bossBattleHasBegun = false;

        [Header("Flags")]
        public MonoBehaviourID monoBehaviourID;
        public FlagsDatabase flagsDatabase;

        // Scene References
        private BGMManager bgmManager;
        private SceneSettings sceneSettings;

        public void Start()
        {
            HideBossHud();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void UpdateUI()
        {
            if (IsBossHUDEnabled())
            {
                if (!isBoss)
                {
                    return;
                }

                if (characterManager.health.GetCurrentHealth() <= 0)
                {
                    HideBossHud();
                    return;
                }

                bossFillBar ??= bossHud.rootVisualElement.Q<IMGUIContainer>("hp-bar");
                bossFillBar.style.width = new Length(characterManager.health.GetCurrentHealth() * 100 / characterManager.health.GetMaxHealth(), LengthUnit.Percent);
            }
        }

        public void ShowBossHud()
        {
            if (bossHud == null)
            {
                return;
            }

            bossHud.enabled = true;
            bossHud.rootVisualElement.Q<Label>("boss-name").text = bossName;
            bossHud.rootVisualElement.style.display = DisplayStyle.Flex;
            bossHud.rootVisualElement.Q<VisualElement>("container").style.marginBottom = characterManager.partnerOrder == 0 ? 0 : 60 * characterManager.partnerOrder;
            UpdateUI();
        }

        public void HideBossHud()
        {
            if (bossHud == null || bossHud?.rootVisualElement == null)
            {
                return;
            }

            bossHud.rootVisualElement.style.display = DisplayStyle.None;
        }

        public bool IsBossHUDEnabled()
        {
            return bossHud != null && bossHud.enabled;
        }

        public void BeginBossBattle()
        {
            if (bossBattleHasBegun)
            {
                return;
            }

            if (characterManager.health.GetCurrentHealth() <= 0)
            {
                return;
            }

            bossBattleHasBegun = true;

            ShowBossHud();

            if (bossMusic != null && GetBGMManager() != null)
            {
                GetBGMManager().PlayMainMusic(bossMusic);
            }

            if (characterManager.partnerOrder == 0)
            {
                // Notify other boss companions that battle has begun
                foreach (CharacterManager partner in characterManager.partners)
                {
                    if (partner.gameObject.activeInHierarchy)
                    {
                        partner.characterBossController.BeginBossBattle();
                    }
                }
            }

            onBattleBegin?.Invoke();

            EventManager.EmitEvent(EventMessages.ON_BOSS_BATTLE_BEGINS);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnAllBossesDead()
        {
            bool isDead = characterManager.health.GetCurrentHealth() <= 0;

            if (isDead)
            {
                LogAnalytic(AnalyticsUtils.OnBossKilled(bossName));
            }

            bool allPartnersAreDead = isDead && characterManager.partners?.Length > 0
                && characterManager.partners.All(partner => partner.health.GetCurrentHealth() <= 0);

            if (characterManager.partners?.Length > 0 ? allPartnersAreDead : isDead)
            {
                if (GetBGMManager() != null)
                {
                    GetBGMManager().ClearMainMusic();
                }

                // Resume map music after killing boss
                GetSceneSettings().EvaluateDayNightMusic();

                EventManager.EmitEvent(EventMessages.ON_BOSS_BATTLE_ENDS);
                onBossDefeated?.Invoke();
                UpdateBossFlag();
            }
        }

        void UpdateBossFlag()
        {
            if (monoBehaviourID == null || flagsDatabase == null)
            {
                return;
            }

            flagsDatabase.AddFlag(monoBehaviourID.ID, "Boss killed: " + bossName);
        }

        public bool IsBoss()
        {
            return isBoss;
        }

        BGMManager GetBGMManager()
        {
            if (bgmManager == null)
            {
                bgmManager = FindAnyObjectByType<BGMManager>(FindObjectsInactive.Include);
            }

            return bgmManager;
        }

        SceneSettings GetSceneSettings()
        {
            if (sceneSettings == null)
            {
                sceneSettings = FindAnyObjectByType<SceneSettings>(FindObjectsInactive.Include);
            }
            return sceneSettings;
        }

        void LogAnalytic(string eventName)
        {
            if (!GameAnalytics.Initialized)
            {
                GameAnalytics.Initialize();
            }

            GameAnalytics.NewDesignEvent(eventName);
        }

        public void TestBossFight()
        {
            characterManager.gameObject.SetActive(true);
            characterManager.targetManager.SetPlayerAsTarget();
            BeginBossBattle();
            ShowBossHud();
        }
    }
}
