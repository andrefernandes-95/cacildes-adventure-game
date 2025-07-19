using System.Linq;
using AF.Health;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(fileName = "Use Equipped Spell", menuName = "Abilities / AI / New Use Equipped Spell", order = 0)]
    public class UseEquippedSpell : Ability
    {
        public override void OnPrepare(CharacterManager characterManager)
        {
            Spell[] equippedSpells = characterManager.characterBaseEquipment.GetEquippedSpells().Where(spell => spell != null && spell.ability != null).ToArray();
            if (equippedSpells.Length > 0)
            {
                characterManager.characterAbilityBaseManager.QueueAbility(equippedSpells[Random.Range(0, equippedSpells.Length)].ability);
            }
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
            return HasAnySpellEquipped(character);
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

        bool HasAnySpellEquipped(CharacterBaseManager characterBaseManager) => characterBaseManager.characterBaseEquipment.GetEquippedAccessories().Any();
    }
}
