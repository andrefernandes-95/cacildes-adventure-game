using UnityEngine;

namespace AF
{
    public class EnableCanShootOnStateEnter : StateMachineBehaviour
    {
        PlayerManager playerManager;

        bool hasEnabled = false;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (playerManager == null)
            {
                animator.TryGetComponent(out playerManager);
            }

            hasEnabled = false;
        }

        private void OnStateUpdate(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (!animator.IsInTransition(0) && hasEnabled == false)
            {
                hasEnabled = true;
                playerManager.playerShootingManager.ResetHasAimShotCooldown();
            }
        }
    }
}
