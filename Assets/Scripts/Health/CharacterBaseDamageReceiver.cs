namespace AF
{
    using System;
    using AF.Combat;
    using AF.Health;
    using GameAnalyticsSDK;
    using UnityEngine;
    using UnityEngine.Events;

    public abstract class CharacterBaseDamageReceiver : MonoBehaviour
    {
        public readonly int hashBackstabExecuted = Animator.StringToHash("AI Humanoid - Backstabbed");

        [Range(0, 1f)] public float pushForceAbsorption = 1;

        [Header("Backstab Options")]
        public bool canBeBackstabbed = true;

        [Header("Visual Effects")]
        [SerializeField] Transform characterVfxRoot;
        [SerializeField] GameObject bloodVfxPrefab;
        GameObject bloodVfxInstance;
        [SerializeField] GameObject bluntVfxPrefab;
        GameObject bluntVfxInstance;
        [SerializeField] GameObject slashVfxPrefab;
        GameObject slashVfxInstance;
        [SerializeField] GameObject pierceVfxPrefab;
        GameObject pierceVfxInstance;
        [SerializeField] GameObject fireVfxPrefab;
        GameObject fireVfxInstance;
        [SerializeField] GameObject frostVfxPrefab;
        GameObject frostVfxInstance;
        [SerializeField] GameObject lightningVfxPrefab;
        GameObject lightningVfxInstance;
        [SerializeField] GameObject magicVfxPrefab;
        GameObject magicVfxInstance;
        [SerializeField] GameObject darknessVfxPrefab;
        GameObject darknessVfxInstance;
        [SerializeField] GameObject waterVfxPrefab;
        GameObject waterVfxInstance;


        [Header("Unity Events")]
        public UnityEvent onDamageReceived;
        public UnityEvent onPhysicalDamage;
        public UnityEvent onFireDamage;
        public UnityEvent onFrostDamage;
        public UnityEvent onMagicDamage;
        public UnityEvent onLightningDamage;
        public UnityEvent onDarknessDamage;
        public UnityEvent onWaterDamage;
        public UnityEvent onBackstabbed;
        public UnityEvent onAttackedWhileWithFlatulence;

        [HideInInspector] public UnityEvent<int> onPhysicalDamageUI;
        [HideInInspector] public UnityEvent<int> onFireDamageUI;
        [HideInInspector] public UnityEvent<int> onFrostDamageUI;
        [HideInInspector] public UnityEvent<int> onLightningDamageUI;
        [HideInInspector] public UnityEvent<int> onMagicDamageUI;
        [HideInInspector] public UnityEvent<int> onDarknessDamageUI;
        [HideInInspector] public UnityEvent<int> onWaterDamageUI;

        /// <summary>
        /// onDamageMofierEvent() receives 3 arguments - damage itself, the attacking character and the damage receiving character
        /// </summary>
        [HideInInspector] public UnityEvent<Damage, CharacterBaseManager, CharacterBaseManager> onDamageModifierEvent = new();


        [Header("Flags")]
        public bool ignoreDamage = false;
        public bool canTakeDamage = true;
        public bool damageOnDodge = false;
        public bool waitingForBackstab = false;
        public bool hasFlatulence = false;
        public bool isTakingDamage = false;
        public bool isBackstabbed = false;

        public abstract CharacterBaseManager GetCharacter();

        public void ResetStates()
        {
            canTakeDamage = true;
            isTakingDamage = false;
            isBackstabbed = false;
        }

        public abstract void HandleIncomingDamage(CharacterBaseManager attacker, UnityAction<Damage> onTakeDamage);

        public abstract void TakeDamage(Damage damage);

        public void TakeDamage(Damage damage, bool callOnDamageReceivedEvent)
        {
            if (hasFlatulence)
            {
                onAttackedWhileWithFlatulence?.Invoke();
            }

            if (!CanTakeDamage(null))
            {
                return;
            }

            ApplyDamage(damage, callOnDamageReceivedEvent);
        }

        /// <summary>
        /// Unity Event
        /// Bypass the CanTakeDamage check
        /// </summary>
        /// <param name="damage"></param>
        public void ApplyDamage(Damage damage)
        {
            ApplyDamage(damage, true);
        }

        public abstract void ApplyDamage(Damage damage, bool callOnDamageReceivedEvent);
        public abstract void SetCanTakeDamage(bool value);

        public virtual bool CanTakeDamage(CharacterBaseManager attacker)
        {
            if (GetCharacter() != null)
            {
                if (!GetCharacter().isConfused)
                {
                    // If attacking ourselves, do not allow damage to be taken
                    if (GetCharacter() == attacker)
                    {
                        return false;
                    }

                    // Don't allow same factions to hit each other
                    if (GetCharacter().IsFromSameFaction(attacker))
                    {
                        return false;
                    }
                }

                // If dead, do not take damage
                if (GetCharacter().health.GetCurrentHealth() <= 0)
                {
                    return false;
                }

                if (GetCharacter().characterBaseDodgeController.isDodging)
                {
                    return false;
                }

            }

            if (ignoreDamage)
            {
                return false;
            }

            if (!canTakeDamage)
            {
                return false;
            }

            if (hasFlatulence)
            {
                return false;
            }

            return true;
        }

        protected void HandleAttackWhileFlatulent()
        {
            if (hasFlatulence)
            {
                onAttackedWhileWithFlatulence?.Invoke();
            }
        }

        protected void LogIncomingDamageNullError(CharacterBaseManager attacker)
        {
            if (!GameAnalytics.Initialized)
            {
                GameAnalytics.Initialize();
            }

            GameAnalytics.NewErrorEvent(GAErrorSeverity.Error, "Incoming Damage was null. Damage Owner was: " + attacker != null ? attacker.gameObject.name : " - null damage owner game object - ");
        }

        protected void RecoverFromStunnedStateWhenAttacked()
        {
            if (waitingForBackstab)
            {
                return;
            }

            if (GetCharacter() != null && GetCharacter().characterPosture.isStunned)
            {
                GetCharacter().characterPosture.RecoverFromStunned();
            }
        }

        protected bool TryParryIncomingDamage(CharacterBaseManager attacker, Damage incomingDamage)
        {
            if (GetCharacter().characterAbstractBlockController.IsAbleToParry(incomingDamage))
            {
                GetCharacter().characterAbstractBlockController.HandleParryEvent();
                attacker.characterAbstractBlockController.HandleParriedEvent(GetCharacter().characterAbstractBlockController.GetPostureDamageFromParry());
                return true;
            }

            return false;
        }

        protected bool TryBlockIncomingDamageForAI(CharacterBaseManager attacker, ref Damage incomingDamage)
        {
            if (!GetCharacter().characterAbstractBlockController.CanBlockDamage(incomingDamage))
            {
                return false;
            }

            incomingDamage = GetCharacter().characterBaseWeaponsManager.GetCurrentShieldDefenseAbsorption(incomingDamage);

            GetCharacter().characterAbstractBlockController.BlockAttack(incomingDamage);
            return true;
        }

        protected bool TryBlockIncomingDamageForPlayer(PlayerManager playerManager, CharacterBaseManager attacker, ref Damage incomingDamage)
        {
            if (!GetCharacter().characterAbstractBlockController.CanBlockDamage(incomingDamage))
            {
                return false;
            }

            if (playerManager.staminaStatManager.HasEnoughStaminaForAction(playerManager.playerWeaponsManager.GetCurrentBlockStaminaCost()))
            {
                incomingDamage = playerManager.playerWeaponsManager.GetCurrentShieldDefenseAbsorption(incomingDamage);

                if (attacker != null && attacker is CharacterManager enemy)
                {
                    playerManager.playerWeaponsManager.ApplyShieldDamageToAttacker(enemy);
                }

                playerManager.staminaStatManager.DecreaseStamina((int)playerManager.playerWeaponsManager.GetCurrentBlockStaminaCost());
                playerManager.characterAbstractBlockController.BlockAttack(incomingDamage);

                if (playerManager.characterAbstractBlockController is PlayerBlockController playerBlockController)
                {
                    playerBlockController.SetCanCounterAttack(true);
                }
            }

            return true;
        }

        protected void HandleAngleHitFrom(CharacterBaseManager attacker)
        {
            if (GetCharacter() != null)
            {
                GetCharacter().characterPoise.angleHitFrom =
                                Vector3.SignedAngle(attacker.transform.forward, GetCharacter().transform.forward, Vector3.up);
            }
        }

        protected void HandlePushForce(Damage damage)
        {
            if (waitingForBackstab)
            {
                return;
            }

            if (damage.pushForce <= 0)
            {
                return;
            }

            if (GetCharacter() == null || GetCharacter().characterPushController == null)
            {
                return;
            }

            var targetPos = GetCharacter().transform.position - Camera.main.transform.position;
            targetPos.y = 0;
            float finalPushForce = Mathf.Clamp(damage.pushForce * pushForceAbsorption, 0, Mathf.Infinity) * 2.5f;

            GetCharacter().characterPushController.ApplyForceSmoothly(
                targetPos.normalized,
                finalPushForce,
                .25f);
        }

        // No need to pass ref in the functions argument since Damage is a class, not a struct
        protected void FilterDamageAbsorption(Damage damage)
        {
            if (GetCharacter() == null)
            {
                return;
            }

            GetCharacter().characterBaseDefenseManager.FilterIncomingDamage(damage);
        }

        protected void HandleEquipmentPassiveFilterEffects(Damage damage)
        {
            if (GetCharacter() != null)
            {
                GetCharacter().characterBaseWeaponsManager.GetCurrentShieldPassiveDamageFilter(damage);
            }
        }

        protected bool HandleDamageFromBackstab(Damage incomingDamage)
        {
            if (!waitingForBackstab)
            {
                return false;
            }

            waitingForBackstab = false;
            isBackstabbed = true;

            GetCharacter().PlayBusyHashedAnimationWithRootMotion(hashBackstabExecuted);

            // Apply Damage
            incomingDamage.physical += GetCharacter().characterPosture.GetPostureDamageBonus();
            GetCharacter().health.TakeDamage(incomingDamage.GetTotalDamage());

            onBackstabbed?.Invoke();

            return true;
        }

        protected bool HandleDamageFromAttack(Damage incomingDamage)
        {
            bool isPostureBroken = GetCharacter().characterPosture.TakePostureDamage(incomingDamage.postureDamage);

            if (isPostureBroken || GetCharacter().characterPosture.isStunned)
            {
                incomingDamage.physical += GetCharacter().characterPosture.GetPostureDamageBonus();
            }

            GetCharacter().health.TakeDamage(incomingDamage.GetTotalDamage());

            // Apply poise damage if not stunned
            if (!isPostureBroken && GetCharacter().characterAbstractBlockController.isBlocking == false)
            {
                GetCharacter().characterPoise.TakePoiseDamage(incomingDamage.poiseDamage);
            }

            return isPostureBroken;
        }

        protected void HandleDamageFromStatusEffects(Damage incomingDamage)
        {
            if (incomingDamage.statusEffects != null && incomingDamage.statusEffects.Length > 0)
            {
                foreach (var statusEffectToApply in incomingDamage.statusEffects)
                {
                    GetCharacter().statusController.InflictStatusEffect(statusEffectToApply.statusEffect, statusEffectToApply.amountPerHit);
                }
            }
        }

        protected void HandleDamageEvents(Damage incomingDamage)
        {
            if (incomingDamage.physical > 0)
            {
                onPhysicalDamageUI?.Invoke(incomingDamage.physical);
                onPhysicalDamage?.Invoke();
            }

            if (incomingDamage.fire > 0)
            {
                onFireDamageUI?.Invoke(incomingDamage.fire);
                onFireDamage?.Invoke();
            }

            if (incomingDamage.frost > 0)
            {
                onFrostDamageUI?.Invoke(incomingDamage.frost);
                onFrostDamage?.Invoke();
            }

            if (incomingDamage.magic > 0)
            {
                onMagicDamageUI?.Invoke(incomingDamage.magic);
                onMagicDamage?.Invoke();
            }

            if (incomingDamage.lightning > 0)
            {
                onLightningDamageUI?.Invoke(incomingDamage.lightning);
                onLightningDamage?.Invoke();
            }

            if (incomingDamage.darkness > 0)
            {
                onDarknessDamageUI?.Invoke(incomingDamage.darkness);
                onDarknessDamage?.Invoke();
            }

            if (incomingDamage.water > 0)
            {
                onWaterDamageUI?.Invoke(incomingDamage.water);
                onWaterDamage?.Invoke();
            }

            HandleDamageEffects(incomingDamage);
        }

        void HandleDamageEffects(Damage incomingDamage)
        {
            if (incomingDamage.physical > 0)
            {
                PlayDamageVfx(ref bloodVfxInstance, bloodVfxPrefab);

                switch (incomingDamage.weaponAttackType)
                {
                    case WeaponAttackType.Blunt:
                        PlayDamageVfx(ref bluntVfxInstance, bluntVfxPrefab);
                        break;
                    case WeaponAttackType.Slash:
                        PlayDamageVfx(ref slashVfxInstance, slashVfxPrefab);
                        break;
                    case WeaponAttackType.Pierce:
                        PlayDamageVfx(ref pierceVfxInstance, pierceVfxPrefab);
                        break;
                }
            }

            if (incomingDamage.fire > 0)
            {
                PlayDamageVfx(ref fireVfxInstance, fireVfxPrefab);
            }
            if (incomingDamage.frost > 0)
            {
                PlayDamageVfx(ref frostVfxInstance, frostVfxPrefab);
            }
            if (incomingDamage.magic > 0)
            {
                PlayDamageVfx(ref magicVfxInstance, magicVfxPrefab);
            }
            if (incomingDamage.lightning > 0)
            {
                PlayDamageVfx(ref lightningVfxInstance, lightningVfxPrefab);
            }
            if (incomingDamage.darkness > 0)
            {
                PlayDamageVfx(ref darknessVfxInstance, darknessVfxPrefab);
            }
            if (incomingDamage.water > 0)
            {
                PlayDamageVfx(ref waterVfxInstance, waterVfxPrefab);
            }
        }

        void PlayDamageVfx(ref GameObject vfxInstance, GameObject vfxPrefab)
        {
            if (vfxPrefab == null || characterVfxRoot == null)
                return;

            if (vfxInstance == null)
            {
                vfxInstance = Instantiate(vfxPrefab, characterVfxRoot);
            }

            vfxInstance.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            vfxInstance.SetActive(false);
            vfxInstance.SetActive(true);
        }

        public void TakeDamagePercentage(float damagePercentage)
        {
            int damageAmount = (int)damagePercentage * GetCharacter().health.GetMaxHealth() / 100;

            ApplyDamage(
                new(
                    physical: damageAmount,
                    fire: 0,
                    frost: 0,
                    magic: 0,
                    lightning: 0,
                    darkness: 0,
                    water: 0,
                    poiseDamage: 1,
                    postureDamage: 2,
                    weaponAttackType: WeaponAttackType.Slash,
                    statusEffects: null,
                    pushForce: 0,
                    canNotBeParried: false,
                    ignoreBlocking: false));
        }
    }
}
