namespace AF
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "Data / Gesture / New Gesture")]
    public class Gesture : ScriptableObject
    {
        public AnimationClip animationClip;
        public bool loop;
        public float crossFade = 0;
    }
}
