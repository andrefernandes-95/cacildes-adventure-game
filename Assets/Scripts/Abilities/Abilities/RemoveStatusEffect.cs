using System.Collections.Generic;
using System.Linq;
using AF.Equipment;
using AF.Health;
using AF.StatusEffects;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(fileName = "Remove Status Effect", menuName = "Abilities / AI / New Remove Status Effect", order = 0)]
    public class RemoveStatusEffect : Ability
    {
        public override void OnPrepare(CharacterManager characterManager)
        {
            characterManager.characterAbilityManager.SetCurrentAbility(this);
            characterManager.RotateTowardsTarget(characterManager.rotationSpeed * 10f);

            Consumable potentialConsumable = GetPotentialConsumableToRemoveStatusEffect(characterManager);

            if (potentialConsumable != null)
            {
                characterManager.characterConsumableManager.Consume(potentialConsumable);
            }
        }

        Consumable GetPotentialConsumableToRemoveStatusEffect(CharacterBaseManager characterBaseManager)
        {
            Consumable potentialConsumable = null;

            foreach (AppliedStatusEffect appliedStatusEffect in characterBaseManager.statusController.appliedStatusEffects)
            {
                StatusEffect statusEffectToRemove = appliedStatusEffect.statusEffect;

                Consumable potentialConsumableToRemoveThisStatusEffect =
                    characterBaseManager.characterBaseInventory.GetConsumables().FirstOrDefault(consumable =>
                        consumable.consumableEffect != null
                        && consumable.consumableEffect is CureStatus cureStatus
                        && cureStatus.statusEffectsToCure.Contains(statusEffectToRemove));

                if (potentialConsumableToRemoveThisStatusEffect != null)
                {
                    potentialConsumable = potentialConsumableToRemoveThisStatusEffect;
                    break;
                }
            }

            return potentialConsumable;
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
            return character.statusController.appliedStatusEffects.Count > 0
                && GetPotentialConsumableToRemoveStatusEffect(character) != null;
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
