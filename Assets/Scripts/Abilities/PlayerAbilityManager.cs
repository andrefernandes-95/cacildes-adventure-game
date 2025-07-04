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
            currentAbility?.OnFinished(playerManager);
            currentAbility = null;
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

        public void ClearChargingEffects()
        {
            playerManager.animator.SetBool("isCharging", false);
            playerManager.playerAbilityManager.chargingAbilityAmount = 0f;
            playerManager.playerAbilityManager.ResetChargeAnimations(playerManager);
            playerManager.playerAbilityManager.CleanupChargingAbilitySpell();
        }
    }
}
