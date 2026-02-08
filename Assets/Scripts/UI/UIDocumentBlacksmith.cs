using System;
using System.Collections.Generic;
using System.Linq;
using AF.Health;
using AF.Inventory;
using AF.Music;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentBlacksmith : MonoBehaviour
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
        public Sprite spellSmithingBackgroundImage;
        public Sprite goldSprite;

        [Header("SFX")]
        public AudioClip sfxOnEnterMenu;
        public float sfxOnEnterMenuVolume = .5f;

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

        public bool isUpgradingSpells = false;

        [Header("Databases")]
        public RecipesDatabase recipesDatabase;
        public InventoryDatabase inventoryDatabase;
        public PlayerStatsDatabase playerStatsDatabase;

        // Last scroll position
        int lastScrollElementIndex = -1;

        VisualElement ItemInfoPreview;
        Label ItemNamePreview;
        Label ItemDescriptionPreview;
        VisualElement ItemSprite;
        VisualElement StatsChangedContainer;
        VisualElement RequirementsPreview;
        VisualElement AttackDifferencesContainer, DefenseDifferencesContainer, ShieldDifferencesContainer;

        Label PhysicalAttack, FireAttack, FrostAttack, WaterAttack, MagicAttack, LightningAttack, DarknessAttack;
        Label PhysicalDefense, FireDefense, FrostDefense, WaterDefense, MagicDefense, LightningDefense, DarknessDefense;
        Label BlockAbsorption;

        ToggleButtonGroup filterButtonGroup;
        Button filterAll;
        Button filterByWeapons;
        Button filterByArmors;

        public enum FilterType
        {
            ALL,
            WEAPONS,
            ARMORS,
        }

        FilterType filterType = FilterType.ALL;

        private void Awake()
        {
            this.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            this.root = uIDocument.rootVisualElement;

            bgmManager.PlaySound(sfxOnEnterMenu, null, sfxOnEnterMenuVolume);
            cursorManager.ShowCursor();

            ItemNamePreview = root.Q<Label>("ItemNamePreview");
            ItemDescriptionPreview = root.Q<Label>("ItemDescriptionPreview");
            ItemSprite = root.Q<VisualElement>("ItemSprite");
            ItemInfoPreview = root.Q<VisualElement>("ItemInfoPreview");

            StatsChangedContainer = root.Q<VisualElement>("StatsChangedContainer");
            RequirementsPreview = root.Q<VisualElement>("RequirementsPreview");

            PhysicalAttack = root.Q<Label>("PhysicalAttack");
            FireAttack = root.Q<Label>("FireAttack");
            FrostAttack = root.Q<Label>("FrostAttack");
            WaterAttack = root.Q<Label>("WaterAttack");
            MagicAttack = root.Q<Label>("MagicAttack");
            LightningAttack = root.Q<Label>("LightningAttack");
            DarknessAttack = root.Q<Label>("DarknessAttack");
            PhysicalDefense = root.Q<Label>("PhysicalDefense");
            FireDefense = root.Q<Label>("FireDefense");
            FrostDefense = root.Q<Label>("FrostDefense");
            WaterDefense = root.Q<Label>("WaterDefense");
            MagicDefense = root.Q<Label>("MagicDefense");
            LightningDefense = root.Q<Label>("LightningDefense");
            DarknessDefense = root.Q<Label>("DarknessDefense");
            BlockAbsorption = root.Q<Label>("BlockAbsorption");

            AttackDifferencesContainer = root.Q<VisualElement>("AttackDifferencesContainer");
            DefenseDifferencesContainer = root.Q<VisualElement>("DefenseDifferencesContainer");
            ShieldDifferencesContainer = root.Q<VisualElement>("ShieldDifferencesContainer");

            filterButtonGroup = root.Q<ToggleButtonGroup>();
            filterAll = filterButtonGroup.Q<Button>("Filter_All");
            filterByWeapons = filterButtonGroup.Q<Button>("Filter_Weapons");
            filterByArmors = filterButtonGroup.Q<Button>("Filter_Armors");

            UIUtils.SetupButton(filterAll, OnFilterAll, soundbank);
            UIUtils.SetupButton(filterByWeapons, OnFilterWeapons, soundbank);
            UIUtils.SetupButton(filterByArmors, OnFilterArmors, soundbank);

            // TODO: For now, only enable filters for keyboard until we add proper gamepad support
            filterButtonGroup.style.display = Gamepad.current == null && isUpgradingSpells == false
                ? DisplayStyle.Flex : DisplayStyle.None;

            DrawUI();
        }

        void OnFilterAll()
        {
            filterType = FilterType.ALL;
            DrawUI();
        }

        void OnFilterWeapons()
        {
            filterType = FilterType.WEAPONS;
            DrawUI();
        }

        void OnFilterArmors()
        {
            filterType = FilterType.ARMORS;
            DrawUI();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OpenBlacksmithMenu()
        {
            isUpgradingSpells = false;

            this.craftActivity = CraftActivity.BLACKSMITH;
            this.gameObject.SetActive(true);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OpenSpellSmithingMenu()
        {
            isUpgradingSpells = true;

            this.craftActivity = CraftActivity.BLACKSMITH;
            this.gameObject.SetActive(true);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OpenAlchemyMenu()
        {
            isUpgradingSpells = false;

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
            isUpgradingSpells = false;
        }

        void ClearPreviews()
        {
            RequirementsPreview.style.opacity = 0;

            ItemNamePreview.text = "";
            ItemDescriptionPreview.text = "";
            ItemInfoPreview.style.display = DisplayStyle.None;

            ClearLabels();
        }

        void SetupActivity()
        {
            root.Q<VisualElement>("ImageBack").style.backgroundImage = new StyleBackground(isUpgradingSpells ? spellSmithingBackgroundImage : blacksmithBackgroundImage);
            root.Q<Label>("CraftActivityTitle").text = Utils.IsPortuguese() ? "Melhorar Equipamento" : "Upgrade Equipment";
        }

        void DrawUI()
        {
            ClearPreviews();

            SetupActivity();

            PopulateScrollView();
        }

        void PopulateScrollView()
        {
            StatsChangedContainer.style.display = DisplayStyle.None;

            var scrollView = this.root.Q<ScrollView>();
            scrollView.verticalScroller.focusable = false;
            scrollView.Clear();

            Button exitButton = new()
            {
                text = Utils.IsPortuguese() ? "Sair" : "Exit"
            };
            exitButton.AddToClassList("primary-button");
            UIUtils.SetupButton(exitButton, () =>
            {
                Close();
            },
            () =>
            {
                scrollView.ScrollTo(exitButton);
            },
            () => { }, false, soundbank);

            scrollView.Add(exitButton);

            PopulateUpgradeableItems();

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

        void PopulateUpgradeableItems()
        {
            var scrollView = this.root.Q<ScrollView>();

            int i = 0;

            List<UpgradableItem> upgradableItems = GetUpgradableItems();

            foreach (UpgradableItem upgradableItem in upgradableItems)
            {
                int currentIndex = i;

                if (ShouldSkipUpgrade(upgradableItem, upgradableItem.level))
                {
                    continue;
                }

                var scrollItem = this.recipeItem.CloneTree();

                scrollItem.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(upgradableItem.sprite);
                scrollItem.Q<Label>("ItemName").text = GetItemName(upgradableItem);
                scrollItem.Q<Label>("ItemDescription").style.display = DisplayStyle.None;

                var craftBtn = scrollItem.Q<Button>("CraftButtonItem");
                var craftLabel = scrollItem.Q<Label>("CraftLabel");
                craftLabel.text = Utils.IsPortuguese() ? "Melhorar" : "Upgrade";

                craftBtn.style.opacity = CraftingUtils.CanImproveItem(playerManager, upgradableItem, playerStatsDatabase.gold) ? 1f : 0.25f;

                UIUtils.SetupButton(craftBtn, () =>
                {
                    lastScrollElementIndex = currentIndex;

                    if (!CraftingUtils.CanImproveItem(playerManager, upgradableItem, playerStatsDatabase.gold))
                    {
                        HandleCraftError(Utils.IsPortuguese()
                            ? "Faltam materiais para melhorar!"
                            : "Missing upgrade materials!");
                        return;
                    }

                    HandleItemUpgrade(upgradableItem);

                    DrawUI();
                },
                () =>
                {
                    ShowRequirements(upgradableItem);
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

        List<UpgradableItem> GetUpgradableItems()
        {
            List<UpgradableItem> upgradableItems = new();

            if (isUpgradingSpells)
            {
                List<Spell> spells = inventoryDatabase.ownedSpells.Where(spell => spell != null && spell.canBeUpgraded).ToList();

                foreach (Spell spell in spells)
                {
                    upgradableItems.Add(spell);
                }

                return upgradableItems;
            }

            if (filterType == FilterType.ALL || filterType == FilterType.WEAPONS)
            {
                List<Weapon> weaponsForUpgrade = inventoryDatabase.ownedWeapons.Where(weapon => weapon != null && weapon.canBeUpgraded).ToList();
                foreach (Weapon wp in weaponsForUpgrade)
                {
                    upgradableItems.Add(wp);
                }
            }

            if (filterType == FilterType.ALL || filterType == FilterType.ARMORS)
            {
                List<Helmet> helmets = inventoryDatabase.ownedHelmets.Where(helmet => helmet != null && helmet.canBeUpgraded).ToList();
                foreach (Helmet helmet in helmets)
                {
                    upgradableItems.Add(helmet);
                }

                List<Armor> armors = inventoryDatabase.ownedArmors.Where(armor => armor != null && armor.canBeUpgraded).ToList();
                foreach (Armor armor in armors)
                {
                    upgradableItems.Add(armor);
                }

                List<Gauntlet> gauntlets = inventoryDatabase.ownedGauntlets.Where(gauntlet => gauntlet != null && gauntlet.canBeUpgraded).ToList();
                foreach (Gauntlet gauntlet in gauntlets)
                {
                    upgradableItems.Add(gauntlet);
                }

                List<Legwear> legwears = inventoryDatabase.ownedLegwears.Where(legwear => legwear != null && legwear.canBeUpgraded).ToList();
                foreach (Legwear legwear in legwears)
                {
                    upgradableItems.Add(legwear);
                }

                List<Accessory> accessories = inventoryDatabase.ownedAccessories.Where(accessory => accessory != null && accessory.canBeUpgraded).ToList();
                foreach (Accessory accessory in accessories)
                {
                    upgradableItems.Add(accessory);
                }
            }

            return upgradableItems;
        }

        void HandleCraftError(string errorMessage)
        {
            soundbank.PlaySound(soundbank.craftError);
            notificationManager.ShowNotification(errorMessage, notificationManager.alchemyLackOfIngredients);
        }

        bool ShouldSkipUpgrade(UpgradableItem upgradableItem, int nextLevel)
        {
            return upgradableItem.canBeUpgraded == false || upgradableItem.upgradeMaterialData == null || nextLevel >= upgradableItem.upgradeMaterialData.upgradeMaterials.Count();
        }

        string GetItemName(UpgradableItem upgradableItem)
        {
            return $"{upgradableItem.GetName()} +{upgradableItem.level} > {upgradableItem.GetName()} +{upgradableItem.level + 1}";
        }

        void HandleItemUpgrade(UpgradableItem upgradableItem)
        {
            playerManager.playerAchievementsManager.achievementForUpgradingFirstWeapon.AwardAchievement();
            soundbank.PlaySound(soundbank.craftSuccess);
            notificationManager.ShowNotification(
                Utils.IsPortuguese() ? "Item melhorado!" : "Item improved", upgradableItem.sprite);

            CraftingUtils.UpgradeItem(
                upgradableItem,
                (goldUsed) => uIDocumentPlayerGold.LoseGold(goldUsed),
                (upgradeMaterialUsed) => playerManager.playerInventory.RemoveItem(upgradeMaterialUsed.Key, upgradeMaterialUsed.Value)
            );

            AnalyticsUtils.OnItemUpgrade(upgradableItem, playerManager);

            if (upgradableItem is Weapon weapon)
            {
                UpdateWeaponIfEquipped(weapon);
            }
            else if (upgradableItem is Spell spell)
            {
                UpdateSpellIfEquipped(spell);
            }
            else if (upgradableItem is Helmet helmet)
            {
                UpdateEquipmentIfEquipped(
                    helmet,
                    playerManager.playerInventory.inventoryDatabase.ownedHelmets,
                    playerManager.equipmentDatabase.helmet,
                    playerManager.characterBaseEquipment.UnequipHelmet,
                    playerManager.characterBaseEquipment.EquipHelmet
                );
            }
            else if (upgradableItem is Armor armor)
            {
                UpdateEquipmentIfEquipped(
                    armor,
                    playerManager.playerInventory.inventoryDatabase.ownedArmors,
                    playerManager.equipmentDatabase.armor,
                    playerManager.characterBaseEquipment.UnequipArmor,
                    playerManager.characterBaseEquipment.EquipArmor
                );
            }
            else if (upgradableItem is Gauntlet gauntlet)
            {
                UpdateEquipmentIfEquipped(
                    gauntlet,
                    playerManager.playerInventory.inventoryDatabase.ownedGauntlets,
                    playerManager.equipmentDatabase.gauntlet,
                    playerManager.characterBaseEquipment.UnequipGauntlets,
                    playerManager.characterBaseEquipment.EquipGauntlets
                );
            }
            else if (upgradableItem is Legwear legwear)
            {
                UpdateEquipmentIfEquipped(
                    legwear,
                    playerManager.playerInventory.inventoryDatabase.ownedLegwears,
                    playerManager.equipmentDatabase.legwear,
                    playerManager.characterBaseEquipment.UnequipLegwear,
                    playerManager.characterBaseEquipment.EquipLegwear
                );
            }
        }

        void ClearLabels()
        {
            AttackDifferencesContainer.style.display = DisplayStyle.None;

            PhysicalAttack.style.display = DisplayStyle.None;
            FireAttack.style.display = DisplayStyle.None;
            FrostAttack.style.display = DisplayStyle.None;
            LightningAttack.style.display = DisplayStyle.None;
            MagicAttack.style.display = DisplayStyle.None;
            DarknessAttack.style.display = DisplayStyle.None;
            WaterAttack.style.display = DisplayStyle.None;

            DefenseDifferencesContainer.style.display = DisplayStyle.None;

            PhysicalDefense.style.display = DisplayStyle.None;
            FireDefense.style.display = DisplayStyle.None;
            FrostDefense.style.display = DisplayStyle.None;
            LightningDefense.style.display = DisplayStyle.None;
            DarknessDefense.style.display = DisplayStyle.None;
            MagicDefense.style.display = DisplayStyle.None;
            WaterDefense.style.display = DisplayStyle.None;

            ShieldDifferencesContainer.style.display = DisplayStyle.None;

            BlockAbsorption.style.display = DisplayStyle.None;
        }

        void DrawAttackDifferences(Weapon weapon)
        {
            var nextLevel = weapon.level + 1;

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
                PhysicalAttack.style.display = DisplayStyle.Flex;
                PhysicalAttack.text =
                    Utils.IsPortuguese()
                        ? $"Ataque Físico: {currentPhysicalAttack} > {nextPhysicalAttack}"
                        : $"Physical Attack: {currentPhysicalAttack} > {nextPhysicalAttack}";
            }
            if (currentFireAttack != 0)
            {
                FireAttack.style.display = DisplayStyle.Flex;
                FireAttack.text =
                    Utils.IsPortuguese()
                        ? $"Ataque de Fogo: {currentFireAttack} > {nextFireAttack}"
                        : $"Fire Attack: {currentFireAttack} > {nextFireAttack}";
            }
            if (currentFrostAttack != 0)
            {
                FrostAttack.style.display = DisplayStyle.Flex;
                FrostAttack.text =
                    Utils.IsPortuguese()
                        ? $"Ataque de Gelo: {currentFrostAttack} > {nextFrostAttack}"
                        : $"Frost Attack: {currentFrostAttack} > {nextFrostAttack}";
            }
            if (currentLightningAttack != 0)
            {
                LightningAttack.style.display = DisplayStyle.Flex;
                LightningAttack.text =
                     Utils.IsPortuguese()
                         ? $"Ataque Elétrico: {currentLightningAttack} > {nextLightningAttack}"
                         : $"Lightning Attack: {currentLightningAttack} > {nextLightningAttack}";
            }
            if (currentMagicAttack != 0)
            {
                MagicAttack.style.display = DisplayStyle.Flex;
                MagicAttack.text =
                    Utils.IsPortuguese()
                        ? $"Ataque Mágico: {currentMagicAttack} > {nextMagicAttack}"
                        : $"Magic Attack: {currentMagicAttack} > {nextMagicAttack}";
            }
            if (currentDarknessAttack != 0)
            {
                DarknessAttack.style.display = DisplayStyle.Flex;
                DarknessAttack.text =
                    Utils.IsPortuguese()
                        ? $"Ataque das Sombras: {currentDarknessAttack} > {nextDarknessAttack}"
                        : $"Darkness Attack: {currentDarknessAttack} > {nextDarknessAttack}";
            }
            if (currentWaterAttack != 0)
            {
                WaterAttack.style.display = DisplayStyle.Flex;
                WaterAttack.text =
                    Utils.IsPortuguese()
                        ? $"Ataque de Água: {currentWaterAttack} > {nextWaterAttack}"
                        : $"Water Attack: {currentWaterAttack} > {nextWaterAttack}";
            }

            AttackDifferencesContainer.style.display = DisplayStyle.Flex;
        }

        void DrawAttackDifferencesForSpell(Spell spell)
        {
            Damage spellDamage = ScalingUtils.GetAbilityDamageForPlayerSpell(spell.ability.GetDamage(playerManager), playerManager, spell);

            var nextLevel = spell.level + 1;

            int currentPhysicalAttack = spell.GetCurrentPhysicalAttackForLevel(spellDamage, spell.level);
            int nextPhysicalAttack = spell.GetCurrentPhysicalAttackForLevel(spellDamage, nextLevel);

            int currentFireAttack = spell.GetFireAttackForLevel(spellDamage, spell.level);
            int nextFireAttack = spell.GetFireAttackForLevel(spellDamage, nextLevel);

            int currentFrostAttack = spell.GetFrostAttackForLevel(spellDamage, spell.level);
            int nextFrostAttack = spell.GetFrostAttackForLevel(spellDamage, nextLevel);

            int currentLightningAttack = spell.GetLightningAttackForLevel(spellDamage, spell.level);
            int nextLightningAttack = spell.GetLightningAttackForLevel(spellDamage, nextLevel);

            int currentMagicAttack = spell.GetMagicAttackForLevel(spellDamage, spell.level);
            int nextMagicAttack = spell.GetMagicAttackForLevel(spellDamage, nextLevel);

            int currentDarknessAttack = spell.GetDarknessAttackForLevel(spellDamage, spell.level);
            int nextDarknessAttack = spell.GetDarknessAttackForLevel(spellDamage, nextLevel);

            int currentWaterAttack = spell.GetWaterAttackForLevel(spellDamage, spell.level);
            int nextWaterAttack = spell.GetWaterAttackForLevel(spellDamage, nextLevel);

            if (currentPhysicalAttack != 0)
            {
                PhysicalAttack.style.display = DisplayStyle.Flex;
                PhysicalAttack.text =
                    Utils.IsPortuguese()
                        ? $"Ataque Físico: {currentPhysicalAttack} > {nextPhysicalAttack}"
                        : $"Physical Attack: {currentPhysicalAttack} > {nextPhysicalAttack}";
            }
            if (currentFireAttack != 0)
            {
                FireAttack.style.display = DisplayStyle.Flex;
                FireAttack.text =
                    Utils.IsPortuguese()
                        ? $"Ataque de Fogo: {currentFireAttack} > {nextFireAttack}"
                        : $"Fire Attack: {currentFireAttack} > {nextFireAttack}";
            }
            if (currentFrostAttack != 0)
            {
                FrostAttack.style.display = DisplayStyle.Flex;
                FrostAttack.text =
                    Utils.IsPortuguese()
                        ? $"Ataque de Gelo: {currentFrostAttack} > {nextFrostAttack}"
                        : $"Frost Attack: {currentFrostAttack} > {nextFrostAttack}";
            }
            if (currentLightningAttack != 0)
            {
                LightningAttack.style.display = DisplayStyle.Flex;
                LightningAttack.text =
                     Utils.IsPortuguese()
                         ? $"Ataque Elétrico: {currentLightningAttack} > {nextLightningAttack}"
                         : $"Lightning Attack: {currentLightningAttack} > {nextLightningAttack}";
            }
            if (currentMagicAttack != 0)
            {
                MagicAttack.style.display = DisplayStyle.Flex;
                MagicAttack.text =
                    Utils.IsPortuguese()
                        ? $"Ataque Mágico: {currentMagicAttack} > {nextMagicAttack}"
                        : $"Magic Attack: {currentMagicAttack} > {nextMagicAttack}";
            }
            if (currentDarknessAttack != 0)
            {
                DarknessAttack.style.display = DisplayStyle.Flex;
                DarknessAttack.text =
                    Utils.IsPortuguese()
                        ? $"Ataque das Sombras: {currentDarknessAttack} > {nextDarknessAttack}"
                        : $"Darkness Attack: {currentDarknessAttack} > {nextDarknessAttack}";
            }
            if (currentWaterAttack != 0)
            {
                WaterAttack.style.display = DisplayStyle.Flex;
                WaterAttack.text =
                    Utils.IsPortuguese()
                        ? $"Ataque de Água: {currentWaterAttack} > {nextWaterAttack}"
                        : $"Water Attack: {currentWaterAttack} > {nextWaterAttack}";
            }

            AttackDifferencesContainer.style.display = DisplayStyle.Flex;
        }

        void DrawDefenseDifferences(ArmorBase armorBase)
        {
            var nextLevel = armorBase.level + 1;

            int currentPhysicalDefense = armorBase.GetCurrentPhysicalDefenseForLevel(armorBase.level);
            int nextPhysicalDefense = armorBase.GetCurrentPhysicalDefenseForLevel(nextLevel);

            int currentFireDefense = armorBase.GetFireDefenseForLevel(armorBase.level);
            int nextFireDefense = armorBase.GetFireDefenseForLevel(nextLevel);

            int currentFrostDefense = armorBase.GetFrostDefenseForLevel(armorBase.level);
            int nextFrostDefense = armorBase.GetFrostDefenseForLevel(nextLevel);

            int currentLightningDefense = armorBase.GetLightningDefenseForLevel(armorBase.level);
            int nextLightningDefense = armorBase.GetLightningDefenseForLevel(nextLevel);

            int currentMagicDefense = armorBase.GetMagicDefenseForLevel(armorBase.level);
            int nextMagicDefense = armorBase.GetMagicDefenseForLevel(nextLevel);

            int currentDarknessDefense = armorBase.GetDarknessDefenseForLevel(armorBase.level);
            int nextDarknessDefense = armorBase.GetDarknessDefenseForLevel(nextLevel);

            int currentWaterDefense = armorBase.GetWaterDefenseForLevel(armorBase.level);
            int nextWaterDefense = armorBase.GetWaterDefenseForLevel(nextLevel);

            if (currentPhysicalDefense != 0)
            {
                PhysicalDefense.style.display = DisplayStyle.Flex;
                PhysicalDefense.text =
                    Utils.IsPortuguese()
                        ? $"Defesa Física: {currentPhysicalDefense} > {nextPhysicalDefense}"
                        : $"Physical Defense: {currentPhysicalDefense} > {nextPhysicalDefense}";
            }
            if (currentFireDefense != 0)
            {
                FireDefense.style.display = DisplayStyle.Flex;
                FireDefense.text =
                    Utils.IsPortuguese()
                        ? $"Defesa de Fogo: {currentFireDefense} > {nextFireDefense}"
                        : $"Fire Defense: {currentFireDefense} > {nextFireDefense}";
            }
            if (currentFrostDefense != 0)
            {
                FrostDefense.style.display = DisplayStyle.Flex;
                FrostDefense.text =
                    Utils.IsPortuguese()
                        ? $"Defesa de Gelo: {currentFrostDefense} > {nextFrostDefense}"
                        : $"Frost Defense: {currentFrostDefense} > {nextFrostDefense}";
            }
            if (currentLightningDefense != 0)
            {
                LightningDefense.style.display = DisplayStyle.Flex;
                LightningDefense.text =
                     Utils.IsPortuguese()
                         ? $"Defesa Elétrica: {currentLightningDefense} > {nextLightningDefense}"
                         : $"Lightning Defense: {currentLightningDefense} > {nextLightningDefense}";
            }
            if (currentMagicDefense != 0)
            {
                MagicDefense.style.display = DisplayStyle.Flex;
                MagicDefense.text =
                    Utils.IsPortuguese()
                        ? $"Defesa Mágica: {currentMagicDefense} > {nextMagicDefense}"
                        : $"Magic Defense: {currentMagicDefense} > {nextMagicDefense}";
            }
            if (currentDarknessDefense != 0)
            {
                DarknessDefense.style.display = DisplayStyle.Flex;
                DarknessDefense.text =
                    Utils.IsPortuguese()
                        ? $"Defesa das Sombras: {currentDarknessDefense} > {nextDarknessDefense}"
                        : $"Darkness Defense: {currentDarknessDefense} > {nextDarknessDefense}";
            }
            if (currentWaterDefense != 0)
            {
                WaterDefense.style.display = DisplayStyle.Flex;
                WaterDefense.text =
                    Utils.IsPortuguese()
                        ? $"Defesa de Água: {currentWaterDefense} > {nextWaterDefense}"
                        : $"Water Defense: {currentWaterDefense} > {nextWaterDefense}";
            }

            DefenseDifferencesContainer.style.display = DisplayStyle.Flex;
        }

        void DrawBlockAbsorptionDifferences(Shield shield)
        {
            var nextLevel = shield.level + 1;

            float currentAbsorption = shield.GetCurrentAbsorption(shield.physicalAbsorption) * 100;
            float nextAbsorption = shield.GetAbsorptionForLevel(shield.physicalAbsorption, nextLevel) * 100;

            if (currentAbsorption != 0)
            {
                BlockAbsorption.style.display = DisplayStyle.Flex;
                BlockAbsorption.text =
                    Utils.IsPortuguese()
                        ? $"Absorção de Dano: {currentAbsorption}% > {nextAbsorption}%"
                        : $"Damage Absorption: {currentAbsorption}% > {nextAbsorption}%";
            }

            ShieldDifferencesContainer.style.display = DisplayStyle.Flex;
        }

        void ClearRequirementsInfo()
        {
            root.Q<VisualElement>("ItemInfo").Clear();
        }

        void DrawItemRequirements(UpgradeMaterialData.UpgradeMaterialEntry upgradeData)
        {
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
        }

        void DrawGoldRequired(UpgradeMaterialData.UpgradeMaterialEntry upgradeData)
        {
            var goldItemEntry = ingredientItem.CloneTree();
            goldItemEntry.Q<IMGUIContainer>("ItemIcon").style.backgroundImage = new StyleBackground(goldSprite);
            goldItemEntry.Q<Label>("Title").text = LocalizationSettings.StringDatabase.GetLocalizedString("UIDocuments", "Gold");

            goldItemEntry.Q<Label>("Amount").text = playerStatsDatabase.gold + " / " + upgradeData.goldCostForUpgrade;
            goldItemEntry.Q<Label>("Amount").style.opacity = playerStatsDatabase.gold >= upgradeData.goldCostForUpgrade ? 1 : 0.25f;

            root.Q<VisualElement>("ItemInfo").Add(goldItemEntry);
        }

        void ShowRequirements(UpgradableItem upgradableItem)
        {
            UpgradeMaterialData.UpgradeMaterialEntry upgradeData = upgradableItem.upgradeMaterialData.upgradeMaterials.ElementAtOrDefault(upgradableItem.level);

            if (upgradeData == null)
            {
                return;
            }

            var nextLevel = upgradableItem.level + 1;
            StatsChangedContainer.style.display = DisplayStyle.Flex;

            // Item preview
            string itemNamePreview = upgradableItem.GetName() + " +" + upgradableItem.level;
            string nextItemPreview = " > +" + nextLevel;

            ItemNamePreview.text = itemNamePreview + nextItemPreview;

            ItemDescriptionPreview.text = upgradableItem.GetDescription();
            ItemSprite.style.backgroundImage = new StyleBackground(upgradableItem.sprite);
            ItemInfoPreview.style.display = DisplayStyle.Flex;

            ClearLabels();

            if (upgradableItem is Weapon weapon)
            {
                DrawAttackDifferences(weapon);
            }

            if (upgradableItem is Spell spell)
            {
                DrawAttackDifferencesForSpell(spell);
            }

            if (upgradableItem is Shield shield)
            {
                DrawBlockAbsorptionDifferences(shield);
            }

            if (upgradableItem is ArmorBase armorBase)
            {
                DrawDefenseDifferences(armorBase);
            }

            // Requirements
            ClearRequirementsInfo();
            DrawItemRequirements(upgradeData);
            DrawGoldRequired(upgradeData);

            RequirementsPreview.style.opacity = 1;
        }

        void UpdateWeaponIfEquipped(Weapon weaponAfterUpgrade)
        {
            if (weaponAfterUpgrade == null)
            {
                return;
            }

            // Update weapon in inventory first
            int indexOfWeaponInInventory = playerManager.playerInventory.inventoryDatabase.ownedWeapons.FindIndex(x => x != null && x.itemID == weaponAfterUpgrade.itemID);
            if (indexOfWeaponInInventory != -1)
            {
                playerManager.playerInventory.inventoryDatabase.ownedWeapons[indexOfWeaponInInventory].level = weaponAfterUpgrade.level;
            }
            else
            {
                return;
            }

            Weapon weaponInInventory = playerManager.playerInventory.inventoryDatabase.ownedWeapons[indexOfWeaponInInventory];

            bool hasFoundMatch = false;

            // Then check if weapon is equipped - if it is, we need to reequip it
            for (int i = 0; i < playerManager.equipmentDatabase.weapons.Length; i++)
            {
                Weapon potentialRightHandWeapon = playerManager.equipmentDatabase.weapons[i];

                if (potentialRightHandWeapon != null && potentialRightHandWeapon.itemID == weaponInInventory.itemID)
                {
                    // Reequip
                    playerManager.characterBaseEquipment.UnequipWeapon(i, true);
                    playerManager.characterBaseEquipment.EquipWeapon(weaponInInventory, i, true);

                    hasFoundMatch = true;
                    break;
                }
            }

            if (hasFoundMatch)
            {
                return;
            }

            for (int i = 0; i < playerManager.equipmentDatabase.shields.Length; i++)
            {
                Weapon potentialLeftHandWeapon = playerManager.equipmentDatabase.shields[i];

                if (potentialLeftHandWeapon != null && potentialLeftHandWeapon.itemID == weaponInInventory.itemID)
                {
                    // Reequip
                    playerManager.characterBaseEquipment.UnequipWeapon(i, false);
                    playerManager.characterBaseEquipment.EquipWeapon(weaponInInventory, i, false);
                    break;
                }
            }
        }

        void UpdateSpellIfEquipped(Spell spellAfterUpgrade)
        {
            if (spellAfterUpgrade == null)
            {
                return;
            }

            // Update weapon in inventory first
            int indexOfSpellInInventory = playerManager.playerInventory.inventoryDatabase.ownedSpells.FindIndex(x => x != null && x.itemID == spellAfterUpgrade.itemID);
            if (indexOfSpellInInventory != -1)
            {
                playerManager.playerInventory.inventoryDatabase.ownedSpells[indexOfSpellInInventory].level = spellAfterUpgrade.level;
            }
            else
            {
                return;
            }

            Spell spellInInventory = playerManager.playerInventory.inventoryDatabase.ownedSpells[indexOfSpellInInventory];

            // Then check if spell is equipped - if it is, we need to reequip it
            for (int i = 0; i < playerManager.equipmentDatabase.spells.Length; i++)
            {
                Spell potentialSpell = playerManager.equipmentDatabase.spells[i];

                if (potentialSpell != null && potentialSpell.itemID == spellInInventory.itemID)
                {
                    // Reequip
                    playerManager.characterBaseEquipment.UnequipSpell(i);
                    playerManager.characterBaseEquipment.EquipSpell(spellInInventory, i);
                    break;
                }
            }
        }

        void UpdateEquipmentIfEquipped<T>(
            T itemAfterUpgrade,
            List<T> ownedItems,
            T currentlyEquippedItem,
            Action UnequipAction,
            Action<T> EquipAction
        ) where T : ArmorBase // Or your common base class/interface
        {
            if (itemAfterUpgrade == null)
                return;

            int indexInInventory = ownedItems.FindIndex(x => x != null && x.itemID == itemAfterUpgrade.itemID);
            if (indexInInventory == -1)
                return;

            ownedItems[indexInInventory].level = itemAfterUpgrade.level;

            T itemInInventory = ownedItems[indexInInventory];

            if (currentlyEquippedItem != null && currentlyEquippedItem.itemID == itemInInventory.itemID)
            {
                UnequipAction?.Invoke();
                EquipAction?.Invoke(itemInInventory);
            }
        }
    }
}
