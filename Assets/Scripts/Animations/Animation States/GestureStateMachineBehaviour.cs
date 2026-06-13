using UnityEngine;

namespace AF
{
    public class GestureStateMachineBehaviour : StateMachineBehaviour
    {
        PlayerManager playerManager;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (playerManager == null)
            {
                animator.TryGetComponent(out playerManager);
            }

            if (playerManager != null)
            {
                playerManager.playerComponentManager.DisablePlayerControl();
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (playerManager != null)
            {
                playerManager.playerComponentManager.EnablePlayerControl();
            }
        }
    }
}
