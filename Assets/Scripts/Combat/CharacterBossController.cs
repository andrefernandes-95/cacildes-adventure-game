using System.Collections;
using System.Collections.Generic;
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
        VisualElement root;

        public IMGUIContainer bossFillBar;
        Label bossHealthLabel;

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

        public void Awake()
        {
            HideBossHud();

            if (IsBoss())
            {
                characterManager.health.onShowHealthbar.AddListener(UpdateUI);
                characterManager.health.onHideHealthbar.AddListener(UpdateUI);
                characterManager.health.onUpdateHealthbar.AddListener(UpdateUI);
            }
        }

        private void OnEnable()
        {
            root = bossHud?.rootVisualElement;
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

                bossFillBar ??= root.Q<IMGUIContainer>("hp-bar");
                bossFillBar.style.width = new Length(characterManager.health.GetCurrentHealth() * 100 / characterManager.health.GetMaxHealth(), LengthUnit.Percent);

                bossHealthLabel ??= root.Q<Label>("boss-health");
                bossHealthLabel.text = $"{Mathf.RoundToInt(characterManager.health.GetCurrentHealth())}/{Mathf.RoundToInt(characterManager.health.GetMaxHealth())}";

                UIUtils.PlayPopAnimation(bossHealthLabel, new Vector3(1.1f, 1.1f, 1.1f));
            }
        }

        public void ShowBossHud()
        {
            if (bossHud == null)
            {
                return;
            }

            bossHud.enabled = true;
            StartCoroutine(ShowBossHUDCoroutine());
        }

        IEnumerator ShowBossHUDCoroutine()
        {
            yield return new WaitForEndOfFrame();
            root ??= bossHud.rootVisualElement;

            root.Q<Label>("boss-name").text = bossName;
            root.style.display = DisplayStyle.Flex;
            root.Q<VisualElement>("container").style.marginBottom = characterManager.partnerOrder == 0 ? 0 : 90 * characterManager.partnerOrder;
            UIUtils.FadeIn(root);

            UpdateUI();
        }

        public void HideBossHud()
        {
            bossHud.enabled = false;
        }

        public bool IsBossHUDEnabled()
        {
            return bossHud != null && bossHud.enabled && root != null;
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
                    if (partner != null && partner.gameObject.activeInHierarchy)
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

            bool allPartnersAreDead = isDead && characterManager.partners?.Length > 0
                && characterManager.partners.All(partner => partner != null && partner.health.GetCurrentHealth() <= 0);

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

        public void TestBossFight()
        {
            characterManager.gameObject.SetActive(true);
            characterManager.targetManager.SetPlayerAsTarget();
            BeginBossBattle();
            ShowBossHud();
        }
    }
}
