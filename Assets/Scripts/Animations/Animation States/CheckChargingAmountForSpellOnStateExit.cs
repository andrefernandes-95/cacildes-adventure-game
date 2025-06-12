using AF.Animations;
using UnityEngine;

namespace AF
{
    public class CheckChargingAmountForSpellOnStateExit : StateMachineBehaviour
    {
        PlayerManager playerManager;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (playerManager == null)
            {
                animator.TryGetComponent(out playerManager);
            }
        }

        // Useful when enemy is delaying an attack, but gets hit or must exit its attack state abruptly, then we need to restore the animation speed to its default state for the next clip
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (playerManager != null)
            {
                playerManager.playerAbilityManager.chargingAbilityAmount = Mathf.Clamp01(stateInfo.normalizedTime);
            }
        }
    }
}
