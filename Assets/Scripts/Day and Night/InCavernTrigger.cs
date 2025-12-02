namespace AF
{
    using UnityEngine;

    public class InCavernTrigger : MonoBehaviour
    {
        CavernManager cavernManager;
        [SerializeField] Cavern cavern;

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                CavernManager cavernManager = GetCavernManager();

                bool isEnteringCavern = cavernManager.IsInCavern() == false;
                Cavern cavernToEnter = isEnteringCavern ? cavern : null;
                GetCavernManager().SetCavern(cavernToEnter);
            }
        }

        CavernManager GetCavernManager()
        {
            if (cavernManager == null)
            {
                cavernManager = FindAnyObjectByType<CavernManager>(FindObjectsInactive.Include);
            }

            return cavernManager;
        }
    }
}
