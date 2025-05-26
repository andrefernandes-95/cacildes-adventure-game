using System.Collections;
using UnityEngine;

namespace AF
{
    public class ExplodingBarrelManager : MonoBehaviour
    {
        [SerializeField] GameObject explosionPrefab;
        [SerializeField] float spawnDelay = 5f;
        [SerializeField] GenericTrigger genericTrigger;

        Coroutine SpawnExplosionCoroutine;

        private void Awake()
        {
        }

        public void TriggerExplosion()
        {
            if (SpawnExplosionCoroutine != null)
            {
                StopCoroutine(SpawnExplosionCoroutine);
            }

            StartCoroutine(SpawnExplosion());
        }


        IEnumerator SpawnExplosion()
        {
            genericTrigger.DisableCapturable();

            explosionPrefab.gameObject.SetActive(false);

            GameObject explosionInstance = Instantiate(explosionPrefab, explosionPrefab.transform.position, Quaternion.identity);
            explosionInstance.SetActive(true);

            if (explosionInstance.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
            {
                rigidbody.isKinematic = false; // Ensure the explosion does not fall due to gravity
            }

            yield return new WaitForSeconds(spawnDelay);

            Destroy(explosionInstance);
            genericTrigger.TurnCapturable();

            explosionPrefab.gameObject.SetActive(true);
        }
    }
}
