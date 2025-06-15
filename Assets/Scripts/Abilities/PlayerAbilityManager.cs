namespace AF
{
    using UnityEngine;

    public class PlayerAbilityManager : CharacterAbilityBaseManager
    {
        [SerializeField] PlayerManager playerManager;

        public GameObject chargingAbilityFX;

        void Awake()
        {
            playerManager.starterAssetsInputs.onChargeAbilityEnd.AddListener(EndChargeAbility);
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
            EndChargeAbility();

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
                if (chargingAbilityFX.TryGetComponent(out ParticleSystem particleSystem))
                {
                    particleSystem.Stop();
                }

                Destroy(chargingAbilityFX, 5f);

                chargingAbilityFX = null;
            }
        }

        void EndChargeAbility()
        {
            playerManager.animator.SetBool("isCharging", false);
        }

        public void QueueAbility(Ability ability)
        {
            Ability clonedAbility = Instantiate(ability);
            clonedAbility.OnPrepare(playerManager);
            playerManager.animator.SetBool("isCharging", true);
        }
    }
}
