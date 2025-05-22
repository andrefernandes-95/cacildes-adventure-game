using AF.Events;
using TigerForge;
using UnityEngine;


namespace AF
{
    public class BowAimingHelper : MonoBehaviour
    {
        public Vector3 crossBowPosition;
        public EquipmentDatabase equipmentDatabase;

        Vector3 originalPosition;

        [Header("Projectile Types")]
        [SerializeField] ProjectileType boltProjectileType;

        private void Awake()
        {
            originalPosition = transform.localPosition;

            EventManager.StartListening(EventMessages.ON_EQUIPMENT_CHANGED, Evaluate);

            Evaluate();
        }

        void Evaluate()
        {
            if (equipmentDatabase.GetCurrentWeapon() != null && equipmentDatabase.GetCurrentWeapon().projectileType == boltProjectileType)
            {
                transform.localPosition = crossBowPosition;
            }
            else
            {
                transform.localPosition = originalPosition;
            }
        }

    }
}
