namespace AF
{
    using UnityEngine;

    public interface IAbilityInstance
    {
        public void CastAbility(CharacterBaseManager caster, CharacterBaseManager target);
    }
}