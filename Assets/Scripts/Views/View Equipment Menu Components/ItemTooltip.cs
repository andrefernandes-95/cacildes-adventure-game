using System;
using AF.Health;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace AF
{
    public class ItemTooltip : MonoBehaviour
    {
        public PlayerManager playerManager;
        [SerializeField] UIDocumentCraftScreen uIDocumentCraftScreen;

        public const string TOOLTIP = "ItemTooltip";
        VisualElement tooltip;
        VisualElement tooltipItemSprite;
        Label tooltipItemName;
        Label tooltipItemDescription;

        [HideInInspector] public VisualElement tooltipEffectsContainer;

        public VisualTreeAsset itemEffectTooltipEntry;

        [Header("UI Documents")]
        public UIDocument uIDocument;
        public VisualElement root;
        [HideInInspector] public Label weaponTypeLabel;

        [HideInInspector] public bool shouldRerender = true;

        [Header("Colors")]
        public Color fire = new Color(255, 102, 42);
        public Color frost = new Color(96, 202, 255);
        public Color lightning = new Color(255, 239, 95);
        public Color magic = new Color(241, 96, 255);
        public Color darkness = new Color(92, 76, 248);
        public Color water = new Color(92, 76, 248);
        public Color requirementsNotMet;


        public Texture2D weaponPhysicalAttackSprite, weaponScalingSprite, weightPenaltySprite, holyWeaponSprite,
        fireSprite, frostSprite, lightningSprite, magicSprite, darknessSprite, bluntSprite, pierceSprite, slashSprite,
        statusEffectsSprite, defenseAbsorptionSprite, poiseSprite, postureSprite, goldCoinSprite, reputationSprite, barterSprite,
        vitalitySprite, enduranceSprite, intelligenceSprite, strengthSprite, dexteritySprite, blacksmithSprite,
        pushForceSprite, heavyAttackSprite, staminaCostSprite, bossTokenSprite, replenishableSprite, spellCastSprite,
        upgradeMaterialSprite, craftingMaterialSprite, projectileSprite, requirementsSprite, waterSprite, cardSprite;

        [Header("Localization")]

        // +{0}% Equip Load
        public LocalizedString equipLoadTooltip_Label;
        // +{0} Poise Points
        public LocalizedString poiseTooltip_Label;
        // +{0} Posture Points
        public LocalizedString postureTooltip_Label;

        // +{0}% gold found on enemies
        public LocalizedString goldFoundOnEnemiesTooltip_Label;

        /*
        +{0} Final Damage\n\n
        Explanation: \n
        +{1} Weapon Base Damage\n
        +{2} ATK [STR Scaling: {3}]\n
        +{4} ATK [DEX Scaling: {5}]\n
        +{6} ATK [INT Scaling: {7}]\n
        */
        public LocalizedString damageExplanationLabel;

        // Holy Weapon
        public LocalizedString holyWeaponLabel;
        // +{0} Fire ATK
        public LocalizedString fireAttackLabel;
        public LocalizedString frostAttackLabel;
        public LocalizedString lightningAttackLabel;
        public LocalizedString magicAttackLabel;
        public LocalizedString darknessAttackLabel;
        public LocalizedString waterAttackLabel;

        // "Damage Type: Blunt"
        public LocalizedString damageTypeBluntLabel;
        public LocalizedString damageTypePierceLabel;
        public LocalizedString damageTypeSlashLabel;

        // "+{0} Push Force"
        public LocalizedString pushForceLabel;
        // "+{0} Posture Damage"
        public LocalizedString postureDamageLabel;

        // "+{0} Heavy Attack Bonus"
        public LocalizedString heavyAttackBonusLabel;
        // "{0} Light ATK Stamina Cost {1} Heavy ATK Stamina Cost"
        public LocalizedString staminaCostLabel;

        // "Ignores enemy shields"
        public LocalizedString ignoresEnemyShields;

        //"Can not be parried"
        public LocalizedString canNotBeParried;

        // "%{0} Physical Damage Absorption When Blocking"
        public LocalizedString physicalDamageAbsorptionWhenBlocking;

        //  "Double coins per enemy kill"
        public LocalizedString doubleCoinsPerEnemyKill;

        //+{0}HP restored with each hit
        public LocalizedString hpRestoredWithEachHit;

        //"{0} Stamina Cost Per Block"
        public LocalizedString staminaCostPerBlock;

        // "%{0} Fire Damage Absorption"
        public LocalizedString fireDMGAbsorption;
        public LocalizedString frostDMGAbsorption;
        public LocalizedString lightningDMGAbsorption;
        public LocalizedString darknessDMGAbsorption;
        // "{0}% Posture Damage Absorption"
        public LocalizedString postureDamageAbsorptionLabel;
        // "{0}% Slash Damage Absorption"
        public LocalizedString slashDamageAbsorptionLabel;

        // "{0}% Pierce Damage Absorption"
        public LocalizedString pierceDamageAbsorptionLabel;

        // "{0}% Blunt Damage Absorption"
        public LocalizedString bluntDamageAbsorptionLabel;

        // "{0} Physical DMG dealt to enemies per block"
        public LocalizedString physicalDmgDealtToEnemiesPerBlockLabel;
        public LocalizedString fireDmgDealtToEnemiesPerBlockLabel;
        public LocalizedString frostDmgDealtToEnemiesPerBlockLabel;
        public LocalizedString lightningDmgDealtToEnemiesPerBlockLabel;
        public LocalizedString magicDmgDealtToEnemiesPerBlockLabel;
        public LocalizedString darknessDmgDealtToEnemiesPerBlockLabel;
        // "+{0} Parry Window Duration Bonus"
        public LocalizedString parryWindowDurationBonusLabel;
        // "+{0} Posture DMG per Parry"
        public LocalizedString postureDamagePerParryLabel;
        // "+{0} Vitality"
        public LocalizedString vitalityBonus;
        public LocalizedString enduranceBonus;
        public LocalizedString intelligenceBonus;
        // "+{0}% Stamina Regen. Speed Bonus"
        public LocalizedString staminaRegenSpeedBonus;
        // $"+{0} Physical Defense"

        // "+{0} Health Points"
        public LocalizedString healthPoints;
        public LocalizedString manaPoints;
        public LocalizedString staminaPoints;
        // "+{0} Physical Attack Damage"
        public LocalizedString physicalAttackDamage;
        public LocalizedString jumpAttackDamage;
        // "Attack increases with lower health"
        public LocalizedString attackIncreasesWithLowerHealth;
        // "Attack decreases with lower reputation"
        public LocalizedString attackIncreasesWithLowerReputation;
        // "+{0}% Spell Damage"
        public LocalizedString moreSpellDamage;
        // "Chance to receive double coins from fallen enemies"
        public LocalizedString chanceToDoubleCoinsFromFallenEnemies;
        // "+{0} Posture Decrease Rate Bonus"
        public LocalizedString postureDecreaseRateBonus;

        // "+{0} Backstab Angle Bonus"
        public LocalizedString backStabAngleBonus;
        // "{0} Mana Points required to cast"

        // "Crafting material (Use in a alchemy table)"
        public LocalizedString craftingMaterialLabel;

        //  "Weapon upgrade material (Give to a blacksmith)"
        public LocalizedString upgradeMaterialLabel;

        //  "Boss token. Someone might be interested in this item."
        public LocalizedString bossTokenLabel;

        // Consume to receive ${0} coins
        public LocalizedString consumeToReceive;

        // Item usage replenishes when resting at a bonfire
        public LocalizedString itemUsageReplenishesWhenRestingAtABonfire;

        // Attack bonus when two handing
        public LocalizedString multiplierWhenTwoHanding;
        [Header("Components")]
        [SerializeField] WeaponTooltip weaponTooltip;
        [SerializeField] ArmorTooltip armorTooltip;
        [SerializeField] AccessoryTooltip accessoryTooltip;
        [SerializeField] SpellTooltip spellTooltip;

        private void OnEnable()
        {
            if (shouldRerender)
            {
                shouldRerender = false;

                SetupRefs();
            }

            tooltip.style.display = DisplayStyle.Flex;
        }

        private void OnDisable()
        {
            tooltip.style.display = DisplayStyle.None;
        }

        public void SetupRefs()
        {
            root = uIDocument.rootVisualElement;

            tooltip = root.Q<VisualElement>(TOOLTIP);
            tooltipItemSprite = root.Q<VisualElement>("ItemTooltipContainer").Q<VisualElement>("ItemSprite");
            tooltipItemName = root.Q<VisualElement>("ItemTooltipContainer").Q<Label>("ItemName");
            tooltipItemDescription = root.Q<VisualElement>("ItemTooltipContainer").Q<Label>("ItemDescription");
            tooltipEffectsContainer = tooltip.Q<VisualElement>("ItemAttributes");
            weaponTypeLabel = tooltip.Q<Label>("WeaponAttackType");
        }

        string GetItemName(Item item)
        {
            string itemName = item.GetName().ToUpper();

            if (item is UpgradableItem upgradableItem)
            {
                itemName += " +" + upgradableItem.level;
            }
            return itemName;
        }

        public void PrepareTooltipForItem(Item item)
        {
            enabled = true;
            tooltipEffectsContainer.Clear();
            tooltipItemSprite.style.backgroundImage = new StyleBackground(item.sprite);
            tooltipItemSprite.style.borderBottomWidth = new StyleFloat(1);
            tooltipItemSprite.style.borderTopWidth = new StyleFloat(1);
            tooltipItemSprite.style.borderLeftWidth = new StyleFloat(1);
            tooltipItemSprite.style.borderRightWidth = new StyleFloat(1);
            tooltipItemSprite.style.unityBackgroundScaleMode = ScaleMode.ScaleAndCrop;

            weaponTypeLabel.style.display = DisplayStyle.None;

            string itemName = GetItemName(item);

            tooltipItemName.text = itemName;
            tooltipItemDescription.text = item.GetDescription();

            if (item is Weapon weapon)
            {
                weaponTooltip.DrawWeaponEffects(weapon);
            }
            else if (item is Shield shield)
            {
                DrawShield(shield);
            }
            else if (item is ArmorBase armorBase)
            {
                armorTooltip.DrawArmorBase(armorBase);

                if (item is Accessory accessory)
                {
                    accessoryTooltip.DrawAccessory(accessory);
                }
            }
            else if (item is Consumable consumable)
            {
                DrawConsumable(consumable);
            }
            else if (item is Spell spell)
            {
                spellTooltip.DrawSpellEffects(spell);
                // DrawSpell(spell);
            }
            else if (item is UpgradeMaterial upgradeMaterial)
            {
                DrawUpgradeMaterial(upgradeMaterial);
            }
            else if (item is CraftingMaterial craftingMaterial)
            {
                DrawCraftingMaterial(craftingMaterial);
            }
            else if (item is Arrow arrow)
            {
                DrawArrow(arrow);
            }
        }

        public void DisplayTooltip(Button parentButton)
        {
            // Get the button's position and size in screen space
            float buttonY = parentButton.worldBound.y;
            float buttonWidth = parentButton.resolvedStyle.width;

            // Get the tooltip's size
            float tooltipSize = tooltip.resolvedStyle.height;

            // Calculate the target position for the tooltip
            Vector2 tooltipPosition = new Vector2(buttonWidth, buttonY / 2);

            // Check if the tooltip would exceed the screen height
            float screenHeight = root.resolvedStyle.height;
            if (tooltipPosition.y + tooltipSize > screenHeight)
            {
                // Adjust the position to be above the button if it would be outside the screen
                float tooltipOffset = tooltipSize;
                tooltipPosition.y = Mathf.Max(buttonY - tooltipOffset, 0f);
            }

            // Position the tooltip
            tooltip.style.display = DisplayStyle.Flex;
        }

        void CreatePoiseTooltip(int poiseBonus)
        {
            if (poiseBonus <= 0)
            {
                return;
            }

            CreateTooltip(
                poiseSprite,
                Color.white,
                String.Format(poiseTooltip_Label.GetLocalizedString(), poiseBonus));
        }
        void CreatePostureTooltip(int postureBonus)
        {
            if (postureBonus <= 0)
            {
                return;
            }

            CreateTooltip(
                postureSprite,
                Color.white,
                String.Format(postureTooltip_Label.GetLocalizedString(), postureBonus));
        }

        void DrawWeaponEffects(Weapon weapon)
        {
            if (weapon.HasRequirements())
            {
                CreateTooltip(
                    requirementsSprite,
                    weapon.AreRequirementsMet(playerManager) ? Color.white : requirementsNotMet,
                    weapon.DrawRequirements(playerManager));
            }

            if (playerManager.characterBaseAttackManager.GetWeaponAttack(weapon) > 0)
            {
            }

            if (weapon.GetWeaponFireAttack(playerManager.characterBaseAttackManager) > 0)
            {
                CreateTooltip(
                    fireSprite,
                    fire,
                    String.Format(
                        fireAttackLabel.GetLocalizedString(),
                        weapon.GetWeaponFireAttack(playerManager.characterBaseAttackManager)));
            }


            if (weapon.GetWeaponLightningAttack(playerManager.playerStatsDatabase.GetCurrentReputation(), playerManager.characterBaseAttackManager) > 0)
            {
                int baseLightningAttack = weapon.GetBaseWeaponLightningAttack();
                int holyDamageScaleFromReputation = weapon.GetWeaponLightningAttack(playerManager.playerStatsDatabase.GetCurrentReputation(), playerManager.characterBaseAttackManager) - baseLightningAttack;

                CreateTooltip(
                lightningSprite,
                lightning,
                TooltipUtils.GetLightiningDamageExplanation(baseLightningAttack, holyDamageScaleFromReputation, 0));
            }

            if (weapon.GetWeaponMagicAttack(playerManager.characterBaseAttackManager) > 0)
            {
                CreateTooltip(
                magicSprite,
                magic,
                TooltipUtils.GetMagicDamageExplanation(playerManager, weapon));
            }

            if (weapon.GetWeaponDarknessAttack(playerManager.characterBaseStats.GetReputation(), playerManager.characterBaseAttackManager) > 0)
            {
                int baseDarknessAttack = weapon.GetBaseWeaponDarknessAttack();
                int holyDamageScaleFromReputation = weapon.GetWeaponDarknessAttack(playerManager.playerStatsDatabase.GetCurrentReputation(), playerManager.characterBaseAttackManager) - baseDarknessAttack;

                CreateTooltip(
                darknessSprite,
                darkness,
                TooltipUtils.GetDarknessDamageExplanation(baseDarknessAttack, holyDamageScaleFromReputation, 0));
            }

            if (weapon.damage.weaponAttackType == WeaponAttackType.Blunt)
            {
                CreateTooltip(bluntSprite, Color.white, damageTypeBluntLabel.GetLocalizedString());
            }
            if (weapon.damage.weaponAttackType == WeaponAttackType.Pierce)
            {
                CreateTooltip(pierceSprite, Color.white, damageTypePierceLabel.GetLocalizedString());
            }
            if (weapon.damage.weaponAttackType == WeaponAttackType.Slash)
            {
                CreateTooltip(slashSprite, Color.white, damageTypeSlashLabel.GetLocalizedString());
            }

            DrawStatusEffects(weapon.damage);


            if (weapon.isHolyWeapon)
            {
                CreateTooltip(holyWeaponSprite, Color.white, holyWeaponLabel.GetLocalizedString());
            }

            if (weapon.damage.pushForce > 0)
            {
                CreateTooltip(pushForceSprite, Color.white, String.Format(
                    pushForceLabel.GetLocalizedString(), weapon.damage.pushForce));
            }

            if (weapon.damage.postureDamage > 0)
            {
                CreateTooltip(postureSprite, Color.white, String.Format(
                    postureDamageLabel.GetLocalizedString(), weapon.damage.postureDamage));
            }

            /*            if (weapon.heavyAttackBonus > 0)
                        {
                            CreateTooltip(heavyAttackSprite, Color.white,
                            String.Format(
                                heavyAttackBonusLabel.GetLocalizedString(), weapon.heavyAttackBonus));
                        }*/

            CreateTooltip(
                staminaCostSprite,
                Color.white,
                String.Format(staminaCostLabel.GetLocalizedString(), weapon.GetLightAttackStaminaCost(), weapon.GetHeavyAttackStaminaCost()));

            /*
        if (weapon.canBeUpgraded && weapon.CanBeUpgradedFurther())
        {
            CreateTooltip(blacksmithSprite, Color.white, weapon.GetMaterialCostForNextLevel());
        } */

            if (weapon.damage.ignoreBlocking)
            {
                CreateTooltip(defenseAbsorptionSprite, Color.white, ignoresEnemyShields.GetLocalizedString());
            }

            if (weapon.damage.canNotBeParried)
            {
                CreateTooltip(defenseAbsorptionSprite, Color.white, canNotBeParried.GetLocalizedString());
            }

            if (weapon.weaponBlockAbsorption != 1)
            {
                CreateTooltip(defenseAbsorptionSprite, Color.white,
                    String.Format(
                        physicalDamageAbsorptionWhenBlocking.GetLocalizedString(),
                        100 - (weapon.weaponBlockAbsorption * 100)));
            }

            if (weapon.doubleCoinsUponKillingEnemies)
            {
                CreateTooltip(goldCoinSprite, Color.white, doubleCoinsPerEnemyKill.GetLocalizedString());
            }

            if (weapon.healthRestoredWithEachHit > 0)
            {
                CreateTooltip(vitalitySprite, Color.white, String.Format(
                    hpRestoredWithEachHit.GetLocalizedString(),
                    weapon.healthRestoredWithEachHit));
            }
        }

        void DrawShield(Shield shield)
        {
            if (shield.blockStaminaCost != 1)
            {
                CreateTooltip(
                    staminaCostSprite,
                    Color.white,
                    String.Format(staminaCostPerBlock.GetLocalizedString(), shield.blockStaminaCost));
            }

            float shieldAbsorptionLevel = shield.GetAbsorptionForLevel(shield.physicalAbsorption, shield.level);
            if (shieldAbsorptionLevel != 1)
            {

                CreateTooltip(
                    defenseAbsorptionSprite,
                    Color.white,
                    String.Format(physicalDamageAbsorptionWhenBlocking.GetLocalizedString(), 100 - (shieldAbsorptionLevel * 100)));
            }

            if (shield.fireAbsorption != 1)
            {
                CreateTooltip(
                    fireSprite,
                    fire,
                    String.Format(fireDMGAbsorption.GetLocalizedString(), 100 - (shield.fireAbsorption * 100)));
            }
            if (shield.frostAbsorption != 1)
            {
                CreateTooltip(
                    frostSprite,
                    frost,
                    String.Format(frostDMGAbsorption.GetLocalizedString(), 100 - (shield.frostAbsorption * 100)));
            }
            if (shield.lightiningAbsorption != 1)
            {
                CreateTooltip(
                    lightningSprite,
                    lightning,
                    String.Format(lightningDMGAbsorption.GetLocalizedString(), 100 - (shield.lightiningAbsorption * 100)));
            }
            if (shield.magicAbsorption != 1)
            {
                CreateTooltip(
                    magicSprite,
                    magic,
                    String.Format(
                        lightningDMGAbsorption.GetLocalizedString(),
                         100 - (shield.magicAbsorption * 100)));
            }
            if (shield.darknessAbsorption != 1)
            {
                CreateTooltip(
                    darknessSprite,
                    darkness,
                    String.Format(
                        darknessDMGAbsorption.GetLocalizedString(),
                         100 - (shield.darknessAbsorption * 100)));
            }

            if (shield.statusEffectBlockResistances != null && shield.statusEffectBlockResistances.Length > 0)
            {
                CreateTooltip(statusEffectsSprite, Color.white, shield.GetFormattedStatusResistances());
            }

            if (shield.statusEffectDelayRates != null && shield.statusEffectDelayRates.Length > 0)
            {
                CreateTooltip(statusEffectsSprite, Color.white, shield.GetFormattedStatusCancellationRates());
            }

            if (shield.poiseBonus != 0)
            {
                CreatePoiseTooltip(shield.poiseBonus);
            }

            if (shield.postureBonus != 0)
            {
                CreatePostureTooltip(shield.postureBonus);
            }

            if (shield.postureDamageAbsorption != 1)
            {

                CreateTooltip(
                    postureSprite,
                    Color.white,
                    String.Format(
                        postureDamageAbsorptionLabel.GetLocalizedString(),
                         100 - (shield.postureDamageAbsorption * 100)));

            }
            if (shield.slashDamageAbsorption != 1)
            {
                CreateTooltip(
                    slashSprite,
                    Color.white,
                    String.Format(
                        slashDamageAbsorptionLabel.GetLocalizedString(),
                         100 - (shield.slashDamageAbsorption * 100)));
            }
            if (shield.pierceDamageAbsorption != 1)
            {
                CreateTooltip(
                    pierceSprite,
                    Color.white,
                    String.Format(
                        pierceDamageAbsorptionLabel.GetLocalizedString(),
                         100 - (shield.pierceDamageAbsorption * 100)));
            }
            if (shield.bluntDamageAbsorption != 1)
            {
                CreateTooltip(
                    bluntSprite,
                    Color.white,
                    String.Format(
                        bluntDamageAbsorptionLabel.GetLocalizedString(),
                         100 - (shield.bluntDamageAbsorption * 100)));
            }

            if (shield.canDamageEnemiesOnShieldAttack)
            {

                if (shield.damageDealtToEnemiesUponBlocking.physical != 0)
                {
                    CreateTooltip(
                        weaponPhysicalAttackSprite,
                        Color.white,
                        String.Format(
                            physicalDmgDealtToEnemiesPerBlockLabel.GetLocalizedString(),
                            shield.damageDealtToEnemiesUponBlocking.physical));
                }

                if (shield.damageDealtToEnemiesUponBlocking.fire != 0)
                {
                    CreateTooltip(
                        fireSprite,
                        fire,
                        String.Format(
                            fireDmgDealtToEnemiesPerBlockLabel.GetLocalizedString(),
                            shield.damageDealtToEnemiesUponBlocking.fire));
                }

                if (shield.damageDealtToEnemiesUponBlocking.frost != 0)
                {

                    CreateTooltip(
                        frostSprite,
                        frost,
                        String.Format(
                            frostDmgDealtToEnemiesPerBlockLabel.GetLocalizedString(),
                            shield.damageDealtToEnemiesUponBlocking.frost));

                }

                if (shield.damageDealtToEnemiesUponBlocking.lightning != 0)
                {
                    CreateTooltip(
                        lightningSprite,
                        lightning,
                        String.Format(
                            lightningDmgDealtToEnemiesPerBlockLabel.GetLocalizedString(),
                            shield.damageDealtToEnemiesUponBlocking.lightning));
                }

                if (shield.damageDealtToEnemiesUponBlocking.magic != 0)
                {
                    CreateTooltip(
                        magicSprite,
                        magic,
                        String.Format(
                            magicDmgDealtToEnemiesPerBlockLabel.GetLocalizedString(),
                            shield.damageDealtToEnemiesUponBlocking.magic));
                }

                if (shield.damageDealtToEnemiesUponBlocking.darkness != 0)
                {
                    CreateTooltip(
                        darknessSprite,
                        darkness,
                        String.Format(
                            darknessDmgDealtToEnemiesPerBlockLabel.GetLocalizedString(),
                            shield.damageDealtToEnemiesUponBlocking.darkness));
                }

                if (shield.damageDealtToEnemiesUponBlocking.water != 0)
                {
                    CreateTooltip(
                        waterSprite,
                        water,
                        String.Format(
                            darknessDmgDealtToEnemiesPerBlockLabel.GetLocalizedString(),
                            shield.damageDealtToEnemiesUponBlocking.water));
                }

                if (shield.damageDealtToEnemiesUponBlocking.statusEffects != null && shield.damageDealtToEnemiesUponBlocking.statusEffects.Length > 0)
                {
                    CreateTooltip(statusEffectsSprite, Color.white, shield.GetFormattedStatusAttacks());
                }
            }

            if (shield.parryWindowBonus != 0)
            {

                CreateTooltip(
                    defenseAbsorptionSprite,
                    Color.white,
                    String.Format(
                        parryWindowDurationBonusLabel.GetLocalizedString(),
                        shield.parryWindowBonus));
            }

            if (shield.parryPostureDamageBonus != 0)
            {
                CreateTooltip(
                    defenseAbsorptionSprite,
                    Color.white,
                    String.Format(
                        postureDamagePerParryLabel.GetLocalizedString(),
                        shield.parryPostureDamageBonus));
            }

            if (shield.vitalityBonus != 0)
            {
                CreateTooltip(
                    vitalitySprite,
                    Color.white,
                    String.Format(
                        vitalityBonus.GetLocalizedString(),
                        shield.vitalityBonus));
            }

            if (shield.enduranceBonus != 0)
            {
                CreateTooltip(
                    enduranceSprite,
                    Color.white,
                    String.Format(
                        enduranceBonus.GetLocalizedString(),
                        shield.enduranceBonus));
            }

            if (shield.intelligenceBonus != 0)
            {
                CreateTooltip(
                    intelligenceSprite,
                    Color.white,
                    String.Format(
                        intelligenceBonus.GetLocalizedString(),
                        shield.intelligenceBonus));
            }

            if (shield.staminaRegenBonus != 1)
            {
                CreateTooltip(
                    staminaCostSprite,
                    Color.white,
                    String.Format(
                        staminaRegenSpeedBonus.GetLocalizedString(),
                        shield.staminaRegenBonus));
            }
        }



        void DrawAccessory(Accessory accessory)
        {
            if (accessory.GetShortDescription() != null && accessory.GetShortDescription().Length > 0)
            {
                CreateTooltip(statusEffectsSprite, Color.white, accessory.GetShortDescription());
            }

            if (accessory.healthBonus > 0)
            {
                CreateTooltip(
                    vitalitySprite,
                    Color.white,
                    String.Format(
                        healthPoints.GetLocalizedString(),
                        accessory.healthBonus
                ));
            }
            if (accessory.magicBonus > 0)
            {
                CreateTooltip(
                    magicSprite,
                    Color.white,
                    String.Format(
                        manaPoints.GetLocalizedString(),
                        accessory.magicBonus
                ));
            }
            if (accessory.staminaBonus > 0)
            {
                CreateTooltip(
                    enduranceSprite,
                    Color.white,
                    String.Format(
                        staminaPoints.GetLocalizedString(),
                        accessory.staminaBonus
                ));
            }
            if (accessory.physicalAttackBonus > 0)
            {
                CreateTooltip(
                    weaponPhysicalAttackSprite,
                    Color.white,
                    String.Format(
                        physicalAttackDamage.GetLocalizedString(),
                        accessory.physicalAttackBonus
                ));
            }
            if (accessory.jumpAttackBonus > 0)
            {
                CreateTooltip(
                    weaponPhysicalAttackSprite,
                    Color.white,
                    String.Format(
                        jumpAttackDamage.GetLocalizedString(),
                        accessory.jumpAttackBonus
                ));
            }
            if (accessory.increaseAttackPowerWithLowerHealth)
            {
                CreateTooltip(weaponPhysicalAttackSprite, Color.white, attackIncreasesWithLowerHealth.GetLocalizedString());
            }
            if (accessory.increaseAttackPowerTheLowerTheReputation)
            {
                CreateTooltip(weaponPhysicalAttackSprite, Color.white, attackIncreasesWithLowerReputation.GetLocalizedString());
            }
            if (accessory.postureDamagePerParry > 0)
            {
                CreateTooltip(
                    postureSprite,
                    Color.white,
                    String.Format(
                        postureDamagePerParryLabel.GetLocalizedString(),
                        accessory.postureDamagePerParry
                ));
            }
            if (accessory.spellDamageBonusMultiplier > 0)
            {
                CreateTooltip(
                    magicSprite,
                    Color.white,
                    String.Format(
                        moreSpellDamage.GetLocalizedString(),
                        accessory.spellDamageBonusMultiplier
                ));
            }
            if (accessory.chanceToDoubleCoinsFromFallenEnemies)
            {
                CreateTooltip(goldCoinSprite, Color.white, chanceToDoubleCoinsFromFallenEnemies.GetLocalizedString());
            }
            if (accessory.postureDecreaseRateBonus > 0)
            {
                CreateTooltip(
                    postureSprite,
                    Color.white,
                    String.Format(
                        postureDecreaseRateBonus.GetLocalizedString(),
                        accessory.postureDecreaseRateBonus
                ));
            }
            if (accessory.backStabAngleBonus > 0)
            {
                CreateTooltip(
                    postureSprite,
                    Color.white,
                    String.Format(
                        backStabAngleBonus.GetLocalizedString(),
                        accessory.backStabAngleBonus
                ));
            }
            if (accessory.twoHandAttackBonusMultiplier > 0)
            {
                CreateTooltip(
                    slashSprite,
                    Color.white,
                    "x" + accessory.twoHandAttackBonusMultiplier + "% " + multiplierWhenTwoHanding.GetLocalizedString()
                );
            }
        }


        void DrawCraftingMaterial(CraftingMaterial craftingMaterial)
        {
            if (craftingMaterial.GetShortDescription() != null && craftingMaterial.GetShortDescription().Length > 0)
            {
                CreateTooltip(statusEffectsSprite, Color.white, craftingMaterial.GetShortDescription());
            }

            CreateTooltip(craftingMaterialSprite, Color.white, craftingMaterialLabel.GetLocalizedString());

            if (CraftingUtils.IsItemAnIngredientOfCurrentLearnedRecipes(uIDocumentCraftScreen, craftingMaterial))
            {
                CraftingRecipe[] craftingRecipes = CraftingUtils.GetRecipesUsingItem(uIDocumentCraftScreen, craftingMaterial).ToArray();
                if (craftingRecipes != null && craftingRecipes.Length > 0)
                {
                    CreateTooltip(craftingMaterialSprite, Color.white, CraftingUtils.GetFormattedTextForRecipesUsingItem(craftingRecipes));
                }
            }
        }

        void DrawUpgradeMaterial(UpgradeMaterial upgradeMaterial)
        {
            if (upgradeMaterial.GetShortDescription() != null && upgradeMaterial.GetShortDescription().Length > 0)
            {
                CreateTooltip(statusEffectsSprite, Color.white, upgradeMaterial.GetShortDescription());
            }

            CreateTooltip(upgradeMaterialSprite, Color.white, upgradeMaterialLabel.GetLocalizedString());
        }

        void DrawConsumable(Consumable consumable)
        {
            if (consumable.GetShortDescription() != null && consumable.GetShortDescription().Length > 0)
            {
                CreateTooltip(statusEffectsSprite, Color.white, consumable.GetShortDescription());
            }
            if (consumable.statusesToRemove != null && consumable.statusesToRemove.Length > 0)
            {
                CreateTooltip(statusEffectsSprite, Color.white, consumable.GetFormattedRemovedStatusEffects());
            }
            if (consumable.statusEffectsWhenConsumed != null && consumable.statusEffectsWhenConsumed.Length > 0)
            {
                string consumableText = consumable.GetFormattedAppliedStatusEffects();

                if (string.IsNullOrEmpty(consumableText) == false)
                {
                    CreateTooltip(statusEffectsSprite, Color.white, consumableText);
                }
            }
            if (consumable.isBossToken)
            {
                CreateTooltip(bossTokenSprite, Color.white, bossTokenLabel.GetLocalizedString());
            }
            if (consumable.canBeConsumedForGold)
            {
                CreateTooltip(goldCoinSprite, Color.white, String.Format(consumeToReceive.GetLocalizedString(), consumable.GetValue()));
            }
            if (consumable.shouldNotRemoveOnUse)
            {
                CreateTooltip(replenishableSprite, Color.white, itemUsageReplenishesWhenRestingAtABonfire.GetLocalizedString());
            }
        }

        void DrawArrow(Arrow arrow)
        {
            if (arrow.damage.physical > 0)
            {
                CreateTooltip(weaponPhysicalAttackSprite, Color.white, TooltipUtils.GetArrowPhysicalDamage(arrow.damage.physical));
            }

            if (arrow.damage.fire > 0)
            {
                CreateTooltip(
                    fireSprite,
                    fire,
                    String.Format(
                        fireAttackLabel.GetLocalizedString(),
                        arrow.damage.fire));
            }

            if (arrow.damage.frost > 0)
            {
                CreateTooltip(
                    frostSprite,
                    frost,
                    String.Format(
                        frostAttackLabel.GetLocalizedString(),
                        arrow.damage.frost));
            }

            if (arrow.damage.lightning > 0)
            {
                CreateTooltip(
                lightningSprite,
                lightning,
                TooltipUtils.GetArrowLightiningDamage(arrow.damage.lightning));
            }

            if (arrow.damage.darkness > 0)
            {
                CreateTooltip(
                darknessSprite,
                magic,
                TooltipUtils.GetArrowDarknessDamage(arrow.damage.darkness));
            }

            if (arrow.damage.magic > 0)
            {
                CreateTooltip(
                magicSprite,
                magic,
                TooltipUtils.GetArrowMagicDamage(arrow.damage.magic));
            }

            DrawStatusEffects(arrow.damage);

            DrawSelfInflictedStatusEffectsForShootingArrow(arrow);

            if (arrow.damage.pushForce > 0)
            {
                CreateTooltip(pushForceSprite, Color.white, String.Format(
                    pushForceLabel.GetLocalizedString(), arrow.damage.pushForce));
            }

            if (arrow.damage.postureDamage > 0)
            {
                CreateTooltip(postureSprite, Color.white, String.Format(
                    postureDamageLabel.GetLocalizedString(), arrow.damage.postureDamage));
            }

            if (arrow.damage.ignoreBlocking)
            {
                CreateTooltip(defenseAbsorptionSprite, Color.white, ignoresEnemyShields.GetLocalizedString());
            }

            if (arrow.damage.canNotBeParried)
            {
                CreateTooltip(defenseAbsorptionSprite, Color.white, canNotBeParried.GetLocalizedString());
            }
        }

        public void DrawStatusEffects(Damage damage)
        {
            if (damage.statusEffects != null && damage.statusEffects.Length > 0)
            {
                foreach (var statusEffect in damage.statusEffects)
                {
                    if (statusEffect != null)
                    {
                        string prefix = Utils.IsPortuguese() ? "de" : "of";
                        string suffix = Utils.IsPortuguese() ? " infligido" : " inflicted";
                        string text = $"+{statusEffect.amountPerHit} {prefix} {statusEffect.statusEffect.GetName()} {suffix}\n";
                        CreateTooltip(Utils.SpriteToTexture2D(statusEffect.statusEffect.icon), statusEffect.statusEffect.barColor, text);
                    }
                }
            }
        }

        void DrawSelfInflictedStatusEffectsForShootingArrow(Arrow arrow)
        {
            if (arrow.statusEffectsInflictedUponShootingArrow != null && arrow.statusEffectsInflictedUponShootingArrow.Length > 0)
            {
                foreach (var statusEffect in arrow.statusEffectsInflictedUponShootingArrow)
                {
                    if (statusEffect != null)
                    {
                        string prefix = Utils.IsPortuguese() ? "de" : "of";
                        string suffix = Utils.IsPortuguese() ? " auto-infligido ao disparar flecha" : " self-inflicted upon firing arrow";
                        string text = $"+{statusEffect.amountPerHit} {prefix} {statusEffect.statusEffect.GetName()} {suffix}\n";
                        CreateTooltip(Utils.SpriteToTexture2D(statusEffect.statusEffect.icon), statusEffect.statusEffect.barColor, text);
                    }
                }
            }
        }


        public void CreateTooltip(Texture2D sprite, Color color, string description)
        {
            VisualElement clone = itemEffectTooltipEntry.CloneTree();

            VisualElement icon = clone.Q<VisualElement>("Icon");
            icon.style.backgroundImage = new StyleBackground(sprite);
            icon.style.unityBackgroundImageTintColor = color;
            icon.style.borderTopColor = color;
            icon.style.borderLeftColor = color;
            icon.style.borderRightColor = color;
            icon.style.borderBottomColor = color;

            Label text = clone.Q<Label>();
            text.text = description;
            text.style.color = color;

            tooltipEffectsContainer.Add(clone);
        }
    }
}
