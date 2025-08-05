using System.Linq;
using AF.Health;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(fileName = "Toggle Combat Stance", menuName = "Abilities / AI / New Toggle Combat Stance", order = 0)]
    public class ToggleCombatStance : Ability
    {
        public override void OnPrepare(CharacterManager characterManager)
        {
            characterManager.characterWeaponsManager.SetIsTwoHanding(!characterManager.characterWeaponsManager.IsTwoHanding());
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
            return true;
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
