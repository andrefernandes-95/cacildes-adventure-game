using AF.Health;
using UnityEngine;

namespace AF
{

    [CreateAssetMenu(fileName = "Unarmed Weapon", menuName = "Items / Weapons / New Unarmed Weapon", order = 0)]
    public class UnarmedWeapon : ScriptableObject
    {
        public Damage damage;

        [Header("Sounds")]
        public WeaponSound weaponSound;

        [Header("Camera Shake Impact Force")]
        public float hitboxImpactImpulse = 0.2f;
    }
}
