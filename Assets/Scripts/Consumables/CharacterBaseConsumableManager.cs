namespace AF
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class CharacterBaseConsumableManager : MonoBehaviour
    {
        Consumable currentConsumable;
        public Consumable CurrentConsumable => currentConsumable;

        public void ResetStates()
        {
            EndConsumableUsage();

            currentConsumable = null;
        }

        protected void SetCurrentConsumable(Consumable consumable)
        {
            this.currentConsumable = consumable;
            StartConsumableUsage();
        }

        void StartConsumableUsage()
        {
            if (currentConsumable != null && currentConsumable.consumableEffect != null)
            {
                currentConsumable.consumableEffect.OnStart(GetCharacter());
            }
        }

        void EndConsumableUsage()
        {
            if (currentConsumable != null && currentConsumable.consumableEffect != null)
            {
                currentConsumable.consumableEffect.OnEnd(GetCharacter());
            }
        }

        public void OnConsumableUse()
        {
            if (currentConsumable != null && currentConsumable.consumableEffect != null)
            {
                currentConsumable.consumableEffect.OnUse(GetCharacter());
            }
        }

        public abstract CharacterBaseManager GetCharacter();

    }
}
