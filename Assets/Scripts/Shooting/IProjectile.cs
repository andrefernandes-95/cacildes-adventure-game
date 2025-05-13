using UnityEngine;

namespace AF.Shooting
{
    public interface IProjectile
    {
        public void Shoot(CharacterBaseManager shooter, Vector3 aimForce, ForceMode forceMode);

        public float GetForwardVelocity();

        public float GetUpwardVelocity();

        public ForceMode GetForceMode();

    }
}
