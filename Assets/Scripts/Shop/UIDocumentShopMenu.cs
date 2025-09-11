using System.Collections.Generic;
using System.Linq;
using AF.Inventory;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;
using static AF.Shop;

namespace AF.Shops
{
    public class UIDocumentShopMenu : MonoBehaviour
    {
        [Header("Player Settings")]
        public Character playerCharacter;

        [Header("Components")]
        public CursorManager cursorManager;
        public PlayerManager playerManager;
        public Soundbank soundbank;

        [Header("UI Components")]
        public UIDocument uiDocument;
        public VisualTreeAsset buySellButton;
        public UIDocumentPlayerGold uIDocumentPlayerGold;
        public NotificationManager notificationManager;
        VisualElement root;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public InventoryDatabase inventoryDatabase;

        Label buyerName, buyerGold, sellerName, sellerGold;
        VisualElement buyerIcon, sellerIcon;

        // Item Preview
        VisualElement itemPreview;
        IMGUIContainer itemPreviewItemIcon;
        Label itemPreviewItemDescription;

        // Last scroll position
        int lastScrollElementIndex = -1;

        // Memoizations
        CharacterShop currentCharacterShop;

        public LocalizedString buyFor_LocalizedString; // Buy for
        public LocalizedString sellFor_LocalizedString; // Sell for
        public LocalizedString coins_LocalizedString; // Coins
        public LocalizedString offer_LocalizedString; // Offer
        public LocalizedString exitShopLabel_LocalizedString; // Exit Shop

