using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Data / New Scene Location")]

    public class SceneLocation : ScriptableObject
    {
        public string id;
        public string englishName;
        public string portugueseName;

        [Header("Music")]
        public AudioClip dayMusic;
        public AudioClip nightMusic;
        public AudioClip[] playlist;
        public AudioClip dayAmbience;
        public AudioClip nightAmbience;

        [Header("Settings")]
        [Tooltip("The clock stops if isInterior is true")]
        public bool isInterior = false;

        [Header("Scene Lightning")]
        public bool useSceneLightSettings = false;
        public Gradient AmbientColor;
        public Gradient DirectionalColor;
        public bool useFog = true;
        public Gradient FogColor;
        public float fogDensity = 0.03f;

        public string GetName()
        {
            if (Utils.IsPortuguese())
            {
                return portugueseName;
            }
            return englishName;
        }
    }
}
