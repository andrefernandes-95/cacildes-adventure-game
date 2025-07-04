using UnityEngine;

namespace AF
{

    public class PlayerAttackManager : CharacterBaseAttackManager
    {
        [SerializeField] PlayerManager playerManager;

        public override bool DoesCharacterMeetWeaponRequirements(Weapon weapon)
        {
            return weapon.AreRequirementsMet(playerManager);
        }

        public override CharacterBaseManager GetCharacter()
        {
            return playerManager;
        }

        public override bool IsHeavyAttacking()
        {
            return playerManager.playerCombatController.isHeavyAttacking;
        }

        public override bool IsInAir()
        {
            return playerManager.thirdPersonController.Grounded == false;
        }

        public override bool IsJumpAttacking()
        {
            return playerManager.playerCombatController.isJumpAttacking;
        }
    }
}
