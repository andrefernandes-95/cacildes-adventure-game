using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AF.Inventory;
using AYellowpaper.SerializedCollections;
using EditorAttributes;
using UnityEngine;
using UnityEngine.Localization;

namespace AF
{
    public class CharacterLoot : MonoBehaviour
    {

        [Header("Loot and Experience")]

        [SerializedDictionary("Item", "Chance To Get")]
        public SerializedDictionary<Item, LootItemAmount> lootTable;
        [SerializeField] bool useLootTable = false;

        [Header("Gold")]
        public int baseGold = 100;
        [HelpBox("If true, will ignore the combatant base gold. Useful for bosses and specific enemies")]
        [SerializeField] bool useBaseGold = false;
        public int bonusGold = 0;

        [Header("Options")]
        [SerializeField] bool lootFromInventory = false;

        [Header("Components")]
        public CharacterManager lootOwner;

        // Scene References
        private PlayerManager playerManager;
        private NotificationManager notificationManager;

        private Soundbank soundbank;

        private UIDocumentPlayerGold uIDocumentPlayerGold;
        private UIDocumentReceivedItemPrompt uIDocumentReceivedItemPrompt;

        [Header("Localization")]
        // "Found: "
        public LocalizedString found;


        public void GiveLoot()
        {
            StartCoroutine(GiveLoot_Coroutine());
        }

        public IEnumerator GiveLoot_Coroutine()
        {
            int goldBasis = !useBaseGold && lootOwner.combatant != null && lootOwner.combatant.characterGold != null
                ? lootOwner.combatant.characterGold.gold
                : baseGold;

            if (lootOwner.characterShop.shop != null)
            {
                // Add gold from shop
                goldBasis += lootOwner.characterShop.shop.shopGold;
            }

            int goldToReceive = goldBasis + bonusGold;

            yield return new WaitForSeconds(1f);

            if (GetPlayerManager().statsBonusController != null)
            {
                var additionalCoinPercentage = GetPlayerManager().statsBonusController.additionalCoinPercentage;

                if (additionalCoinPercentage != 0)
                {
                    var additionalCoin = (int)Mathf.Ceil(goldToReceive * additionalCoinPercentage / 100);

                    goldToReceive += additionalCoin;
                }

                if (GetPlayerManager().statsBonusController.ShouldDoubleCoinFromFallenEnemy())
                {
                    goldToReceive *= 2;
                }
            }

            GetLoot();

            yield return new WaitForSeconds(0.2f);

            GetUIDocumentPlayerGold().AddGold(goldToReceive);
        }

