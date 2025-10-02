using System.Collections;
using System.Collections.Generic;
using AF.Health;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class OnDamageCollisionAbstractManager : MonoBehaviour
    {
        [Header("Projectile Settings")]
        public Projectile projectile;

        [Header("Damage Settings")]
        public Damage damage;
        List<CharacterBaseDamageReceiver> damageReceivers = new();
        Coroutine ResetDamageReceiversCoroutine;
        public CharacterBaseManager damageOwner;
        public float damageCooldown = 1f;

        [Header("Events")]
        public UnityEvent onParticleDamage;
        [HideInInspector] public UnityEvent onColliding = new UnityEvent();

        [Header("Nighttime Options")]
        public bool doubleDamageOnNightTime = false;
        public GameSession gameSession;

        [Header("Healing Options")]
        public float healingAmount = -1f;

        private void OnEnable()
        {
            damageReceivers.Clear();
        }

        public void OnCollision(GameObject other)
        {
            other.TryGetComponent<CharacterBaseDamageReceiver>(out var damageReceiver);

            if (damageReceiver == null && other.TryGetComponent<CharacterManager>(out var characterManager))
            {
                damageReceiver = characterManager.characterBaseDamageReceiver;
            }

            HandleDamage(damageReceiver);
        }

        void HandleDamage(CharacterBaseDamageReceiver damageReceiver)
        {
            if (damageOwner != null && damageOwner.characterBaseDamageReceiver == damageReceiver)
            {
                return;
            }

            if (damageReceivers.Contains(damageReceiver))
            {
                return;
            }

            damageReceivers.Add(damageReceiver);

            if (projectile != null)
            {
                projectile.HandleCollision(damageReceiver);
            }
            else if (healingAmount != -1)
            {
                if (healingAmount > 0)
                {
                    damageReceiver.GetCharacter().health.RestoreHealth(healingAmount);
                }
            }
            else if (damage != null && damageReceiver != null)
            {
                Damage copiedDamage = damage.Clone();

                if (doubleDamageOnNightTime && gameSession != null && gameSession.IsNightTime())
                {
                    copiedDamage.physical *= 2;
                    copiedDamage.fire *= 2;
                    copiedDamage.frost *= 2;
                    copiedDamage.magic *= 2;
                    copiedDamage.darkness *= 2;
                    copiedDamage.lightning *= 2;
                    copiedDamage.water *= 2;
                }

                if (damageReceiver.GetCharacter() is PlayerManager playerManager)
                {
                    (playerManager.characterBaseDamageReceiver as PlayerDamageReceiver).TryBlockIncomingDamageForPlayer(playerManager, null, ref copiedDamage);
                }
                damageReceiver.TakeDamage(copiedDamage);

                if (damageOwner != null && damageReceiver.GetCharacter() is CharacterManager aiCharacter && aiCharacter.targetManager != null)
                {
                    aiCharacter.targetManager.SetTarget(damageOwner);
                }

                if (damageOwner is PlayerManager)
                {
                    damageReceiver?.GetCharacter().health?.onDamageFromPlayer?.Invoke();
                }
            }

            onParticleDamage?.Invoke();

            onColliding?.Invoke();

            if (ResetDamageReceiversCoroutine != null)
            {
                StopCoroutine(ResetDamageReceiversCoroutine);
            }

            ResetDamageReceiversCoroutine = StartCoroutine(ResetDamageReceivers_Coroutine());
        }

        IEnumerator ResetDamageReceivers_Coroutine()
        {
            yield return new WaitForSeconds(damageCooldown);

            damageReceivers.Clear();
        }
    }
}
