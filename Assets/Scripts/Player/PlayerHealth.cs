using AF.Events;
using AF.Health;
using AF.Stats;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;

namespace AF
{
    public class PlayerHealth : CharacterBaseHealth
    {

        [Header("Components")]
        [SerializeField] PlayerManager playerManager;
        public NotificationManager notificationManager;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public GameSession gameSession;

        [Header("Events")]
        public UnityEvent onDyingInArena;

        [Header("Options")]
        public bool isTutorialScene = false;


        private void Awake()
        {
            // Initialize Health
            if (playerStatsDatabase.currentHealth == -1)
            {
                SetCurrentHealth(GetMaxHealth());
            }
        }


        public override int GetMaxHealth()
        {
            int baseValue = Formulas.CalculateStatForLevel(
                playerStatsDatabase.maxHealth + playerManager.statsBonusController.healthBonus,
                playerManager.playerStats.GetVitality(),
                playerStatsDatabase.levelMultiplierForHealth);

            int extraBasedOnHealthMultiplier = (int)(baseValue * playerManager.statsBonusController.healthBonusMultiplier);
            baseValue += extraBasedOnHealthMultiplier;

            if (hasHealthCutInHalf)
            {
                return (int)baseValue / 2;
            }

            return baseValue;
        }


        public void SubtractAmountMultipliedByTimeDeltaTime(float amount)
        {
            TakeDamage(amount * Time.deltaTime);
        }

        public void RestoreHealthPercentage(int amount)
        {
            var percentage = GetMaxHealth() * amount / 100;
            RestoreHealth(percentage);
        }

        public float GetHealthPointsForGivenVitality(int vitality)
        {
            return Formulas.CalculateStatForLevel((int)GetCurrentHealth(), vitality, playerStatsDatabase.levelMultiplierForHealth);
        }

        public override void RestoreHealth(float value)
        {
            SetCurrentHealth(Mathf.Clamp(
                playerStatsDatabase.currentHealth + value, 0, GetMaxHealth()));

            onRestoreHealth?.Invoke();
            ShowHealthRestoredText((int)value);
        }

        public override void TakeDamage(float value)
        {
            if (value <= 0 || GetCurrentHealth() <= 0)
            {
                return;
            }

            SetCurrentHealth(Mathf.Clamp(
                playerStatsDatabase.currentHealth - value, 0, GetMaxHealth()));

            onTakeDamage?.Invoke();

            if (GetCurrentHealth() <= 0)
            {
                if (isTutorialScene)
                {
                    RestoreFullHealth();
                    return;
                }

                if (value < 999 && playerManager.statsBonusController.chanceToRestoreHealthUponDeath && Random.Range(0, 1f) >= 0.5f)
                {
                    RestoreHealthPercentage(50);
                    notificationManager.ShowNotification(LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "You were saved from death."));
                    return;
                }

                HandleDeath();
            }

            onHealthChange?.Invoke();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="amount"></param>
        public void TakeDamageWithoutOnTakeDamageEvent(float amount)
        {
            SetCurrentHealth(Mathf.Clamp(
                playerStatsDatabase.currentHealth - amount, 0, GetMaxHealth()));

            if (GetCurrentHealth() <= 0)
            {
                HandleDeath();
            }

            onHealthChange?.Invoke();
        }

        void HandleDeath()
        {
            if (gameSession.isParticipatingInArenaEvent)
            {
                EventManager.EmitEvent(EventMessages.ON_ARENA_LOST);
                gameSession.SetIsParticipatingInArenaEvent(false);
                onDyingInArena?.Invoke();
                return;
            }

            onDeath?.Invoke();
        }

        public override float GetCurrentHealth()
        {
            return playerStatsDatabase.currentHealth;
        }

        public override void RestoreFullHealth()
        {
            RestoreHealthPercentage(100);
        }

        public override void SetCurrentHealth(float value)
        {
            this.playerStatsDatabase.SetCurrentHealth(value);
            onHealthChange?.Invoke();
        }

        public override void SetMaxHealth(int value)
        {
            onHealthChange?.Invoke();
        }
    }
}
