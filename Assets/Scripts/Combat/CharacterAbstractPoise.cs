using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public abstract class CharacterAbstractPoise : MonoBehaviour
    {
        public int currentPoiseHitCount = 0;

        public float angleHitFrom;

        [Header("Components")]
        public CharacterBaseManager characterManager;

        [Header("Settings")]
        [Tooltip("How many hits can the enemy take and continuing attacking")]
        public float maxTimeBeforeResettingPoise = 5f;

        [Header("Unity Events")]
        public UnityEvent onPoiseDamagedEvent;

        Coroutine ResetPoiseCoroutine;

        public abstract void ResetStates();

        public virtual bool TakePoiseDamage(int poiseDamage)
        {
            if (characterManager.characterPosture.isStunned)
            {
                return false;
            }

            if (characterManager.health.GetCurrentHealth() <= 0)
            {
                return false;
            }

            currentPoiseHitCount = poiseDamage > 0 ? Mathf.Clamp(currentPoiseHitCount + 1 + poiseDamage, 0, GetMaxPoiseHits()) : 0;

            if (ResetPoiseCoroutine != null)
            {
                StopCoroutine(ResetPoiseCoroutine);
            }

            bool hasBrokenPoise = false;

            if (currentPoiseHitCount >= GetMaxPoiseHits())
            {
                hasBrokenPoise = true;

                currentPoiseHitCount = 0;

                if (CanCallPoiseDamagedEvent())
                {
                    onPoiseDamagedEvent?.Invoke();
                    PlayHitReaction();
                }

                characterManager.health.PlayPostureHit();
            }
            else
            {
                StartCoroutine(ResetPoise());
            }

            return hasBrokenPoise;
        }

        IEnumerator ResetPoise()
        {
            yield return new WaitForSeconds(maxTimeBeforeResettingPoise);
            currentPoiseHitCount = 0;
        }

        public int GetMaxPoiseHits()
        {
            return characterManager.combatant.maximumPoise;
        }

        public abstract bool CanCallPoiseDamagedEvent();
        public abstract void PlayHitReaction();
    }
}
