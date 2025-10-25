using UnityEngine;

namespace AF
{
    public class ThrowWeaponHelper : MonoBehaviour
    {
        [HideInInspector] public CharacterBaseManager attacker;
        [HideInInspector] public Transform target;
        [HideInInspector] public CharacterWeaponHitbox characterWeaponHitbox;

        public float rotationDuration = 0.5f;

        [HideInInspector] public bool shouldRotateOnUpdate = true;

        [Header("Settings")]
        float launchForce = 15f;
        float upwardArcForce = 3f;

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

            if (!shouldRotateOnUpdate)
            {
                Vector3 forwardDir = attacker.transform.forward;
                forwardDir.y = 0; // Optional: remove tilt if you want a flat forward direction
                Quaternion lookRot = Quaternion.LookRotation(forwardDir);

                // Rotate spear 90 degrees so it points like an arrow
                lookRot *= Quaternion.Euler(90f, 0f, 0f);

                transform.rotation = lookRot;
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
                Vector3 toTarget = target.position - transform.position;
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
            if (characterWeaponHitbox.TryGetComponent<MeshRenderer>(out var meshRenderer))
            {
                meshRenderer.enabled = false;
            }

            characterWeaponHitbox.GetComponent<BoxCollider>().enabled = false;
            characterWeaponHitbox.GetComponent<Rigidbody>().Sleep();
            characterWeaponHitbox.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
        }

        void Update()
        {
            if (shouldRotateOnUpdate)
            {
                transform.Rotate(Vector3.right, 1000 * Time.deltaTime, Space.Self);
            }
        }
    }
}
