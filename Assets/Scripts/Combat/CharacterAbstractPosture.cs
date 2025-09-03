using System.Collections;
using AF.Health;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace AF
{
    public abstract class CharacterAbstractPosture : MonoBehaviour
    {
        [Header("Posture")]
        public float currentPostureDamage;
        public readonly float POSTURE_BREAK_BONUS_MULTIPLIER = 4.5f;

        [Header("Unity Events")]
        public UnityEvent onPostureBreakDamage;
        public UnityEvent onDamageWhileStunned;

        [HideInInspector] public UnityEvent onShowPostureBar;
        [HideInInspector] public UnityEvent onHidePostureBar;
        [HideInInspector] public UnityEvent onUpdatePostureBar;


        [Header("Components")]
        public CharacterBaseHealth health;
        public Slider postureBarSlider;

        [Header("Optional AI Components")]
        public CharacterBaseManager characterBaseManager;
        public bool isStunned = false;

        private Coroutine postureDecayRoutine;

        public void ResetStates()
        {
            isStunned = false;
        }

        public virtual int GetMaxPostureDamage()
        {
            return characterBaseManager.combatant.maximumPosture;
        }

        public virtual bool TakePostureDamage(int extraPostureDamage)
        {
            int postureDamage = extraPostureDamage;
            currentPostureDamage = Mathf.Clamp(currentPostureDamage + postureDamage, 0, GetMaxPostureDamage());

            onUpdatePostureBar?.Invoke();
            onShowPostureBar?.Invoke();

            if (postureDecayRoutine != null)
            {
                StopCoroutine(postureDecayRoutine);
            }
            postureDecayRoutine = StartCoroutine(BeginDecreasingPosture());

            if (currentPostureDamage >= GetMaxPostureDamage())
            {
                BreakPosture();
                return true;
            }

            return false;
        }

        IEnumerator BeginDecreasingPosture()
        {
            yield return new WaitForSeconds(1f);

            while (currentPostureDamage > 0 && health.GetCurrentHealth() > 0)
            {
                currentPostureDamage -= Time.deltaTime * GetPostureDecreateRate();
                currentPostureDamage = Mathf.Max(0, currentPostureDamage);
                onUpdatePostureBar?.Invoke();
                yield return null;
            }

            onHidePostureBar?.Invoke();
        }


        public void BreakPosture()
        {
            if (CanPlayPostureDamagedEvent())
            {
                onPostureBreakDamage?.Invoke();
            }

            HandlePostureBreak();
        }

        public void HandlePostureBreak()
        {
            currentPostureDamage = 0f;
            isStunned = true;
            postureDecayRoutine = null;

            characterBaseManager.health.PlayPostureBroke();
            onHidePostureBar?.Invoke();
        }

        public void RecoverFromStunned()
        {
            isStunned = false;

            // Do not interrupt backstab animation
            if (characterBaseManager.characterBaseDamageReceiver.isBackstabbed)
            {
                return;
            }

            onDamageWhileStunned?.Invoke();
        }

        public abstract float GetPostureDecreateRate();

        public abstract bool CanPlayPostureDamagedEvent();

        public int GetPostureDamageBonus()
        {
            return (int)(health.GetMaxHealth() * 12.5f) / 100;
        }
    }
}
