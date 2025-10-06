using UnityEngine;

namespace AF
{
    public class RotateTowardsTarget : MonoBehaviour
    {
        [Header("References")]
        public Transform objectToRotate; // the object that rotates
        public Transform target;         // the target to face

        [Header("Settings")]
        public float rotationSpeed = 5f; // degrees per second

        public void Rotate()
        {
            if (objectToRotate == null || target == null)
                return;

            // Get direction but only keep Y axis
            Vector3 direction = target.position - objectToRotate.position;
            direction.y = 0f; // ignore vertical difference

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                objectToRotate.rotation = Quaternion.Slerp(
                    objectToRotate.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
    }
}
