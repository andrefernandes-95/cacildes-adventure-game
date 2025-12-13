using System.Linq;
using AF.Health;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Equipment Effects/Bonus Damage On Enemy Type")]
    public class BonusDamageOnEnemyTypeEquipmentEffect : EquipmentEffect
    {
        [SerializeField] Combatant[] enemies;

        [Header("Damage Multipliers")]
        [SerializeField] float damageBonusMultiplier = 1f;

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
            if (damageReceiver != null && enemies.Contains(damageReceiver.combatant))
            {
                if (damageBonusMultiplier != 1f)
                {
                    if (damage.physical > 0) damage.physical = (int)(damage.physical * damageBonusMultiplier);
                    if (damage.fire > 0) damage.fire = (int)(damage.fire * damageBonusMultiplier);
                    if (damage.frost > 0) damage.frost = (int)(damage.frost * damageBonusMultiplier);
                    if (damage.magic > 0) damage.magic = (int)(damage.magic * damageBonusMultiplier);
                    if (damage.lightning > 0) damage.lightning = (int)(damage.lightning * damageBonusMultiplier);
                    if (damage.darkness > 0) damage.darkness = (int)(damage.darkness * damageBonusMultiplier);
                    if (damage.water > 0) damage.water = (int)(damage.water * damageBonusMultiplier);
                }
            }
        }

        #endregion

        #region TOOLTIP

        public override string GetEquipmentEffectTooltip()
        {
            string text = "";

            if (damageBonusMultiplier != 1f && enemies.Length > 0)
            {
                if (Utils.IsPortuguese())
                {
                    text += $"+{damageBonusMultiplier * 10}% dano bónus contra {enemies[0].combatantName.GetLocalizedString()}";
                }
                else
                {
                    text += $"+{damageBonusMultiplier * 10}% bonus damage against {enemies[0].combatantName.GetLocalizedString()}";
                }

                text += "\n";
            }

            return text.TrimEnd('\n');
        }

        #endregion
    }
}
