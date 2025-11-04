namespace AF
{
    using UnityEngine;

    public class InCavernTrigger : MonoBehaviour
    {
        DayNightManager dayNightManager;

        enum CavernTrigger
        {
            ENTERING_CAVERN,
            EXITING_CAVERN
        }

        [SerializeField] CavernTrigger cavernTrigger = CavernTrigger.ENTERING_CAVERN;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                GetDayNightManager().isInCavern = cavernTrigger == CavernTrigger.ENTERING_CAVERN;
            }
        }

        DayNightManager GetDayNightManager()
        {
            if (dayNightManager == null)
            {
                dayNightManager = FindAnyObjectByType<DayNightManager>(FindObjectsInactive.Include);
            }

            return dayNightManager;
        }
    }
}