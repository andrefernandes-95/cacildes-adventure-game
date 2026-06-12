namespace AF
{
    using UnityEngine;

    public class EmptyManaDependant : MonoBehaviour
    {
        [SerializeField] PlayerManager playerManager;

        [Tooltip("If a spell is assigned, the children will activate only when we don't have enough mana to cast the spell")]
        [SerializeField] Spell spellToCheckManaAvailability;

        private void Awake()
        {
            playerManager.manaManager.onDecreaseMana.AddListener(Evaluate);
        }

        private void Start()
        {
            Evaluate();
        }

        public void Evaluate()
        {
            bool isActive = false;

            if (spellToCheckManaAvailability != null)
            {
                isActive = playerManager.manaManager.GetCurrentMana() <= spellToCheckManaAvailability.GetManaCost();
            }
            else
            {
                isActive = playerManager.manaManager.GetCurrentMana() <= 0;
            }

            Utils.UpdateTransformChildren(transform, isActive);
        }
    }
}
