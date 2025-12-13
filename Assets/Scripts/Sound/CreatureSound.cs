using UnityEngine;

namespace AF
{
    [CreateAssetMenu(fileName = "Creature Sound", menuName = "Sound / New Creature Sound", order = 0)]
    public class CreatureSound : ScriptableObject
    {
        [Header("Sounds")]
        public AudioClip[] ambush;
        public AudioClip[] targetSpotted;
        public AudioClip[] attacks;
        public AudioClip[] roars;
        public AudioClip[] hurt;
        public AudioClip[] death;
        public AudioClip[] footstep;

    }
}
