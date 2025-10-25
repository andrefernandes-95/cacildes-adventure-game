using UnityEngine;

namespace AF
{

    [CreateAssetMenu(menuName = "Data / Status Effect / Behaviours / New Cut Health In Half")]
    public class CutHealthInHalf : StatusEffectBehaviour
    {

        public override void OnApplied(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
            InstantiateStartVfx(characterBaseManager, statusEffect);
            characterBaseManager.health.SetHasHealthCutInHealth(true);
        }

        public override void OnUpdate(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
            InstantiateUpdateVfx(characterBaseManager, statusEffect);
        }

        public override void OnRemoved(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
            characterBaseManager.health.SetHasHealthCutInHealth(false);
        }
    }
}
