using UnityEngine;

namespace AF
{
    public abstract class StatusEffectBehaviour : ScriptableObject
    {

        [Header("VFX")]
        [SerializeField] GameObject onStartVfx;
        [SerializeField] GameObject onUpdateVfx;

        protected void InstantiateStartVfx(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
            if (onStartVfx != null)
            {
                characterBaseManager.statusController.InstantiateStartVfx(statusEffect, onStartVfx);
            }
        }

        protected void InstantiateUpdateVfx(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
            if (onUpdateVfx != null)
            {
                characterBaseManager.statusController.InstantiateUpdateVfx(statusEffect, onUpdateVfx);
            }
        }

        public abstract void OnApplied(CharacterBaseManager characterBaseManager, StatusEffect statusEffect);
        public abstract void OnUpdate(CharacterBaseManager characterBaseManager, StatusEffect statusEffect);
        public abstract void OnRemoved(CharacterBaseManager characterBaseManager, StatusEffect statusEffect);

    }
}
