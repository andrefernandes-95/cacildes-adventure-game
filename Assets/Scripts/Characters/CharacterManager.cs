using AF.Animations;
using AF.Combat;
using AF.Equipment;
using AF.Events;
using AF.Health;
using AF.Shooting;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;
using AF.Companions;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Linq;
using AF.Shops;
using AF.Detection;

namespace AF
{
    public class CharacterManager : CharacterBaseManager
    {

        public CompanionID companionID;
        public CharacterCombatController characterCombatController;
        public TargetManager targetManager;

        public CharacterBaseShooter characterBaseShooter;
        public CharacterWeaponsManager characterWeaponsManager;
        public CharacterBossController characterBossController;
        public ExecutionManager executionManager;
        public CharacterGravity characterGravity;
        public StateManager stateManager;
        public CharacterAbilityManager characterAbilityManager;
        public CharacterBlockController characterBlockController;
        public CharacterDodgeController characterDodgeController;
        public CharacterActivityManager characterActivityManager;
        public CharacterConsumableManager characterConsumableManager;
        public CharacterLoot characterLoot;
        public CharacterShop characterShop;
        public CharacterBuffManager characterBuffManager;
        public CharacterTeleportManager characterTeleportManager;
        public CharacterWeaknessesManager characterWeaknessesManager;
        public CharacterWeaponBuffManager characterWeaponBuffManager;
        public Sight sight;
        public CharacterGesture characterGesture;
        public CharacterBackstabController characterBackstabController;

        CharacterAnimationEventListener characterAnimationEventListener => GetComponent<CharacterAnimationEventListener>();

        // Animator Overrides
        [HideInInspector] public AnimatorOverrideController animatorOverrideController;

        Vector3 initialPosition;
        Quaternion initialRotation;

        [Header("Settings")]
        public float patrolSpeed = 2f;
        public float chaseSpeed = 4.5f;
        public float rotationSpeed = 6f;
        public float minimumAgentMagnitudeToAllowSpeed = 0.1f;

        [Header("Cutting Distance To Target")]
        public float cutDistanceToTargetSpeed = 2;
        public float cutDistanceRotationSpeedMultiplier = 2f;
        public bool isCuttingDistanceToTarget = false;

        [Header("Settings")]
        public bool canRevive = true;
        public bool shouldReturnToInitialPositionOnRevive = true;

        [Tooltip("If true, will ignore weapon's attack speeds and stick to the Speed defined in CharacterAnimationEventListener()")]
        public bool ignoreWeaponAnimationSpeed = false;

        [Header("Face Target Settings")]
        [SerializeField] float maximumAngleToAttackTarget = 30f;
        [SerializeField] bool faceTarget = false;

        [Header("Partners")]
        public CharacterManager[] partners;
        public int partnerOrder = 0;

        [Header("Events")]
        public UnityEvent onResetStates;
        public UnityEvent onForceAgressionTowardsPlayer;

        // Scene Reference
        PlayerManager playerManager;

        int defaultAnimationHash;

        public GameSession gameSession;


        [Header("Unarmed Animations Overrides")]
        [SerializeField] List<AnimationOverride> oh_unarmedAnimationOverrides = new();
        [SerializeField] List<AnimationOverride> th_unarmedAnimationOverrides = new();

        [HideInInspector] AIHumanoidAnimationOverrideHelper aIHumanoidAnimationOverrideHelper;
        [HideInInspector] GenericCreatureAnimationOverrideHelper genericCreatureAnimationOverrideHelper;

        public string speedParameter = "Speed";

        [HideInInspector] public bool isRunningFromMoveTowardsEvent = false;

        const float NAVMESH_AGENT_HUMANOID_STEP_OFFSET = 0.5f;

