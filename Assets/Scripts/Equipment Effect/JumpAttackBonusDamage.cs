using AF.Health;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Equipment Effects/Jump Attack Bonus Damage")]
    public class JumpAttackBonusDamage : EquipmentEffect
    {
        [Header("Bonus Damage")]
        [SerializeField, Range(0, 100f)] float jumpAttackBonusDamageMultiplier = 1f;

        #region EQUIP / UNEQUIP

        public override void OnEquip(CharacterManager characterManager)
        {
            characterManager.onEnhanceAttackDamageWithEquipmentEffect.AddListener(OnProcess);
        }

        public override void OnEquip(PlayerManager playerManager)
        {
            playerManager.onEnhanceAttackDamageWithEquipmentEffect.AddListener(OnProcess);
        }

        public override void OnUnequip(CharacterManager characterManager)
        {
            characterManager.onEnhanceAttackDamageWithEquipmentEffect.RemoveListener(OnProcess);
        }

        public override void OnUnequip(PlayerManager playerManager)
        {
            playerManager.onEnhanceAttackDamageWithEquipmentEffect.RemoveListener(OnProcess);
        }

        #endregion

        #region DAMAGE PROCESSING

        void OnProcess(Damage damage, CharacterBaseManager attacker, CharacterBaseManager damageReceiver)
        {
            if (jumpAttackBonusDamageMultiplier <= 1f || attacker == null)
                return;

            bool isJumpAttacking = attacker.characterBaseAttackManager.IsJumpAttacking();

            if (isJumpAttacking)
            {
                damage.Multiply(jumpAttackBonusDamageMultiplier);
            }
        }

        #endregion

        #region TOOLTIP

        public override string GetEquipmentEffectTooltip()
        {
            if (jumpAttackBonusDamageMultiplier <= 1f)
                return "";

            if (Utils.IsPortuguese())
            {
                return $"+{jumpAttackBonusDamageMultiplier}% dano bónus em ataques aéreos";
            }
            else
            {
                return $"+{jumpAttackBonusDamageMultiplier}% bonus damage with aerial attacks";
            }
        }

        #endregion
    }
}
