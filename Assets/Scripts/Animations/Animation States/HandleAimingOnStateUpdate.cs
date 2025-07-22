using AF.Animations;
using UnityEngine;

namespace AF
{
    public class HandleAimingOnStateUpdate : StateMachineBehaviour
    {
        CharacterManager characterManager;

        float exitTime;


        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (characterManager == null)
            {
                animator.TryGetComponent(out characterManager);
            }

            exitTime = Random.Range(0.25f, 1f);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            characterManager.FaceTarget();

            if (stateInfo.normalizedTime >= exitTime)
            {
                animator.Play("Aim Fire");
            }
        }

        // Useful when enemy is delaying an attack, but gets hit or must exit its attack state abruptly, then we need to restore the animation speed to its default state for the next clip
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
        }
    }
}
