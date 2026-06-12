using UnityEngine;

namespace AF.DLC
{
    public class DLCPackDependant : MonoBehaviour
    {
        [Header("DLC Reference")]
        public SteamDLC dlc;

        private void Awake()
        {
            Utils.UpdateTransformChildren(transform, false);
        }

        private void Start()
        {
            Evaluate();
        }

        public void Evaluate()
        {
            if (dlc == null)
            {
                Debug.LogWarning($"No DLC assigned for {name}");
                return;
            }

            bool owned = dlc.IsOwned();
            Utils.UpdateTransformChildren(transform, owned);
        }
    }
}
