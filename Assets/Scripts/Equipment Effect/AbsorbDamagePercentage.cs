using AF.Health;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Equipment Effects/Absorb Damage Percentage")]
    public class AbsorbDamagePercentage : EquipmentEffect
    {
        [Header("Absorption Percentages")]
        [SerializeField, Range(0, 100f)] float physicalDamageAbsorbedPercentage = 0;
        [SerializeField, Range(0, 100f)] float fireDamageAbsorbedPercentage = 0;
        [SerializeField, Range(0, 100f)] float frostDamageAbsorbedPercentage = 0;
        [SerializeField, Range(0, 100f)] float lightningDamageAbsorbedPercentage = 0;
        [SerializeField, Range(0, 100f)] float magicDamageAbsorbedPercentage = 0;
        [SerializeField, Range(0, 100f)] float darknessDamageAbsorbedPercentage = 0;
        [SerializeField, Range(0, 100f)] float waterDamageAbsorbedPercentage = 0;

        #region EQUIP / UNEQUIP

        public override void OnEquip(CharacterManager characterManager)
        {
            characterManager.characterBaseDamageReceiver.onDamageModifierEvent.AddListener(OnProcess);
        }

        public override void OnEquip(PlayerManager playerManager)
        {
            playerManager.characterBaseDamageReceiver.onDamageModifierEvent.AddListener(OnProcess);
        }

        public override void OnUnequip(CharacterManager characterManager)
        {
            characterManager.characterBaseDamageReceiver.onDamageModifierEvent.RemoveListener(OnProcess);
        }

        public override void OnUnequip(PlayerManager playerManager)
        {
            playerManager.characterBaseDamageReceiver.onDamageModifierEvent.RemoveListener(OnProcess);
        }

        #endregion

        #region DAMAGE PROCESSING

        void OnProcess(Damage damage, CharacterBaseManager attacker, CharacterBaseManager damageReceiver)
        {
            if (physicalDamageAbsorbedPercentage > 0)
            {
                damage.physical = Mathf.RoundToInt(damage.physical * (1f - (physicalDamageAbsorbedPercentage / 100f)));
            }
            if (fireDamageAbsorbedPercentage > 0)
            {
                damage.fire = Mathf.RoundToInt(damage.fire * (1f - (fireDamageAbsorbedPercentage / 100f)));
            }
            if (frostDamageAbsorbedPercentage > 0)
            {
                damage.frost = Mathf.RoundToInt(damage.frost * (1f - (frostDamageAbsorbedPercentage / 100f)));
            }
            if (lightningDamageAbsorbedPercentage > 0)
            {
                damage.lightning = Mathf.RoundToInt(damage.lightning * (1f - (lightningDamageAbsorbedPercentage / 100f)));
            }
            if (magicDamageAbsorbedPercentage > 0)
            {
                damage.magic = Mathf.RoundToInt(damage.magic * (1f - (magicDamageAbsorbedPercentage / 100f)));
            }
            if (darknessDamageAbsorbedPercentage > 0)
            {
                damage.darkness = Mathf.RoundToInt(damage.darkness * (1f - (darknessDamageAbsorbedPercentage / 100f)));
            }
            if (waterDamageAbsorbedPercentage > 0)
            {
                damage.water = Mathf.RoundToInt(damage.water * (1f - (waterDamageAbsorbedPercentage / 100f)));
            }
        }

        #endregion

        #region TOOLTIP

        public override string GetEquipmentEffectTooltip()
        {
            string text = "";

            void AppendText(string en, string pt, float value)
            {
                if (value <= 0) return;

                if (Utils.IsPortuguese())
                {
                    text += $"{value}% {pt} absorvido";
                }
                else
                {
                    text += $"{value}% {en} damage absorbed";
                }
                text += "\n";
            }

            AppendText("physical", "dano físico", physicalDamageAbsorbedPercentage);
            AppendText("fire", "dano de fogo", fireDamageAbsorbedPercentage);
            AppendText("frost", "dano de gelo", frostDamageAbsorbedPercentage);
            AppendText("lightning", "dano elétrico", lightningDamageAbsorbedPercentage);
            AppendText("magic", "dano mágico", magicDamageAbsorbedPercentage);
            AppendText("darkness", "dano sombrio", darknessDamageAbsorbedPercentage);
            AppendText("water", "dano de água", waterDamageAbsorbedPercentage);

            return text.TrimEnd('\n');
        }

        #endregion
    }
}
