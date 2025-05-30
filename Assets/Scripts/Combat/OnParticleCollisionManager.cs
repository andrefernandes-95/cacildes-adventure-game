using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class OnParticleCollisionManager : OnDamageCollisionAbstractManager
    {
        public UnityEvent onAnyCollisionEvent;

        [Header("Audio")]
        [SerializeField] AudioSource collisionSound;
        [SerializeField] float collisionSoundCooldown = 0.1f;
        float lastCollisionSound = -1f;

        private void OnParticleCollision(GameObject other)
        {
            OnCollision(other);

            onAnyCollisionEvent?.Invoke();

            if (collisionSound != null && Time.time - lastCollisionSound > collisionSoundCooldown)
            {
                collisionSound.Play();
                lastCollisionSound = Time.time;
            }
        }
    }
}
