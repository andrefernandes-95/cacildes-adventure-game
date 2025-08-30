using UnityEngine;

namespace AF
{

    [CreateAssetMenu(menuName = "Data / Status Effect / Behaviours / New Weakness Over Time")]
    public class WeaknessOverTime : StatusEffectBehaviour
    {
        public WeaponElementType weaponElementType = WeaponElementType.None;

        [Tooltip("How much the incoming attack will increase due to weakness")]
        public float attackMultiplier = 0.25f;

        public override void OnApplied(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
            InstantiateStartVfx(characterBaseManager, statusEffect);
            characterBaseManager.characterBaseWeaknessesManager.Add(this);
        }

        public override void OnUpdate(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
            InstantiateUpdateVfx(characterBaseManager, statusEffect);
        }

        public override void OnRemoved(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
            characterBaseManager.characterBaseWeaknessesManager.Remove(this);
        }

    }
}
