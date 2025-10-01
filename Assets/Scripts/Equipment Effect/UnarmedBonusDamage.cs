using AF.Health;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Equipment Effects/Unarmed Bonus Damage")]
    public class UnarmedBonusDamage : EquipmentEffect
    {
        [Header("Bonus Damage")]
        [SerializeField, Range(0, 100f)] float bonusPhysicalDamageWhenUnarmed = 0;

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

        void OnProcess(Damage damage, CharacterBaseManager attacker)
        {
            if (bonusPhysicalDamageWhenUnarmed <= 0 || attacker == null)
                return;

            bool isUnarmedAttack =
                (attacker.characterBaseAttackManager.attackingHitboxType == HitboxType.RIGHT_HAND &&
                 attacker.characterBaseWeaponsManager.GetCurrentRightWeapon() == null)
             || (attacker.characterBaseAttackManager.attackingHitboxType == HitboxType.LEFT_HAND &&
                 attacker.characterBaseWeaponsManager.GetCurrentLeftWeapon() == null);

            if (isUnarmedAttack)
            {
                damage.physical = Mathf.RoundToInt(damage.physical * (1f + (bonusPhysicalDamageWhenUnarmed / 100f)));
            }
        }

        #endregion

        #region TOOLTIP

        public override string GetEquipmentEffectTooltip()
        {
            if (bonusPhysicalDamageWhenUnarmed <= 0)
                return "";

            if (Utils.IsPortuguese())
            {
                return $"+{bonusPhysicalDamageWhenUnarmed}% dano bónus em ataques desarmados";
            }
            else
            {
                return $"+{bonusPhysicalDamageWhenUnarmed}% bonus damage with unarmed attacks";
            }
        }

        #endregion
    }
}
