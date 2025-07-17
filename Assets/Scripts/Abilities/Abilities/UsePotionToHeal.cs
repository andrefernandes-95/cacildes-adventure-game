using System.Collections.Generic;
using System.Linq;
using AF.Equipment;
using AF.Health;
using AF.StatusEffects;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(fileName = "Use Potion To Heal", menuName = "Abilities / AI / New Use Potion To Heal Effect", order = 0)]
    public class UsePotionToHeal : Ability
    {
        [SerializeField] Consumable potion;
        [SerializeField][Range(0f, 100f)] float maximumHealthBeforeAttemptingToHeal = 30f;

        public override void OnPrepare(CharacterManager characterManager)
        {
            characterManager.characterAbilityManager.SetCurrentAbility(this);
            characterManager.RotateTowardsTarget(characterManager.rotationSpeed * 10f);

            Consumable potentialConsumable = GetPotion(characterManager);

            if (potentialConsumable != null)
            {
                characterManager.characterConsumableManager.Consume(potentialConsumable);
            }
        }

        Consumable GetPotion(CharacterBaseManager characterManager)
        {
            return characterManager.characterBaseInventory.GetConsumables().FirstOrDefault(
                c => c.EqualsTo(potion)
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
            return character.health.GetCurrentHealthPercentage() <= maximumHealthBeforeAttemptingToHeal && GetPotion(character) != null;
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
