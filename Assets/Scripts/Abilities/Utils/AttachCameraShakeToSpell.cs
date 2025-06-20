namespace AF
{
    using UnityEngine;
    using Cinemachine;
    using EditorAttributes;

    [RequireComponent(typeof(CinemachineImpulseSource))]
    public class AttachCameraShakeToSpell : MonoBehaviour
    {
        CinemachineImpulseSource cinemachineImpulseSource => GetComponent<CinemachineImpulseSource>();


        [SerializeField] float initialForce = 0.2f;
        [SerializeField] float collisionForce = 0.5f;


        [HelpBox("Optional")]
        [SerializeField] OnDamageCollisionAbstractManager damageCollider;

        private void Start()
        {
            if (damageCollider == null && this.TryGetComponent(out OnDamageCollisionAbstractManager onDamageCollisionAbstractManager))
            {
                onDamageCollisionAbstractManager.onColliding.AddListener(() => ShakeCamera(collisionForce));
            }

            ShakeCamera(initialForce);
        }

        void ShakeCamera(float value)
        {
            cinemachineImpulseSource.GenerateImpulse(value);
        }
    }

}
