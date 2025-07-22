namespace AF
{
    using UnityEngine;

    public class HandleIsBlockingOnStateUpdate : StateMachineBehaviour
    {
        PlayerManager playerManager;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (playerManager == null)
            {
                animator.TryGetComponent(out playerManager);
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            playerManager.playerBlockController.SetIsBlocking(true);
        }

        // Useful when enemy is delaying an attack, but gets hit or must exit its attack state abruptly, then we need to restore the animation speed to its default state for the next clip
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            playerManager.playerBlockController.SetIsBlocking(false);
        }
    }
}
