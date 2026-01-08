using System.Collections.Generic;
using System.Linq;
using AF.Inventory;
using AF.Ladders;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Settings;

namespace AF
{
    public class PlayerInventory : CharacterBaseInventory
    {
        public Consumable currentConsumedItem;


        [Header("UI Components")]
        public NotificationManager notificationManager;
        public UIDocumentPlayerHUDV2 uIDocumentPlayerHUDV2;
        public UIDocumentPlayerGold uIDocumentPlayerGold;

        [Header("Components")]
        public PlayerManager playerManager;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public InventoryDatabase inventoryDatabase;

        [Header("Flags")]
        public bool isConsumingItem = false;

        [Header("Events")]
        public UnityEvent onResetState;

        [Header("Ashes Edge Case")]
        public bool disableAshesUsage = false;
        public Item ashes;
        public UnityEvent onDisabledAshes;


        public void ResetStates()
        {
            isConsumingItem = false;
            onResetState?.Invoke();
        }

        public new void ReplenishItems()
        {
            inventoryDatabase.ReplenishItems();
            uIDocumentPlayerHUDV2.UpdateEquipment();
        }

        void HandleItemAchievements(Item item)
        {
            if (item is Weapon)
            {
                int numberOfWeapons = inventoryDatabase.GetWeaponsCount();

                if (numberOfWeapons <= 0)
                {
                    playerManager.playerAchievementsManager.achievementOnAcquiringFirstWeapon.AwardAchievement();
                }
                else if (numberOfWeapons == 10)
                {
                    playerManager.playerAchievementsManager.achievementOnAcquiringTenWeapons.AwardAchievement();
                }
            }
            else if (item is Spell)
            {
                int numberOfSpells = inventoryDatabase.GetSpellsCount();

                if (numberOfSpells <= 0)
                {
                    playerManager.playerAchievementsManager.achievementOnAcquiringFirstSpell.AwardAchievement();
                }
            }
        }

        public void AddItem(Item item, int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                switch (item)
                {
                    case Shield shield:
                        AddShield(shield);
                        break;
                    case Weapon weapon:
                        AddWeapon(weapon);
                        break;
                    case Spell spell:
                        AddSpell(spell);
                        break;
                    case Arrow arrow:
                        AddArrow(arrow);
                        break;
                    case Helmet helmet:
                        AddHelmet(helmet);
                        break;
                    case Gauntlet gauntlet:
                        AddGauntlet(gauntlet);
                        break;
                    case Armor armor:
                        AddArmor(armor);
                        break;
                    case Legwear legwear:
                        AddLegwear(legwear);
                        break;
                    case Consumable consumable:
                        AddConsumable(consumable);
                        break;
                    case Accessory accessory:
                        AddAccessory(accessory);
                        break;
                    case UpgradeMaterial upgradeMaterial:
                        AddUpgradeMaterial(upgradeMaterial);
                        break;
                    case CraftingMaterial craftingMaterial:
                        AddCraftingMaterial(craftingMaterial);
                        break;
                    case KeyItem keyItem:
                        AddKeyItem(keyItem);
                        break;
                    default:
                        // Handle other item types if needed
                        break;
                }
            }
        }

        public void RemoveItem(Item item, int quantity)
        {
            inventoryDatabase.RemoveItem(item, quantity);

            uIDocumentPlayerHUDV2.UpdateEquipment();
        }

