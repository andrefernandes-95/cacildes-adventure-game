using AF.Animations;
using UnityEngine;

namespace AF
{
    public class HandleAIChargingAbilityOnStateUpdate : StateMachineBehaviour
    {
        CharacterManager characterManager;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (characterManager == null)
            {
                animator.TryGetComponent(out characterManager);
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (characterManager != null
                && characterManager.targetManager.currentTarget != null
                && Vector3.Distance(characterManager.transform.position, characterManager.targetManager.currentTarget.transform.position) > characterManager.agent.stoppingDistance
            )
            {
                characterManager.characterAbilityManager.SetIsCharging(false);
            }
        }

        // Useful when enemy is delaying an attack, but gets hit or must exit its attack state abruptly, then we need to restore the animation speed to its default state for the next clip
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (characterManager != null)
            {
                characterManager.characterAbilityManager.chargingAbilityAmount = stateInfo.normalizedTime;
            }
        }
    }
}
