using System.Collections.Generic;
using AF.Animations;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Weapon / New Weapon Block Animation")]
    public class WeaponBlockAnimation : ScriptableObject
    {
        [Header("Blocking")]
        [SerializeField] AnimationClip block_noLockOn_Idle;
        [SerializeField] AnimationClip block_noLockOn_Run;
        [SerializeField] AnimationClip block_lockOn_idle;
        [SerializeField] AnimationClip block_lockOn_run_backwards;
        [SerializeField] AnimationClip block_lockOn_run_backwards_right;
        [SerializeField] AnimationClip block_lockOn_run_backwards_left;
        [SerializeField] AnimationClip block_lockOn_run_left;
        [SerializeField] AnimationClip block_lockOn_run_right;
        [SerializeField] AnimationClip block_lockOn_run_forward_left;
        [SerializeField] AnimationClip block_lockOn_run_forward_right;
        [SerializeField] AnimationClip block_lockOn_run_forward;
        [SerializeField] AnimationClip block_hit_reaction;
        [SerializeField] AnimationClip block_guardCounterAttack;
        [SerializeField] AnimationClip block_parrying;

        public List<AnimationOverride> GetTwoHandBlockingAnimations()
        {
            List<AnimationOverride> animationOverrides = new();

            if (block_noLockOn_Idle != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Block - Idle", animationClip = block_noLockOn_Idle });
            }

            if (block_noLockOn_Run != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Block - Run", animationClip = block_noLockOn_Run });
            }

            if (block_lockOn_run_backwards != null)
            {
                animationOverrides.Add(new() { animationName = "ARPG_Warrior_Guard_Run_Backward_Rootmotion", animationClip = block_lockOn_run_backwards });
            }
            if (block_lockOn_run_backwards_left != null)
            {
                animationOverrides.Add(new() { animationName = "ARPG_Warrior_Guard_Run_Backward_Left_Rootmotion", animationClip = block_lockOn_run_backwards_left });
            }
            if (block_lockOn_run_backwards_right != null)
            {
                animationOverrides.Add(new() { animationName = "ARPG_Warrior_Guard_Run_Backward_Right_Rootmotion", animationClip = block_lockOn_run_backwards_right });
            }
            if (block_lockOn_run_forward != null)
            {
                animationOverrides.Add(new() { animationName = "ARPG_Warrior_Guard_Run_Forward_Rootmotion", animationClip = block_lockOn_run_forward });
            }
            if (block_lockOn_run_forward_left != null)
            {
                animationOverrides.Add(new() { animationName = "ARPG_Warrior_Guard_Run_Forward_Left_Rootmotion", animationClip = block_lockOn_run_forward_left });
            }
            if (block_lockOn_run_forward_right != null)
            {
                animationOverrides.Add(new() { animationName = "ARPG_Warrior_Guard_Run_Forward_Right_Rootmotion", animationClip = block_lockOn_run_forward_right });
            }
            if (block_lockOn_run_right != null)
            {
                animationOverrides.Add(new() { animationName = "ARPG_Warrior_Guard_Run_Rightward_Rootmotion", animationClip = block_lockOn_run_right });
            }
            if (block_lockOn_run_left != null)
            {
                animationOverrides.Add(new() { animationName = "ARPG_Warrior_Guard_Run_Leftward_Rootmotion", animationClip = block_lockOn_run_left });
            }
            if (block_lockOn_idle != null)
            {
                animationOverrides.Add(new() { animationName = "ARPG_Warrior_Guard", animationClip = block_lockOn_idle });
            }

            if (block_hit_reaction != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Parry Hit Reaction", animationClip = block_hit_reaction });
            }
            if (block_guardCounterAttack != null)
            {
                animationOverrides.Add(new() { animationName = "Counter Attack", animationClip = block_guardCounterAttack });
            }
            if (block_parrying != null)
            {
                animationOverrides.Add(new() { animationName = "Cacildes - Parrying", animationClip = block_parrying });
            }

            return animationOverrides;
        }
    }
}
