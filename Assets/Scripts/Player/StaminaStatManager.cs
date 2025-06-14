using System.Collections;
using AF.Stats;
using UnityEngine;

namespace AF
{
    public class StaminaStatManager : MonoBehaviour
    {

        [Header("Regeneration Settings")]
        public float STAMINA_REGENERATION_RATE = 20f;
        public float STAMINA_REGENERATION_RATE_BONUS = 0f;
        public float negativeStaminaRegenerationBonus = 0f;
        public const float EMPTY_STAMINA_REGENERATION_DELAY = 1f;
        public bool shouldRegenerateStamina = false;

        [Header("Combat Stamina")]
        public int unarmedLightAttackStaminaCost = 25;
        public int GetUnarmedHeavyAttackStaminaCost() => unarmedLightAttackStaminaCost * 2;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;

        [Header("Components")]
        public StatsBonusController playerStatsBonusController;

        public StarterAssetsInputs inputs;

        public EquipmentGraphicsHandler equipmentGraphicsHandler;

        public PlayerManager playerManager;

        Coroutine RegenerateEmptyStaminaCoroutine;

        private void Start()
        {
            if (playerStatsDatabase.currentStamina == -1)
            {
                playerStatsDatabase.currentStamina = GetMaxStamina();
            }
            else if (playerStatsDatabase.currentStamina < GetMaxStamina())
            {
                shouldRegenerateStamina = true;
            }
        }

        public int GetMaxStamina()
        {
            return Formulas.CalculateStatForLevel(
                playerStatsDatabase.maxStamina + playerStatsBonusController.staminaBonus,
                playerStatsBonusController.GetCurrentEndurance(),
                playerStatsDatabase.levelMultiplierForStamina);
        }

        public float GetCurrentStaminaPercentage()
        {
            return playerStatsDatabase.currentStamina * 100 / GetMaxStamina();
        }

        public void DecreaseStamina(float amount)
        {
            shouldRegenerateStamina = false;

            playerStatsDatabase.currentStamina = Mathf.Clamp(playerStatsDatabase.currentStamina - amount, 0, GetMaxStamina());

            if (RegenerateEmptyStaminaCoroutine != null)
            {
                StopCoroutine(RegenerateEmptyStaminaCoroutine);
            }

            RegenerateEmptyStaminaCoroutine = StartCoroutine(RegenerateEmptyStamina());
        }

        IEnumerator RegenerateEmptyStamina()
        {
            yield return new WaitForSeconds(EMPTY_STAMINA_REGENERATION_DELAY);

            shouldRegenerateStamina = true;
        }

        private void Update()
        {
            if (shouldRegenerateStamina)
            {
                if (inputs.sprint)
                {
                    return;
                }

                HandleStaminaRegen();
            }
        }

        float GetStaminaRegenerationRate()
        {
            float value = STAMINA_REGENERATION_RATE + playerStatsBonusController.staminaRegenerationBonus - negativeStaminaRegenerationBonus + STAMINA_REGENERATION_RATE_BONUS;

            if (GetCurrentStaminaPercentage() <= 25)
            {
                value *= 1.75f;
            }
            else if (GetCurrentStaminaPercentage() <= 50)
            {
                value *= 1.25f;
            }

            return value;
        }

        void HandleStaminaRegen()
        {
            var finalRegenerationRate = GetStaminaRegenerationRate();


            if (playerManager.characterBlockController.isBlocking)
            {
                finalRegenerationRate = finalRegenerationRate / 4;
            }

            playerStatsDatabase.currentStamina = Mathf.Clamp(playerStatsDatabase.currentStamina + finalRegenerationRate * Time.deltaTime, 0f, GetMaxStamina());

            if (playerStatsDatabase.currentStamina >= GetMaxStamina())
            {
                shouldRegenerateStamina = false;
            }
        }

        public bool HasEnoughStaminaForAction(float actionStaminaCost)
        {
            bool canPerform = playerStatsDatabase.currentStamina - actionStaminaCost > 0;
            if (!canPerform)
            {
                playerManager.uIDocumentPlayerHUDV2.DisplayInsufficientStamina();
            }

            return canPerform;
        }

        public void RestoreStaminaPercentage(float amount)
        {
            var percentage = this.GetMaxStamina() * amount / 100;
            var nextValue = Mathf.Clamp(playerStatsDatabase.currentStamina + percentage, 0, this.GetMaxStamina());

            playerStatsDatabase.currentStamina = nextValue;
        }

