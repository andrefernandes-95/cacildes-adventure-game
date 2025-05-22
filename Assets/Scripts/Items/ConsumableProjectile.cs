using AF.Health;
using UnityEngine;

namespace AF
{
    [CreateAssetMenu(menuName = "Items / Item / New Consumable Projectile")]
    public class ConsumableProjectile : Item
    {
        public ProjectileType projectileType;
        public Projectile projectile;
    }
}
