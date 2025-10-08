using AF.Animations;
using UnityEngine;

namespace AF
{
    public class ManageDrinkingSpeedOnState : StateMachineBehaviour
    {
        CharacterBaseManager character;

        float defaultSpeed = 1;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (character == null)
            {
                animator.TryGetComponent(out character);
            }

            if (character != null)
            {
                defaultSpeed = animator.speed;
                character.onPreparingToDrinkConsumable?.Invoke(character);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.speed = defaultSpeed;
        }
    }
}
