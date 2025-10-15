namespace AF
{
    using System.Collections;
    using UnityEngine;

    public class SpawnMultipleInCharacterFront : MonoBehaviour, IAbilityInstance
    {
        [SerializeField] Vector3 upOffset = Vector3.zero;
        [SerializeField] float forwardOffset = 2f;

        [SerializeField] GameObject[] particlePrefabs;

        [SerializeField] float intervalBetweenParticleSpawns = .5f;

        Vector3 originPosition;
        Vector3 originForward;

        public void CastAbility(CharacterBaseManager caster, CharacterBaseManager target)
        {

            StartCoroutine(SpawnMultiple(caster, target));
        }

        IEnumerator SpawnMultiple(CharacterBaseManager caster, CharacterBaseManager target)
        {
            originPosition = caster.transform.position;
            originForward = caster.transform.forward;

            for (int i = 0; i < particlePrefabs.Length; i++)
            {
                GameObject instance = Instantiate(particlePrefabs[i]);
                instance.SetActive(false);

                OnDamageCollisionAbstractManager[] onParticleCollisionManagers =
                    Utils.CollectComponentsFromGameObject<OnDamageCollisionAbstractManager>(instance);

                foreach (OnDamageCollisionAbstractManager onParticleCollisionManager in onParticleCollisionManagers)
                {
                    onParticleCollisionManager.damageOwner = caster;

                }

                if (target == null)
                {
                    instance.transform.position = originPosition + upOffset + (originForward * forwardOffset);
                }
                else
                {
                    instance.transform.position = target.transform.position;
                }

                originPosition = instance.transform.position;

                instance.SetActive(true);
                yield return new WaitForSeconds(intervalBetweenParticleSpawns);
            }
        }
    }
}