        public void RestoreStaminaPoints(float amount)
        {
            var nextValue = Mathf.Clamp(playerStatsDatabase.currentStamina + amount, 0, this.GetMaxStamina());

            playerStatsDatabase.currentStamina = nextValue;
        }


        public float GetStaminaPointsForGivenEndurance(int endurance)
        {
            return playerStatsDatabase.maxStamina + (int)Mathf.Ceil(endurance * playerStatsDatabase.levelMultiplierForStamina);
        }

        void DecreaseAttackStamina(bool isHeavyAttack, bool isAttackingWithLeftHand)
        {
            int lightAttackStaminaCost = 0;

            bool isDualWieldingAnHeavyAttack = isHeavyAttack && equipmentDatabase.CanPowerStance();
            bool isDualWieldingALightAttack = isAttackingWithLeftHand && equipmentDatabase.CanPowerStance();
            bool isDualWieldingAnAttack = isDualWieldingAnHeavyAttack || isDualWieldingALightAttack;

            // Only take into account right hand if not attacking with left hand or we are dual wielding
            if (!isAttackingWithLeftHand || isDualWieldingAnAttack)
            {
                lightAttackStaminaCost = isHeavyAttack ? GetUnarmedHeavyAttackStaminaCost() : unarmedLightAttackStaminaCost;
                Weapon rightWeapon = equipmentDatabase.GetCurrentWeapon();
                if (rightWeapon != null)
                {
                    lightAttackStaminaCost += isHeavyAttack ? rightWeapon.GetHeavyAttackStaminaCost() : rightWeapon.GetLightAttackStaminaCost();
                }
            }

            // Only take into account left hand if attacking with left hand or we are dual wielding
            if (isAttackingWithLeftHand || isDualWieldingAnAttack)
            {
                Weapon leftWeapon = equipmentDatabase.GetCurrentLeftWeapon();
                if (leftWeapon != null)
                {
                    lightAttackStaminaCost += isHeavyAttack ? leftWeapon.GetHeavyAttackStaminaCost() : leftWeapon.GetLightAttackStaminaCost();
                }
            }

            DecreaseStamina(lightAttackStaminaCost);
        }

        public void DecreaseLightAttackStamina(bool isAttackingWithLeftHand)
        {
            DecreaseAttackStamina(false, isAttackingWithLeftHand);
        }

        public void DecreaseHeavyAttackStamina()
        {
            DecreaseAttackStamina(true, false);
        }

        public bool HasEnoughStaminaForLightAttack(bool isAttackingWithLeftHand)
        {
            int staminaCost = equipmentDatabase.GetCurrentWeapon() != null
                ? equipmentDatabase.GetCurrentWeapon().GetLightAttackStaminaCost() : unarmedLightAttackStaminaCost;

            bool isDualWieldingALightAttack = isAttackingWithLeftHand && equipmentDatabase.CanPowerStance();
            if (isDualWieldingALightAttack)
            {
                Weapon leftWeapon = equipmentDatabase.GetCurrentLeftWeapon();
                if (leftWeapon != null)
                {
                    staminaCost += leftWeapon.GetLightAttackStaminaCost();
                }
            }

            return HasEnoughStaminaForAction(staminaCost);
        }

        public bool HasEnoughStaminaForHeavyAttack()
        {
            int staminaCost = equipmentDatabase.GetCurrentWeapon() != null
                ? equipmentDatabase.GetCurrentWeapon().GetHeavyAttackStaminaCost() : GetUnarmedHeavyAttackStaminaCost();

            if (equipmentDatabase.CanPowerStance())
            {
                Weapon leftWeapon = equipmentDatabase.GetCurrentLeftWeapon();
                if (leftWeapon != null)
                {
                    staminaCost += leftWeapon.GetHeavyAttackStaminaCost();
                }
            }

            return HasEnoughStaminaForAction(staminaCost);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="value"></param>
        public void SetNegativeStaminaRegenerationBonus(int value)
        {
            negativeStaminaRegenerationBonus = value;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="value"></param>
        public void ResetNegativeStaminaRegenerationBonus()
        {
            negativeStaminaRegenerationBonus = 0f;
        }

        public void SetStaminaRegenerationBonus(float value)
        {
            this.STAMINA_REGENERATION_RATE_BONUS = value;
        }
    }
}
