using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace AF
{
    public class CharacterTeleportManager : MonoBehaviour
    {
        [Header("Components")]
        public CharacterManager characterManager;

        [Header("Teleport Options")]
        public List<Transform> teleportPoints = new();

        public UnityEvent onTeleport;

        /// <summary>
        /// UnityEvent
        /// </summary>
        public void TeleportEnemy()
        {
            Transform targetPosition = teleportPoints.Count > 0 ? teleportPoints[Random.Range(0, teleportPoints.Count)] : characterManager.transform;
            characterManager.Teleport(targetPosition.position);

            Vector3 lookRot = targetPosition.position - characterManager.transform.position;
            lookRot.y = 0;
            characterManager.transform.rotation = Quaternion.LookRotation(lookRot);

            onTeleport?.Invoke();
        }

    }
}
