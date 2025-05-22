using System.Collections;
using AF.Health;
using AF.Shooting;
using UnityEngine;

namespace AF
{
    [RequireComponent(typeof(Rigidbody))]
    public class ArrowProjectile : MonoBehaviour, IProjectile
    {
        [Header("Projectile Settings")]
        [SerializeField] ForceMode forceMode = ForceMode.Force;

        [SerializeField] float forwardVelocity = 2000f;
        [SerializeField] float upwardVelocity = 1f;
        [SerializeField] float timeBeforeDestroying = 5f;

        [Header("Arrow")]
        [SerializeField] Arrow arrow;

        [HideInInspector] Damage calculatedDamage;

        new Rigidbody rigidbody => GetComponent<Rigidbody>();

        CharacterBaseManager shooter;

        bool hasCollided = false;

        public ForceMode GetForceMode()
        {
            return forceMode;
        }

        public float GetForwardVelocity()
        {
            return forwardVelocity;
        }

        public float GetUpwardVelocity()
        {
            return upwardVelocity;
        }

        public void Shoot(CharacterBaseManager shooter, Vector3 aimForce, ForceMode forceMode)
        {
            if (arrow == null)
            {
                Debug.Log($"Attempted to shoot ArrowProjectile {this.name} without arrow assigned");
                return;
            }

            this.shooter = shooter;

            if (this.shooter is PlayerManager playerManager)
            {
                // Scale projectile based on current player stats
                calculatedDamage = arrow.damage.Clone();
                calculatedDamage.ScaleProjectile(playerManager.attackStatManager, playerManager.attackStatManager.equipmentDatabase.GetCurrentWeapon());
            }

            rigidbody.AddForce(transform.forward * GetForwardVelocity(), forceMode);
        }

        void OnTriggerEnter(Collider other)
        {
            if (hasCollided)
            {
                return;
            }

            other.TryGetComponent(out DamageReceiver damageReceiver);

            if (CanDamageTarget(damageReceiver))
            {
                HandleCollision(damageReceiver);
            }
        }

        bool CanDamageTarget(DamageReceiver damageReceiver)
        {
            if (damageReceiver == null)
            {
                return false;
            }

            // Do not damage ourselves
            if (shooter.transform.root == damageReceiver.character.transform.root)
            {
                return false;
            }

            return true;
        }

        public void HandleCollision(DamageReceiver damageReceiver)
        {
            hasCollided = true;

            damageReceiver.TakeDamage(calculatedDamage);

            if (shooter != null
                && damageReceiver?.character is CharacterManager characterManager
                && characterManager.targetManager != null)
            {
                characterManager.targetManager.SetTarget(shooter);
            }

            StartCoroutine(HandleDestroy_Coroutine());
        }

        IEnumerator HandleDestroy_Coroutine()
        {
            yield return new WaitForSeconds(timeBeforeDestroying);

            Destroy(this.gameObject);
        }
    }
}
