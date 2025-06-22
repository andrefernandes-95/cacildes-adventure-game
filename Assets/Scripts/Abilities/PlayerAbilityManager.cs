namespace AF
{
    using UnityEngine;

    public class PlayerAbilityManager : CharacterAbilityBaseManager
    {
        [SerializeField] PlayerManager playerManager;

        void Awake()
        {
            playerManager.starterAssetsInputs.onChargeAbilityEnd.AddListener(() => SetIsCharging(false));
        }

        public override void ResetStates()
        {
            // Clear current ability
            currentAbility = null;

            // Clean Warmup Spell Effects
            CleanupChargingAbilitySpell();

            // Reset Charging Settings
            SetIsCharging(false);
            chargingAbilityAmount = 0f;
            ResetChargeAnimations(playerManager);
        }

        public override void OnPrepareAbility()
        {
            if (currentAbility != null)
            {
                currentAbility.OnPrepare(playerManager);
            }
        }

        public override void OnUseAbility()
        {
            SetIsCharging(false);

            if (currentAbility != null)
            {
                currentAbility.OnUse(playerManager);
            }

            CleanupChargingAbilitySpell();
        }

        protected override CharacterBaseManager GetCharacter()
        {
            return playerManager;
        }

        protected override void DequeueAbilities()
        {
            if (!CanUseAbility() || queuedAbilities.Count == 0)
                return;

            var selectedAbility = queuedAbilities[0];
            queuedAbilities.RemoveAt(0);

            if (selectedAbility != null)
            {
                selectedAbility.OnPrepare(playerManager);
            }
        }
    }
}
