using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Consumable Effect / Restore Attribute")]
    public class RestoreAttribute : DrinkableConsumableEffect
    {
        [Header("Attribute Type")]
        [SerializeField] bool restoreHealth = false;
        [SerializeField] bool restoreMana = false;
        [SerializeField] bool restoreStamina = false;

        [Header("Restore By Points")]
        [SerializeField] bool useWholeValues = false;
        [SerializeField] int amountToRestore = 200;

        [Header("Restore By Percentage")]
        [SerializeField] bool usePercentage = false;
        [SerializeField][Range(0f, 1f)] float amountInPercentage = .8f;

        public override void OnUse(CharacterBaseManager characterBaseManager)
        {
            base.OnUse(characterBaseManager);

            if (restoreHealth)
            {
                if (usePercentage)
                {
                    int points = (int)(amountInPercentage * characterBaseManager.health.GetMaxHealth());

                    characterBaseManager.health.RestoreHealth(points);
                }
                else if (useWholeValues)
                {
                    characterBaseManager.health.RestoreHealth(amountToRestore);
                }
            }

            if (restoreMana && characterBaseManager is PlayerManager playerManager)
            {
                if (usePercentage)
                {
                    playerManager.manaManager.RestoreManaPercentage(amountInPercentage);
                }
                else if (useWholeValues)
                {
                    playerManager.manaManager.RestoreManaPoints(amountToRestore);
                }
            }

            if (restoreStamina && characterBaseManager is PlayerManager playerManager2)
            {
                if (usePercentage)
                {
                    playerManager2.staminaStatManager.RestoreStaminaPercentage(amountInPercentage);
                }
                else if (useWholeValues)
                {
                    playerManager2.staminaStatManager.RestoreStaminaPoints(amountToRestore);
                }
            }
        }
    }
}
