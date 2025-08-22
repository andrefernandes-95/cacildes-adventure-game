using System.Linq;
using AF.Health;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(fileName = "Use Spell", menuName = "Abilities / AI / New Use Spell", order = 0)]
    public class UseSpell : Ability
    {
        [SerializeField] Spell spell;

        public override void OnPrepare(CharacterManager characterManager)
        {
            characterManager.characterAbilityBaseManager.QueueAbility(spell.ability);
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
            return spell != null;
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
