namespace AF
{
    using UnityEngine;

    public class EmptyArrowsDependant : MonoBehaviour
    {
        [SerializeField] PlayerManager playerManager;

        private void Awake()
        {
            playerManager.playerShootingManager.onShootBow.AddListener(Evaluate);
        }

        private void Start()
        {
            Evaluate();
        }

        public void Evaluate()
        {
            bool isActive = playerManager.playerInventory.GetArrows().Count <= 0;

            Utils.UpdateTransformChildren(transform, isActive);
        }
    }
}
