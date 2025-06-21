using System.Collections.Generic;
using AF.Animations;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Weapon / New Weapon Animation")]
    public class WeaponAnimation : ScriptableObject
    {
        [Header("One Hand")]
        [SerializeField] AnimationClip oh_Idle;
        [SerializeField] AnimationClip oh_Walk;
        [SerializeField] AnimationClip oh_Run;
        [SerializeField] AnimationClip oh_Sprint;
        [SerializeField] AnimationClip oh_right_LightAttack1;
        [SerializeField] AnimationClip oh_right_LightAttack2;
        [SerializeField] AnimationClip oh_right_LightAttack3;
        [SerializeField] AnimationClip oh_right_LightAttack4;
        [SerializeField] AnimationClip oh_left_LightAttack1;
        [SerializeField] AnimationClip oh_left_LightAttack2;
        [SerializeField] AnimationClip oh_HeavyAttack1;
        [SerializeField] AnimationClip oh_HeavyAttack2;
        [SerializeField] AnimationClip oh_HeavyAttack3;
        [SerializeField] AnimationClip oh_PowerStanceAttack1;
        [SerializeField] AnimationClip oh_PowerStanceAttack2;
        [SerializeField] AnimationClip oh_PowerStance_HeavyAttack1;
        [SerializeField] AnimationClip oh_PowerStance_HeavyAttack2;

        [Header("Two Handing")]
        [SerializeField] AnimationClip th_Idle;
        [SerializeField] AnimationClip th_Walk;
        [SerializeField] AnimationClip th_Run;
        [SerializeField] AnimationClip th_Sprint;
        [SerializeField] AnimationClip th_LightAttack1;
        [SerializeField] AnimationClip th_LightAttack2;
        [SerializeField] AnimationClip th_LightAttack3;
        [SerializeField] AnimationClip th_LightAttack4;
        [SerializeField] AnimationClip th_HeavyAttack1;
        [SerializeField] AnimationClip th_HeavyAttack2;
        [SerializeField] AnimationClip th_HeavyAttack3;

        [Header("Blocking")]
        [SerializeField] WeaponBlockAnimation weaponBlockAnimation;

        [Header("Damage")]
        [SerializeField] AnimationClip takeDamage;
        [SerializeField] AnimationClip dying;
        [SerializeField] AnimationClip knockdown;
        [SerializeField] AnimationClip knockdownGetup;

        public List<AnimationOverride> GetOneHandAnimations()
        {
            List<AnimationOverride> animationOverrides = new();

            if (oh_Idle != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Idle", animationClip = oh_Idle });
            }

            if (oh_Walk != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Walk", animationClip = oh_Walk });
            }

            if (oh_Run != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Run", animationClip = oh_Run });
            }

            if (oh_Sprint != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Sprint", animationClip = oh_Sprint });
            }

            if (oh_right_LightAttack1 != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Light Attack - 1", animationClip = oh_right_LightAttack1 });
            }
            if (oh_right_LightAttack2 != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Light Attack - 2", animationClip = oh_right_LightAttack2 });
            }
            if (oh_right_LightAttack3 != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Light Attack - 3", animationClip = oh_right_LightAttack3 });
            }
            if (oh_right_LightAttack4 != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Light Attack - 4", animationClip = oh_right_LightAttack4 });
            }

            if (oh_HeavyAttack1 != null)
            {
                animationOverrides.Add(new()
                {
                    animationName = "Cacildes - Heavy Attack - 1",
                    animationClip = oh_HeavyAttack1
                });
            }
            if (oh_HeavyAttack2 != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Heavy Attack - 2", animationClip = oh_HeavyAttack2 });
            }
            if (oh_HeavyAttack3 != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Heavy Attack - 3", animationClip = oh_HeavyAttack3 });
            }

            return animationOverrides;
        }

        public List<AnimationOverride> GetLeftHandAnimations()
        {
            List<AnimationOverride> animationOverrides = new();

            if (oh_left_LightAttack1 != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Left Light Attack - 1", animationClip = oh_left_LightAttack1 });
            }
            if (oh_left_LightAttack2 != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Left Light Attack - 2", animationClip = oh_left_LightAttack2 });
            }
            if (oh_PowerStanceAttack1 != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Power Stance Attack - 1", animationClip = oh_PowerStanceAttack1 });
            }
            if (oh_PowerStanceAttack2 != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Power Stance Attack - 2", animationClip = oh_PowerStanceAttack2 });
            }
            if (oh_PowerStance_HeavyAttack1 != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Heavy Power Stance Attack - 1", animationClip = oh_PowerStance_HeavyAttack1 });
            }
            if (oh_PowerStance_HeavyAttack2 != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Heavy Power Stance Attack - 2", animationClip = oh_PowerStance_HeavyAttack2 });
            }

            return animationOverrides;
        }

        public List<AnimationOverride> GetTwoHandAnimations()
        {
            List<AnimationOverride> animationOverrides = new();

            if (th_Idle != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Idle", animationClip = th_Idle });
            }

            if (th_Walk != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Walk", animationClip = th_Walk });
            }

            if (th_Run != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Run", animationClip = th_Run });
            }

            if (th_Sprint != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Sprint", animationClip = th_Sprint });
            }

            if (th_LightAttack1 != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Light Attack - 1", animationClip = th_LightAttack1 });
            }
            if (th_LightAttack2 != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Light Attack - 2", animationClip = th_LightAttack2 });
            }
            if (th_LightAttack3 != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Light Attack - 3", animationClip = th_LightAttack3 });
            }
            if (th_LightAttack4 != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Light Attack - 4", animationClip = th_LightAttack4 });
            }

            if (th_HeavyAttack1 != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Heavy Attack - 1", animationClip = th_HeavyAttack1 });
            }
            if (th_HeavyAttack2 != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Heavy Attack - 2", animationClip = th_HeavyAttack2 });
            }
            if (th_HeavyAttack3 != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Heavy Attack - 3", animationClip = th_HeavyAttack3 });
            }

            animationOverrides.AddRange(weaponBlockAnimation.GetTwoHandBlockingAnimations());

            return animationOverrides;
        }

        public List<AnimationOverride> GetRightHandAnimationsForAI()
        {
            List<AnimationOverride> animationOverrides = new();

            // Locomotion
            if (oh_Idle != null) animationOverrides.Add(new() { animationName = "AI Humanoid - Idle", animationClip = oh_Idle });
            if (oh_Walk != null) animationOverrides.Add(new() { animationName = "AI Humanoid - Patrolling", animationClip = oh_Walk });
            if (oh_Run != null) animationOverrides.Add(new() { animationName = "AI Humanoid - Chasing", animationClip = oh_Run });

            // Combat
            if (oh_Idle != null) animationOverrides.Add(new() { animationName = "AI Humanoid - Combat Idle", animationClip = oh_Idle }); // Could be replaced with separate combat idle
            if (takeDamage != null) animationOverrides.Add(new() { animationName = "AI Humanoid - Taking Damage", animationClip = takeDamage });

            // Light Attacks
            if (oh_right_LightAttack1 != null) animationOverrides.Add(new() { animationName = "Cacildes - Light Attack - 1", animationClip = oh_right_LightAttack1 });
            if (oh_right_LightAttack2 != null) animationOverrides.Add(new() { animationName = "Cacildes - Light Attack - 2", animationClip = oh_right_LightAttack2 });
            if (oh_right_LightAttack3 != null) animationOverrides.Add(new() { animationName = "Cacildes - Light Attack - 3", animationClip = oh_right_LightAttack3 });
            if (oh_right_LightAttack4 != null) animationOverrides.Add(new() { animationName = "Cacildes - Light Attack - 4", animationClip = oh_right_LightAttack4 });

            // Heavy Attacks
            if (oh_HeavyAttack1 != null) animationOverrides.Add(new() { animationName = "Cacildes - Heavy Attack - 1", animationClip = oh_HeavyAttack1 });
            if (oh_HeavyAttack2 != null) animationOverrides.Add(new() { animationName = "Cacildes - Heavy Attack - 2", animationClip = oh_HeavyAttack2 });
            if (oh_HeavyAttack3 != null) animationOverrides.Add(new() { animationName = "Cacildes - Heavy Attack - 3", animationClip = oh_HeavyAttack3 });

            // Other
            if (dying != null) animationOverrides.Add(new() { animationName = "AI Humanoid - Dying", animationClip = dying });
            if (knockdown != null) animationOverrides.Add(new() { animationName = "AI Humanoid - Posture Break", animationClip = knockdown });
            if (knockdownGetup != null) animationOverrides.Add(new() { animationName = "AI Humanoid - Posture Break - Exit", animationClip = knockdownGetup });

            return animationOverrides;
        }

        public List<AnimationOverride> GetLeftHandAnimationsForAI()
        {
            List<AnimationOverride> animationOverrides = new();

            // Left Light Attacks
            if (oh_left_LightAttack1 != null) animationOverrides.Add(new() { animationName = "Cacildes - Left Light Attack - 1", animationClip = oh_left_LightAttack1 });
            if (oh_left_LightAttack2 != null) animationOverrides.Add(new() { animationName = "Cacildes - Left Light Attack - 2", animationClip = oh_left_LightAttack2 });

            // Power Stance Attacks
            if (oh_PowerStanceAttack1 != null) animationOverrides.Add(new() { animationName = "Cacildes - Power Stance Attack - 1", animationClip = oh_PowerStanceAttack1 });
            if (oh_PowerStanceAttack2 != null) animationOverrides.Add(new() { animationName = "Cacildes - Power Stance Attack - 2", animationClip = oh_PowerStanceAttack2 });

            // Heavy Power Stance Attacks
            if (oh_PowerStance_HeavyAttack1 != null) animationOverrides.Add(new() { animationName = "Cacildes - Heavy Power Stance Attack - 1", animationClip = oh_PowerStance_HeavyAttack1 });
            if (oh_PowerStance_HeavyAttack2 != null) animationOverrides.Add(new() { animationName = "Cacildes - Heavy Power Stance Attack - 2", animationClip = oh_PowerStance_HeavyAttack2 });

            return animationOverrides;
        }

        public List<AnimationOverride> GetTwoHandAnimationsForAI()
        {
            List<AnimationOverride> animationOverrides = new();

            // Locomotion
            if (th_Idle != null) animationOverrides.Add(new() { animationName = "AI Humanoid - Idle", animationClip = th_Idle });
            if (th_Walk != null) animationOverrides.Add(new() { animationName = "AI Humanoid - Patrolling", animationClip = th_Walk });
            if (th_Run != null) animationOverrides.Add(new() { animationName = "AI Humanoid - Chasing", animationClip = th_Run });

            // Combat
            if (th_Idle != null) animationOverrides.Add(new() { animationName = "AI Humanoid - Combat Idle", animationClip = th_Idle }); // You can change this if you have a separate clip
            if (takeDamage != null) animationOverrides.Add(new() { animationName = "AI Humanoid - Taking Damage", animationClip = takeDamage });

            // Light Attacks
            if (th_LightAttack1 != null) animationOverrides.Add(new() { animationName = "Cacildes - Light Attack - 1", animationClip = th_LightAttack1 });
            if (th_LightAttack2 != null) animationOverrides.Add(new() { animationName = "Cacildes - Light Attack - 2", animationClip = th_LightAttack2 });
            if (th_LightAttack3 != null) animationOverrides.Add(new() { animationName = "Cacildes - Light Attack - 3", animationClip = th_LightAttack3 });
            if (th_LightAttack4 != null) animationOverrides.Add(new() { animationName = "Cacildes - Light Attack - 4", animationClip = th_LightAttack4 });

            // Heavy Attacks
            if (th_HeavyAttack1 != null) animationOverrides.Add(new() { animationName = "Cacildes - Heavy Attack - 1", animationClip = th_HeavyAttack1 });
            if (th_HeavyAttack2 != null) animationOverrides.Add(new() { animationName = "Cacildes - Heavy Attack - 2", animationClip = th_HeavyAttack2 });
            if (th_HeavyAttack3 != null) animationOverrides.Add(new() { animationName = "Cacildes - Heavy Attack - 3", animationClip = th_HeavyAttack3 });

            // Other
            if (dying != null) animationOverrides.Add(new() { animationName = "AI Humanoid - Dying", animationClip = dying });
            if (knockdown != null) animationOverrides.Add(new() { animationName = "AI Humanoid - Posture Break", animationClip = knockdown });
            if (knockdownGetup != null) animationOverrides.Add(new() { animationName = "AI Humanoid - Posture Break - Exit", animationClip = knockdownGetup });

            return animationOverrides;
        }
    }
}
