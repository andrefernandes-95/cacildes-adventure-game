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
using UnityEditorInternal;
using System.Collections.Generic;
using System.Linq;


namespace AF
{
    public class CharacterManager : CharacterBaseManager
    {

        [Header("Combatant Info")]
        public Combatant combatant;

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
        // Animator Overrides
        [HideInInspector] public AnimatorOverrideController animatorOverrideController;

        Vector3 initialPosition;
        Quaternion initialRotation;

        [Header("Settings")]
        public float patrolSpeed = 2f;
        public float chaseSpeed = 4.5f;
        public float rotationSpeed = 6f;

        [Header("Cutting Distance To Target")]
        public float cutDistanceToTargetSpeed = 2;
        public float cutDistanceRotationSpeedMultiplier = 2f;
        public bool isCuttingDistanceToTarget = false;

        [Header("Settings")]
        public bool canRevive = true;
        public bool shouldReturnToInitialPositionOnRevive = true;

        [Header("Face Target Settings")]
        public bool faceTarget = false;
        public float faceTargetDuration = 0.25f;
        public bool alwaysFaceTarget = false;

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


        private void Awake()
        {
            SetupAnimatorOverrides();

            initialPosition = transform.position;
            initialRotation = transform.rotation;

            agent.enabled = false;

            EventManager.StartListening(EventMessages.ON_LEAVING_BONFIRE, Revive);
        }

        void SetupAnimatorOverrides()
        {

            animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = animatorOverrideController;
        }

        private void Start()
        {
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

            characterBlockController.ResetStates();

            characterPoise.ResetStates();

            executionManager.ResetStates();
            characterAbilityManager.ResetStates();
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
            animatorOverrideController.ApplyOverrides(clipOverrides);
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

            // Apply right-hand weapon overrides
            Weapon currentWeapon = characterWeaponsManager.GetCurrentRightWeapon();
            if (currentWeapon != null)
            {
                AddOrReplaceOverride(currentWeapon.weaponAnimationData.GetRightHandAnimationsForAI(), overrides);

                if (characterWeaponsManager.IsTwoHanding())
                {
                    AddOrReplaceOverride(currentWeapon.weaponAnimationData.GetTwoHandAnimationsForAI(), overrides);
                }
            }

            // Apply left-hand weapon overrides if not two-handing
            Weapon leftWeapon = characterWeaponsManager.GetCurrentLeftWeapon();
            if (leftWeapon != null && !characterWeaponsManager.IsTwoHanding())
            {
                AddOrReplaceOverride(leftWeapon.weaponAnimationData.GetLeftHandAnimationsForAI(), overrides);

                // If left weapons is a range weapon, override the animations for shooting
                if (leftWeapon.weaponRangeAnimation != null)
                {
                    AddOrReplaceOverride(leftWeapon.weaponRangeAnimation.GetAnimations(), overrides);
                }
            }

            // Lastly, check for any additional weapon animation overrides that have the highest priority
            if (currentWeapon != null)
            {
                if (characterWeaponsManager.IsTwoHanding() && currentWeapon.th_weaponAnimationOverrides.Count > 0)
                {
                    AddOrReplaceOverride(currentWeapon.th_weaponAnimationOverrides, overrides);
                }
                else if (currentWeapon.oh_weaponAnimationOverrides.Count > 0)
                {
                    AddOrReplaceOverride(currentWeapon.oh_weaponAnimationOverrides, overrides);
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
                animatorOverrideController.ApplyOverrides(clipOverrides);
            }

            animator.runtimeAnimatorController = animatorOverrideController;
        }

        private void OnAnimatorMove()
        {
            if (faceTarget || alwaysFaceTarget)
            {
                RotateTowardsTarget(rotationSpeed);
            }

            if (animator.applyRootMotion && characterController.enabled)
            {
                // If Agent is Enabled and we are not performing action, use Navmesh to position character
                if (agent.enabled && !isBusy)
                {
                    Vector3 worldDeltaPosition = agent.nextPosition - transform.position;
                    worldDeltaPosition.y = 0f;

                    Vector3 direction = worldDeltaPosition.normalized + Physics.gravity;

                    float speed = targetManager.currentTarget != null ? chaseSpeed : patrolSpeed;

                    characterController.Move(direction * speed * Time.deltaTime);

                    // Manually rotate to face agent's path direction
                    Vector3 toTarget = agent.steeringTarget - transform.position;
                    toTarget.y = 0;

                    if (toTarget.sqrMagnitude > 0.001f)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(toTarget);
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                    }
                }
                else
                {
                    // Apply animator's root rotation
                    transform.rotation *= animator.deltaRotation;

                    // Apply root motion position and gravity
                    Vector3 rootMotionPosition = animator.deltaPosition + Physics.gravity * Time.deltaTime;

                    if (isCuttingDistanceToTarget)
                    {
                        HandleCuttingDistance(ref rootMotionPosition);
                    }
                    characterController.Move(rootMotionPosition);
                }
            }
        }

        void HandleCuttingDistance(ref Vector3 rootMotionPosition)
        {
            float distanceToTarget = Vector3.Distance(targetManager.currentTarget.transform.position, transform.position);

            if (distanceToTarget >= agent.stoppingDistance)
            {
                rootMotionPosition *= cutDistanceToTargetSpeed;
            }

            if (distanceToTarget >= 0)
            {
                RotateTowardsTarget(rotationSpeed);
            }
        }

        public override Damage GetAttackDamage()
        {
            if (characterAbilityManager.currentAbility != null)
            {
                return characterAbilityManager.currentAbility.GetDamage(this);
            }

            return characterCombatController.GetCurrentDamage();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void FaceTarget()
        {
            faceTarget = true;
            Invoke(nameof(ResetFaceTargetFlag), faceTargetDuration);
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
        public void SetAlwaysFaceTarget(bool value)
        {
            alwaysFaceTarget = value;
        }

        public void ResetFaceTargetFlag()
        {
            faceTarget = false;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void FacePlayer()
        {
            var lookPos = GetPlayerManager().transform.position - transform.position;
            lookPos.y = 0;
            transform.rotation = Quaternion.LookRotation(lookPos);
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
            stateManager.ResetDefaultState();

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

            if (IsValidPosition(hit.position))
            {
                characterController.enabled = false;
                agent.enabled = false;
                transform.position = hit.position;
                characterController.enabled = true;
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
    }
}
