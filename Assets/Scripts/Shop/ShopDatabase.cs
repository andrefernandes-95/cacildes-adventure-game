using AF.Events;
using TigerForge;
using UnityEngine;

namespace AF
{
    public class ShopDatabase : MonoBehaviour
    {
        public Shop[] allShops;

        [Header("References")]
        [SerializeField] GameSession gameSession;

        private void Start()
        {
            EventManager.StartListening(EventMessages.ON_HOUR_CHANGED, OnHourChanged);

            // On load, try to evaluate if any stock needs restock
            OnHourChanged();
        }

        void OnHourChanged()
        {
            foreach (Shop shop in allShops)
            {
                shop.TryRestock(gameSession.daysPassed);
            }
        }
    }
}
