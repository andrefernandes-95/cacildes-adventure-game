using System;
using System.Linq;
using AF.Health;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AF
{
    [CreateAssetMenu(menuName = "Weapon Effects/Bonus Damage On Specific Locations")]
    public class BonusDamageOnSpecificLocations : WeaponEffect
    {
        [SerializeField] SceneLocation[] locations;

        [Header("Damage Multipliers")]
        [SerializeField] float damageBonusMultiplier = 1f;

        [Header("Translation")]
        [SerializeField] string englishLabel = "+{0}% damage on snowy locations";
        [SerializeField] string portugueseLabel = "+{0}% dano em lugares enevados";

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
            if (damageReceiver != null && locations.Any(loc => loc.id.Equals(SceneManager.GetActiveScene().name)))
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

        public override string GetWeaponEffectTooltip()
        {
            string text = "";

            if (damageBonusMultiplier != 1f)
            {
                if (Utils.IsPortuguese())
                {
                    text += String.Format(englishLabel, damageBonusMultiplier * 10);
                }
                else
                {
                    text += String.Format(portugueseLabel, damageBonusMultiplier * 10);
                }

                text += "\n";
            }

            return text.TrimEnd('\n');
        }

        #endregion
    }
}