        private void Awake()
        {
            if (TryGetComponent<AIHumanoidAnimationOverrideHelper>(out var aIHumanoidAnimationOverrideHelperResult))
            {
                this.aIHumanoidAnimationOverrideHelper = aIHumanoidAnimationOverrideHelperResult;
            }
            if (TryGetComponent<GenericCreatureAnimationOverrideHelper>(out var genericCreatureAnimationOverrideHelperResult))
            {
                this.genericCreatureAnimationOverrideHelper = genericCreatureAnimationOverrideHelperResult;
            }

            SetupAnimatorOverrides();

            initialPosition = transform.position;
            initialRotation = transform.rotation;

            agent.enabled = false;

            characterController.stepOffset = NAVMESH_AGENT_HUMANOID_STEP_OFFSET;

            EventManager.StartListening(EventMessages.ON_LEAVING_BONFIRE, Revive);
        }

        void SetupAnimatorOverrides()
        {
            animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = animatorOverrideController;
        }

        private void Start()
        {

            UpdateAnimationsBasedOnEquippedWeapons();

            defaultAnimationHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        }

        public override void ResetStates()
        {
            isCuttingDistanceToTarget = false;
            animator.applyRootMotion = true;
            isBusy = false;

            characterPosture.ResetStates();
            characterCombatController.ResetStates();
            characterWeaponsManager.ResetStates();
            characterBaseDamageReceiver?.ResetStates();
            onResetStates?.Invoke();

            characterAbstractBlockController.ResetStates();

            characterPoise.ResetStates();

            executionManager.ResetStates();
            characterAbilityManager.ResetStates();
            characterActivityManager.ResetStates();
            characterConsumableManager.ResetStates();
            characterDodgeController.ResetStates();

            characterGesture.ResetStates();
            characterBackstabController.ResetStates();

            faceTarget = false;
        }

        private void Update()
        {
            if (
                // If in battle
                !IsTargetInView() && IsBusy() == false
                || faceTarget)
            {

                if (health.GetCurrentHealth() <= 0)
                {
                    faceTarget = false;
                    return;
                }

                if (agent.enabled)
                {
                    return;
                }

                RotateTowardsTarget(rotationSpeed);
            }

            HandleSpeed();
        }

        public void UpdateAnimatorOverrideControllerClips(string animationName, AnimationClip animationClip)
        {
            if (animatorOverrideController == null)
            {
                SetupAnimatorOverrides();
            }

            var clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
            animatorOverrideController.GetOverrides(clipOverrides);
            clipOverrides[animationName] = animationClip;
            ApplyClipOverrides(clipOverrides);
        }