        private void GetLoot()
        {
            var itemsToReceive = new SerializedDictionary<Item, int>();

            bool hasPlayedFanfare = false;

            if (useLootTable)
            {
                foreach (var dropCurrency in lootTable)
                {
                    if (dropCurrency.Value.ignoreIfPlayerOwns && playerManager.playerInventory.inventoryDatabase.HasItem(dropCurrency.Key))
                    {
                        continue;
                    }

                    float calc_dropChance = Random.Range(0, 100f);

                    if (calc_dropChance <= dropCurrency.Value.chanceToGet)
                    {
                        if (hasPlayedFanfare == false)
                        {
                            GetSoundbank().PlaySound(GetSoundbank().uiItemReceived);
                            hasPlayedFanfare = true;
                        }

                        itemsToReceive.Add(dropCurrency.Key, dropCurrency.Value.amount);
                    }
                }
            }
            else if (lootOwner.combatant != null && lootOwner.combatant.loot.Count > 0)
            {
                foreach (LootableItem lootableItem in lootOwner.combatant.loot)
                {
                    float calc_dropChance = Random.Range(0, 1f);

                    if (calc_dropChance <= lootableItem.item.dropRateOnEnemies)
                    {
                        if (hasPlayedFanfare == false)
                        {
                            GetSoundbank().PlaySound(GetSoundbank().uiItemReceived);
                            hasPlayedFanfare = true;
                        }

                        itemsToReceive.Add(lootableItem.item, lootableItem.amount);
                    }
                }
            }


            bool isBoss = lootOwner.characterBossController.IsBoss();

            List<UIDocumentReceivedItemPrompt.ItemsReceived> itemsToDisplay = new();
            List<UIDocumentReceivedItemPrompt.ItemsReceived> cardsToDisplay = new();

            if (lootFromInventory)
            {
                AddInventoryToLootTab(itemsToReceive);
            }

            foreach (var item in itemsToReceive)
            {
                GetPlayerManager().playerInventory.AddItem(item.Key, item.Value);

                if (isBoss && GetUIDocumentReceivedItemPrompt() != null)
                {
                    itemsToDisplay
                        .Add(new()
                        {
                            itemName = item.Key.GetName(),
                            quantity = 1,
                            sprite = item.Key.sprite,
                            isCard = false
                        });
                }
                else
                {
                    GetNotificationManager().ShowNotification(found.GetLocalizedString() + " " + item.Key.GetName(), item.Key.sprite);
                }
            }

            if (isBoss && itemsToDisplay.Count > 0)
            {
                GetUIDocumentReceivedItemPrompt().gameObject.SetActive(true);
                var combinedList = new List<UIDocumentReceivedItemPrompt.ItemsReceived>();
                combinedList.AddRange(itemsToDisplay);
                combinedList.AddRange(cardsToDisplay);
                GetUIDocumentReceivedItemPrompt().DisplayItemsReceived(combinedList);
            }
            else if (cardsToDisplay.Count > 0)
            {
                StartCoroutine(DisplayCardsWithDelay(cardsToDisplay));
            }

            // Distribute to companions
            GiveLootToCompanions();
        }

        void AddInventoryToLootTab(Dictionary<Item, int> items)
        {
            foreach (Weapon wp in lootOwner.characterBaseInventory.GetWeapons())
            {
                if (!items.ContainsKey(wp) && wp.ShouldDrop())
                {
                    items.Add(wp, 1);
                }
            }
            foreach (Shield shield in lootOwner.characterBaseInventory.GetShields())
            {
                if (!items.ContainsKey(shield) && shield.ShouldDrop())
                {
                    items.Add(shield, 1);
                }
            }
            foreach (Arrow arrow in lootOwner.characterBaseInventory.GetArrows())
            {
                if (!items.ContainsKey(arrow) && arrow.ShouldDrop())
                {
                    items.Add(arrow, Random.Range(3, 9));
                }
            }
            foreach (Spell skill in lootOwner.characterBaseInventory.GetSpells())
            {
                if (!items.ContainsKey(skill) && skill.ShouldDrop())
                {
                    items.Add(skill, 1);
                }
            }
            foreach (Helmet helmet in lootOwner.characterBaseInventory.GetHelmets())
            {
                if (!items.ContainsKey(helmet) && helmet.ShouldDrop())
                {
                    items.Add(helmet, 1);
                }
            }
            foreach (Armor armor in lootOwner.characterBaseInventory.GetArmors())
            {
                if (!items.ContainsKey(armor) && armor.ShouldDrop())
                {
                    items.Add(armor, 1);
                }
            }
            foreach (Legwear legwear in lootOwner.characterBaseInventory.GetLegwears())
            {
                if (!items.ContainsKey(legwear) && legwear.ShouldDrop())
                {
                    items.Add(legwear, 1);
                }
            }
            foreach (Accessory accessory in lootOwner.characterBaseInventory.GetAccessories())
            {
                if (!items.ContainsKey(accessory) && accessory.ShouldDrop())
                {
                    items.Add(accessory, 1);
                }
            }
            foreach (Consumable consumable in lootOwner.characterBaseInventory.GetConsumables())
            {
                if (!items.ContainsKey(consumable) && consumable.ShouldDrop())
                {
                    items.Add(consumable, 1);
                }
            }
            foreach (UpgradeMaterial upgradeMaterial in lootOwner.characterBaseInventory.GetUpgradeMaterials())
            {
                if (!items.ContainsKey(upgradeMaterial) && upgradeMaterial.ShouldDrop())
                {
                    items.Add(upgradeMaterial, 1);
                }
            }
            foreach (CraftingMaterial craftingMaterial in lootOwner.characterBaseInventory.GetCraftingMaterials())
            {
                if (!items.ContainsKey(craftingMaterial) && craftingMaterial.ShouldDrop())
                {
                    items.Add(craftingMaterial, 1);
                }
            }
        }

