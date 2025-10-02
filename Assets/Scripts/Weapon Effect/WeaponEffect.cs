using UnityEngine;

namespace AF
{
    public abstract class WeaponEffect : ScriptableObject
    {
        public abstract void OnEquip(CharacterManager characterManager);
        public abstract void OnUnequip(CharacterManager characterManager);
        public abstract void OnEquip(PlayerManager playerManager);
        public abstract void OnUnequip(PlayerManager playerManager);
        public abstract string GetWeaponEffectTooltip();
    }
}