        public void UpdateAnimationsBasedOnEquippedWeapons()
        {
            if (animatorOverrideController == null)
            {
                SetupAnimatorOverrides();
            }

            animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            var clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
            animatorOverrideController.GetOverrides(clipOverrides);

            Dictionary<string, AnimationOverride> overrides = new();

            if (aIHumanoidAnimationOverrideHelper != null || genericCreatureAnimationOverrideHelper != null)
            {
                Dictionary<string, AnimationClip> clipOverridesForAINonHumanoid =
                    aIHumanoidAnimationOverrideHelper != null
                        ? aIHumanoidAnimationOverrideHelper.GetClipOverrides()
                        : genericCreatureAnimationOverrideHelper.GetClipOverrides();

                List<AnimationOverride> list = new();
                foreach (var entry in clipOverridesForAINonHumanoid)
                {
                    list.Add(new()
                    {
                        animationName = entry.Key,
                        animationClip = entry.Value,
                    });
                }

                AddOrReplaceOverride(list, overrides);
            }

            // IS HUMANOID
            if (genericCreatureAnimationOverrideHelper == null)
            {
                // Always apply unarmed first
                AddOrReplaceOverride(oh_unarmedAnimationOverrides, overrides);

                if (characterWeaponsManager.IsTwoHanding())
                {
                    AddOrReplaceOverride(th_unarmedAnimationOverrides, overrides);
                }

                // Apply right-hand weapon overrides
                Weapon currentWeapon = characterWeaponsManager.GetCurrentRightWeapon();
                if (currentWeapon != null)
                {
                    WeaponAnimation currentWeaponAnimationData = currentWeapon.aIWeaponAnimationData != null
                        ? currentWeapon.aIWeaponAnimationData : currentWeapon.weaponAnimationData;

                    if (currentWeaponAnimationData != null)
                    {
                        AddOrReplaceOverride(currentWeaponAnimationData.GetRightHandAnimationsForAI(this), overrides);

                        if (characterWeaponsManager.IsTwoHanding())
                        {
                            AddOrReplaceOverride(currentWeaponAnimationData.GetTwoHandAnimationsForAI(this), overrides);
                        }
                    }
                }

                // Apply left-hand weapon overrides if not two-handing
                Weapon leftWeapon = characterWeaponsManager.GetCurrentLeftWeapon();
                if (leftWeapon != null && !characterWeaponsManager.IsTwoHanding())
                {
                    WeaponAnimation leftWeaponAnimationData = leftWeapon.aIWeaponAnimationData != null
                    ? leftWeapon.aIWeaponAnimationData : leftWeapon.weaponAnimationData;

                    if (leftWeaponAnimationData != null)
                    {
                        AddOrReplaceOverride(leftWeaponAnimationData.GetLeftHandAnimationsForAI(), overrides);
                    }

                    // If left weapons is a range weapon, override the animations for shooting
                    if (leftWeapon.weaponRangeAnimation != null)
                    {
                        AddOrReplaceOverride(leftWeapon.weaponRangeAnimation.GetAnimations(true), overrides);
                    }
                }

                // Lastly, check for any additional weapon animation overrides that have the highest priority
                if (currentWeapon != null)
                {
                    if (characterWeaponsManager.IsTwoHanding())
                    {
                        if (currentWeapon.th_weaponAnimationOverrides.Count > 0)
                        {
                            AddOrReplaceOverride(currentWeapon.th_weaponAnimationOverrides, overrides);
                        }
                    }
                    else if (currentWeapon.oh_weaponAnimationOverrides.Count > 0)
                    {
                        AddOrReplaceOverride(currentWeapon.oh_weaponAnimationOverrides, overrides);
                    }
                }
            }

            // Apply all collected overrides in one go
            UpdateAnimationOverrides(animator, clipOverrides, overrides.Values.ToList());
        }

        void UpdateAnimationOverrides(Animator animator, AnimationClipOverrides clipOverrides, List<AnimationOverride> clips)
        {
            foreach (var animationOverride in clips)
            {
                clipOverrides[animationOverride.animationName] = animationOverride.animationClip;
            }

            ApplyClipOverrides(clipOverrides);

            animator.runtimeAnimatorController = animatorOverrideController;
        }

        private void OnAnimatorMove()
        {
            Vector3 gravity = characterGravity != null && characterGravity.ignoreGravity ? new Vector3(0, characterGravity.initialY, 0) : Physics.gravity;

            if (animator.applyRootMotion && characterController.enabled)
            {
                // If Agent is Enabled and we are not performing action, use Navmesh to position character
                if (agent.enabled && !isBusy)
                {
                    Vector3 worldDeltaPosition = agent.nextPosition - transform.position;
                    worldDeltaPosition.y = 0f;

                    Vector3 direction = worldDeltaPosition.normalized;

                    float speed = ShouldRun() ? chaseSpeed : patrolSpeed;

                    // Apply gravity separately
                    Vector3 velocity = direction * speed;
                    velocity.y += gravity.y * Time.deltaTime;

                    characterController.Move(velocity * Time.deltaTime);

                    HandleAgentRotation();
                }
                else
                {
                    // Apply animator's root rotation
                    transform.rotation *= animator.deltaRotation;

                    // Apply root motion position and gravity
                    Vector3 rootMotionPosition = animator.deltaPosition + gravity * Time.deltaTime;

                    if (isCuttingDistanceToTarget)
                    {
                        HandleCuttingDistance(ref rootMotionPosition);
                    }

                    characterController.Move(rootMotionPosition);
                }
            }
        }

