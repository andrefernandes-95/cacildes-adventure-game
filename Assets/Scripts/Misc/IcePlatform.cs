using System.Collections;
using UnityEngine;

namespace AF
{
    [RequireComponent(typeof(Cinemachine.CinemachineImpulseSource))]
    public class IcePlatform : MonoBehaviour
    {
        [Header("Timing")]
        public float breakDelay = 1f;
        public float respawnDelay = 10f;

        [Header("Effects")]
        public ParticleSystem breakParticles;
        public AudioSource audioSource;

        [Header("Sliding")]
        public float iceSlideForce = 6f;   // how strong the slide feels
        public float randomDrift = 0.3f;

        [Header("Components")]
        public MeshRenderer[] renderersToDisable;
        public MeshCollider[] collidersToDisable;

        private bool isBroken = false;
        private Coroutine breakRoutine;

        PlayerManager playerManager;

        bool slidePlayer = false;

        bool IsPlayer(GameObject other)
        {
            if (other.CompareTag("Player"))
            {
                if (playerManager == null) playerManager = other.gameObject.GetComponent<PlayerManager>();

                return true;
            }

            return false;
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (IsPlayer(collision.gameObject) && !isBroken)
            {
                breakRoutine ??= StartCoroutine(BreakAfterDelay());

                slidePlayer = true;
            }
        }

        void Update()
        {
            if (slidePlayer && playerManager != null)
            {
                Vector3 slideDir = playerManager.transform.forward.normalized;

                // Add slight random sideways drift
                float finalRandomDrift = Random.Range(-randomDrift, randomDrift); // tweak intensity
                Vector3 sideDir = playerManager.transform.right * finalRandomDrift;

                // Combine forward slide with random sideways motion
                Vector3 finalDir = (slideDir + sideDir).normalized;

                playerManager.characterController.Move(iceSlideForce * Time.deltaTime * finalDir);
            }
        }

        private void OnTriggerExit(Collider collision)
        {
            if (IsPlayer(collision.gameObject))
            {
                slidePlayer = false;
            }
        }

        IEnumerator BreakAfterDelay()
        {
            yield return new WaitForSeconds(breakDelay);
            BreakPlatform();
        }

        void BreakPlatform()
        {
            if (isBroken) return;
            isBroken = true;

            foreach (var r in renderersToDisable) r.enabled = false;
            foreach (var c in collidersToDisable) c.enabled = false;

            if (breakParticles != null)
            {
                breakParticles.Play();
            }

            if (audioSource != null)
            {
                audioSource.Play();
            }

            StartCoroutine(RespawnAfterDelay());
        }

        IEnumerator RespawnAfterDelay()
        {
            yield return new WaitForSeconds(respawnDelay);

            foreach (var r in renderersToDisable) r.enabled = true;
            foreach (var c in collidersToDisable) c.enabled = true;

            isBroken = false;
            breakRoutine = null;
        }

    }

}
