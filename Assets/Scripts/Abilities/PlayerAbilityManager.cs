namespace AF
{
    using UnityEngine;

    public class PlayerAbilityManager : CharacterAbilityBaseManager
    {
        [SerializeField] PlayerManager playerManager;

        public GameObject chargingAbilityFX;

        void Awake()
        {
            playerManager.starterAssetsInputs.onUseAbility.AddListener(ChargeAbility);
        }

        public void ResetStates()
        {
            currentAbility = null;
            CleanupChargingAbilitySpell();
            playerManager.animator.SetBool("isCharging", false);
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
            if (currentAbility != null)
            {
                currentAbility.OnUse(playerManager);
            }

            CleanupChargingAbilitySpell();
        }

        void CleanupChargingAbilitySpell()
        {
            // Clean up charging spells if we were interrupted before
            if (chargingAbilityFX != null)
            {
                Destroy(chargingAbilityFX);
                chargingAbilityFX = null;
            }
        }

        void ChargeAbility()
        {
            if (currentAbility != null)
            {
                playerManager.animator.SetBool("isCharging", true);
            }
        }
    }
}
