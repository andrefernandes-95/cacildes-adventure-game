using System.Collections;
using System.Collections.Generic;
using AF.Health;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public abstract class CharacterAbstractBlockController : MonoBehaviour
    {
        [Header("Components")]
        public CharacterBaseManager characterManager;
        public string hashBlock = "Block";

        [Header("Parrying Settings")]
        public string hashParrying = "Parrying";
        public string hashParried = "Parried";
        public string hashPrepareParry = "Prepare Parry";
        public UnityEvent onParryEvent;
        public float baseUnarmedParryWindow = .4f;
        public float parryTimer = Mathf.Infinity;
        public UnityEvent onParriedEvent;
        public int basePostureDamageFromParry = 20;

        [Header("Counter Attack Settings")]
        [Tooltip("The amount that multiplier the current attack power if we attack immediately after a parry")]
        public float counterAttackMultiplier = 1.5f;
        public float maxCounterAttackWindowAfterParry = 0.85f;

        float currentCounterAttackWindow = Mathf.Infinity;

        Coroutine counterAttackWindowCoroutine;

        [Header("Blocking Settings")]
        [Tooltip("The effectivness of the shield. If 1f, the shield will not give any bonus. If higher, the shield is less effective.")]
        public float blockMultiplier = 1.1f;
        public int unarmedStaminaCostPerBlock = 50;
        [Range(0, 1f)] public float unarmedDefenseAbsorption = .8f;

        [Header("Unity Events")]
        public UnityEvent onBlockDamageEvent;

        // Flags
        public bool isBlocking = false;

        public UnityAction onBlockChanged;

        // GameObject
        Dictionary<Shield, GameObject> blockVisualEffects = new();
        [SerializeField] GameObject unarmedBlockVfxPrefab;
        GameObject unarmedBlockVfxInstance;
        [SerializeField] GameObject parryVfxPrefab;
        GameObject unarmedParryVfxInstance;

        public virtual void ResetStates()
        {
            isBlocking = false;
        }

        public void SetIsBlocking(bool value)
        {
            isBlocking = value;

            onBlockChanged?.Invoke();
        }

        public virtual void BlockAttack(Damage damage)
        {
            characterManager.characterPosture.TakePostureDamage((int)(damage.postureDamage * blockMultiplier));

            PlayBlockVfx();

            onBlockDamageEvent?.Invoke();
        }

        public bool CanBlockDamage(Damage damage)
        {
            if (damage.ignoreBlocking)
            {
                return false;
            }

            if (!isBlocking)
            {
                return false;
            }

            return (characterManager.characterPosture.currentPostureDamage + (int)(damage.postureDamage * blockMultiplier)) < characterManager.characterPosture.GetMaxPostureDamage();
        }

        public abstract void BeginParrying();
        public abstract bool CanUseParrying();
        public abstract bool IsAbleToParry(Damage damage);

        public void HandleParryEvent()
        {
            onParryEvent?.Invoke();

            characterManager.PlayBusyAnimationWithRootMotion(hashParrying);

            if (characterManager is CharacterManager aiCharacter)
            {
                aiCharacter.FaceTarget();
            }

            currentCounterAttackWindow = 0f;
            if (counterAttackWindowCoroutine != null)
            {
                StopCoroutine(counterAttackWindowCoroutine);
            }
            counterAttackWindowCoroutine = StartCoroutine(HandleCounterAttackWindowCoroutine());

            PlayParryVfx();
        }

        public void HandleParriedEvent(int receivedPostureDamageFromParry)
        {
            onParriedEvent?.Invoke();

            characterManager.PlayBusyAnimationWithRootMotion(hashParried);

            characterManager.characterPosture.TakePostureDamage(
                receivedPostureDamageFromParry
            );
        }

        public bool IsWithinCounterAttackWindow()
        {
            return currentCounterAttackWindow < maxCounterAttackWindowAfterParry;
        }

        IEnumerator HandleCounterAttackWindowCoroutine()
        {
            yield return new WaitForSeconds(maxCounterAttackWindowAfterParry);
            currentCounterAttackWindow = maxCounterAttackWindowAfterParry;
        }

        public abstract float GetUnarmedParryWindow();

        public abstract int GetPostureDamageFromParry();

        public void PlayBlockVfx()
        {
            Shield currentShield = characterManager.characterBaseWeaponsManager.GetCurrentLeftWeapon() as Shield;

            if (currentShield != null)
            {
                if (blockVisualEffects.ContainsKey(currentShield) && blockVisualEffects[currentShield] != null)
                {
                    blockVisualEffects[currentShield].SetActive(false);
                    blockVisualEffects[currentShield].SetActive(true);
                }
                else if (currentShield.blockFx != null)
                {
                    GameObject instance = Instantiate(currentShield.blockFx, characterManager.characterTransformHelper.leftHand);
                    blockVisualEffects.Add(currentShield, instance);
                    blockVisualEffects[currentShield].SetActive(false);
                    blockVisualEffects[currentShield].SetActive(true);
                }
            }
            else if (unarmedBlockVfxPrefab != null)
            {
                if (unarmedBlockVfxInstance == null)
                {
                    unarmedBlockVfxInstance = Instantiate(unarmedBlockVfxPrefab, characterManager.characterTransformHelper.leftHand);
                }

                unarmedBlockVfxInstance.SetActive(false);
                unarmedBlockVfxInstance.SetActive(true);
            }
        }

        public void PlayParryVfx()
        {
            if (parryVfxPrefab != null)
            {
                if (unarmedParryVfxInstance == null)
                {
                    unarmedParryVfxInstance = Instantiate(parryVfxPrefab, characterManager.characterTransformHelper.leftHand);
                }

                unarmedParryVfxInstance.SetActive(false);
                unarmedParryVfxInstance.SetActive(true);
            }
        }
    }
}
