using System.Collections;
using System.Collections.Generic;
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

        List<Collider> collisions = new();

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

            // Scale projectile based on current player stats
            calculatedDamage = arrow.damage.Clone();
            calculatedDamage.ScaleProjectile(shooter.characterBaseAttackManager, shooter.characterBaseWeaponsManager.GetCurrentLeftWeapon());


            // Calculate direction toward target
            Vector3 direction = transform.forward;

            if (shooter.GetTarget() != null)
            {
                direction = (shooter.GetTarget().transform.position + (shooter.GetTarget().transform.up * .75f) - transform.position).normalized;
            }

            // Apply force in target direction
            rigidbody.AddForce(direction * GetForwardVelocity(), forceMode);
        }

        void OnTriggerEnter(Collider other)
        {
            if (collisions.Contains(other))
            {
                return;
            }

            collisions.Add(other);

            other.TryGetComponent(out CharacterBaseDamageReceiver damageReceiver);

            if (CanDamageTarget(damageReceiver))
            {
                HandleCollision(damageReceiver);
            }
        }

        bool CanDamageTarget(CharacterBaseDamageReceiver damageReceiver)
        {
            if (damageReceiver == null)
            {
                return false;
            }

            // Do not damage ourselves
            if (shooter.transform.root == damageReceiver.GetCharacter()?.transform.root)
            {
                return false;
            }

            return true;
        }

        public void HandleCollision(CharacterBaseDamageReceiver damageReceiver)
        {

            damageReceiver.TakeDamage(calculatedDamage);

            if (shooter != null
                && damageReceiver?.GetCharacter() is CharacterManager characterManager
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
