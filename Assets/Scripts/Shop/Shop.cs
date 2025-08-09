namespace AF
{
    using System.Collections.Generic;
    using System.Linq;
    using AF.Inventory;
    using AF.Shops;
    using AF.Stats;
    using EditorAttributes;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;

    [CreateAssetMenu(fileName = "Shop", menuName = "Shop / New Shop", order = 0)]
    public class Shop : ScriptableObject
    {

        [System.Serializable]
        public class ShopItem
        {
            public Item item;
            public int stock;

            [Tooltip("Do not show if player owns one in the inventory")]
            public bool dontShowIfPlayerAreadyOwns = false;

            [Tooltip("If true, this item will only appear after a certain quest has been completed")]
            public QuestObjective requiredQuestObjective;

            [HideInInspector] public UnityEvent onItemSold;
        }

        [Header("Info")]
        public Character character;
        public int shopGold;

        [Header("Shop")]
        public List<ShopItem> stock = new();
        public List<ShopItem> itemsBoughtFromPlayer = new();
        public List<ShopItem> uniqueItemsThatNeverRestock = new();
        public List<Item> itemsBought = new();
        public List<Item> itemsBoughtThatNeverRestock = new();

        [Header("Restock Options")]
        public int daysToRestock = 3;

        [Header("Selling Options")]
        public bool canBuyWeapons = false;
        public bool canBuySkills = false;
        public bool canBuyHelmets = false;
        public bool canBuyArmors = false;
        public bool canBuyGauntlets = false;
        public bool canBuyLegwear = false;
        public bool canBuyAccessories = false;
        public bool canBuyBossTokens = false;
        public bool canBuyArrows = false;
        public bool canBuyConsumables = false;
        public bool canBuyCraftingMaterials = false;
        public bool canBuyUpgradeMaterials = false;
        public List<Item> canBuyTheseItems = new();

        [Header("Restock Settings")]
        public bool autoRestock = true;
        public int restockIntervalDays = 3;
        private int lastRestockDay = -1; // Use -1 to force initial restock

        [Header("Item Based Discount Settings")]
        public Item itemInInventoryThatGivesDiscounts;
        [Range(0, 1f)] public float discountForItemInInventory = 0.15f;

        [Header("Quest Based Discount Settings")]
        public QuestObjective questObjectiveThatGivesDiscounts;
        [Range(0, 1f)] public float discountForQuestObjective = 0.15f;



#if UNITY_EDITOR
        private void OnEnable()
        {
            // No need to populate the list; it's serialized directly
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                // Clear the list when exiting play mode
                Clear();
            }
        }
#endif
        public void Clear()
        {
            itemsBought.Clear();
            itemsBoughtFromPlayer.Clear();
            itemsBoughtThatNeverRestock.Clear();
        }

        public void TryRestock(int currentDay)
        {
            if (!autoRestock)
                return;

            if (lastRestockDay == -1 || currentDay - lastRestockDay >= restockIntervalDays)
            {
                Restock();
                lastRestockDay = currentDay;
            }
        }

        public void Restock()
        {
            itemsBought.Clear();
        }

        public List<ShopItem> GetAvailableItemsForSale(PlayerManager playerManager)
        {
            List<ShopItem> availableItemsForSale = new();

            foreach (ShopItem shopItemTemplate in stock)
            {
                // Count items bought that match this item
                int totalBought = itemsBought.Count(item => item.EqualsTo(shopItemTemplate.item));

                // Compute current stock based on allowed restocking
                int currentStock = shopItemTemplate.stock;

                // Subtract total bought from stock
                currentStock -= totalBought;

                if (currentStock <= 0)
                {
                    continue; // Out of stock
                }

                if (!ShouldShowItem(shopItemTemplate, playerManager))
                {
                    continue;
                }

                // All checks passed, add to available items
                ShopItem shopItemForSale = new()
                {
                    item = shopItemTemplate.item,
                    stock = currentStock,
                    dontShowIfPlayerAreadyOwns = shopItemTemplate.dontShowIfPlayerAreadyOwns,
                    requiredQuestObjective = shopItemTemplate.requiredQuestObjective,
                    onItemSold = new UnityEvent()
                };

                shopItemForSale.onItemSold.AddListener(() =>
                {
                    itemsBought.Add(shopItemForSale.item);
                });

                availableItemsForSale.Add(shopItemForSale);
            }

            foreach (ShopItem itemBoughtFromPlayer in itemsBoughtFromPlayer)
            {
                ShopItem shopItemForSale = new()
                {
                    item = itemBoughtFromPlayer.item,
                    stock = 1,
                    dontShowIfPlayerAreadyOwns = false,
                    requiredQuestObjective = null,
                };

                shopItemForSale.onItemSold.AddListener(() =>
                {
                    itemsBoughtFromPlayer.Remove(shopItemForSale);
                });

                availableItemsForSale.Add(shopItemForSale);
            }

            foreach (ShopItem uniqueItemTemplate in uniqueItemsThatNeverRestock)
            {
                if (itemsBoughtThatNeverRestock.Contains(uniqueItemTemplate.item))
                {
                    continue;
                }

                if (!ShouldShowItem(uniqueItemTemplate, playerManager))
                {
                    continue;
                }

                ShopItem shopItemForSale = new()
                {
                    item = uniqueItemTemplate.item,
                    stock = 1,
                    dontShowIfPlayerAreadyOwns = uniqueItemTemplate.dontShowIfPlayerAreadyOwns,
                    requiredQuestObjective = uniqueItemTemplate.requiredQuestObjective,
                    onItemSold = new UnityEvent()
                };

                shopItemForSale.onItemSold.AddListener(() =>
                {
                    itemsBoughtThatNeverRestock.Add(shopItemForSale.item);
                });

                availableItemsForSale.Add(shopItemForSale);
            }

            return availableItemsForSale;
        }

        bool ShouldShowItem(ShopItem item, PlayerManager playerManager)
        {
            if (item.dontShowIfPlayerAreadyOwns &&
                playerManager.playerInventory.inventoryDatabase.HasItem(item.item))
            {
                return false;
            }

            if (item.requiredQuestObjective != null &&
                !item.requiredQuestObjective.questParent.IsObjectiveCompleted(item.requiredQuestObjective))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Evaluates the sellers inventory according to this shop's buying preferences
        /// </summary>
        /// <param name="playerManager"></param>
        /// <returns></returns>
        public List<ShopItem> GetDesirableItemsFromSeller(PlayerManager playerManager)
        {
            List<Item> items = new();

            if (canBuyTheseItems.Count > 0)
            {
                foreach (Item item in canBuyTheseItems)
                {
                    if (playerManager.playerInventory.inventoryDatabase.HasItem(item) && IsItemWithinBudget(item))
                    {
                        items.Add(item);
                    }
                }
            }

            if (canBuyWeapons)
            {
                foreach (Item item in playerManager.playerInventory.GetWeapons())
                {
                    if (IsItemWithinBudget(item))
                    {
                        items.Add(item);
                    }
                }
            }

            if (canBuyArrows)
            {
                foreach (Item item in playerManager.playerInventory.GetArrows())
                {
                    if (IsItemWithinBudget(item))
                    {
                        items.Add(item);
                    }
                }
            }

            if (canBuySkills)
            {
                foreach (Item item in playerManager.playerInventory.GetSpells())
                {
                    if (IsItemWithinBudget(item))
                    {
                        items.Add(item);
                    }
                }
            }

            if (canBuyHelmets)
            {
                foreach (Item item in playerManager.playerInventory.GetHelmets())
                {
                    if (IsItemWithinBudget(item))
                    {
                        items.Add(item);
                    }
                }
            }

            if (canBuyArmors)
            {
                foreach (Item item in playerManager.playerInventory.GetArmors())
                {
                    if (IsItemWithinBudget(item))
                    {
                        items.Add(item);
                    }
                }
            }

            if (canBuyGauntlets)
            {
                foreach (Item item in playerManager.playerInventory.GetGauntlets())
                {
                    if (IsItemWithinBudget(item))
                    {
                        items.Add(item);
                    }
                }
            }

            if (canBuyLegwear)
            {
                foreach (Item item in playerManager.playerInventory.GetLegwears())
                {
                    if (IsItemWithinBudget(item))
                    {
                        items.Add(item);
                    }
                }
            }

            if (canBuyAccessories)
            {
                foreach (Item item in playerManager.playerInventory.GetAccessories())
                {
                    if (IsItemWithinBudget(item))
                    {
                        items.Add(item);
                    }
                }
            }

            if (canBuyConsumables)
            {
                foreach (Item item in playerManager.playerInventory.GetConsumables())
                {
                    if (item is Consumable consumable && consumable.shouldNotRemoveOnUse && !consumable.isBossToken && IsItemWithinBudget(item))
                    {
                        items.Add(item);
                    }
                }
            }

            if (canBuyBossTokens)
            {
                foreach (Item item in playerManager.playerInventory.GetConsumables())
                {
                    if (item is Consumable consumable && consumable.isBossToken && IsItemWithinBudget(item))
                    {
                        items.Add(item);
                    }
                }
            }

            if (canBuyCraftingMaterials)
            {
                foreach (Item item in playerManager.playerInventory.GetCraftingMaterials())
                {
                    if (IsItemWithinBudget(item))
                    {
                        items.Add(item);
                    }
                }
            }

            if (canBuyUpgradeMaterials)
            {
                foreach (Item item in playerManager.playerInventory.GetUpgradeMaterials())
                {
                    if (IsItemWithinBudget(item))
                    {
                        items.Add(item);
                    }
                }
            }

            List<ShopItem> availableItemsForSale = new();

            foreach (Item item in items)
            {
                ShopItem shopItem = new()
                {
                    item = item,
                    stock = 1,
                    dontShowIfPlayerAreadyOwns = false,
                    requiredQuestObjective = null,
                    onItemSold = new UnityEvent()
                };

                shopItem.onItemSold.AddListener(() =>
                {
                    itemsBoughtFromPlayer.Add(shopItem);
                });

                availableItemsForSale.Add(shopItem);
            }

            return availableItemsForSale;
        }

        bool IsItemWithinBudget(Item item) => item.itemValue.value < shopGold;

        public int GetItemEvaluation(Item item, InventoryDatabase inventoryDatabase, StatsBonusController statsBonusController, bool isBuying)
        {
            float discountPercentage = GetDiscounts(inventoryDatabase, statsBonusController, isBuying).Select(x => x.percentage).Sum();

            return ShopUtils.GetItemFinalPrice(item, isBuying, Mathf.Min(1f, discountPercentage));
        }

        public string GetShopDiscountsDescription(InventoryDatabase inventoryDatabase, StatsBonusController statsBonusController, bool isBuying)
        {
            List<string> discounts = GetDiscounts(inventoryDatabase, statsBonusController, isBuying).Select(x => x.label).ToList();

            if (discounts.Count <= 0)
            {
                return "";
            }

            if (Utils.IsPortuguese())
            {
                return $"Descontos: \n {string.Join("\n", discounts)}";
            }

            return $"Discounts: \n {string.Join("\n", discounts)}";
        }

        private List<(string label, float percentage)> GetDiscounts(InventoryDatabase inventoryDatabase, StatsBonusController statsBonusController, bool isBuying)
        {
            List<(string label, float percentage)> discountDescriptions = new();

            if (itemInInventoryThatGivesDiscounts != null && inventoryDatabase.HasItem(itemInInventoryThatGivesDiscounts))
            {
                float discount = discountForItemInInventory;

                string label = Utils.IsPortuguese()
                    ? $"{discount * 100}% desconto por possuir {itemInInventoryThatGivesDiscounts.GetName()} no inventário"
                    : $"{discount * 100}% discount for owning {itemInInventoryThatGivesDiscounts.GetName()} in inventory";

                discountDescriptions.Add((label, discount));
            }

            if (statsBonusController.discountPercentage > 0)
            {
                float discount = statsBonusController.discountPercentage;

                string label = Utils.IsPortuguese()
                    ? $"{discount * 100}% desconto bónus de equipamento atual"
                    : $"{discount * 100}% discount from current equipment";

                discountDescriptions.Add((label, discount));
            }

            if (questObjectiveThatGivesDiscounts != null &&
                questObjectiveThatGivesDiscounts.questParent.IsObjectiveCompleted(questObjectiveThatGivesDiscounts))
            {
                float discount = discountForQuestObjective;

                string label = Utils.IsPortuguese()
                    ? $"{discount * 100}% desconto por completar objetivo: {questObjectiveThatGivesDiscounts.GetDescription()}"
                    : $"{discount * 100}% discount for completing objective: {questObjectiveThatGivesDiscounts.GetDescription()}";

                discountDescriptions.Add((label, discount));
            }

            return discountDescriptions;
        }
    }

}
