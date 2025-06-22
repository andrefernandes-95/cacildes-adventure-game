using UnityEngine;

namespace AF
{
    public class ThrowWeaponHelper : MonoBehaviour
    {
        [HideInInspector] public CharacterBaseManager attacker;
        [HideInInspector] public Transform target;
        [HideInInspector] public CharacterWeaponHitbox characterWeaponHitbox;

        public float rotationDuration = 0.5f;
        private bool rotateTowardsTargetBriefly = false;

        [Header("Settings")]
        float rotationTimer = 2f;
        float rotationSpeed = 25f;
        float launchForce = 15f;
        float upwardArcForce = 1f;

        Rigidbody rb;

        public void Initialize(CharacterBaseManager attacker)
        {
            this.characterWeaponHitbox = GetComponentInChildren<CharacterWeaponHitbox>(true);
            this.characterWeaponHitbox.character = attacker;
            target = attacker.GetTarget() != null ? attacker.GetTarget().transform : null;

            Utils.UpdateTransformChildrenWhere(characterWeaponHitbox.transform, (childObject) => childObject.GetComponent<IKHelper>() == null);

            if (characterWeaponHitbox.TryGetComponent<CharacterTwoHandRef>(out var twoHandRef))
            {
                twoHandRef.enabled = false;
            }

            characterWeaponHitbox.shouldDisableHitboxOnStart = false;
            characterWeaponHitbox.gameObject.SetActive(true);
            characterWeaponHitbox.EnableHitbox();

            this.characterWeaponHitbox.onDamageInflicted.AddListener(DisableWeaponEffects);

            rb = characterWeaponHitbox.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;

            Launch(); // launch with arc

            if (target != null)
            {
                rotateTowardsTargetBriefly = true;
                rotationTimer = rotationDuration;
            }
        }

        void Launch()
        {
            if (target == null)
            {
                // Throw straight forward with arc
                Vector3 direction = transform.forward;
                Vector3 velocity = direction * launchForce + Vector3.up * upwardArcForce;
                rb.linearVelocity = velocity;
            }
            else
            {
                // Throw toward target with arc
                Vector3 toTarget = (target.position - transform.position);
                toTarget.y = 0f;

                Vector3 direction = toTarget.normalized;
                Vector3 velocity = direction * launchForce + Vector3.up * upwardArcForce;
                rb.linearVelocity = velocity;
            }
        }

        void DisableWeaponEffects()
        {
            characterWeaponHitbox.trailRenderer.enabled = false;
            characterWeaponHitbox.DisableHitbox();
            characterWeaponHitbox.GetComponent<MeshRenderer>().enabled = false;
            characterWeaponHitbox.GetComponent<BoxCollider>().enabled = false;
            characterWeaponHitbox.GetComponent<Rigidbody>().Sleep();
            characterWeaponHitbox.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }

        void Update()
        {
            if (rotateTowardsTargetBriefly && rotationTimer > 0f)
            {
                RotateTowardsTarget();
                rotationTimer -= Time.deltaTime;

                if (rotationTimer <= 0f)
                {
                    rotateTowardsTargetBriefly = false;
                    DisableWeaponEffects();
                }
            }
        }

        void RotateTowardsTarget()
        {
            Vector3 direction = (target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
