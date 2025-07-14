using AF.Health;
using UnityEngine;
using UnityEngine.AI;

namespace AF
{
    [CreateAssetMenu(fileName = "Dodge", menuName = "Abilities / Reactions / Dodge", order = 0)]
    public class Dodge : Ability
    {
        public override void OnPrepare(CharacterManager characterManager)
        {
            characterManager.characterAbilityManager.SetCurrentAbility(this);
            characterManager.RotateTowardsTarget(characterManager.rotationSpeed * 10f);

            characterManager.characterDodgeController.HandleDodge();
        }

        public override void OnPrepare(PlayerManager playerManager)
        {
        }

        public override void OnUse(PlayerManager playerManager)
        {
        }

        public override void OnUse(CharacterManager characterManager)
        {
        }

        public override bool CanUseAbility(CharacterBaseManager character)
        {
            return character.characterBaseDodgeController.CanDodge();
        }

        public override Damage GetDamage(CharacterBaseManager attacker)
        {
            return damage;
        }

        public override void OnFinished(CharacterManager characterManager)
        {
        }

        public override void OnFinished(PlayerManager playerManager)
        {
        }
    }
}
