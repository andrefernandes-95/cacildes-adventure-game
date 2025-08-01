using System.Collections.Generic;
using System.Linq;
using AF.Events;
using AF.Inventory;
using AF.Music;
using GameAnalyticsSDK;
using TigerForge;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentCraftScreen : MonoBehaviour
    {
        public enum CraftActivity
        {
            ALCHEMY,
            COOKING,
            BLACKSMITH,
        }

        public CraftActivity craftActivity;

        [Header("UI")]
        public VisualTreeAsset recipeItem;
        public VisualTreeAsset ingredientItem;
        public Sprite alchemyBackgroundImage;
        public Sprite cookingBackgroundImage;
        public Sprite blacksmithBackgroundImage;

        public Sprite goldSprite;

        [Header("SFX")]
        public AudioClip sfxOnEnterMenu;

        [Header("UI Components")]
        public UIDocument uIDocument;
        [HideInInspector] public VisualElement root;
        public UIDocumentBonfireMenu uIDocumentBonfireMenu;
        public UIDocumentPlayerGold uIDocumentPlayerGold;

        [Header("Components")]
        public NotificationManager notificationManager;
        public UIManager uiManager;
        public PlayerManager playerManager;
        public CursorManager cursorManager;
        public BGMManager bgmManager;
        public Soundbank soundbank;

        [HideInInspector] public bool returnToBonfire = false;

        [Header("Databases")]
        public RecipesDatabase recipesDatabase;
        public InventoryDatabase inventoryDatabase;
        public PlayerStatsDatabase playerStatsDatabase;

        // Last scroll position
        int lastScrollElementIndex = -1;

        [Header("Localization")]
        // "Crafting Table"
        public LocalizedString CraftingTable_LocalizedString;

        // "Weapon Upgrades"
        public LocalizedString WeaponUpgrades_LocalizedString;
        // "Return"
        public LocalizedString Return_LocalizedString;
        // "Craft"
        public LocalizedString Craft_LocalizedString;
        // "Cook"
        public LocalizedString Cook_LocalizedString;
        // "Upgrade"
        public LocalizedString Upgrade_LocalizedString;
        // "Next Physical Damage: "
        public LocalizedString NextPhysicalDamage_LocalizedString;
        // "Next Fire Bonus: "
        public LocalizedString NextFireBonus_LocalizedString;
        // "Next Frost Bonus: "
        public LocalizedString NextFrostBonus_LocalizedString;
        // "Next Lightning Bonus: "
        public LocalizedString NextLightningBonus_LocalizedString;
        // "Next Magic Bonus: "
        public LocalizedString NextMagicBonus_LocalizedString;
        // "Next Darkness Bonus: "
        public LocalizedString NextDarknessBonus_LocalizedString;
        // "Next Darkness Bonus: "
        public LocalizedString NextWaterBonus_LocalizedString;
        // "Gold"
        public LocalizedString Gold_LocalizedString;


        private void Awake()
        {
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            this.root = uIDocument.rootVisualElement;

            bgmManager.PlaySound(sfxOnEnterMenu, null);
            cursorManager.ShowCursor();

            DrawUI();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OpenBlacksmithMenu()
        {
            LogAnalytic(AnalyticsUtils.OnUIButtonClick("Blacksmith"));

            this.craftActivity = CraftActivity.BLACKSMITH;
            this.gameObject.SetActive(true);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OpenAlchemyMenu()
        {
            LogAnalytic(AnalyticsUtils.OnUIButtonClick("Alchemy"));

            this.craftActivity = CraftActivity.ALCHEMY;
            this.gameObject.SetActive(true);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnClose()
        {
            if (!this.isActiveAndEnabled)
            {
                return;
            }

            Close();
        }

        public void Close()
        {
            if (returnToBonfire)
            {
                returnToBonfire = false;

                uIDocumentBonfireMenu.gameObject.SetActive(true);
                cursorManager.ShowCursor();
                this.gameObject.SetActive(false);
                return;
            }

            playerManager.playerComponentManager.EnableComponents();
            playerManager.playerComponentManager.EnableCharacterController();

            this.gameObject.SetActive(false);
            cursorManager.HideCursor();
        }

        void ClearPreviews()
        {
            root.Q<VisualElement>("IngredientsListPreview").style.opacity = 0;

            root.Q<Label>("WeaponLevelPreview").text = "";
            root.Q<Label>("WeaponLevelPreview").style.display = DisplayStyle.None;
            root.Q<Label>("PhysicalAttack").style.display = DisplayStyle.None;
            root.Q<Label>("FireAttack").style.display = DisplayStyle.None;
            root.Q<Label>("FrostAttack").style.display = DisplayStyle.None;
            root.Q<Label>("LightningAttack").style.display = DisplayStyle.None;
            root.Q<Label>("MagicAttack").style.display = DisplayStyle.None;
            root.Q<Label>("DarknessAttack").style.display = DisplayStyle.None;
            root.Q<Label>("WaterAttack").style.display = DisplayStyle.None;
        }

        void SetupActivity()
        {
            string targetActivityTitleText = "";
            StyleBackground targetBackground = null;

            if (craftActivity == CraftActivity.ALCHEMY)
            {
                targetActivityTitleText = CraftingTable_LocalizedString.GetLocalizedString();
                targetBackground = new StyleBackground(alchemyBackgroundImage);
            }
            else if (craftActivity == CraftActivity.BLACKSMITH)
            {
                targetActivityTitleText = WeaponUpgrades_LocalizedString.GetLocalizedString();
                targetBackground = new StyleBackground(blacksmithBackgroundImage);
            }
            root.Q<VisualElement>("ImageBack").style.backgroundImage = targetBackground;
            root.Q<Label>("CraftActivityTitle").text = targetActivityTitleText;
        }

        void DrawUI()
        {
            ClearPreviews();

            SetupActivity();

            PopulateScrollView(recipesDatabase.craftingRecipes.ToArray());
        }

        void PopulateScrollView(CraftingRecipe[] ownedCraftingRecipes)
        {
            root.Q<VisualElement>("WeaponNextUpgradeDescription").style.display = DisplayStyle.None;

            var scrollView = this.root.Q<ScrollView>();
            scrollView.Clear();

            Button exitButton = new()
            {
                text = Return_LocalizedString.GetLocalizedString()
            };
            exitButton.AddToClassList("primary-button");
            UIUtils.SetupButton(exitButton, () =>
            {
                Close();
            }, soundbank);

            scrollView.Add(exitButton);


            if (craftActivity == CraftActivity.BLACKSMITH)
            {
                PopulateWeaponsScrollView();
            }
            else
            {
                PopulateCraftingScroll(scrollView, ownedCraftingRecipes);
            }

            if (lastScrollElementIndex == -1)
            {
                scrollView.ScrollTo(exitButton);
                exitButton.Focus();
            }
            else
            {
                Invoke(nameof(GiveFocus), 0f);
            }

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


        public string GetItemDescription(CraftingRecipe recipe)
        {
            if (recipe.resultingItem == null)
            {
                return "";
            }

            string itemDescription = recipe.resultingItem.GetShortDescription()?.Length > 0 ?
                                     recipe.resultingItem.GetShortDescription().Substring(
                                        0, System.Math.Min(60, recipe.resultingItem.GetShortDescription().Length)) : "";
            return itemDescription + (recipe.resultingItem.GetShortDescription()?.Length > 60 ? "..." : "");
        }

        void PopulateCraftingScroll(ScrollView scrollView, CraftingRecipe[] ownedCraftingRecipes)
        {
            if (ownedCraftingRecipes.Length <= 0)
            {
                return;
            }

            int i = 0;
            foreach (var recipe in ownedCraftingRecipes)
            {
                int currentIndex = i;
                var scrollItem = this.recipeItem.CloneTree();

                scrollItem.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(recipe.resultingItem?.sprite);
                scrollItem.Q<Label>("ItemName").text = recipe.resultingItem?.GetName() + $" ({recipe.resultingAmount})";

                scrollItem.Q<Label>("ItemDescription").text = GetItemDescription(recipe);
                scrollItem.Q<Label>("ItemDescription").style.display = DisplayStyle.Flex;

                var craftBtn = scrollItem.Q<Button>("CraftButtonItem");
                var craftLabel = scrollItem.Q<Label>("CraftLabel");
                craftLabel.text = GetCraftLabel();

                craftBtn.style.opacity = CraftingUtils.CanCraftItem(inventoryDatabase, recipe) ? 1f : 0.25f;

                UIUtils.SetupButton(craftBtn,
                () =>
                {
                    lastScrollElementIndex = currentIndex;

                    if (!CraftingUtils.CanCraftItem(inventoryDatabase, recipe))
                    {
                        HandleCraftError(LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Missing ingredients!"));
                        return;
                    }

                    if (ShouldRuinMixture(recipe))
                    {
                        HandleCraftError(LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Crafting failed! Try again..."));
                        return;
                    }

                    HandleCraftSuccess(recipe);

                    DrawUI();
                },
                () =>
                {
                    ShowRequiredIngredients(recipe);
                    scrollView.ScrollTo(craftBtn);
                },
                () =>
                {
                },
                true,
                soundbank);

                scrollView.Add(craftBtn);

                i++;
            }
        }

        void PopulateWeaponsScrollView()
        {
            var scrollView = this.root.Q<ScrollView>();

            int i = 0;
            foreach (Weapon weapon in inventoryDatabase.ownedWeapons.Where(weapon => weapon != null && weapon.canBeUpgraded))
            {
                int currentIndex = i;

                if (ShouldSkipUpgrade(weapon, weapon.level))
                {
                    continue;
                }

                var scrollItem = this.recipeItem.CloneTree();

                scrollItem.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(weapon.sprite);
                scrollItem.Q<Label>("ItemName").text = GetWeaponName(weapon);
                scrollItem.Q<Label>("ItemDescription").style.display = DisplayStyle.None;

                var craftBtn = scrollItem.Q<Button>("CraftButtonItem");
                var craftLabel = scrollItem.Q<Label>("CraftLabel");
                craftLabel.text = GetCraftLabel();

                craftBtn.style.opacity = CraftingUtils.CanImproveItem(playerManager, weapon, playerStatsDatabase.gold) ? 1f : 0.25f;

                UIUtils.SetupButton(craftBtn, () =>
                {
                    lastScrollElementIndex = currentIndex;

                    if (!CraftingUtils.CanImproveItem(playerManager, weapon, playerStatsDatabase.gold))
                    {
                        HandleCraftError(LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Missing ingredients!"));
                        return;
                    }

                    HandleWeaponUpgrade(weapon);

                    DrawUI();
                },
                () =>
                {
                    ShowRequirements(weapon);
                    scrollView.ScrollTo(craftBtn);
                },
                () =>
                {
                },
                true,
                soundbank);

                scrollView.Add(craftBtn);

                i++;
            }
        }

        // Helper methods
        string GetCraftLabel()
        {
            return craftActivity switch
            {
                CraftActivity.ALCHEMY => Craft_LocalizedString.GetLocalizedString(),
                CraftActivity.COOKING => Cook_LocalizedString.GetLocalizedString(),
                CraftActivity.BLACKSMITH => Upgrade_LocalizedString.GetLocalizedString(),
                _ => "",
            };
        }

        void HandleCraftError(string errorMessage)
        {
            soundbank.PlaySound(soundbank.craftError);
            notificationManager.ShowNotification(errorMessage, notificationManager.alchemyLackOfIngredients);
        }

        bool ShouldRuinMixture(CraftingRecipe recipe)
        {
            var ingredientThatCanRuinMixture = recipe.ingredients.FirstOrDefault(x => x.ingredient.chanceToRuinMixture > 0);
            return ingredientThatCanRuinMixture != null && Random.Range(0, 100) < ingredientThatCanRuinMixture.ingredient.chanceToRuinMixture;
        }

        void HandleCraftSuccess(CraftingRecipe recipe)
        {
            if (recipe.resultingItem == null)
            {
                return;
            }

            if (craftActivity == CraftActivity.COOKING)
            {
                playerManager.playerAchievementsManager.achievementForCookingFirstMeal.AwardAchievement();
            }
            else if (craftActivity == CraftActivity.ALCHEMY)
            {
                playerManager.playerAchievementsManager.achievementForBrewingFirstPotion.AwardAchievement();
            }

            LogAnalytic(AnalyticsUtils.OnUIButtonClick("CraftItem"), new() {
                { "item_created", recipe.resultingItem.name }
            });

            soundbank.PlaySound(soundbank.craftSuccess);
            playerManager.playerInventory.AddItem(recipe.resultingItem, recipe.resultingAmount);
            notificationManager.ShowNotification(LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Received") + $" x{recipe.resultingAmount} " + recipe.resultingItem?.GetName(), recipe.resultingItem?.sprite);

            foreach (var ingredient in recipe.ingredients)
            {
                for (int i = 0; i < ingredient.amount; i++)
                {
                    inventoryDatabase.RemoveCraftingMaterial(ingredient.ingredient);
                }
            }
        }


        bool ShouldSkipUpgrade(Weapon wp, int nextLevel)
        {
            return wp.canBeUpgraded == false || wp.upgradeMaterialData == null || nextLevel >= wp.upgradeMaterialData.upgradeMaterials.Count();
        }

        string GetWeaponName(Weapon wp)
        {
            return $"{wp.GetName()} +{wp.level} > {wp.GetName()} +{wp.level + 1}";
        }

        void HandleWeaponUpgrade(Weapon wp)
        {
            playerManager.playerAchievementsManager.achievementForUpgradingFirstWeapon.AwardAchievement();
            soundbank.PlaySound(soundbank.craftSuccess);
            notificationManager.ShowNotification(LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Weapon improved!"), wp.sprite);

            LogAnalytic(AnalyticsUtils.OnUIButtonClick("UpgradeWeapon"), new() {
                { "weapon_upgraded", wp.name }
            });

            CraftingUtils.UpgradeItem(
                wp,
                (goldUsed) => uIDocumentPlayerGold.LoseGold(goldUsed),
                (upgradeMaterialUsed) => playerManager.playerInventory.RemoveItem(upgradeMaterialUsed.Key, upgradeMaterialUsed.Value)
            );

            // when we upgrade the weapon, we need to force the item to be reequiped in order for the cloned weapon to receive the update as well
            foreach (Weapon equippedWeapon in playerManager.equipmentDatabase.weapons)
            {
                if (equippedWeapon != null && equippedWeapon.itemID == wp.itemID)
                {
                    if (playerManager.playerWeaponsManager.currentWeaponInstance != null
                    && playerManager.playerWeaponsManager.currentWeaponInstance.weapon != null &&
                    playerManager.playerWeaponsManager.currentWeaponInstance.weapon.itemID == wp.itemID)
                    {
                        playerManager.playerWeaponsManager.currentWeaponInstance.weapon.level = wp.level;
                    }

                    equippedWeapon.level = wp.level;
                    EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
                }
            }

            foreach (Weapon equippedShield in playerManager.equipmentDatabase.shields)
            {
                if (equippedShield != null && equippedShield.itemID == wp.itemID)
                {
                    if (playerManager.playerWeaponsManager.currentShieldInstance != null
                    && playerManager.playerWeaponsManager.currentShieldInstance.weapon != null &&
                    playerManager.playerWeaponsManager.currentShieldInstance.weapon.itemID == wp.itemID)
                    {
                        playerManager.playerWeaponsManager.currentShieldInstance.weapon.level = wp.level;
                    }

                    equippedShield.level = wp.level;
                    EventManager.EmitEvent(EventMessages.ON_EQUIPMENT_CHANGED);
                }
            }
        }

        void ShowRequiredIngredients(CraftingRecipe recipe)
        {
            root.Q<VisualElement>("ItemInfo").Clear();

            foreach (var ingredient in recipe.ingredients)
            {
                var ingredientItemEntry = ingredientItem.CloneTree();
                ingredientItemEntry.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(ingredient.ingredient.sprite);
                ingredientItemEntry.Q<Label>("Title").text = ingredient.ingredient.GetName();

                var playerOwnedIngredientAmount =
                    CraftingUtils.GetCraftingMaterialAmountInInventory(inventoryDatabase, ingredient.ingredient);

                ingredientItemEntry.Q<Label>("Amount").text = playerOwnedIngredientAmount + " / " + ingredient.amount;
                ingredientItemEntry.Q<Label>("Amount").style.opacity = playerOwnedIngredientAmount >= ingredient.amount ? 1 : 0.25f;

                root.Q<VisualElement>("ItemInfo").Add(ingredientItemEntry);
            }

            root.Q<VisualElement>("IngredientsListPreview").style.opacity = 1;
        }
        void ShowRequirements(Weapon weapon)
        {

            UpgradeMaterialData.UpgradeMaterialEntry upgradeData = weapon.upgradeMaterialData.upgradeMaterials.ElementAtOrDefault(weapon.level);

            if (upgradeData == null)
            {
                return;
            }

            var nextLevel = weapon.level + 1;
            root.Q<VisualElement>("WeaponNextUpgradeDescription").style.display = DisplayStyle.Flex;

            // Weapon preview
            root.Q<Label>("WeaponLevelPreview").text = weapon.GetName() + " +" + nextLevel;
            root.Q<Label>("PhysicalAttack").style.display = DisplayStyle.None;
            root.Q<Label>("FireAttack").style.display = DisplayStyle.None;
            root.Q<Label>("FrostAttack").style.display = DisplayStyle.None;
            root.Q<Label>("LightningAttack").style.display = DisplayStyle.None;
            root.Q<Label>("MagicAttack").style.display = DisplayStyle.None;
            root.Q<Label>("DarknessAttack").style.display = DisplayStyle.None;


            int currentPhysicalAttack = weapon.GetCurrentPhysicalAttackForLevel(weapon.level);
            int nextPhysicalAttack = weapon.GetCurrentPhysicalAttackForLevel(nextLevel);

            int currentFireAttack = weapon.GetFireAttackForLevel(weapon.level);
            int nextFireAttack = weapon.GetFireAttackForLevel(nextLevel);

            int currentFrostAttack = weapon.GetFrostAttackForLevel(weapon.level);
            int nextFrostAttack = weapon.GetFrostAttackForLevel(nextLevel);

            int currentLightningAttack = weapon.GetLightningAttackForLevel(weapon.level);
            int nextLightningAttack = weapon.GetLightningAttackForLevel(nextLevel);

            int currentMagicAttack = weapon.GetMagicAttackForLevel(weapon.level);
            int nextMagicAttack = weapon.GetMagicAttackForLevel(nextLevel);

            int currentDarknessAttack = weapon.GetDarknessAttackForLevel(weapon.level);
            int nextDarknessAttack = weapon.GetDarknessAttackForLevel(nextLevel);

            int currentWaterAttack = weapon.GetWaterAttackForLevel(weapon.level);
            int nextWaterAttack = weapon.GetWaterAttackForLevel(nextLevel);

            if (currentPhysicalAttack != 0)
            {
                root.Q<Label>("PhysicalAttack").style.display = DisplayStyle.Flex;
                root.Q<Label>("PhysicalAttack").text =
                    Utils.IsPortuguese()
                        ? $"Ataque Físico: {currentPhysicalAttack} > {nextPhysicalAttack}"
                        : $"Physical Attack: {currentPhysicalAttack} > {nextPhysicalAttack}";
            }
            if (currentFireAttack != 0)
            {
                root.Q<Label>("FireAttack").style.display = DisplayStyle.Flex;
                root.Q<Label>("FireAttack").text =
                    Utils.IsPortuguese()
                        ? $"Ataque de Fogo: {currentFireAttack} > {nextFireAttack}"
                        : $"Fire Attack: {currentFireAttack} > {nextFireAttack}";
            }
            if (currentFrostAttack != 0)
            {
                root.Q<Label>("FrostAttack").style.display = DisplayStyle.Flex;
                root.Q<Label>("FrostAttack").text =
                    Utils.IsPortuguese()
                        ? $"Ataque de Gelo: {currentFrostAttack} > {nextFrostAttack}"
                        : $"Frost Attack: {currentFrostAttack} > {nextFrostAttack}";
            }
            if (currentLightningAttack != 0)
            {
                root.Q<Label>("LightningAttack").style.display = DisplayStyle.Flex;
                root.Q<Label>("LightningAttack").text =
                    Utils.IsPortuguese()
                        ? $"Ataque Elétrico: {currentLightningAttack} > {nextLightningAttack}"
                        : $"Lightning Attack: {currentLightningAttack} > {nextLightningAttack}";
            }
            if (currentMagicAttack != 0)
            {
                root.Q<Label>("MagicAttack").style.display = DisplayStyle.Flex;
                root.Q<Label>("MagicAttack").text =
                    Utils.IsPortuguese()
                        ? $"Ataque Mágico: {currentMagicAttack} > {nextMagicAttack}"
                        : $"Magic Attack: {currentMagicAttack} > {nextMagicAttack}";
            }
            if (currentDarknessAttack != 0)
            {
                root.Q<Label>("DarknessAttack").style.display = DisplayStyle.Flex;
                root.Q<Label>("DarknessAttack").text =
                    Utils.IsPortuguese()
                        ? $"Ataque das Sombras: {currentDarknessAttack} > {nextDarknessAttack}"
                        : $"Darkness Attack: {currentDarknessAttack} > {nextDarknessAttack}";
            }
            if (currentWaterAttack != 0)
            {
                root.Q<Label>("WaterAttack").style.display = DisplayStyle.Flex;
                root.Q<Label>("WaterAttack").text =
                    Utils.IsPortuguese()
                        ? $"Ataque de Água: {currentWaterAttack} > {nextWaterAttack}"
                        : $"Water Attack: {currentWaterAttack} > {nextWaterAttack}";
            }

            // Requirements

            root.Q<VisualElement>("ItemInfo").Clear();

            UpgradeMaterial upgradeMaterialItem = upgradeData.upgradeMaterial;
            int amountRequiredFoUpgrade = upgradeData.amount;

            var ingredientItemEntry = ingredientItem.CloneTree();
            ingredientItemEntry.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(upgradeMaterialItem.sprite);
            ingredientItemEntry.Q<Label>("Title").text = upgradeMaterialItem.GetName();

            var playerOwnedIngredientAmount = playerManager.characterBaseInventory.GetUpgradeMaterialAmount(upgradeMaterialItem);

            ingredientItemEntry.Q<Label>("Amount").text = playerOwnedIngredientAmount + " / " + amountRequiredFoUpgrade;
            ingredientItemEntry.Q<Label>("Amount").style.opacity =
                playerOwnedIngredientAmount >= amountRequiredFoUpgrade ? 1 : 0.25f;

            root.Q<VisualElement>("ItemInfo").Add(ingredientItemEntry);

            // Add Gold

            var goldItemEntry = ingredientItem.CloneTree();
            goldItemEntry.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(goldSprite);
            goldItemEntry.Q<Label>("Title").text = LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Gold");

            goldItemEntry.Q<Label>("Amount").text = playerStatsDatabase.gold + " / " + upgradeData.goldCostForUpgrade;
            goldItemEntry.Q<Label>("Amount").style.opacity = playerStatsDatabase.gold >= upgradeData.goldCostForUpgrade ? 1 : 0.25f;

            root.Q<VisualElement>("ItemInfo").Add(goldItemEntry);
            root.Q<VisualElement>("IngredientsListPreview").style.opacity = 1;
        }

        void LogAnalytic(string eventName)
        {
            if (!GameAnalytics.Initialized)
            {
                GameAnalytics.Initialize();
            }

            GameAnalytics.NewDesignEvent(eventName);
        }
        void LogAnalytic(string eventName, Dictionary<string, object> values)
        {
            if (!GameAnalytics.Initialized)
            {
                GameAnalytics.Initialize();
            }

            GameAnalytics.NewDesignEvent(eventName, values);
        }
    }
}