        bool CanConsumeItem(Consumable consumable)
        {
            if (isConsumingItem)
            {
                return false;
            }

            if (consumable.isRenewable && inventoryDatabase.GetItemAmount(consumable) <= 0)
            {
                notificationManager.ShowNotification(
                    LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Consumable depleted"),
                    notificationManager.notEnoughSpells);

                return false;
            }

            if (playerManager.playerCombatController.isCombatting)
            {
                notificationManager.ShowNotification(
                    LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Can't consume item at this time."),
                    notificationManager.systemError);

                return false;
            }


            if (playerManager.thirdPersonController.isSwimming)
            {

                notificationManager.ShowNotification(
                    LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Can't consume item at this time."),
                    notificationManager.systemError);
                return false;
            }

            if (playerManager.characterPosture.isStunned)
            {
                notificationManager.ShowNotification(
                    LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Can't consume item at this time."),
                    notificationManager.systemError);

                return false;
            }

            if (playerManager.playerDodgeController.isDodging)
            {
                notificationManager.ShowNotification(
                    LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Can't consume item at this time."),
                    notificationManager.systemError);
                return false;
            }

            if (!playerManager.thirdPersonController.Grounded)
            {
                notificationManager.ShowNotification(
                    LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Can't consume item at this time."),
                    notificationManager.systemError);
                return false;
            }

            if (playerManager.climbController.climbState != ClimbState.NONE)
            {
                notificationManager.ShowNotification(
                    LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Can't consume item at this time."),
                    notificationManager.systemError);
                return false;
            }

            if (playerManager.isBusy)
            {
                return false;
            }

            if (playerStatsDatabase.currentHealth <= 0)
            {
                return false;
            }

            if (disableAshesUsage && consumable == ashes)
            {
                notificationManager.ShowNotification(
                    LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Can't consume item at this time."),
                    notificationManager.systemError);
                return false;
            }

            return true;
        }

        public void PrepareItemForConsuming(Consumable consumable)
        {
            if (!CanConsumeItem(consumable))
            {
                return;
            }

            this.currentConsumedItem = consumable;

            if (consumable.shouldHideEquipmentWhenConsuming)
            {
                playerManager.playerWeaponsManager.HideEquipment();
            }

            if (consumable.isBossToken || consumable.canBeConsumedForGold)
            {
                uIDocumentPlayerGold.AddGold((int)consumable.GetValue());
            }

            isConsumingItem = true;
            foreach (StatusEffect statusEffect in currentConsumedItem.statusEffectsWhenConsumed)
            {
                playerManager.statusController.statusEffectInstances.FirstOrDefault(x => x.Key == statusEffect).Value?.onConsumeStart?.Invoke();
            }

            playerManager.playerComponentManager.DisableCharacterController();
            playerManager.playerComponentManager.DisableComponents();
        }

        public void FinishItemConsumption()
        {
            if (currentConsumedItem == null)
            {
                return;
            }

            playerManager.playerComponentManager.EnableCharacterController();
            playerManager.playerComponentManager.EnableComponents();

            if (currentConsumedItem.shouldNotRemoveOnUse == false)
            {
                if (playerManager.statsBonusController.chanceToNotLoseItemUponConsumption && UnityEngine.Random.Range(0f, 1f) > 0.8f)
                {
                    notificationManager.ShowNotification(
                        LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Consumable depleted"),
                        notificationManager.notEnoughSpells);


                    notificationManager.ShowNotification(
                        LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "The item has been preserved for future use.")
                    );
                }
                else
                {
                    playerManager.playerInventory.RemoveItem(currentConsumedItem, 1);
                }
            }

            if (currentConsumedItem.statusesToRemove != null && currentConsumedItem.statusesToRemove.Length > 0)
            {
                foreach (StatusEffect statusEffectToRemove in currentConsumedItem.statusesToRemove)
                {
                    var appliedStatusEffectPair = playerManager.statusController.GetActiveEffects().FirstOrDefault(
                        x => x.Key == statusEffectToRemove);

                    if (!appliedStatusEffectPair.Equals(default(KeyValuePair<StatusEffect, AF.StatusEffects.StatusEffectState>)))
                    {
                        playerManager.statusController.RemoveEffect(appliedStatusEffectPair.Key);
                    }
                }
            }

            foreach (StatusEffect statusEffect in currentConsumedItem.statusEffectsWhenConsumed)
            {
                playerManager.statusController.InflictStatusEffect(statusEffect);
            }

