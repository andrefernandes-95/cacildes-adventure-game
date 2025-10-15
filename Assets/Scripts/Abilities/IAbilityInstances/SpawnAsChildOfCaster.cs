namespace AF
{
    using System.Collections;
    using UnityEngine;

    public class SpawnAsChildOfCaster : MonoBehaviour, IAbilityInstance
    {
        [Header("Shield Settings")]
        [SerializeField] GameObject prefab;
        [SerializeField] float duration = 5f;
        [SerializeField] Vector3 offset;

        GameObject activeShield;

        public void CastAbility(CharacterBaseManager caster, CharacterBaseManager target)
        {
            StartCoroutine(SpawnFireShield(caster));
        }

        IEnumerator SpawnFireShield(CharacterBaseManager caster)
        {
            if (prefab == null)
                yield break;

            // Instantiate the shield
            activeShield = Instantiate(prefab, caster.transform.position + offset, caster.transform.rotation);

            // Optionally follow the caster
            activeShield.transform.SetParent(caster.transform, worldPositionStays: true);
            activeShield.SetActive(true);

            // Wait for duration
            yield return new WaitForSeconds(duration);

            // Deactivate & destroy the shield
            if (activeShield != null)
            {
                activeShield.SetActive(false);
                Destroy(activeShield);
                Destroy(this.gameObject);
            }
        }
    }
}
