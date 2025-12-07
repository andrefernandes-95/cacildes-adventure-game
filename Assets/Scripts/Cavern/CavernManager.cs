namespace AF
{
    using AF.Music;
    using CI.QuickSave;
    using UnityEditor;
    using UnityEngine;

#if UNITY_EDITOR

    [CustomEditor(typeof(CavernManager), editorForChildClasses: true)]
    public class CavernManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CavernManager cavernManager = target as CavernManager;

            if (GUILayout.Button("Update Lightning"))
            {
                cavernManager.UpdateLightning();
            }
        }
    }
#endif
    public class CavernManager : MonoBehaviour, ISaveable
    {

        [Header("Cavern Light Settings")]
        public Cavern currentCavern;

        [Header("Components")]
        [SerializeField] SceneSettings sceneSettings;
        [SerializeField] BGMManager bGMManager;
        [SerializeField] DayNightManager dayNightManager;
        [SerializeField] TempDataFromSaveFile tempDataFromSaveFile;

        void Start()
        {
            if (tempDataFromSaveFile.cavernFromSaveFile != null)
            {
                SetCavern(tempDataFromSaveFile.cavernFromSaveFile);
                tempDataFromSaveFile.cavernFromSaveFile = null;
            }
        }

        public bool IsInCavern() => currentCavern != null;

        public void SetCavern(Cavern cavern)
        {
            currentCavern = cavern;

            if (currentCavern != null)
            {
                OnEnterCavern();
            }
            else
            {
                OnExitCavern();
            }

            UpdateLightning();
        }

        public void UpdateLightning()
        {
            dayNightManager.UpdateLighting();
        }

        void OnEnterCavern()
        {
            sceneSettings.isPlayingMusicFromThePlaylist = false;

            if (currentCavern.cavernMusic.Length > 0)
            {
                bGMManager.PlayMusic(currentCavern.cavernMusic[Random.Range(0, currentCavern.cavernMusic.Length)]);
            }

            if (currentCavern.cavernAmbience.Length > 0)
            {
                bGMManager.PlayAmbience(currentCavern.cavernAmbience[Random.Range(0, currentCavern.cavernAmbience.Length)]);
            }

            if (currentCavern.displayCavernName)
            {
                sceneSettings.DisplaySceneName(Utils.IsPortuguese() ? currentCavern.cavernName_Pt : currentCavern.cavernName_En);
            }
        }

        void OnExitCavern()
        {
            sceneSettings.EvaluateDayNightMusic();
        }

        public void OnSaveData(QuickSaveWriter quickSaveWriter)
        {
            quickSaveWriter.Write(SaveKeys.CURRENT_CAVERN, currentCavern != null ? currentCavern.name : "");
        }

        public void OnLoadData(QuickSaveReader quickSaveReader)
        {
            if (quickSaveReader.TryRead(SaveKeys.CURRENT_CAVERN, out string cavernName))
            {
                Cavern targetCavern = Resources.Load<Cavern>("Caverns/" + cavernName);
                if (targetCavern != null)
                {
                    tempDataFromSaveFile.cavernFromSaveFile = targetCavern;
                }
            }
        }
    }
}
