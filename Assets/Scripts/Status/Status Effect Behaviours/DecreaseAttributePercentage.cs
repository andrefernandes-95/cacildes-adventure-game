using UnityEngine;

namespace AF
{

    [CreateAssetMenu(menuName = "Data / Status Effect / Behaviours / New Decrease Attribute Percentage")]
    public class DecreaseAttributePercentage : StatusEffectBehaviour
    {
        [Range(0f, 100f)][SerializeField] float percentage = 30;

        [SerializeField] bool isHealth = false;
        [SerializeField] bool isMana = false;
        [SerializeField] bool isStamina = false;

        public override void OnApplied(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
            InstantiateStartVfx(characterBaseManager, statusEffect);

            if (isHealth)
            {
                int amount = (int)(characterBaseManager.health.GetMaxHealth() * percentage / 100);
                characterBaseManager.health.TakeDamage(amount);
            }

            if (characterBaseManager is PlayerManager playerManager)
            {
                if (isStamina)
                {
                    int amount = (int)(playerManager.staminaStatManager.GetMaxStamina() * percentage / 100);
                    playerManager.staminaStatManager.DecreaseStamina(amount);
                }

                if (isMana)
                {
                    int amount = (int)(playerManager.manaManager.GetMaxMana() * percentage / 100);
                    playerManager.manaManager.DecreaseMana(amount);
                }
            }
        }

        public override void OnUpdate(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
        }

        public override void OnRemoved(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
        }

    }
}
