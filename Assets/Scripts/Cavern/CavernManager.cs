namespace AF
{
    using AF.Music;
    using UnityEngine;

    public class CavernManager : MonoBehaviour
    {

        [Header("Cavern Light Settings")]
        public Cavern currentCavern;
        public Gradient CavernAmbientColor;
        public Gradient CavernDirectionalColor;
        public Gradient CavernFogColor;
        public float CavernFogDensity = 0.03f;

        [Header("Components")]
        [SerializeField] SceneSettings sceneSettings;
        [SerializeField] BGMManager bGMManager;

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
        }

        void OnEnterCavern()
        {
            if (currentCavern.cavernMusic.Length > 0)
            {
                bGMManager.PlayMusic(currentCavern.cavernMusic[Random.Range(0, currentCavern.cavernMusic.Length)]);
            }

            if (currentCavern.cavernAmbience.Length > 0)
            {
                bGMManager.PlayAmbience(currentCavern.cavernAmbience[Random.Range(0, currentCavern.cavernAmbience.Length)]);
            }
        }

        void OnExitCavern()
        {
            sceneSettings.EvaluateDayNightMusic();
        }
    }
}