        void HandleSpeed()
        {
            if (isCuttingDistanceToTarget)
            {
                return;
            }

            if (isBusy)
            {
                animator.SetFloat(speedParameter, 0f);
                return;
            }

            // Patrolling / Running / Fleeing
            else if (agent.enabled && agent.velocity.magnitude > 0.1f && !characterPushController.IsPushed())
            {
                float speed = ShouldRun() ? 1f : 0.5f;
                animator.SetFloat(speedParameter, speed);
            }
            else
            {
                animator.SetFloat(speedParameter, 0f);
            }
        }

        void HandleCuttingDistance(ref Vector3 rootMotionPosition)
        {
            if (targetManager.currentTarget == null)
            {
                return;
            }

            float distanceToTarget = Vector3.Distance(targetManager.currentTarget.transform.position, transform.position);

            if (distanceToTarget >= agent.stoppingDistance)
            {
                rootMotionPosition *= cutDistanceToTargetSpeed;
            }

            if (rootMotionPosition.x <= 0 && rootMotionPosition.z <= 0)
            {
                rootMotionPosition = cutDistanceToTargetSpeed * Time.deltaTime * transform.forward;
            }

            if (distanceToTarget >= 0)
            {
                RotateTowardsTarget(rotationSpeed);
            }
        }

        public override Damage GetAttackDamage(CharacterBaseManager damageReceiver)
        {
            if (characterAbilityManager.currentAbility != null)
            {
                Damage abilityDamage = characterAbilityManager.currentAbility.GetDamage(this);
                onEnhanceAttackDamageWithEquipmentEffect?.Invoke(abilityDamage, this, damageReceiver);
                return abilityDamage;
            }

            // Fallback for animation-based attacks like ambushes,
            // Evaluate hitboxes
            Damage characterAttackDamage = characterBaseAttackManager.GetAttackDamage();
            onEnhanceAttackDamageWithEquipmentEffect?.Invoke(characterAttackDamage, this, damageReceiver);
            return characterAttackDamage;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void FaceTarget()
        {
            faceTarget = true;
        }

        public void FaceTargetImmediately()
        {
            if (targetManager.currentTarget == null)
            {
                return;
            }

            Vector3 lookDirection = targetManager.currentTarget.transform.position - transform.position;
            lookDirection.y = 0;
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void FacePlayer()
        {
            if (!CanFacePlayer())
            {
                return;
            }

            var lookPos = GetPlayerManager().transform.position - transform.position;
            lookPos.y = 0;
            transform.rotation = Quaternion.LookRotation(lookPos);
        }

        bool CanFacePlayer()
        {
            if (characterActivityManager.currentActivity != null)
            {
                return false;
            }

            return true;
        }

        PlayerManager GetPlayerManager()
        {
            if (playerManager == null)
            {
                playerManager = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include);
            }

            return playerManager;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void FaceInitialRotation()
        {
            transform.rotation = initialRotation;
        }

        public void Revive()
        {
            if (characterBossController.IsBoss() || !canRevive)
            {
                return;
            }

            agent.enabled = false;

            targetManager.ClearTarget();
            stateManager?.ResetDefaultState();
            characterPosture.ResetCumulativePosture();

            if (health is CharacterHealth characterHealth)
            {
                characterHealth.Revive();

                if (IsCompanion() == false)
                {
                    if (shouldReturnToInitialPositionOnRevive)
                    {
                        characterController.enabled = false;
                        transform.SetPositionAndRotation(initialPosition, initialRotation);
                        characterController.enabled = true;
                    }
                }

                ResetStates();

                characterPosture.currentPostureDamage = 0;

                if (defaultAnimationHash != -1)
                {
                    animator.Play(defaultAnimationHash);
                }
            }

            RestoreOriginalMaterials();
        }

        public string GetCharacterID()
        {
            return companionID.GetCompanionID();
        }

        public bool IsCompanion()
        {
            return companionID != null;
        }

        public void TeleportNearPlayer()
        {
            Vector3 desiredPosition = GetPlayerManager().transform.position + (GetPlayerManager().transform.forward * -4.5f);
            NavMesh.SamplePosition(desiredPosition, out NavMeshHit hit, 15f, NavMesh.AllAreas);

            bool agentEnabledCachedValue = agent.enabled;
            if (IsValidPosition(hit.position))
            {
                characterController.enabled = false;
                agent.enabled = false;
                transform.position = hit.position;
                characterController.enabled = true;
                agent.enabled = agentEnabledCachedValue;
            }
        }

        public void Teleport(Vector3 desiredPosition)
        {
            NavMesh.SamplePosition(desiredPosition, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);

            if (IsValidPosition(hit.position))
            {
                characterController.enabled = false;
                transform.position = hit.position;
                characterController.enabled = true;
            }
        }

        public void Teleport(Vector3 desiredPosition, Quaternion desiredRotation)
        {
            characterController.enabled = false;
            transform.position = desiredPosition;
            transform.rotation = desiredRotation;
            characterController.enabled = true;
        }

        private bool IsValidPosition(Vector3 position)
        {
            // Check for Infinity or NaN values
            return !float.IsInfinity(position.x) && !float.IsInfinity(position.y) && !float.IsInfinity(position.z) &&
                   !float.IsNaN(position.x) && !float.IsNaN(position.y) && !float.IsNaN(position.z);
        }

        public void SetAgentDestination(Vector3 targetPosition)
        {
            if (agent.enabled)
            {
                NavMeshPath navMeshPath = new();
                agent.CalculatePath(targetPosition, navMeshPath);
                agent.SetPath(navMeshPath);
            }
        }

        public bool IsTargetInView()
        {
            if (targetManager.currentTarget == null)
            {
                return true;
            }

            Vector3 lookDirection = targetManager.currentTarget.transform.position - transform.position;
            lookDirection.y = 0;

            // Check angle to target
            float angleToTarget = Vector3.Angle(transform.forward, lookDirection);
            return angleToTarget <= maximumAngleToAttackTarget;
        }

        public void RotateTowardsTarget(float rotationSpeed)
        {
            if (targetManager.currentTarget == null)
            {
                return;
            }

            Vector3 lookDirection = targetManager.currentTarget.transform.position - transform.position;
            lookDirection.y = 0;

            float speed = rotationSpeed;
            if (isCuttingDistanceToTarget)
            {
                speed *= cutDistanceRotationSpeedMultiplier;
            }
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), Time.deltaTime * speed);
        }

