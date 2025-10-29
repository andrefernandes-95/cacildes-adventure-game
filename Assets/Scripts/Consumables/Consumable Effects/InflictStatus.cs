using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Consumable Effect / Inflict Status")]
    public class InflictStatus : DrinkableConsumableEffect
    {
        public StatusEffect[] statusEffectsToInflict;

        public override void OnUse(CharacterBaseManager characterBaseManager)
        {
            base.OnUse(characterBaseManager);

            foreach (StatusEffect statusEffect in statusEffectsToInflict)
            {
                characterBaseManager.statusController.InflictStatusEffect(statusEffect);
            }
        }
    }
}
