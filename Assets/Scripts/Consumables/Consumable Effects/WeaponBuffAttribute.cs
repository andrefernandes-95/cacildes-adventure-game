using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Consumable Effect / Weapon Buff")]
    public class WeaponBuffAttribute : ConsumableEffect
    {
        [Header("Animation")]
        [SerializeField] string rightWeaponBuffAnimation = "Buff Right Weapon";
        [SerializeField] string leftWeaponBuffAnimation = "Buff Left Weapon";

        [Header("Buff Settings")]
        public WeaponBuffType weaponElementType = WeaponBuffType.None;
        public int baseDamage = 50;
        public float durationInSeconds = 90f;

        [Header("Status Effects")]
        public StatusEffect statusEffect;
        public int statusEffectAmountApplied = 6;

        public override void OnStart(CharacterBaseManager characterBaseManager)
        {
            characterBaseManager.PlayBusyAnimationWithRootMotion(rightWeaponBuffAnimation);
        }

        public override void OnUse(CharacterBaseManager characterBaseManager)
        {
            characterBaseManager.characterBaseWeaponBuffManager.AddBuff(this);
        }

        public override void OnEnd(CharacterBaseManager characterBaseManager)
        {
        }
    }
}
