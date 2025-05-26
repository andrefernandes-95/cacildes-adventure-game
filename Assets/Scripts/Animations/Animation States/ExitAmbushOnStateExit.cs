using AF.Animations;
using UnityEngine;

namespace AF
{
    public class ExitAmbushOnStateExit : StateMachineBehaviour
    {
        CharacterManager character;
        AmbushState ambushState;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (character == null)
            {
                animator.TryGetComponent(out character);
            }

            if (character != null && ambushState == null)
            {
                ambushState = character.GetComponentInChildren<AmbushState>(true);
            }
        }


        // Useful when enemy is delaying an attack, but gets hit or must exit its attack state abruptly, then we need to restore the animation speed to its default state for the next clip
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (ambushState != null)
            {
                ambushState.FinishAmbush();
            }
        }
    }
}
