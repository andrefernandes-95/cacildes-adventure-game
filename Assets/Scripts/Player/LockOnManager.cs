using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AF.Events;
using Cinemachine;
using TigerForge;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AF
{

    public class LockOnManager : MonoBehaviour
    {
        public readonly int hashIsLockedOn = Animator.StringToHash("IsLockedOn");
        public readonly int hashStrafeHorizontal = Animator.StringToHash("StrafeHorizontal");
        public readonly int hashStrafeVertical = Animator.StringToHash("StrafeVertical");

        [Header("UI")]
        public GameObject lockOnUi;
        [Header("Flags")]
        public bool isLockedOn = false;

        [Header("Components")]
        public PlayerManager playerManager;
        public Transform playerHeadRef;
        public Soundbank soundbank;
        public StarterAssetsInputs inputs;

        [Header("Cameras")]
        public GameObject defaultCamera;
        public GameObject lockOnCamera;

        [Header("Lock On Settings")]
        public float maximumLockOnDistance = 15;
        public float MAX_TIME_BEFORE_DISENGAGING = 1f;

        [Header("Lock On References")]

        public LockOnRef nearestLockOnTarget;

        public LockOnRef leftLockTarget;
        public LockOnRef rightLockTarget;

        [Header("Layers")]
        public LayerMask detectionLayer;
        public LayerMask blockLayers;


        [Header("Target Switching")]
        public float mouseXSwitchThreshold = 0.5f;
        public float maxTargetSwitchingCooldown = 1f;
        [HideInInspector] public float targetSwitchingCooldown = Mathf.Infinity;
        Vector2 previousInputsLook = Vector2.zero;

        // Internal
        public List<LockOnRef> availableTargets = new List<LockOnRef>();

        bool evaluatingIfShouldDisengage = false;
        Coroutine CheckIfShouldDisengageCoroutine;
        Coroutine EvaluateLockOnAfterKillingEnemyCoroutine;

        CinemachineVirtualCamera cinemachineVirtualCamera;
        CinemachineFramingTransposer cinemachineFramingTransposer;
        float defaultTrackedOffsetY;

        List<LockOnRef> _allPossibleTargets = new();
        bool hasLoadedAllPossibleTargets = false;

        private void Awake()
        {
            EventManager.StartListening(EventMessages.ON_CHARACTER_KILLED, OnEnemyKilledCheckIfShouldDisengageLockOn);

            cinemachineVirtualCamera = lockOnCamera.GetComponent<Cinemachine.CinemachineVirtualCamera>();
            cinemachineFramingTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            defaultTrackedOffsetY = cinemachineFramingTransposer.m_TrackedObjectOffset.y;
        }

        private void Start()
        {
            DisableLockOn();
        }

        private void Update()
        {
            // Camera.main.useOcclusionCulling = !isLockedOn;

            if (targetSwitchingCooldown < maxTargetSwitchingCooldown)
            {
                targetSwitchingCooldown += Time.deltaTime;
            }

            if (nearestLockOnTarget != null)
            {
                if (Vector3.Distance(playerManager.transform.position, nearestLockOnTarget.transform.position) > maximumLockOnDistance)
                {
                    DisableLockOn();
                    return;
                }

                playerManager.animator.SetFloat(hashStrafeHorizontal, inputs.move.x);
                playerManager.animator.SetFloat(hashStrafeVertical, inputs.move.y);

                UpdateLockOnYPosition(nearestLockOnTarget.transform);

                /*
                if (!evaluatingIfShouldDisengage)
                {
                    if (IsViewBlocked())
                    {
                        // Something was hit between the player and the target
                        evaluatingIfShouldDisengage = true;

                        if (CheckIfShouldDisengageCoroutine != null)
                        {
                            StopCoroutine(CheckIfShouldDisengageCoroutine);
                        }

                        CheckIfShouldDisengageCoroutine = StartCoroutine(CheckIfShouldDisengage_Coroutine());
                    }
                }
                */
            }

            if (isLockedOn && Vector2.Distance(previousInputsLook, inputs.look) >= mouseXSwitchThreshold)
            {
                HandleTargetSwitching();
            }

            previousInputsLook = inputs.look;
        }

        bool IsViewBlocked(Transform target)
        {
            Vector3 start = playerHeadRef.position;
            Vector3 direction = target.position - start;
            float distance = direction.magnitude;

            if (Physics.Raycast(start, direction.normalized, out RaycastHit hit, distance, blockLayers))
            {
                // If the hit is not part of the target hierarchy, it's blocking the view
                return !hit.transform.IsChildOf(target);
            }

            return false;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnLockOnInput()
        {
            if (isLockedOn)
            {
                DisableLockOn();
            }
            else
            {
                HandleLockOnClick(false);
            }
        }

        public void SnapPlayerRotationToLockOnTarget()
        {
            if (nearestLockOnTarget == null)
            {
                return;
            }

            Vector3 targetRot = nearestLockOnTarget.transform.position - playerManager.animator.transform.position;
            targetRot.y = 0;
            var t = Quaternion.LookRotation(targetRot);

            playerManager.transform.rotation = t;
        }

        public void EnableLockOn()
        {
            lockOnCamera.gameObject.SetActive(true);
            defaultCamera.gameObject.SetActive(false);

            this.lockOnUi.gameObject.SetActive(true);

            playerManager.animator.SetBool(hashIsLockedOn, true);

            isLockedOn = true;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="duration"></param>
        public void DisableLockOnAfter(float duration)
        {
            IEnumerator DisableLockOnAfter()
            {
                yield return new WaitForSeconds(duration);
                DisableLockOn();
            }

            StartCoroutine(DisableLockOnAfter());
        }

        public void DisableLockOn()
        {
            Camera.main.GetComponent<Cinemachine.CinemachineBrain>().m_DefaultBlend.m_Time = 0f;
            this.lockOnUi.gameObject.SetActive(false);
            isLockedOn = false;
            defaultCamera.gameObject.SetActive(true);
            lockOnCamera.gameObject.SetActive(false);
            playerManager.animator.SetBool(hashIsLockedOn, false);

            nearestLockOnTarget = null;
            rightLockTarget = null;
            leftLockTarget = null;

            playerManager.animator.SetFloat(hashStrafeHorizontal, 0);
            playerManager.animator.SetFloat(hashStrafeVertical, 0);
        }

        bool CanLockOn()
        {
            if (playerManager.playerShootingManager.isAiming)
            {
                return false;
            }

            if (playerManager.thirdPersonController.isSwimming)
            {
                return false;
            }

            return true;
        }

        List<LockOnRef> GetAllValidTargets()
        {
            if (!hasLoadedAllPossibleTargets)
            {
                hasLoadedAllPossibleTargets = true;
                _allPossibleTargets = FindObjectsByType<LockOnRef>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
            }

            return _allPossibleTargets;
        }

        List<LockOnRef> FindValidTargets(bool targetEnemiesInActiveBattle = false)
        {
            List<LockOnRef> allTargets = GetAllValidTargets();
            List<LockOnRef> validTargets = new();

            foreach (var target in allTargets)
            {
                if (target.transform.root == playerManager.transform.root)
                {
                    continue;
                }

                float distance = Vector3.Distance(playerManager.transform.position, target.transform.position);
                if (distance > maximumLockOnDistance)
                {
                    continue;
                }

                if (targetEnemiesInActiveBattle)
                {
                    var cm = target.GetComponentInParent<CharacterManager>();
                    if (cm?.targetManager?.currentTarget == null)
                    {
                        continue;
                    }
                }

                if (!target.CanLockOn())
                {
                    continue;
                }

                if (!InScreen(target))
                {
                    continue;
                }

                if (IsViewBlocked(target.transform))
                {
                    continue;
                }

                validTargets.Add(target);
            }

            return validTargets;
        }

        LockOnRef SelectNearestTarget(List<LockOnRef> targets)
        {
            float closest = float.MaxValue;
            LockOnRef nearest = null;

            foreach (LockOnRef target in targets)
            {
                float dist = Vector3.Distance(target.transform.position, playerManager.transform.position);
                if (dist < closest)
                {
                    closest = dist;
                    nearest = target;
                }
            }

            return nearest;
        }

        public void HandleLockOnClick(bool shouldLookForActiveEnemies)
        {
            if (!CanLockOn()) return;

            availableTargets = FindValidTargets(shouldLookForActiveEnemies);
            nearestLockOnTarget = SelectNearestTarget(availableTargets);

            if (nearestLockOnTarget != null)
            {
                soundbank.PlaySound(soundbank.uiLockOn);
                UpdateCameraProperties(nearestLockOnTarget.transform);
                SnapPlayerRotationToLockOnTarget();
                EnableLockOn();
                targetSwitchingCooldown = 0f;
            }
            else
            {
                DisableLockOn();
            }
        }


        void UpdateCameraProperties(Transform lockOnTarget)
        {
            cinemachineVirtualCamera.m_LookAt = lockOnTarget;

            UpdateLockOnYPosition(lockOnTarget);
        }

        void UpdateLockOnYPosition(Transform lockOnTarget)
        {
            float value = defaultTrackedOffsetY + (lockOnTarget.transform.position.y - playerHeadRef.transform.position.y) / 4;

            cinemachineFramingTransposer.m_TrackedObjectOffset.y = Mathf.Clamp(
                value,
                defaultTrackedOffsetY - 1f,
                defaultTrackedOffsetY + .5f);
        }

        bool InScreen(LockOnRef target)
        {
            Vector3 viewportPosition = Camera.main.WorldToViewportPoint(target.transform.position);

            // Check if the target is within the viewport bounds (and in front of the camera)
            if (viewportPosition.z < 0 ||
                viewportPosition.x < 0 || viewportPosition.x > 1 ||
                viewportPosition.y < 0 || viewportPosition.y > 1)
            {
                return false;
            }

            // Check for occlusion (line of sight)
            Vector3 direction = target.transform.position - Camera.main.transform.position;
            float distance = direction.magnitude;

            // Optional: exclude the target's layer if needed, or use a custom LayerMask
            if (Physics.Raycast(Camera.main.transform.position, direction.normalized, out RaycastHit hit, distance, blockLayers))
            {
                // If the hit object is not the target, something is blocking the view
                if (hit.transform != target.transform && hit.transform.root != target.transform.root)
                {
                    return false;
                }
            }

            return true;
        }


        public void HandleTargetSwitching()
        {
            bool lookedRight = inputs.look.x > 0 || (Gamepad.current != null && Gamepad.current.rightStick.right.IsActuated());
            bool lookedLeft = inputs.look.x < 0 || (Gamepad.current != null && Gamepad.current.rightStick.left.IsActuated());

            if (nearestLockOnTarget == null || lookedRight == false && lookedLeft == false)
            {
                return;
            }

            inputs.look.x = 0;
            inputs.look.y = 0;

            availableTargets.Clear();
            leftLockTarget = null;
            rightLockTarget = null;

            // Define the lock-on sphere's radius and center position
            float lockOnSphereRadius = 13f;
            Vector3 lockOnSphereCenter = playerHeadRef.transform.position;

            // Find all colliders within the lock-on sphere
            Collider[] colliders = Physics.OverlapSphere(lockOnSphereCenter, lockOnSphereRadius);

            foreach (var collider in colliders)
            {
                LockOnRef enemy = collider.GetComponent<LockOnRef>();

                if (enemy != null)
                {
                    // Calculate the direction and distance from the player to the target
                    Vector3 lockTargetDirection = enemy.transform.position - lockOnSphereCenter;
                    float distanceFromTarget = lockTargetDirection.magnitude;

                    if (enemy.transform.root != playerHeadRef.transform.root && InScreen(enemy) && distanceFromTarget <= maximumLockOnDistance)
                    {
                        availableTargets.Add(enemy);
                    }
                }
            }

            float shortestDistanceLeftTarget = Mathf.Infinity;
            float shortestDistanceRightTarget = Mathf.Infinity;

            foreach (var target in availableTargets)
            {
                Vector3 relativePlayerPosition = playerManager.transform.InverseTransformPoint(target.transform.position);
                float distanceToPlayer = Vector3.Distance(target.transform.position, playerManager.transform.position);

                if (relativePlayerPosition.x < 0.00 && distanceToPlayer < shortestDistanceLeftTarget)
                {
                    shortestDistanceLeftTarget = distanceToPlayer;
                    if (target.CanLockOn() && nearestLockOnTarget != target)
                    {
                        leftLockTarget = target;
                    }
                }
                else if (relativePlayerPosition.x > 0.00 && distanceToPlayer < shortestDistanceRightTarget)
                {
                    shortestDistanceRightTarget = distanceToPlayer;
                    if (target.CanLockOn() && nearestLockOnTarget != target)
                    {
                        rightLockTarget = target;
                    }
                }
            }

            if (lookedLeft && leftLockTarget != null)
            {
                if (targetSwitchingCooldown >= maxTargetSwitchingCooldown)
                {
                    SwitchLockOnTarget(leftLockTarget);
                    targetSwitchingCooldown = 0f;
                }
            }
            else if (lookedRight && rightLockTarget != null)
            {
                if (targetSwitchingCooldown >= maxTargetSwitchingCooldown)
                {
                    SwitchLockOnTarget(rightLockTarget);
                    targetSwitchingCooldown = 0f;
                }
            }
        }

        public void SwitchLockOnTarget(LockOnRef newTarget)
        {
            if (!newTarget.CanLockOn())
            {
                return;
            }

            RaycastHit[] hits;
            Vector3 direction = newTarget.transform.position - playerHeadRef.transform.position;

            hits = Physics.RaycastAll(playerHeadRef.transform.position, direction, maximumLockOnDistance, detectionLayer);

            foreach (var hit in hits)
            {
                LockOnRef component = hit.collider.GetComponent<LockOnRef>() ?? hit.collider.GetComponentInChildren<LockOnRef>();

                if (component != null && component == newTarget)
                {
                    nearestLockOnTarget = newTarget;

                    UpdateCameraProperties(nearestLockOnTarget.transform);
                    soundbank.PlaySound(soundbank.uiLockOnSwitchTarget);

                    SnapPlayerRotationToLockOnTarget();
                    break;
                }
            }
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnEnemyKilledCheckIfShouldDisengageLockOn()
        {
            if (!isLockedOn)
            {
                return;
            }

            if (EvaluateLockOnAfterKillingEnemyCoroutine != null)
            {
                StopCoroutine(EvaluateLockOnAfterKillingEnemyCoroutine);
            }

            EvaluateLockOnAfterKillingEnemyCoroutine = StartCoroutine(EvaluateLockOnAfterKillingEnemy_Coroutine());
        }

        IEnumerator EvaluateLockOnAfterKillingEnemy_Coroutine()
        {
            yield return new WaitForSeconds(.5f);
            HandleLockOnClick(true);
        }
    }
}
