namespace AF
{
    using UnityEngine;

    public class PlayerAbilityManager : CharacterAbilityBaseManager
    {
        [SerializeField] PlayerManager playerManager;

        void Awake()
        {
            playerManager.starterAssetsInputs.onChargeAbilityEnd.AddListener(EndChargeAbility);
        }

        public override void ResetStates()
        {
            currentAbility = null;
            CleanupChargingAbilitySpell();
            playerManager.animator.SetBool("isCharging", false);
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
            EndChargeAbility();

            if (currentAbility != null)
            {
                currentAbility.OnUse(playerManager);
            }

            CleanupChargingAbilitySpell();
        }

        public void QueueChargingAbility(Ability ability)
        {
            ability.OnPrepare(playerManager);
            playerManager.animator.SetBool("isCharging", true);
        }

        public float GetChargingAmountMultiplier()
        {
            if (chargingAbilityAmount <= 0.25f)
            {
                return 1f;
            }

            float chargingAbilityMultiplier = 1 + chargingAbilityAmount * chargingAbilityMultiplierBonus;

            if (chargingAbilityAmount >= 1f)
            {
                chargingAbilityMultiplier *= chargingAbilityMultiplierBonusForFullCharge;
            }

            return chargingAbilityMultiplier;
        }

        protected override CharacterBaseManager GetCharacter()
        {
            return playerManager;
        }
    }
}
