using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Consumable Effect / Cure Status")]
    public class CureStatus : DrinkableConsumableEffect
    {
        public StatusEffect[] statusEffectsToCure;

        public override void OnUse(CharacterBaseManager characterBaseManager)
        {
            base.OnUse(characterBaseManager);

            foreach (StatusEffect statusEffectToCure in statusEffectsToCure)
            {
                characterBaseManager.statusController.RemoveEffect(statusEffectToCure);
            }
        }
    }
}
