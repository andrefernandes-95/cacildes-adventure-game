namespace AF
{
    using UnityEngine;

    public class CharacterConsumableManager : CharacterBaseConsumableManager
    {
        [SerializeField] CharacterManager characterManager;

        public void Consume(Consumable consumable)
        {
            if (!CanConsume(consumable))
            {
                return;
            }

            characterManager.characterBaseInventory.RemoveConsumable(consumable);

            SetCurrentConsumable(consumable);
        }

        bool CanConsume(Consumable consumable)
        {
            if (consumable == null)
            {
                return false;
            }

            if (characterManager.health.GetCurrentHealth() <= 0)
            {
                return false;
            }

            if (consumable.isRenewable && characterManager.characterBaseInventory.GetConsumableAmount(consumable) <= 0)
            {
                return false;
            }

            if (
                characterManager.isBusy
                || characterManager.characterPosture.isStunned
                || characterManager.characterDodgeController.isDodging
                || !characterManager.characterController.isGrounded
                )
            {
                return false;
            }

            return true;
        }

        public override CharacterBaseManager GetCharacter()
        {
            return characterManager;
        }
    }
}
