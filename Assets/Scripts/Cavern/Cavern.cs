namespace AF
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Cavern", menuName = "Data / Cavern", order = 0)]
    public class Cavern : ScriptableObject
    {
        public AudioClip[] cavernMusic;

        public AudioClip[] cavernAmbience;

    }
}