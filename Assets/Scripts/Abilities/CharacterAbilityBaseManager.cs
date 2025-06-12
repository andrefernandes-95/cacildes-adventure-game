namespace AF
{
    using UnityEngine;

    public abstract class CharacterAbilityBaseManager : MonoBehaviour
    {
        public Ability currentAbility;

        [Header("Charging Ability")]
        public float chargingAbilityAmount = 0f;

        public void PrepareAbility(Ability ability)
        {
            this.currentAbility = ability;
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
