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
        public float maximumLockOnDistanceOnSwitchingTargets = 3;
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

        // Internal
        public List<LockOnRef> availableTargets = new List<LockOnRef>();

        Coroutine EvaluateLockOnAfterKillingEnemyCoroutine;

        CinemachineVirtualCamera cinemachineVirtualCamera;
        CinemachineFramingTransposer cinemachineFramingTransposer;
        float defaultTrackedOffsetY;

        List<LockOnRef> _allPossibleTargets = new();
        bool hasLoadedAllPossibleTargets = false;

        private float mouseLookAccum = 0f;
        private float mouseLookTimer = 0f;
        private const float mouseSwitchCooldown = 0.2f; // seconds
        [SerializeField] float mouseAccumThreshold = 15f; // degrees or pixels worth of turn


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
            // Accumulate mouse input over time
            mouseLookAccum += inputs.look.x;
            mouseLookTimer -= Time.deltaTime;

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

            if (isLockedOn && mouseLookAccum != 0f)
            {
                HandleTargetSwitching();
            }

            // After evaluation of target switching based on mouse accum, check if we should reset it
            if (MouseLookedLeft() || MouseLookedRight())
            {
                // Reset after switch
                mouseLookAccum = 0f;
                mouseLookTimer = mouseSwitchCooldown;
            }
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

        public void SetHasLoadedAllPossibleTargets(bool value)
        {
            this.hasLoadedAllPossibleTargets = value;
        }

        List<LockOnRef> GetAllValidTargets()
        {
            if (!hasLoadedAllPossibleTargets)
            {
                SetHasLoadedAllPossibleTargets(true);
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
                if (target == null)
                {
                    continue;
                }

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
            cinemachineFramingTransposer.m_TrackedObjectOffset.y = defaultTrackedOffsetY;
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

        private void ScanForTargets()
        {
            availableTargets.Clear();

            float radius = maximumLockOnDistance;
            Vector3 center = playerHeadRef.position;

            Collider[] colliders = Physics.OverlapSphere(center, radius);

            foreach (var collider in colliders)
            {
                if (!collider.TryGetComponent<LockOnRef>(out var enemy))
                {
                    continue;
                }

                if (!InScreen(enemy) || IsViewBlocked(enemy.transform))
                {
                    continue;
                }

                if (enemy.CanLockOn())
                {
                    availableTargets.Add(enemy);
                }
            }
        }

        public void HandleTargetSwitching()
        {
            if (!CanSwitchTarget(out bool lookedLeft, out bool lookedRight))
            {
                return;
            }

            // Reset inputs to avoid drifting when switching
            inputs.look = Vector2.zero;

            if (targetSwitchingCooldown < maxTargetSwitchingCooldown)
            {
                return;
            }

            // Scan for valid targets based on current camera view
            ScanForTargets();
            EvaluateLockTargets();

            if (lookedLeft && leftLockTarget != null)
            {
                SwitchLockOnTarget(leftLockTarget);
                targetSwitchingCooldown = 0f;  // Reset cooldown immediately after switching
            }
            else if (lookedRight && rightLockTarget != null)
            {
                SwitchLockOnTarget(rightLockTarget);
                targetSwitchingCooldown = 0f;  // Reset cooldown immediately after switching
            }
        }

        private bool MouseLookedLeft()
        {
            return mouseLookAccum <= -mouseAccumThreshold && mouseLookTimer <= 0f;
        }

        private bool MouseLookedRight()
        {
            return mouseLookAccum >= mouseAccumThreshold && mouseLookTimer <= 0f;
        }

        private bool CanSwitchTarget(out bool lookedLeft, out bool lookedRight)
        {
            bool mouseLeft = MouseLookedLeft();
            bool mouseRight = MouseLookedRight();

            bool gamepadLeft = Gamepad.current?.rightStick.left.IsActuated() ?? false;
            bool gamepadRight = Gamepad.current?.rightStick.right.IsActuated() ?? false;

            lookedLeft = mouseLeft || gamepadLeft;
            lookedRight = mouseRight || gamepadRight;

            return nearestLockOnTarget != null && (lookedLeft || lookedRight);
        }

        private void EvaluateLockTargets()
        {
            leftLockTarget = null;
            rightLockTarget = null;

            float shortestLeft = Mathf.Infinity;
            float shortestRight = Mathf.Infinity;

            foreach (var target in availableTargets)
            {
                if (target == nearestLockOnTarget)
                {
                    continue; // Don't switch back to the current target
                }

                Vector3 targetDirection = target.transform.position - playerManager.transform.position;
                float distance = Vector3.Distance(target.transform.position, playerManager.transform.position);

                float angle = Vector3.SignedAngle(playerManager.transform.forward, targetDirection, Vector3.up);

                if (angle < 0 && distance < shortestLeft)  // Target is on the left
                {
                    shortestLeft = distance;
                    leftLockTarget = target;
                }
                else if (angle > 0 && distance < shortestRight)  // Target is on the right
                {
                    shortestRight = distance;
                    rightLockTarget = target;
                }
            }
        }


        public void SwitchLockOnTarget(LockOnRef newTarget)
        {
            if (!newTarget.CanLockOn())
            {
                return;
            }

            // Get the direction and distance to the new target
            Vector3 direction = newTarget.transform.position - playerHeadRef.transform.position;
            float distanceToTarget = direction.magnitude;

            // Check if the target is within the lock-on distance
            if (distanceToTarget > maximumLockOnDistanceOnSwitchingTargets)
            {
                return; // If the target is out of range, don't perform any further checks
            }

            // Perform a raycast to check if the target is visible and not blocked by obstacles
            if (Physics.Raycast(playerHeadRef.transform.position, direction.normalized, out RaycastHit hit, distanceToTarget, detectionLayer))
            {
                // Check if the raycast hit the target or something related to it
                if (hit.transform == newTarget.transform || hit.transform.root == newTarget.transform.root)
                {
                    // Target is not blocked, proceed with the switch
                    nearestLockOnTarget = newTarget;

                    // Update camera properties, play sound, and snap the player rotation
                    UpdateCameraProperties(nearestLockOnTarget.transform);
                    soundbank.PlaySound(soundbank.uiLockOnSwitchTarget);

                    SnapPlayerRotationToLockOnTarget();
                }
                else
                {
                    // Target is blocked by something else
                    nearestLockOnTarget = null;
                }
            }
            else
            {
                // No obstacles detected, lock-on is valid
                nearestLockOnTarget = newTarget;

                // Update camera properties, play sound, and snap the player rotation
                UpdateCameraProperties(nearestLockOnTarget.transform);
                soundbank.PlaySound(soundbank.uiLockOnSwitchTarget);

                SnapPlayerRotationToLockOnTarget();
            }

            // After switching, prevent further switches for a short duration
            targetSwitchingCooldown = maxTargetSwitchingCooldown;
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
