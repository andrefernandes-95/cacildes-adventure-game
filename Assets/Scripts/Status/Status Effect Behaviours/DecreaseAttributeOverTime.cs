using UnityEngine;

namespace AF
{

    [CreateAssetMenu(menuName = "Data / Status Effect / Behaviours / New Decrease Attribute Over Time")]
    public class DecreaseAttributeOverTime : StatusEffectBehaviour
    {
        [SerializeField] float damageOverTime = 2f;

        [SerializeField] bool isHealth = false;
        [SerializeField] bool isMana = false;
        [SerializeField] bool isStamina = false;

        public override void OnApplied(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
            InstantiateStartVfx(characterBaseManager, statusEffect);
        }

        public override void OnUpdate(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
            InstantiateUpdateVfx(characterBaseManager, statusEffect);

            float value = damageOverTime * Time.deltaTime;
            if (isHealth)
            {
                characterBaseManager.health.TakeDamage(value);
            }

            if (characterBaseManager is PlayerManager playerManager)
            {
                if (isStamina)
                {
                    playerManager.staminaStatManager.DecreaseStamina(value);
                }

                if (isMana)
                {
                    playerManager.manaManager.DecreaseMana(value);
                }
            }
        }

        public override void OnRemoved(CharacterBaseManager characterBaseManager, StatusEffect statusEffect)
        {
        }

    }
}
