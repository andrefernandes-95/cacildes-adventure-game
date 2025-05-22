using AF.Health;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Item / New Arrow")]
    public class Arrow : ConsumableProjectile
    {
        public ArrowProjectile arrowProjectile;

        public Damage damage;
        public bool loseUponFiring = true;

        [Header("Options")]
        [Tooltip("Applicable to arrows like blood soaked arrow which causes bleed build up to the user upon firing")]
        public StatusEffectEntry[] statusEffectsInflictedUponShootingArrow;
    }
}