        private void Start()
        {
            this.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            currentCharacterShop = null;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnClose()
        {
            if (this.isActiveAndEnabled)
            {
                ExitShop();
            }
        }

        void SetupRefs()
        {
            this.root = uiDocument.rootVisualElement;

            var buyer = root.Q<VisualElement>("Buyer");
            buyerName = buyer.Q<Label>("Name");
            buyerGold = buyer.Q<Label>("Gold");
            buyerIcon = buyer.Q<VisualElement>("BuyerIcon");

            var seller = root.Q<VisualElement>("Seller");
            sellerName = seller.Q<Label>("Name");
            sellerGold = seller.Q<Label>("Gold");
            sellerIcon = seller.Q<VisualElement>("SellerIcon");

            this.itemPreview = root.Q<VisualElement>("ItemPreview");
            this.itemPreviewItemIcon = itemPreview.Q<IMGUIContainer>("ItemIcon");
            this.itemPreviewItemDescription = itemPreview.Q<Label>("Description");
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="characterShop"></param>
        public void BuyFromCharacter(CharacterShop characterShop)
        {
            currentCharacterShop = characterShop;

            characterShop?.onShopOpen?.Invoke();
            gameObject.SetActive(true);
            playerManager.playerComponentManager.DisableComponents();

            Invoke(nameof(DisplayCursor), 0f);
            DrawBuyMenu();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="characterShop"></param>
        public void SellToCharacter(CharacterShop characterShop)
        {
            currentCharacterShop = characterShop;

            characterShop?.onShopOpen?.Invoke();
            gameObject.SetActive(true);
            playerManager.playerComponentManager.DisableComponents();

            Invoke(nameof(DisplayCursor), 0f);
            DrawSellMenu();
        }

        void DisplayCursor()
        {
            cursorManager.ShowCursor();
        }

        private void OnEnable()
        {
            SetupRefs();
            DisplayCursor();
        }

        Button SetupExitButton(ScrollView scrollView)
        {
            Button exitButton = new() { text = exitShopLabel_LocalizedString.GetLocalizedString() };
            exitButton.AddToClassList("primary-button");

            UIUtils.SetupButton(exitButton, () =>
            {
                ExitShop();
            }
            ,
            () =>
            {
                root.Q<ScrollView>().ScrollTo(exitButton);
                exitButton.Focus();
            },
            () =>
            {

            },
            true, soundbank);

            scrollView.Add(exitButton);

            return exitButton;
        }

        void ExitShop()
        {
            currentCharacterShop?.onShopExit?.Invoke();
            playerManager.playerComponentManager.EnableComponents();
            this.gameObject.SetActive(false);
            cursorManager.HideCursor();
        }

        void SetupCharactersGUI(Shop shop, bool playerIsBuying)
        {
            if (playerIsBuying)
            {
                buyerName.text = playerManager.gameSettings.playerName;
                buyerGold.text = playerStatsDatabase.gold.ToString() + " " + coins_LocalizedString.GetLocalizedString();
                buyerIcon.style.backgroundImage = new StyleBackground(playerManager.GetPlayerPortrait());

                sellerName.text = shop.character.GetCharacterName();
                sellerGold.text = shop.shopGold.ToString();
                sellerIcon.style.backgroundImage = new StyleBackground(shop.character.avatar);
            }
            else
            {
                buyerName.text = shop.character.GetCharacterName();
                buyerGold.text = shop.shopGold.ToString() + " " + coins_LocalizedString.GetLocalizedString();
                buyerIcon.style.backgroundImage = new StyleBackground(shop.character.avatar);

                sellerName.text = playerManager.gameSettings.playerName;
                sellerGold.text = playerStatsDatabase.gold.ToString();
                sellerIcon.style.backgroundImage = new StyleBackground(playerManager.GetPlayerPortrait());
            }

            root.Q<Label>("AppliedDiscountsLabel").text = shop.GetShopDiscountsDescription(inventoryDatabase, playerManager.statsBonusController, playerIsBuying);
        }

        void DrawBuyMenu()
        {
            Shop shop = currentCharacterShop.shop;
            SetupCharactersGUI(shop, true);
            DrawItemsList(shop.GetAvailableItemsForSale(playerManager), true);
        }

        void DrawSellMenu()
        {
            SetupCharactersGUI(currentCharacterShop.shop, false);

            DrawItemsList(currentCharacterShop.shop.GetDesirableItemsFromSeller(playerManager), false);
        }

        void DrawBuySellLabel(Button buySellButton, Item item, bool isPlayerBuying)
        {
            buySellButton.Q<VisualElement>("RequiredItemSprite").style.display = DisplayStyle.None;
            buySellButton.Q<VisualElement>("OriginalValueContainer").style.display = DisplayStyle.None;

            Label buySellLabel = buySellButton.Q<Label>("BuySellLabel");
            Label currentValueLabel = buySellButton.Q<Label>("CurrentValue");

            if (ShopUtils.ItemRequiresCoinsToBeBought(item))
            {
                int finalValue = currentCharacterShop.shop.GetItemEvaluation(item, inventoryDatabase, playerManager.statsBonusController, isPlayerBuying);

                if (item.GetValue() != finalValue)
                {
                    buySellButton.Q<Label>("OriginalValue").text = item.GetValue().ToString();
                    buySellButton.Q<VisualElement>("OriginalValueContainer").style.display = DisplayStyle.Flex;
                }

                buySellLabel.text = (isPlayerBuying ? buyFor_LocalizedString.GetLocalizedString() : sellFor_LocalizedString.GetLocalizedString()) + " ";
                currentValueLabel.text = finalValue + " " + coins_LocalizedString.GetLocalizedString();
            }
            else if (item.tradingItemRequirements != null && item.tradingItemRequirements.Count > 0)
            {
                buySellButton.Q<VisualElement>("RequiredItemSprite").style.backgroundImage = new StyleBackground(item.tradingItemRequirements.ElementAt(0).Key.sprite);
                buySellButton.Q<VisualElement>("RequiredItemSprite").style.display = DisplayStyle.Flex;
                buySellLabel.text = offer_LocalizedString.GetLocalizedString() + " ";
                currentValueLabel.text = item.tradingItemRequirements.ElementAt(0).Key.GetName() + "";
            }
        }

        bool PlayerCanBuy(Shop characterShop, Item item)
        {
            if (item.tradingItemRequirements != null && item.tradingItemRequirements.Count > 0)
            {
                bool canBuy = true;

                foreach (var requiredTradingItem in item.tradingItemRequirements)
                {
                    List<Item> requiredItems = inventoryDatabase.ownedConsumables.Where(consumable => consumable != null && consumable.EqualsTo(requiredTradingItem.Key)).OfType<Item>().ToList();

                    if (requiredItems.Count < requiredTradingItem.Value)
                    {
                        canBuy = false;
                        break;
                    }
                }

                return canBuy;
            }

            int finalValue = characterShop.GetItemEvaluation(item, inventoryDatabase, playerManager.statsBonusController, true);

            return playerStatsDatabase.gold >= finalValue;
        }

        bool ShopCanBuy(Shop shop, Item item)
        {
            int finalValue = shop.GetItemEvaluation(item, inventoryDatabase, playerManager.statsBonusController, false);

            return shop.shopGold >= finalValue;
        }

        void DrawItemsList(List<ShopItem> itemsToSell, bool playerIsBuying)
        {
            root.Q<ScrollView>().Clear();

            HideItemPreview();

            Button exitButton = SetupExitButton(root.Q<ScrollView>());

            int i = 0;
            foreach (var shopitem in itemsToSell)
            {
                int currentIndex = i;
                Item item = shopitem.item;

                VisualElement cloneButton = buySellButton.CloneTree();
                Button buySellItemButton = cloneButton.Q<Button>("BuySellButton");

                cloneButton.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(item.sprite);

                string itemName = item.GetName();
                if (item is UpgradableItem upgradableItem)
                {
                    itemName += $" +{upgradableItem.level}";
                }
                itemName += $" ({shopitem.stock})";

                cloneButton.Q<Label>("ItemName").text = itemName;

                bool playerCanBuy = playerIsBuying && PlayerCanBuy(currentCharacterShop.shop, item);
                bool playerCanSell = !playerIsBuying && ShopCanBuy(currentCharacterShop.shop, item);

                buySellItemButton.style.opacity = (playerIsBuying && playerCanBuy || !playerIsBuying && playerCanSell) ? 1 : 0.5f;

                DrawBuySellLabel(buySellItemButton, item, playerIsBuying);

                buySellItemButton.RegisterCallback<PointerEnterEvent>((ev) =>
                {
                    RenderItemPreview(item);
                });

                buySellItemButton.RegisterCallback<PointerOutEvent>((ev) =>
                {
                    HideItemPreview();
                });

                UIUtils.SetupButton(buySellItemButton,
                () =>
                {
                    lastScrollElementIndex = currentIndex;

                    if (playerCanBuy)
                    {
                        BuyItem(shopitem);
                    }
                    else if (playerCanSell)
                    {
                        SellItem(shopitem);
                    }
                },
                () =>
                {
                    RenderItemPreview(item);

                    root.Q<ScrollView>().ScrollTo(buySellItemButton);
                    buySellItemButton.Focus();
                },
                () =>
                {
                    HideItemPreview();
                },
                false, soundbank);

                root.Q<ScrollView>().Add(buySellItemButton);

                i++;
            }

            if (lastScrollElementIndex == -1)
            {
                exitButton.Focus();
            }

            Invoke(nameof(GiveFocus), 0f);
        }

        void GiveFocus()
        {
            UIUtils.ScrollToLastPosition(
                lastScrollElementIndex,
                root.Q<ScrollView>(),
                () =>
                {
                    lastScrollElementIndex = -1;
                }
            );
        }

        void BuyItem(ShopItem shopItem)
        {
            Item item = shopItem.item;

            int price = currentCharacterShop.shop.GetItemEvaluation(
                item,
                inventoryDatabase,
                playerManager.statsBonusController,
                true);

            if (!PlayerCanBuy(currentCharacterShop.shop, item))
            {
                return;
            }

            ShopUtils.BuyItem(
                item,
                (goldLost) =>
                {

                    uIDocumentPlayerGold.LoseGold(price);
                    currentCharacterShop.shop.shopGold += price;
                },
                (onItemsTraded) =>
                {
                    foreach (var tradedItem in onItemsTraded)
                    {

                        playerManager.playerInventory.RemoveConsumable(tradedItem.Key as Consumable);
                    }
                },
                (receivedItem) =>
                {
                    // Give item to player
                    playerManager.playerInventory.AddItem(item, 1);
                    soundbank.PlaySound(soundbank.uiItemReceived);

                    notificationManager.ShowNotification(
                        LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Bought") + " " + item.GetName() + "", item.sprite);

                    shopItem.onItemSold?.Invoke();

                    DrawBuyMenu();
                }
            );
        }

        void SellItem(ShopItem shopItem)
        {
            Item item = shopItem.item;

            int price = currentCharacterShop.shop.GetItemEvaluation(
                        item,
                        inventoryDatabase,
                        playerManager.statsBonusController,
                        false);


            uIDocumentPlayerGold.AddGold(price);
            currentCharacterShop.shop.shopGold -= price;

            // Remove item from player
            playerManager.playerInventory.RemoveItem(item, 1);

            shopItem.onItemSold?.Invoke();

            soundbank.PlaySound(soundbank.uiItemReceived);
            notificationManager.ShowNotification(
                LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Sold") + " " + item.GetName() + "", item.sprite);

            DrawSellMenu();
        }


        void RenderItemPreview(Item item)
        {
            if (item == null)
            {
                return;
            }

            itemPreviewItemIcon.style.backgroundImage = new StyleBackground(item.sprite);
            itemPreviewItemDescription.text = item.GetDescription();
            itemPreview.style.opacity = 1;
        }

        void HideItemPreview()
        {
            itemPreview.style.opacity = 0;
        }
    }
}
