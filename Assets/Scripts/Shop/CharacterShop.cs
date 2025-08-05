using UnityEngine;
using UnityEngine.Events;

namespace AF.Shops
{
    public class CharacterShop : MonoBehaviour
    {
        public Shop shop;

        [Header("Events")]
        public UnityEvent onShopOpen;
        public UnityEvent onShopExit;

        // Scene References
        UIDocumentShopMenu uIDocumentShopMenu;

        public void BuyFromCharacter()
        {
            GetUIDocumentShopMenu()?.BuyFromCharacter(this);
        }

        public void SellToCharacter()
        {
            GetUIDocumentShopMenu()?.SellToCharacter(this);
        }

        UIDocumentShopMenu GetUIDocumentShopMenu()
        {
            if (uIDocumentShopMenu == null)
            {
                uIDocumentShopMenu = FindAnyObjectByType<UIDocumentShopMenu>(FindObjectsInactive.Include);
            }

            return uIDocumentShopMenu;
        }

    }
}
