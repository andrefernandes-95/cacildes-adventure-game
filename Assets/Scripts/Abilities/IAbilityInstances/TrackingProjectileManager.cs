namespace AF
{
    using UnityEngine;
    using UnityEngine.Events;

    public class TrackingProjectile : MonoBehaviour, IAbilityInstance
    {
        [Header("Spawn Position")]
        [SerializeField] Vector3 upOffset = Vector3.zero;
        [SerializeField] Vector3 targetUpOffset = Vector3.zero;

        [Tooltip("Time in seconds for the projectile to reach the target.")]
        public float travelDuration = 1.5f;

        [Tooltip("Maximum height of the arc.")]
        public float arcHeight = 3f;

        [Tooltip("Whether to rotate toward movement direction.")]
        public bool rotateTowardTarget = true;

        [Tooltip("Destroy when close to destination.")]
        public float arrivalThreshold = 0.1f;

        private Vector3 startPoint;
        private Vector3 controlPoint;
        public Transform homingDestination;
        Vector3 fallbackDestination;

        private float elapsedTime = 0f;
        private bool initialized = false;

        [Header("Events")]
        [SerializeField] UnityEvent onColliding;

        public void Initialize(Vector3 start, float arcOverride = -1f)
        {
            startPoint = start;

            float height = arcOverride >= 0 ? arcOverride : arcHeight;

            Vector3 midPoint = (start + GetDestinationPosition()) / 2f;
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
                                 Mathf.Pow(t, 2) * GetDestinationPosition();

            if (rotateTowardTarget && t < 1f)
            {
                float nextT = Mathf.Clamp01(t + 0.02f);
                Vector3 nextPos = Mathf.Pow(1 - nextT, 2) * startPoint +
                                  2 * (1 - nextT) * nextT * controlPoint +
                                  Mathf.Pow(nextT, 2) * GetDestinationPosition();

                Vector3 forward = (nextPos - currentPos).normalized;
                if (forward != Vector3.zero)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(forward), Time.deltaTime * 10f);
                }
            }

            transform.position = currentPos;

            if (Vector3.Distance(transform.position, GetDestinationPosition()) <= arrivalThreshold || t >= 1f)
            {
                OnColliding();
            }
        }


        public void OnColliding()
        {
            onColliding?.Invoke();
            Destroy(gameObject);
        }

        public void CastAbility(CharacterBaseManager caster, CharacterBaseManager target)
        {
            this.homingDestination = target != null ? target.transform : null;
            this.fallbackDestination = caster.transform.position + caster.transform.forward * 20f;
            Initialize(caster.transform.position + upOffset);
        }

        Vector3 GetDestinationPosition()
        {
            if (homingDestination != null)
            {
                return homingDestination.position + targetUpOffset;
            }

            return fallbackDestination; // Arbitrary forward target
        }
    }
}
