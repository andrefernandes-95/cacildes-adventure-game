namespace AF
{
    using UnityEngine;

    [RequireComponent(typeof(Collider))]
    public class LightManager : MonoBehaviour
    {
        [Header("Lights To Control")]
        public Light[] lightsToToggle;

        private void Awake()
        {
            // Ensure trigger
            Collider c = GetComponent<Collider>();
            c.isTrigger = true;

            // Cache lights if not manually assigned
            if (lightsToToggle == null || lightsToToggle.Length == 0)
            {
                lightsToToggle = GetComponentsInChildren<Light>(true);
            }

            // Start disabled
            SetLightsEnabled(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                SetLightsEnabled(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                SetLightsEnabled(false);
            }
        }

        private void SetLightsEnabled(bool value)
        {
            foreach (var l in lightsToToggle)
            {
                if (l != null) l.enabled = value;
            }
        }
    }
}
