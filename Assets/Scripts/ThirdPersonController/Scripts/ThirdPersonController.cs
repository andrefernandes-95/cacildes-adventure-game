﻿using UnityEngine;
using UnityEngine.InputSystem;
using AF.Ladders;
using AF.Health;
using UnityEngine.Events;

namespace AF
{
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Stamina")]
        public int jumpStaminaCost = 15;
        public float runStaminaCost = .05f;

        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float WalkSpeed = 2.0f;

        public float RunSpeed = 7.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 10f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float LockOnSpeed = 6.5f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        float DefaultJumpHeight;
        public float JumpHeightBonus = 0f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;
        float DefaultGravity;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;
        public bool rotateWithCamera = false;

        public float jumpAttackVelocity = -5f;

        // cinemachine
        public float _cinemachineTargetYaw = 0f;
        private float _cinemachineTargetPitch = 0f;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        public float verticalVelocityBonus = 0f;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDIsMoving;
        private int _animIDGrounded;
        private int _animIDJump;
        [HideInInspector] public int _animIDFreeFall;
        private int _animIDMotionSpeed;

        [Header("Components")]
        public PlayerManager playerManager;
        private PlayerInput _playerInput;
        [HideInInspector] public StarterAssetsInputs _input;
        public UIManager uIManager;

        public GameObject _mainCamera;
        public UIDocumentReceivedItemPrompt uIDocumentReceivedItemPrompt;

        public MenuManager menuManager;

        public LockOnManager lockOnManager;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private float maxEnableCooldown = 0.25f;
        private float enableCooldown = Mathf.Infinity;

        float defaultFieldOfView;

        public Cinemachine.CinemachineVirtualCamera virtualCamera;

        [Header("Sprint FOV")]
        public float sprintFieldOfViewSpeedBonus = 10f;
        public float sprintFieldOfViewSpeedTransition = 2f;

        [Header("Fall Damage")]
        bool trackFallDamage = true;
        public float minimumFallHeightToTakeDamage = 5f;
        public float damageMultiplierPerMeter = 25f;
        float fallBeganHeight;
        bool PreviousGrounded = true;
        public Vector3 lastGroundedPosition;

        public AudioSource jumpAndDodgeAudiosource;

        public bool canMove = true;

        public bool canRotateCharacter = true;

        [Header("Sliding Options")]
        public bool isSliding = false;
        public bool Sliding
        {
            get
            {
                return isSliding;
            }

            set
            {
                playerManager.animator.SetBool("IsSliding", value);

                isSliding = value;
            }
        }
        public float minSlidingRotation = 0f;
        public float maxSlidingRotation = 160f;
        public float slidingSpeed = 10f;

        [Header("Swimming Options")]
        public bool isSwimming = false;
        public Transform water;
        public float waterOffsetY = 0.5f;
        public float staminaDrainedPerSecondWhenSwimming = 10f;
        public float damageCausedBySwimmingWithoutStamina = 5f;
        public float swimDamageCooldownTimer = 0f;
        public float maxSwimDamageCooldownTimer = 1f;
        public GameObject swimmingWaterRippleFx;
        public UnityEvent onSwimEvent;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;

        private void OnEnable()
        {
            this.enableCooldown = 0f;
        }

