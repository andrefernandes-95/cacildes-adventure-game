using UnityEngine;

namespace AF
{

    public class CharacterAttackManager : CharacterBaseAttackManager
    {

        [SerializeField] CharacterManager characterManager;

        public override CharacterBaseManager GetCharacter()
        {
            return characterManager;
        }

        public override bool IsHeavyAttacking()
        {
            return false;
        }

        public override bool IsJumpAttacking()
        {
            return false;
        }

        public override bool IsInAir()
        {
            return characterManager.characterController.isGrounded == false;
        }

    }
}
