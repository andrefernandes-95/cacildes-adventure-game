namespace AF
{
    using UnityEngine;

    public class TrackingProjectile : MonoBehaviour
    {
        [Tooltip("Time in seconds for the projectile to reach the target.")]
        public float travelDuration = 1.5f;

        [Tooltip("Maximum height of the arc.")]
        public float arcHeight = 3f;

        [Tooltip("Whether to rotate toward movement direction.")]
        public bool rotateTowardTarget = true;

        [Tooltip("Destroy when close to destination.")]
        public float arrivalThreshold = 0.1f;

        [Header("FX")]
        public GameObject explosionFx;

        [Header("SFX")]
        public AudioClip explosionSfx;

        private Vector3 startPoint;
        private Vector3 controlPoint;
        public Vector3 destination;

        private float elapsedTime = 0f;
        private bool initialized = false;

        public void Initialize(Vector3 start, float arcOverride = -1f)
        {
            startPoint = start;

            float height = arcOverride >= 0 ? arcOverride : arcHeight;

            Vector3 midPoint = (start + destination) / 2f;
            controlPoint = midPoint + Vector3.up * height;

            initialized = true;
        }

        private void Update()
        {
            if (!initialized) return;

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / travelDuration);

            // Bezier interpolation
            Vector3 currentPos = Mathf.Pow(1 - t, 2) * startPoint +
                                 2 * (1 - t) * t * controlPoint +
                                 Mathf.Pow(t, 2) * destination;

            if (rotateTowardTarget && t < 1f)
            {
                float nextT = Mathf.Clamp01(t + 0.02f);
                Vector3 nextPos = Mathf.Pow(1 - nextT, 2) * startPoint +
                                  2 * (1 - nextT) * nextT * controlPoint +
                                  Mathf.Pow(nextT, 2) * destination;

                Vector3 forward = (nextPos - currentPos).normalized;
                if (forward != Vector3.zero)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forward), Time.deltaTime * 10f);
                }
            }

            transform.position = currentPos;

            if (Vector3.Distance(transform.position, destination) <= arrivalThreshold || t >= 1f)
            {
                InstantiateDestructibleFX();
                Destroy(gameObject);
            }
        }

        void InstantiateDestructibleFX()
        {
            if (explosionFx == null) return;

            GameObject instance = Instantiate(explosionFx, transform.position, Quaternion.identity);
            instance.transform.parent = null;

            DestroyableParticle destroyableParticle = instance.AddComponent<DestroyableParticle>();
            destroyableParticle.destroyAfter = 5;

            if (explosionSfx != null)
            {
                AudioSource audioSource = Utils.CreateAudioSource(instance, explosionSfx);
                audioSource.Play();
            }
        }
    }
}