        private void Awake()
        {
            defaultFieldOfView = virtualCamera.m_Lens.FieldOfView;
            DefaultGravity = Gravity;
            DefaultJumpHeight = JumpHeight;

            if (isSliding)
            {
                playerManager.animator.SetBool("IsSliding", true);
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = playerManager.animator;

            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        public bool CanControlPlayer()
        {
            return menuManager.isMenuOpen == false;
        }

        private void Update()
        {
            if (enableCooldown < maxEnableCooldown)
            {
                enableCooldown += Time.deltaTime;
            }

            bool isClimbing = playerManager.climbController.climbState != ClimbState.NONE;

            if (isClimbing == false)
            {
                if (water != null)
                {
                    HandleSwimLogic();
                }

                JumpAndGravity();
                GroundedCheck();

                if (CanControlPlayer())
                {
                    Move();
                }
            }
            else
            {
                Climb();
            }

            Rotate();
            CameraRotation();

        }


        void HandleSwimLogic()
        {
            bool wasSwimming = isSwimming;
            isSwimming = playerManager.transform.position.y - waterOffsetY <= water?.transform?.position.y;
            playerManager.animator.SetBool("IsSwimming", isSwimming);

            if (wasSwimming == false && isSwimming)
            {
                playerManager.ResetStates();

                swimDamageCooldownTimer = 0f;
                playerManager.animator.SetTrigger("TriggerSwim");
                swimmingWaterRippleFx.SetActive(true);
            }
            else if (wasSwimming && isSwimming == false)
            {
                swimmingWaterRippleFx.SetActive(false);
            }

            if (isSwimming)
            {
                canRotateCharacter = true;

                swimDamageCooldownTimer += Time.deltaTime;

                playerManager.staminaStatManager.DecreaseStamina(staminaDrainedPerSecondWhenSwimming * Time.deltaTime);

                /*if (playerManager.staminaStatManager.GetCurrentStaminaPercentage() <= 0)
                {
                    if (swimDamageCooldownTimer > maxSwimDamageCooldownTimer)
                    {
                        swimDamageCooldownTimer = 0;
                        playerManager.health.TakeDamage(damageCausedBySwimmingWithoutStamina);
                    }
                }*/
            }
        }

        public void OnSwimAnimationEvent()
        {
            onSwimEvent?.Invoke();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDIsMoving = Animator.StringToHash("IsMoving");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        public bool GetTrackFallDamage()
        {
            return trackFallDamage;
        }

        public void SetTrackFallDamage(bool value)
        {
            fallBeganHeight = playerManager.characterController.transform.position.y;
            this.trackFallDamage = value;
        }

        float GetCurrentFallHeight()
        {
            float fallEndHeight = playerManager.characterController.transform.position.y;
            return Mathf.Abs(fallBeganHeight - fallEndHeight);
        }

        bool ShouldTakeFallDamage()
        {
            if (isSliding)
            {
                return false;
            }

            return GetCurrentFallHeight() > minimumFallHeightToTakeDamage && trackFallDamage;
        }

        private void GroundedCheck()
        {
            Grounded = Physics.CheckSphere(
                new(
                    playerManager.transform.position.x,
                    playerManager.characterController.transform.position.y - GroundedOffset,
                    playerManager.characterController.transform.position.z),
                GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore) || playerManager.characterController.isGrounded;

            if (Grounded)
            {
                lastGroundedPosition = playerManager.transform.position;
            }

            if (PreviousGrounded == true && Grounded == false)
            {
                fallBeganHeight = playerManager.characterController.transform.position.y;
            }
            else if (PreviousGrounded == false && Grounded == true)
            {
                // Takes fall damage?
                if (ShouldTakeFallDamage())
                {
                    playerManager.damageReceiver.TakeDamage(new Damage()
                    {
                        physical = (int)(GetCurrentFallHeight() * damageMultiplierPerMeter),
                        fire = 0,
                        frost = 0,
                        magic = 0,
                        lightning = 0,
                        darkness = 0,
                        water = 0,
                        postureDamage = 0,
                        poiseDamage = 0,
                        weaponAttackType = 0f,
                        statusEffects = null,
                        pushForce = 0f,
                    });
                }
            }

            PreviousGrounded = Grounded;

            // update animator if using character
            if (_hasAnimator)
            {
                playerManager.animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        public void RotateWithCamera()
        {
            Vector3 targetDir = Camera.main.transform.forward;
            targetDir.y = 0;
            targetDir.Normalize();

            if (targetDir == Vector3.zero)
                targetDir = playerManager.transform.forward;

            float rs = 12;

            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation = Quaternion.Slerp(playerManager.transform.rotation, tr, rs * Time.deltaTime);

            playerManager.transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = 1.0f;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Climb()
        {
            float targetSpeed = WalkSpeed;

            if (_input.move == Vector2.zero)
            {
                targetSpeed = 0.0f;
            }

            _speed = targetSpeed;


            _animationBlend = Mathf.Lerp(_animationBlend, _input.sprint ? 1.5f : 1f, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f)
            {
                _animationBlend = 0f;
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            targetDirection.x = 0;
            targetDirection.z = 0;

            float direction = _input.move.y;

            if (direction > 0)
            {
                _verticalVelocity = _speed * Time.deltaTime;
            }
            else if (direction < 0)
            {
                _verticalVelocity = _speed * Time.deltaTime * -1;
            }
            else
            {
                _verticalVelocity = 0f;
            }

            playerManager.climbController.Climb(_speed * Time.deltaTime * direction);

            // move the player
            if (playerManager.characterController.enabled)
            {
                playerManager.characterController.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                                 new Vector3(0.0f, _verticalVelocity + verticalVelocityBonus, 0.0f) * Time.deltaTime);
            }

            // update animator if using character
            if (_hasAnimator && CanControlPlayer())
            {
                playerManager.animator.SetFloat(_animIDSpeed, _animationBlend);
                playerManager.animator.SetBool(_animIDIsMoving, _animationBlend > 0);
            }
            else
            {
                playerManager.animator.SetFloat(_animIDSpeed, 0f);
                playerManager.animator.SetBool(_animIDIsMoving, false);
            }
        }

        private void Rotate()
        {
            if (!CanControlPlayer())
            {
                return;
            }

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving


            if (rotateWithCamera)
            {
                RotateWithCamera();
            }
            else if (_input.move != Vector2.zero && canRotateCharacter == true)
            {

                if (
                    lockOnManager.nearestLockOnTarget != null
                    && lockOnManager.isLockedOn
                    && playerManager.dodgeController.isDodging == false)
                {

                    Vector3 targetRot = lockOnManager.nearestLockOnTarget.transform.position - playerManager.characterController.transform.position;
                    targetRot.y = 0;
                    var t = Quaternion.LookRotation(targetRot);

                    playerManager.characterController.transform.rotation = Quaternion.Lerp(transform.rotation, t, 100f * Time.deltaTime);
                }
                else
                {
                    _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                      _mainCamera.transform.eulerAngles.y;
                    float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                        RotationSmoothTime);

                    if (isSliding)
                    {
                        rotation = Mathf.Clamp(rotation, minSlidingRotation, maxSlidingRotation);
                    }

                    // rotate to face input direction relative to camera position
                    playerManager.characterController.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }
            }
        }

        public bool IsSprinting()
        {
            return _input.sprint && playerStatsDatabase.currentStamina > 1f && _input.move != Vector2.zero;
        }

        private void Move()
        {
            if (!playerManager.characterController.enabled)
            {
                return;
            }

            // set target speed based on move speed, sprint speed and if sprint is pressed
            bool isSprinting = IsSprinting();
            float targetSpeed = isSprinting ? SprintSpeed : (_input.toggleWalk ? WalkSpeed : RunSpeed);

            if (playerManager.playerBlockController.isBlocking || playerManager.playerShootingManager.IsAimingBowOrCrossbow())
            {
                targetSpeed = WalkSpeed;
            }


            if (lockOnManager.isLockedOn)
            {
                targetSpeed = LockOnSpeed;

                // Give more speed to gamepad left stick when strafing 
                if (Gamepad.current != null && Gamepad.current.leftStick.IsActuated())
                {
                    targetSpeed *= 1.25f;
                }

                if (isSprinting)
                {
                    targetSpeed *= 1.25f;
                }
            }

            var weightSpeed = playerManager.statsBonusController.weightPenalty > 0 ? playerManager.statsBonusController.weightPenalty : 0;

            targetSpeed -= weightSpeed;

            if (_input.sprint)
            {
                playerManager.staminaStatManager.DecreaseStamina(runStaminaCost * Time.deltaTime);
            }

            if (isSprinting)
            {
                virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, defaultFieldOfView + sprintFieldOfViewSpeedBonus, sprintFieldOfViewSpeedTransition * Time.deltaTime);
            }
            else
            {
                virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView, defaultFieldOfView, sprintFieldOfViewSpeedTransition * Time.deltaTime);
            }

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (
                playerManager.climbController.climbState == ClimbState.ENTERING
                || playerManager.climbController.climbState == ClimbState.EXITING
                || playerManager.playerCombatController.isCombatting
            )
            {
                _speed = 0f;
            }
            else
            {
                // DARK SOULS
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            if (playerManager.climbController.climbState == ClimbState.CLIMBING)
            {
                targetDirection.x = 0;
                targetDirection.z = 0;

                float direction = _input.move.y;

                if (direction > 0)
                {
                    _verticalVelocity = _speed * Time.deltaTime;
                }
                else if (direction < 0)
                {
                    _verticalVelocity = _speed * Time.deltaTime * -1;
                }
                else
                {
                    _verticalVelocity = 0f;
                }

                _animIDMotionSpeed = (int)(_speed * 2f);
                playerManager.climbController.Climb(_speed * Time.deltaTime * direction);
            }

            if (playerManager.dodgeController.isDodging || playerManager.IsBusy())
            {
                targetDirection = Vector3.zero;
            }

            if (isSliding)
            {
                playerManager.characterController.Move(transform.forward * slidingSpeed * Time.deltaTime +
                                 new Vector3(0.0f, _verticalVelocity + verticalVelocityBonus, 0.0f) * Time.deltaTime);
            }
            else if ((
                // If is locked on
                lockOnManager.nearestLockOnTarget != null && lockOnManager.isLockedOn
                // Or aiming with camera for bow or spell casting
                || rotateWithCamera
                ) && playerManager.isBusy == false)
            {
                float lockOnSpeed = _input.move.x != 0 && _input.move.y != 0 ? _speed : _speed * 1.5f;

                Vector3 targetPos = (transform.forward * (lockOnSpeed) * _input.move.y + playerManager.characterController.transform.right * (lockOnSpeed) * _input.move.x);
                targetPos.y = isSwimming ? 0 : _verticalVelocity + verticalVelocityBonus;

                playerManager.characterController.Move(targetPos * Time.deltaTime);
            }
            else if (playerManager.characterController.enabled)
            {
                if (_speed > 0)
                {
                    _speed += GetMovementSpeedBonus();
                }

                float finalVerticalVelocity = _verticalVelocity + verticalVelocityBonus;
                if (isSwimming)
                {
                    finalVerticalVelocity = 0f;
                }

                playerManager.characterController.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                                 new Vector3(0.0f, finalVerticalVelocity, 0.0f) * Time.deltaTime);
            }

            // update animator if using character
            if (_hasAnimator)
            {
                playerManager.animator.SetFloat(_animIDSpeed, _animationBlend);
                playerManager.animator.SetBool(_animIDIsMoving, _input.move.magnitude > 0);

                // Get movement penalties
                var jumpWeightSpeed = playerManager.statsBonusController.weightPenalty > 0 ? playerManager.statsBonusController.weightPenalty : 0;

                playerManager.animator.SetFloat(_animIDMotionSpeed, inputMagnitude - jumpWeightSpeed);
            }
        }

        public void StopMovement()
        {
            // update animator if using character
            if (_hasAnimator)
            {
                playerManager.animator.SetFloat(_animIDSpeed, 0f);
                playerManager.animator.SetFloat(_animIDMotionSpeed, 0f);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    playerManager.animator.SetBool(_animIDJump, false);
                    playerManager.animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump
                    && _jumpTimeoutDelta <= 0.0f
                    && enableCooldown >= maxEnableCooldown
                    && uIDocumentReceivedItemPrompt.isActiveAndEnabled == false
                    && CanControlPlayer()
                    && uIManager.IsShowingGUI() == false
                )
                {
                    if (CanJump())
                    {
                        _input.jump = false;


                        // the square root of H * -2 * G = how much velocity needed to reach desired height
                        _verticalVelocity = Mathf.Sqrt((GetCurrentJumpHeight()) * -2f * Gravity);

                        var weightSpeed = playerManager.statsBonusController.weightPenalty > 0 ? playerManager.statsBonusController.weightPenalty : 0;

                        _verticalVelocity -= weightSpeed;


                        if (isSliding)
                        {

                            playerManager.characterController.Move(transform.forward * (5f * Time.deltaTime));
                        }

                        // update animator if using character
                        if (_hasAnimator)
                        {
                            playerManager.staminaStatManager.DecreaseStamina(jumpStaminaCost);

                            playerManager.animator.Play("JumpStart");
                        }
                    }
                    else
                    {
                        _input.jump = false;
                    }
                }
                else
                {
                    _input.jump = false;
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator && !isSwimming)
                    {
                        playerManager.animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            if (!playerManager.playerCombatController.isJumpAttacking)
            {
                // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
                if (_verticalVelocity < _terminalVelocity)
                {
                    var jumpAttackVelocityFinal = jumpAttackVelocity;
                    if (equipmentDatabase.GetCurrentWeapon() != null)
                    {
                        jumpAttackVelocityFinal = equipmentDatabase.GetCurrentWeapon().jumpAttackVelocity;
                    }

                    _verticalVelocity += (Gravity * Time.deltaTime) + (playerManager.playerCombatController.isJumpAttacking ? jumpAttackVelocityFinal : 0f);
                }
            }
            /*else if ((equipmentDatabase.GetCurrentWeapon() == null) || (equipmentDatabase.GetCurrentWeapon() != null && equipmentDatabase.GetCurrentWeapon().stopInAir == true))
            {
                _verticalVelocity = 0f;
            }*/
            else
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        bool CanJump()
        {
            if (playerManager.IsBusy())
            {
                return false;
            }

            return playerManager.staminaStatManager.HasEnoughStaminaForAction(jumpStaminaCost) &&
                        playerManager.dodgeController.isDodging == false &&
                        playerManager.playerCombatController.isCombatting == false
                        && canMove;
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="value"></param>
        public void SetCanRotateCharacter(bool value)
        {
            this.canRotateCharacter = value;
        }

        /// <summary>
        ///  Unity Event
        /// </summary>
        /// <param name="value"></param>
        public void SetGravity(float value)
        {
            this.Gravity = value;
        }

        public void ResetGravityToDefaultValue()
        {
            this.Gravity = DefaultGravity;
        }

        public void SetJumpHeight(float value)
        {
            this.JumpHeight = value;
        }

        public void ResetJumpHeight()
        {
            this.JumpHeight = DefaultJumpHeight;
        }

        public void SetLockCameraPosition(bool value)
        {
            this.LockCameraPosition = value;
        }


        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="value"></param>
        public void SetSlidingSpeed(float value)
        {
            slidingSpeed = value;
        }

        public void SetIsSliding(bool value)
        {
            Sliding = value;
        }

        int GetCurrentJumpHeight()
        {
            float JumpWeightBonus = 0;
            float _JumpHeightBonus = JumpHeightBonus;
            if (playerManager.equipmentGraphicsHandler.IsLightWeight())
            {
                JumpWeightBonus = .5f;
            }
            else if (playerManager.equipmentGraphicsHandler.IsHeavyWeight())
            {
                JumpWeightBonus = -1;
            }

            if (playerManager.statsBonusController.GetCurrentDexterity() > 1)
            {
                // DEX 1: 1
                // DEX 10: 1.7
                // DEX 25: 2.23
                // DEX 50: 2.65
                // DEX 100: 3.16
                _JumpHeightBonus += Mathf.Pow(playerManager.statsBonusController.GetCurrentDexterity(), .15f) / 2;
            }

            return (int)(JumpHeight + _JumpHeightBonus + JumpWeightBonus);
        }

        int GetMovementSpeedBonus()
        {
            int movementSpeedBonus = playerManager.statsBonusController.movementSpeedBonus;

            if (playerManager.statsBonusController.GetCurrentDexterity() > 1)
            {
                movementSpeedBonus += (int)Mathf.Pow(playerManager.statsBonusController.GetCurrentDexterity(), .115f);
            }

            return movementSpeedBonus;
        }

    }
}
