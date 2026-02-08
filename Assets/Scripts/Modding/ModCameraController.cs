using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

namespace AF.ModTools
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class ModCameraController : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 15f;
        public float fastMoveMultiplier = 3f;
        public float mouseSensitivity = 2f;
        [SerializeField] CinemachineVirtualCamera vcam;
        float yaw;
        float pitch;

        public bool isLocked = true;

        [HideInInspector] public UnityEvent<bool> onLockEvent;

        public void EnableCamera()
        {
            vcam.enabled = true;
            gameObject.SetActive(true);
        }

        public void DisableCamera()
        {
            vcam.enabled = false;
            gameObject.SetActive(false);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                isLocked = !isLocked;
                onLockEvent?.Invoke(isLocked);
            }

            if (!isLocked)
            {
                HandleRotation();
                HandleMovement();
            }
        }

        void HandleRotation()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * 100f * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * 100f * Time.deltaTime;

            yaw += mouseX;
            pitch -= mouseY;
            pitch = Mathf.Clamp(pitch, -89f, 89f);

            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }

        void HandleMovement()
        {
            float speed = moveSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
                speed *= fastMoveMultiplier;

            Vector3 direction = new Vector3(
                Input.GetAxisRaw("Horizontal"),
                Input.GetKey(KeyCode.E) ? 1f : Input.GetKey(KeyCode.Q) ? -1f : 0f,
                Input.GetAxisRaw("Vertical")
            );

            Vector3 move =
                transform.forward * direction.z +
                transform.right * direction.x +
                transform.up * direction.y;

            transform.position += move * speed * Time.deltaTime;
        }
    }
}
