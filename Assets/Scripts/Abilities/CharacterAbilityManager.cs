using System.Collections.Generic;
using UnityEngine;

namespace AF
{

    public class CharacterAbilityManager : CharacterAbilityBaseManager
    {
        [SerializeField] CharacterManager characterManager;

        public override void ResetStates()
        {
            currentAbility = null;
            characterManager.animator.SetBool("isCharging", false);
            chargingAbilityAmount = 0f;
            ResetChargeAnimations(characterManager);
            DequeueAbilities();
        }


        public override void OnPrepareAbility()
        {
            if (currentAbility != null)
            {
                currentAbility.OnPrepare(characterManager);
            }
        }

        public override void OnUseAbility()
        {
            SetIsCharging(false);

            if (currentAbility != null)
            {
                currentAbility.OnUse(characterManager);
            }

            CleanupChargingAbilitySpell();
        }

        protected override CharacterBaseManager GetCharacter()
        {
            return characterManager;
        }

        public void ComboToNextAbility()
        {
            characterManager.characterAbilityManager.currentAbility = null;
            DequeueAbilities();
        }

        protected override void DequeueAbilities()
        {
            if (!CanUseAbility() || queuedAbilities.Count == 0)
                return;

            var selectedAbility = queuedAbilities[0];
            queuedAbilities.RemoveAt(0);

            if (selectedAbility != null)
            {
                selectedAbility.OnPrepare(characterManager);
            }
        }
    }
}