        IEnumerator DisplayCardsWithDelay(List<UIDocumentReceivedItemPrompt.ItemsReceived> cardsToDisplay)
        {
            yield return new WaitForSeconds(0.5f);
            ShowCards(cardsToDisplay);
        }

        void ShowCards(List<UIDocumentReceivedItemPrompt.ItemsReceived> cardsToDisplay)
        {
            if (cardsToDisplay.Count <= 0)
            {
                return;
            }

            // If an enemy is actively fighting, dont show card
            if (Utils.HasEnemyFighting())
            {
                cardsToDisplay.ForEach(card =>
                {
                    GetNotificationManager().ShowNotification(found.GetLocalizedString() + " " + card.itemName, card.sprite);
                });
            }
            else
            {
                GetUIDocumentReceivedItemPrompt().gameObject.SetActive(true);
                GetUIDocumentReceivedItemPrompt().DisplayItemsReceived(cardsToDisplay);
            }
        }

        PlayerManager GetPlayerManager()
        {
            if (playerManager == null)
            {
                playerManager = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include);
            }

            return playerManager;
        }

        Soundbank GetSoundbank()
        {
            if (soundbank == null)
            {
                soundbank = FindAnyObjectByType<Soundbank>(FindObjectsInactive.Include);
            }

            return soundbank;
        }

        NotificationManager GetNotificationManager()
        {
            if (notificationManager == null)
            {
                notificationManager = FindAnyObjectByType<NotificationManager>(FindObjectsInactive.Include);
            }

            return notificationManager;
        }

        UIDocumentPlayerGold GetUIDocumentPlayerGold()
        {
            if (uIDocumentPlayerGold == null)
            {
                uIDocumentPlayerGold = FindAnyObjectByType<UIDocumentPlayerGold>(FindObjectsInactive.Include);
            }

            return uIDocumentPlayerGold;
        }

        UIDocumentReceivedItemPrompt GetUIDocumentReceivedItemPrompt()
        {
            if (uIDocumentReceivedItemPrompt == null)
            {
                uIDocumentReceivedItemPrompt = FindAnyObjectByType<UIDocumentReceivedItemPrompt>(FindObjectsInactive.Include);
            }

            return uIDocumentReceivedItemPrompt;
        }

        void GiveLootToCompanions()
        {
            var itemsToReceive = new SerializedDictionary<Item, int>();

            if (lootFromInventory)
            {
                AddInventoryToLootTab(itemsToReceive);
            }

            if (useLootTable)
            {
                foreach (var dropCurrency in lootTable)
                {
                    float calc_dropChance = Random.Range(0, 100f);

                    if (calc_dropChance <= dropCurrency.Value.chanceToGet)
                    {
                        itemsToReceive.Add(dropCurrency.Key, dropCurrency.Value.amount);
                    }
                }
            }
            else if (lootOwner.combatant != null && lootOwner.combatant.loot.Count > 0)
            {
                foreach (LootableItem lootableItem in lootOwner.combatant.loot)
                {
                    float calc_dropChance = Random.Range(0, 1f);

                    if (calc_dropChance <= lootableItem.item.dropRateOnEnemies)
                    {
                        itemsToReceive.Add(lootableItem.item, lootableItem.amount);
                    }
                }
            }

            playerManager.companionsSceneManager.GiveLootToCompanions(itemsToReceive.Select(x => x.Key).ToList());
        }
    }
}
