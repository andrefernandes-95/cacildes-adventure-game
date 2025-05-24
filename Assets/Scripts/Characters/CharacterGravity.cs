using System.Collections;
using UnityEngine;

namespace AF
{
    public class CharacterGravity : MonoBehaviour
    {
        public CharacterManager characterManager;
        public bool ignoreGravity = false;

        [SerializeField] float _verticalVelocity = 0f;
        private float _terminalVelocity = 53.0f;

        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        [Tooltip("The height the player can jump")]
        public float JumpHeight = 3f;
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

        public bool shouldJumpToTarget = false;

        [SerializeField]
        float jumpTowardsTargetSpeed = 25f;

        Transform jumpTarget;

        Coroutine ResetJumpTargetCoroutine;

        void Start()
        {
            if (ignoreGravity)
            {
                this.gameObject.SetActive(false);
            }
        }

        private void JumpAndGravity()
        {
            if (characterManager.characterController.isGrounded)
            {
                _fallTimeoutDelta = FallTimeout;

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (shouldJumpToTarget && _jumpTimeoutDelta <= 0.0f && characterManager.targetManager.currentTarget != null)
                {
                    shouldJumpToTarget = false;

                    jumpTarget = characterManager.targetManager.currentTarget.transform;
                    float heightDifference = jumpTarget.position.y - characterManager.transform.position.y;
                    float calculatedJumpHeight = Mathf.Max(JumpHeight, heightDifference + JumpHeightBonus);
                    _verticalVelocity = Mathf.Sqrt(calculatedJumpHeight * -2f * Gravity);

                    characterManager.animator.Play("JumpStart");
                }
            }

            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
            }

            _verticalVelocity += Gravity * Time.deltaTime;
        }

        private void Update()
        {
            JumpAndGravity();

            characterManager.animator.SetBool("isGrounded", characterManager.characterController.isGrounded);

            if (!characterManager.characterController.enabled ||
                (characterManager.characterController.isGrounded && characterManager.agent.enabled))
            {
                return;
            }

            Vector3 movement = Vector3.up * _verticalVelocity;

            if (jumpTarget != null)
            {
                Vector3 horizontalDirection = jumpTarget.position - characterManager.transform.position;
                horizontalDirection.y = 0; // ignore vertical component for horizontal movement
                characterManager.transform.rotation = Quaternion.LookRotation(horizontalDirection);

                horizontalDirection = horizontalDirection.normalized;
                movement += horizontalDirection * jumpTowardsTargetSpeed;
            }

            if (jumpTarget != null && _verticalVelocity <= 0)
            {
                jumpTarget = null;
            }

            characterManager.characterController.Move(movement * Time.deltaTime);
        }
    }
}
