using UnityEngine;

namespace AF
{

    [CreateAssetMenu(menuName = "Data / Status Effect / Behaviours / New Confusion")]
    public class Confusion : StatusEffectBehaviour
    {

        public override void OnApplied(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
            InstantiateStartVfx(characterBaseManager, statusEffect);
            characterBaseManager.SetIsConfused(true);
        }

        public override void OnUpdate(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
            InstantiateUpdateVfx(characterBaseManager, statusEffect);
        }

        public override void OnRemoved(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
            characterBaseManager.SetIsConfused(false);
        }
    }
}
