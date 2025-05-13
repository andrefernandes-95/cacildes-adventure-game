using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace AF.Inventory
{
    public class ProjectileSpawner : MonoBehaviour
    {
        public PlayerManager playerManager;
        public LockOnManager lockOnManager;
        Projectile queuedProjectile;

        public SerializedDictionary<Arrow, GameObject> arrowPlaceholderInstances = new();

        private void Awake()
        {
            HideArrowPlaceholders();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="projectile"></param>
        public void SpawnProjectile(Projectile projectile)
        {
            queuedProjectile = projectile;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ThrowProjectile()
        {
            if (queuedProjectile == null)
            {
                return;
            }

            Throw(queuedProjectile.gameObject);
            queuedProjectile = null;
        }

        public void Throw(GameObject projectile)
        {

            if (lockOnManager.nearestLockOnTarget != null)
            {
                var rotation = lockOnManager.nearestLockOnTarget.transform.position - playerManager.transform.position;
                rotation.y = 0;
                playerManager.transform.rotation = Quaternion.LookRotation(rotation);
            }

            GameObject instanceGO = Instantiate(projectile, playerManager.transform.position + playerManager.transform.up, Quaternion.identity);
            Projectile instance = instanceGO.GetComponent<Projectile>();

            if (lockOnManager.nearestLockOnTarget != null)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lockOnManager.nearestLockOnTarget.transform.position - instance.transform.position);
                instance.transform.rotation = targetRotation;
            }
            else
            {
                instance.transform.rotation = playerManager.transform.rotation;
            }

            instance.Shoot(playerManager, instance.GetForwardVelocity() * instance.transform.forward, instance.forceMode);
        }

        public void ShowArrowPlaceholder(Arrow arrow)
        {
            if (arrowPlaceholderInstances.ContainsKey(arrow))
            {
                arrowPlaceholderInstances[arrow].SetActive(true);
            }
        }
        public void HideArrowPlaceholders()
        {
            foreach (KeyValuePair<Arrow, GameObject> keyValuePair in arrowPlaceholderInstances)
            {
                keyValuePair.Value.SetActive(false);
            }
        }
    }
}
