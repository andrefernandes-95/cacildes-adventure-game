namespace AF
{
    using UnityEngine;

    public class SpawnAtSameRotationAsOwner : MonoBehaviour, IAbilityInstance
    {
        public void CastAbility(CharacterBaseManager caster, CharacterBaseManager target)
        {
            transform.rotation = caster.transform.rotation;
        }
    }
}
