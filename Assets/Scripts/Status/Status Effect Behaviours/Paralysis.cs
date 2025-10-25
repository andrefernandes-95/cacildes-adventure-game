using UnityEngine;

namespace AF
{

    [CreateAssetMenu(menuName = "Data / Status Effect / Behaviours / New Paralysis")]
    public class Paralysis : StatusEffectBehaviour
    {

        public override void OnApplied(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
            InstantiateStartVfx(characterBaseManager, statusEffect);
            characterBaseManager.OnParalyzedStart();
        }

        public override void OnUpdate(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
            InstantiateUpdateVfx(characterBaseManager, statusEffect);
        }

        public override void OnRemoved(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
            characterBaseManager.OnParalyzedEnd();
        }
    }
}
