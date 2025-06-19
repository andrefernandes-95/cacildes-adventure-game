namespace AF
{
    using UnityEngine;

    public class SpawnAtCharacterFeet : MonoBehaviour, IAbilityInstance
    {
        [SerializeField] Vector3 upOffset = Vector3.zero;

        public void CastAbility(CharacterBaseManager caster, CharacterBaseManager target)
        {
            transform.position = caster.transform.position + upOffset;
        }
    }
}