            currentConsumedItem = null;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void AllowAshes()
        {
            disableAshesUsage = false;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void DisableAshes()
        {
            onDisabledAshes?.Invoke();
            disableAshesUsage = true;
        }

        public override List<Weapon> GetWeapons()
        {
            return inventoryDatabase.ownedWeapons;
        }

        public override List<Shield> GetShields()
        {
            return inventoryDatabase.ownedWeapons.Where(weapon => weapon is Shield).OfType<Shield>().ToList();
        }

        public override List<Arrow> GetArrows()
        {
            return inventoryDatabase.ownedArrows;
        }

        public override List<Spell> GetSpells()
        {
            return inventoryDatabase.ownedSpells;
        }

        public override List<Accessory> GetAccessories()
        {
            return inventoryDatabase.ownedAccessories;
        }

        public override List<Consumable> GetConsumables()
        {
            return inventoryDatabase.ownedConsumables;
        }

        public override List<Helmet> GetHelmets()
        {
            return inventoryDatabase.ownedHelmets;
        }

        public override List<Armor> GetArmors()
        {
            return inventoryDatabase.ownedArmors;
        }

        public override List<Gauntlet> GetGauntlets()
        {
            return inventoryDatabase.ownedGauntlets;
        }

        public override List<Legwear> GetLegwears()
        {
            return inventoryDatabase.ownedLegwears;
        }

        public override List<CraftingMaterial> GetCraftingMaterials()
        {
            return inventoryDatabase.ownedCraftingMaterials;
        }

        public override List<UpgradeMaterial> GetUpgradeMaterials()
        {
            return inventoryDatabase.ownedUpgradeMaterials;
        }

        public override List<KeyItem> GetKeyItems()
        {
            return inventoryDatabase.ownedKeyItems;
        }

        public override Weapon AddWeapon(Weapon weapon)
        {
            Weapon clone = Instantiate(weapon);
            clone.itemID = GenerateItemId();
            clone.level = 0;
            inventoryDatabase.ownedWeapons.Add(clone);

            AnalyticsUtils.OnItemFound(playerManager, clone, "weapon");
            return clone;
        }

        public override Shield AddShield(Shield shield)
        {
            Shield clone = Instantiate(shield);
            clone.itemID = GenerateItemId();
            clone.level = 0;
            inventoryDatabase.ownedWeapons.Add(clone);
            AnalyticsUtils.OnItemFound(playerManager, clone, "shield");
            return clone;
        }

        public override Helmet AddHelmet(Helmet helmet)
        {
            Helmet clone = Instantiate(helmet);
            clone.itemID = GenerateItemId();
            clone.level = 0;
            inventoryDatabase.ownedHelmets.Add(clone);
            return clone;
        }

        public override Armor AddArmor(Armor armor)
        {
            Armor clone = Instantiate(armor);
            clone.itemID = GenerateItemId();
            clone.level = 0;
            inventoryDatabase.ownedArmors.Add(clone);
            AnalyticsUtils.OnItemFound(playerManager, clone, "armor");
            return clone;
        }

        public override Gauntlet AddGauntlet(Gauntlet gauntlet)
        {
            Gauntlet clone = Instantiate(gauntlet);
            clone.itemID = GenerateItemId();
            clone.level = 0;
            inventoryDatabase.ownedGauntlets.Add(clone);
            return clone;
        }

        public override Legwear AddLegwear(Legwear legwear)
        {
            Legwear clone = Instantiate(legwear);
            clone.itemID = GenerateItemId();
            clone.level = 0;
            inventoryDatabase.ownedLegwears.Add(clone);
            return clone;
        }

        public override Accessory AddAccessory(Accessory accessory)
        {
            Accessory clone = Instantiate(accessory);
            clone.itemID = GenerateItemId();
            clone.level = 0;
            inventoryDatabase.ownedAccessories.Add(clone);
            AnalyticsUtils.OnItemFound(playerManager, clone, "accessory");
            return clone;
        }

        public override Arrow AddArrow(Arrow arrow)
        {
            Arrow clone = Instantiate(arrow);
            clone.itemID = GenerateItemId();
            inventoryDatabase.ownedArrows.Add(clone);
            return clone;
        }

        public override Spell AddSpell(Spell spell)
        {
            Spell clone = Instantiate(spell);
            clone.itemID = GenerateItemId();
            clone.level = 0;
            inventoryDatabase.ownedSpells.Add(clone);
            AnalyticsUtils.OnItemFound(playerManager, clone, "spell");
            return clone;
        }

        public override Consumable AddConsumable(Consumable consumable)
        {
            Consumable clone = Instantiate(consumable);
            clone.itemID = GenerateItemId();
            inventoryDatabase.ownedConsumables.Add(clone);
            AnalyticsUtils.OnItemFound(playerManager, clone, "consumable");
            return clone;
        }

        public override UpgradeMaterial AddUpgradeMaterial(UpgradeMaterial upgradeMaterial)
        {
            UpgradeMaterial clone = Instantiate(upgradeMaterial);
            clone.itemID = GenerateItemId();
            inventoryDatabase.ownedUpgradeMaterials.Add(clone);
            AnalyticsUtils.OnItemFound(playerManager, clone, "upgradeMaterial");
            return clone;
        }

        public override CraftingMaterial AddCraftingMaterial(CraftingMaterial craftingMaterial)
        {
            CraftingMaterial clone = Instantiate(craftingMaterial);
            clone.itemID = GenerateItemId();
            inventoryDatabase.ownedCraftingMaterials.Add(clone);
            AnalyticsUtils.OnItemFound(playerManager, clone, "craftingMaterial");
            return clone;
        }

        public override KeyItem AddKeyItem(KeyItem keyItem)
        {
            KeyItem clone = Instantiate(keyItem);
            clone.itemID = GenerateItemId();
            inventoryDatabase.ownedKeyItems.Add(clone);
            AnalyticsUtils.OnItemFound(playerManager, clone, "item");
            return clone;
        }

        public override void RemoveConsumable(Consumable consumable)
        {
            if (consumable.shouldNotRemoveOnUse == false)
            {
                if (playerManager.statsBonusController.chanceToNotLoseItemUponConsumption && UnityEngine.Random.Range(0f, 1f) > 0.8f)
                {
                    notificationManager.ShowNotification(
                        LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Consumable depleted"),
                        notificationManager.notEnoughSpells);


                    notificationManager.ShowNotification(
                        LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "The item has been preserved for future use.")
                    );
                }
                else if (consumable.isRenewable)
                {
                    inventoryDatabase.idsOfUsedConsumables.Add(consumable.itemID);
                }
                else
                {
                    inventoryDatabase.RemoveConsumable(consumable);
                }
            }
        }

        public override int GetConsumableAmount(Consumable consumable)
        {
            return inventoryDatabase.GetConsumableAmount(consumable);
        }

        List<Consumable> GetAllConsumableInstances(Consumable consumable)
        {
            return inventoryDatabase.ownedConsumables.Where(c => c.EqualsTo(consumable)).OfType<Consumable>().ToList();
        }

        /// <summary>
        /// Gets all consumables that are available, meaning that for some like orange juice, are still awaiting their usage
        /// </summary>
        /// <param name="consumable"></param>
        /// <returns></returns>
        public List<Consumable> GetAvailableConsumables(Consumable consumable)
        {
            List<Consumable> allConsumableInstances = playerManager.playerInventory.GetAllConsumableInstances(consumable);

            List<Consumable> allAvailableConsumables = new();
            foreach (Consumable c in allConsumableInstances)
            {
                if (!playerManager.playerInventory.inventoryDatabase.idsOfUsedConsumables.Contains(c.itemID))
                {
                    allAvailableConsumables.Add(c);
                }
            }

            return allAvailableConsumables;
        }

    }
}