        public float GetAngleOfCurrentTarget()
        {
            if (targetManager.currentTarget == null)
            {
                return 0;
            }

            Vector3 directionToTarget = targetManager.currentTarget.transform.position - transform.position;

            float viewableAngle = Vector3.Angle(transform.forward, directionToTarget);
            Vector3 cross = Vector3.Cross(transform.forward, directionToTarget);

            if (cross.y < 0)
            {
                viewableAngle = -viewableAngle;
            }

            return viewableAngle;
        }

        public override AnimatorOverrideController GetAnimatorOverrideController()
        {
            return animatorOverrideController;
        }

        public override CharacterBaseManager GetTarget()
        {
            return targetManager.currentTarget;
        }

        public bool ShouldRun()
        {
            return GetTarget() != null || IsCompanion() || isRunningFromMoveTowardsEvent;
        }

        public void HandleAgentRotation()
        {
            if (agent.velocity.magnitude > 0.01f)
            {
                Vector3 lookDir = agent.desiredVelocity;
                lookDir.y = 0;

                Quaternion targetRotation = Quaternion.LookRotation(lookDir);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }

        public override void OnParalyzedStart()
        {
            stateManager.enabled = false;
            animator.speed = 0f;
        }

        public override void OnParalyzedEnd()
        {
            stateManager.enabled = true;
            animator.speed = GetDefaultAnimatorSpeed();
        }

        public override float GetDefaultAnimatorSpeed()
        {
            return characterAnimationEventListener != null ? characterAnimationEventListener.animatorSpeed : 1f;
        }

    }
}
