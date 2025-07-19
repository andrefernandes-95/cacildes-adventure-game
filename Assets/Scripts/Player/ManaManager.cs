using System.Collections;
using AF.Events;
using AF.Stats;
using TigerForge;
using UnityEngine;

namespace AF
{
    public class ManaManager : MonoBehaviour
    {

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;

        [Header("Components")]
        public PlayerManager playerManager;

        [Header("Regeneration Settings")]
        public float MANA_REGENERATION_RATE = 20f;

        private void Start()
        {
            // Initialize Mana
            if (playerStatsDatabase.currentMana == -1)
            {
                SetCurrentMana(GetMaxMana());
            }
        }

        void SetCurrentMana(float mana)
        {
            playerStatsDatabase.SetCurrentMana(mana);
        }

        private void Update()
        {
            if (playerManager.statsBonusController.shouldRegenerateMana && playerStatsDatabase.currentMana < playerStatsDatabase.maxMana)
            {
                HandleManaRegen();
            }
        }

        void HandleManaRegen()
        {
            var finalRegenerationRate = MANA_REGENERATION_RATE + playerManager.statsBonusController.staminaRegenerationBonus;

            SetCurrentMana(Mathf.Clamp(playerStatsDatabase.currentMana + finalRegenerationRate * Time.deltaTime, 0f, GetMaxMana()));
        }

        public int GetMaxMana()
        {
            return Formulas.CalculateStatForLevel(
                playerStatsDatabase.maxMana + playerManager.statsBonusController.magicBonus,
                playerManager.playerStats.GetIntelligence(),
                playerStatsDatabase.levelMultiplierForMana);
        }

        public float GetCurrentMana()
        {
            return playerStatsDatabase.currentMana;
        }

        public void DecreaseMana(float amount)
        {
            SetCurrentMana(Mathf.Clamp(playerStatsDatabase.currentMana - amount, 0, GetMaxMana()));
        }

        public bool HasEnoughManaForSpell(Spell spell)
        {
            if (spell == null)
            {
                return false;
            }

            return HasEnoughManaForAction((int)spell.manaCostPerCast);
        }

        public bool HasEnoughManaForAction(int actionCost)
        {
            bool canPerform = playerStatsDatabase.currentMana - actionCost > 0;
            if (!canPerform)
            {
                playerManager.uIDocumentPlayerHUDV2.DisplayInsufficientMana();
            }

            return canPerform;
        }

        public void RestoreFullMana()
        {
            SetCurrentMana(GetMaxMana());
        }

        public void RestoreManaPercentage(float amount)
        {
            var percentage = this.GetMaxMana() * amount / 100;
            var nextValue = Mathf.Clamp(playerStatsDatabase.currentMana + percentage, 0, this.GetMaxMana());

            SetCurrentMana(nextValue);
        }

        public void RestoreManaPoints(float amount)
        {
            var nextValue = Mathf.Clamp(playerStatsDatabase.currentMana + amount, 0, this.GetMaxMana());

            SetCurrentMana(nextValue);
        }

        public float GetManaPointsForGivenIntelligence(int intelligence)
        {
            return playerStatsDatabase.maxMana + (int)Mathf.Ceil(intelligence * playerStatsDatabase.levelMultiplierForMana);
        }


        public float GetCurrentManaPercentage()
        {
            return playerStatsDatabase.currentMana * 100 / GetMaxMana();
        }
    }
}
