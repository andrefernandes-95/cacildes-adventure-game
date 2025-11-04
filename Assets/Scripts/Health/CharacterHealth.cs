
using AF.Companions;
using AF.Events;
using EditorAttributes;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Health
{
    public class CharacterHealth : CharacterBaseHealth
    {
        public GameSession gameSession;
        public CharacterManager characterManager;
        public CompanionsDatabase companionsDatabase;

        [HideInInspector] public UnityEvent onHealthSettingsChanged;

        [SerializeField]
        protected int maxHealth = 100;

        [HelpBox("For some cases, like a boss, we might want to set a different max health basis value")]
        [SerializeField] bool overrideMaxHealth = false;

        [SerializeField] float m_currentHealth;

        [Header("Events")]
        public UnityEvent onHalfHealth;
        bool hasRunHalthHealthEvent = false;
        public UnityEvent onRevive;

        [Header("Options")]
        public int bonusHealth = 0;
        public int bonusHealthFromCompanions = 0;

        // Components
        LockOnRef _characterLockOnRef;

        public void Awake()
        {
            SetCurrentHealth(GetMaxHealth());

            EventManager.StartListening(EventMessages.ON_PARTY_CHANGED, UpdateHealthSettings);
        }

        void UpdateHealthSettings()
        {
            onHealthSettingsChanged?.Invoke();
        }

        public override void RestoreHealth(float value)
        {
            SetCurrentHealth(GetCurrentHealth() + value);

            ShowHealthRestoredText((int)value);

            onRestoreHealth?.Invoke();
        }

        public override void TakeDamage(float value)
        {
            ShowHealthbar();

            if (value <= 0 || GetCurrentHealth() <= 0)
            {
                HideHealthbar();
                return;
            }

            SetCurrentHealth(GetCurrentHealth() - value);

            if (hasRunHalthHealthEvent == false && GetCurrentHealth() <= GetMaxHealth() / 2)
            {
                hasRunHalthHealthEvent = true;
                onHalfHealth?.Invoke();
            }

            onTakeDamage?.Invoke();

            CheckIfShouldDie();

            onHealthChange?.Invoke();
        }

        public void CheckIfShouldDie()
        {
            if (GetCurrentHealth() <= 0)
            {
                HandleEnemyDeath();
            }
        }

        void HandleEnemyDeath()
        {
            PlayDeath();

            CheckIfHasBeenKilledWithRightWeapon();

            EventManager.EmitEvent(EventMessages.ON_CHARACTER_KILLED);

            onDeath?.Invoke();

            // Disable state machine
            characterManager.stateManager.gameObject.SetActive(false);

            // Give Loot
            characterManager.characterLoot.GiveLoot();

            // Play Death Animation
            PlayDeathAnimation();

            // Disable enemy colliders so they don't block doors and other places
            HandleCollisions(false);
            HideHealthbar();

            // If is boss, handle boss stuff
            if (characterManager.characterBossController != null && characterManager.characterBossController.IsBoss())
            {
                characterManager.characterBossController.OnAllBossesDead();
            }
        }

        public void PlayDeathAnimation()
        {
            // Play Death Animation
            characterManager.PlayBusyAnimationWithRootMotion("Dying");
        }


        public override int GetMaxHealth()
        {
            int maxHealthValue = characterManager.combatant != null && overrideMaxHealth == false
                ? characterManager.combatant.maximumHealth : this.maxHealth;

            int value = Utils.ScaleWithCurrentNewGameIteration(maxHealthValue + bonusHealth + bonusHealthFromCompanions, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);

            int extraBasedOnHealthMultiplier = (int)(value * characterManager.statsBonusController.healthBonusMultiplier);
            value += extraBasedOnHealthMultiplier;

            if (hasHealthCutInHalf)
            {
                return (int)value / 2;
            }

            return value;
        }

        public override float GetCurrentHealth()
        {
            return m_currentHealth;
        }

        public override void RestoreFullHealth()
        {
            RestoreHealth(GetMaxHealth());
        }

        public void Revive()
        {
            hasRunHalthHealthEvent = false;
            RestoreFullHealth();
            onRevive?.Invoke();

            if (characterManager.stateManager != null)
            {
                characterManager.stateManager.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log($"{characterManager.name} has not state manager assigned");
            }

            HandleCollisions(true);

            HideHealthbar();
        }

        void HandleCollisions(bool activate)
        {
            characterManager.characterController.enabled = activate;

            var lockOnRef = GetLockOnRef();
            if (lockOnRef != null && lockOnRef.TryGetComponent<SphereCollider>(out var sphereCollider))
            {
                sphereCollider.enabled = activate;
            }
        }

        public override void SetCurrentHealth(float value)
        {
            this.m_currentHealth = Mathf.Clamp(value, 0, GetMaxHealth());
            UpdateHealthbar();
        }

        public override void SetMaxHealth(int value)
        {
            this.maxHealth = value;
        }

        public void IncreaseBonusHealth(int value)
        {
            this.bonusHealth += value;
        }

        public override void SetHasHealthCutInHealth(bool value)
        {
            base.SetHasHealthCutInHealth(value);

            UpdateHealthSettings();
        }

        LockOnRef GetLockOnRef()
        {
            if (_characterLockOnRef == null)
            {
                _characterLockOnRef = characterManager.GetComponentInChildren<LockOnRef>();
            }

            return _characterLockOnRef;
        }

        void UpdateHealthbar()
        {
            onUpdateHealthbar?.Invoke();
        }

        void ShowHealthbar()
        {
            onShowHealthbar?.Invoke();
        }

        void HideHealthbar()
        {
            onHideHealthbar?.Invoke();
        }
    }

}
