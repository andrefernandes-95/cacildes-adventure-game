namespace AF
{
    using UnityEngine;

    public abstract class CharacterAbilityBaseManager : MonoBehaviour
    {
        public Ability currentAbility;

        [Header("Charging Ability")]
        public float chargingAbilityAmount = 0f;
        public float chargingAbilityMultiplierBonus = 1f;
        public float chargingAbilityMultiplierBonusForFullCharge = 1.5f;

        public void PrepareAbility(Ability ability)
        {
            this.currentAbility = ability;
            chargingAbilityAmount = 0f;
        }

        /// <summary>
        /// Animation Event
        /// </summary>
        public abstract void OnPrepareAbility();

        /// <summary>
        /// Animation Event
        /// </summary>
        public abstract void OnUseAbility();

        public virtual bool CanUseAbility()
        {
            if (currentAbility != null)
            {
                return false;
            }

            return true;
        }
    }
}
