using System.Collections.Generic;
using System.Linq;
using AF.Equipment;
using AF.Health;
using AF.StatusEffects;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(fileName = "Use Consumable", menuName = "Abilities / AI / New Use Consumable", order = 0)]
    public class UseConsumable : Ability
    {
        [SerializeField] Consumable consumable;
        [SerializeField][Range(0f, 100f)] float maximumHealthBeforeAttemptingTUseConsumable = 70f;

        public override void OnPrepare(CharacterManager characterManager)
        {
            characterManager.characterAbilityManager.SetCurrentAbility(this);
            characterManager.RotateTowardsTarget(characterManager.rotationSpeed * 10f);

            Consumable potentialConsumable = GetPossibleConsumable(characterManager);

            if (potentialConsumable != null)
            {
                characterManager.characterConsumableManager.Consume(potentialConsumable);
            }
        }

        Consumable GetPossibleConsumable(CharacterBaseManager characterManager)
        {
            return characterManager.characterBaseInventory.GetConsumables().FirstOrDefault(
                c => c.EqualsTo(consumable)
            );
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
            return character.health.GetCurrentHealthPercentage() <= maximumHealthBeforeAttemptingTUseConsumable && GetPossibleConsumable(character) != null;
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
