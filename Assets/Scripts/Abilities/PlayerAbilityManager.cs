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
            chargingAbilityAmount = 0f;
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
                foreach (ParticleSystem ps in Utils.CollectComponentsFromGameObject<ParticleSystem>(chargingAbilityFX))
                {
                    if (ps != null)
                    {
                        ps.Stop();
                    }
                }
                foreach (AudioSource audioSource in Utils.CollectComponentsFromGameObject<AudioSource>(chargingAbilityFX))
                {
                    if (audioSource != null)
                    {
                        audioSource.Stop();
                    }
                }

                Destroy(chargingAbilityFX, 2f);

                chargingAbilityFX = null;
            }
        }

        void EndChargeAbility()
        {
            playerManager.animator.SetBool("isCharging", false);
        }

        public void QueueAbility(Ability ability)
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

    }
}
