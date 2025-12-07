namespace AF
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Cavern", menuName = "Data / Cavern", order = 0)]
    public class Cavern : ScriptableObject
    {
        public string cavernName_Pt;
        public string cavernName_En;
        public bool displayCavernName = false;

        [Header("Music Settings")]
        public AudioClip[] cavernMusic;

        public AudioClip[] cavernAmbience;

        [Header("Lighting Settings")]
        public Gradient CavernAmbientColor;
        public Gradient CavernDirectionalColor;
        public Gradient CavernFogColor;
        public float CavernFogDensity = 0.03f;
    }
}