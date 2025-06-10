using System.Collections.Generic;
using AF.Animations;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Weapon / New Weapon Range Animation")]
    public class WeaponRangeAnimation : ScriptableObject
    {
        [Header("Blocking")]
        [SerializeField] AnimationClip sprint;
        [SerializeField] AnimationClip run;
        [SerializeField] AnimationClip aimIdle;
        [SerializeField] AnimationClip aimWalk;
        [SerializeField] AnimationClip aimFire;
        [SerializeField] AnimationClip aimFireLockedOn;

        public List<AnimationOverride> GetAnimations()
        {
            List<AnimationOverride> animationOverrides = new();

            if (sprint != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Sprint", animationClip = sprint });
            }
            if (run != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Run", animationClip = run });
            }
            if (aimIdle != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Aim Idle", animationClip = aimIdle });
            }
            if (aimWalk != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Aim Walk", animationClip = aimWalk });
            }
            if (aimFire != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Aim Fire", animationClip = aimFire });
            }
            if (aimFireLockedOn != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Locked On - Aim Fire", animationClip = aimFireLockedOn });
            }

            return animationOverrides;
        }
    }
}
