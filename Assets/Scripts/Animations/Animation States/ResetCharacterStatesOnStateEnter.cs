using UnityEngine;

namespace AF
{
    public class ResetCharacterStatesOnStateEnter : StateMachineBehaviour
    {
        CharacterBaseManager character;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (character == null)
            {
                animator.TryGetComponent(out character);
            }

            // Trigger death state for non-player characters only.
            // Exclude the player, since if they die during the arena, we don’t want to play
            // the death animation (as arena logic handles player defeat separately).
            if (character.health.GetCurrentHealth() <= 0 && character is not PlayerManager)
            {
                character.PlayBusyAnimationWithRootMotion("Dying");
                return;
            }

            character.ResetStates();
        }
    }
}
