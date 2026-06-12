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
                int current = (int)characterBaseManager.health.GetCurrentHealth();
                int amount = Mathf.Max(
                    1,
                    Mathf.RoundToInt(current * percentage / 100f)
                );

                characterBaseManager.health.TakeDamage(amount);
            }

            if (characterBaseManager is PlayerManager playerManager)
            {
                if (isStamina)
                {
                    int current = (int)playerManager.staminaStatManager.GetCurrentStamina();
                    int amount = Mathf.Max(
                        1,
                        Mathf.RoundToInt(current * percentage / 100f)
                    );

                    playerManager.staminaStatManager.DecreaseStamina(amount);
                }

                if (isMana)
                {
                    int current = (int)playerManager.manaManager.GetCurrentMana();
                    int amount = Mathf.Max(
                        1,
                        Mathf.RoundToInt(current * percentage / 100f)
                    );

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
