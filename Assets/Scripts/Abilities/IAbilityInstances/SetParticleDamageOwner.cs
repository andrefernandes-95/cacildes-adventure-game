namespace AF
{
    using UnityEngine;

    public class SetParticleDamageOwner : MonoBehaviour, IAbilityInstance
    {
        [SerializeField] OnParticleCollisionManager[] onParticleCollisionManagers;

        public void CastAbility(CharacterBaseManager caster, CharacterBaseManager target)
        {
            foreach (OnParticleCollisionManager onParticleCollisionManager in onParticleCollisionManagers)
            {
                onParticleCollisionManager.damageOwner = caster;
            }
        }
    }
}
