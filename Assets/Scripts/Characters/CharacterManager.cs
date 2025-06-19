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
            damageReceiver?.ResetStates();
            onResetStates?.Invoke();

            characterBlockController.ResetStates();

            characterPoise.ResetStates();

            executionManager.ResetStates();
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

        private void OnAnimatorMove()
        {
            if (faceTarget || alwaysFaceTarget)
            {
                RotateTowardsTarget();
            }

            if (animator.applyRootMotion)
            {
                // Apply animator's root rotation
                transform.rotation *= animator.deltaRotation;

                // Apply root motion position and gravity
                Vector3 rootMotionPosition = animator.deltaPosition + Physics.gravity * Time.deltaTime;

                if (characterController.enabled)
                {
                    HandleCuttingDistance(ref rootMotionPosition);

                    characterController.Move(rootMotionPosition);
                }
            }
        }

        void HandleCuttingDistance(ref Vector3 rootMotionPosition)
        {
            if (!isCuttingDistanceToTarget)
            {
                return;
            }

            float distanceToTarget = Vector3.Distance(targetManager.currentTarget.transform.position, transform.position);

            if (distanceToTarget >= agent.stoppingDistance)
            {
                rootMotionPosition *= cutDistanceToTargetSpeed;
            }

            if (distanceToTarget >= 0)
            {
                RotateTowardsTarget();
            }
        }

        public override Damage GetAttackDamage()
        {
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

            targetManager.ClearTarget();

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
            if (!agent.enabled)
            {
                return;
            }

            NavMeshPath navMeshPath = new();
            agent.CalculatePath(targetPosition, navMeshPath);
            agent.SetPath(navMeshPath);
        }

        public void RotateTowardsTargetAgent()
        {
            transform.rotation = agent.transform.rotation;
        }

        public void RotateTowardsTarget()
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

        public void SetSpeed(float speed)
        {
            animator.SetFloat("Speed", Mathf.Clamp01(speed));
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
    }
}
