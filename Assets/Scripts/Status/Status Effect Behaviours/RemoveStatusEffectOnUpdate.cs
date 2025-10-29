using UnityEngine;

namespace AF
{

    [CreateAssetMenu(menuName = "Data / Status Effect / Behaviours / Remove Status Effect OnUpdate")]
    public class RemoveStatusEffectOnUpdate : StatusEffectBehaviour
    {
        [SerializeField] StatusEffect statusEffectToRemoveOnUpdate;

        public override void OnApplied(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
            InstantiateStartVfx(characterBaseManager, statusEffect);
        }

        public override void OnUpdate(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
            InstantiateUpdateVfx(characterBaseManager, statusEffect);

            characterBaseManager.statusController.RemoveEffect(statusEffectToRemoveOnUpdate);
        }

        public override void OnRemoved(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
        }

    }
}
