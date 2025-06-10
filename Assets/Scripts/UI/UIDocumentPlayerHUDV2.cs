using System;
using System.Collections.Generic;
using System.Linq;
using AF.Events;
using AF.Inventory;
using AF.Stats;
using DG.Tweening;
using TigerForge;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace AF
{

    public class UIDocumentPlayerHUDV2 : MonoBehaviour
    {

        UIDocument uIDocument => GetComponent<UIDocument>();
        public VisualElement root;

        VisualElement healthContainer;
        VisualElement healthFill;
        VisualElement staminaContainer;
        VisualElement staminaFill;
        VisualElement manaContainer;
        VisualElement manaFill;

        [Header("Graphic Settings")]
        public float healthContainerBaseWidth = 180;
        public float staminaContainerBaseWidth = 150;
        public float manaContainerBaseWidth = 150;
        float _containerMultiplierPerLevel = 10f;

        Label quickItemName, arrowsLabel;
        IMGUIContainer shieldBlockedIcon;


        [Header("Components")]
        public StatsBonusController playerStatsBonusController;

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;
        public InventoryDatabase inventoryDatabase;
        public PlayerStatsDatabase playerStatsDatabase;
        public QuestsDatabase questsDatabase;
        public GameSettings gameSettings;

        [Header("Unequipped Textures")]
        public Texture2D unequippedSpellSlot;
        public Texture2D unequippedWeaponSlot;
        public Texture2D unequippedConsumableSlot;
        public Texture2D unequippedShieldSlot;
        public Texture2D unequippedArrowSlot;

        [Header("Components")]
        public PlayerManager playerManager;
        public EquipmentGraphicsHandler equipmentGraphicsHandler;

        IMGUIContainer spellSlotContainer, consumableSlotContainer, weaponSlotContainer, shieldSlotContainer;

        [Header("Animations")]
        public Vector3 popEffectWhenSwitchingSlots = new Vector3(0.8f, 0.8f, 0.8f);

        VisualElement equipmentContainer;

        Label combatStanceIndicatorLabel;

        public StarterAssetsInputs starterAssetsInputs;

        [Header("Localization")]
        public LocalizedString oneHandIndicator_LocalizedString;
        public LocalizedString twoHandIndicator_LocalizedString;

        PlayerHealth playerHealth;

        public Color staminaOriginalColor;
        public Color manaOriginalColor;

        public Color highlightColor;

        [Header("Scenes To Show In-Game Controls")]
        public List<string> scenesToDisplayInGameControls = new();

        [Header("Quest Objectives")]
        [SerializeField] VisualTreeAsset highlightedMissinEntry;

        private void Awake()
        {
            EventManager.StartListening(
                EventMessages.ON_EQUIPMENT_CHANGED,
                UpdateEquipment);

            EventManager.StartListening(
                EventMessages.ON_TWO_HANDING_CHANGED,
                UpdateEquipment);

            EventManager.StartListening(
                EventMessages.ON_QUEST_TRACKED,
                UpdateQuestTracking);

            EventManager.StartListening(
                EventMessages.ON_QUESTS_PROGRESS_CHANGED,
                UpdateQuestTracking);

            EventManager.StartListening(EventMessages.ON_PLAYER_HUD_VISIBILITY_CHANGED, EvaluatePlayerHUD);

            EventManager.StartListening(EventMessages.ON_TWO_HANDING_CHANGED, UpdateCombatStanceIndicator);

        }

        void EvaluatePlayerHUD()
        {
        }

        private void OnEnable()
        {
            playerHealth = playerManager.health as PlayerHealth;
            this.root = this.uIDocument.rootVisualElement;

            root.Q("InGameControls").style.display =
                scenesToDisplayInGameControls.Contains(SceneManager.GetActiveScene().name)
                ? DisplayStyle.Flex : DisplayStyle.None;

            healthContainer = root.Q<VisualElement>("Health");
            healthFill = root.Q<VisualElement>("HealthFill");
            staminaContainer = root.Q<VisualElement>("Stamina");
            staminaFill = root.Q<VisualElement>("StaminaFill");
            manaContainer = root.Q<VisualElement>("Mana");
            manaFill = root.Q<VisualElement>("ManaFill");

            quickItemName = root.Q<Label>("QuickItemName");
            arrowsLabel = root.Q<Label>("ArrowsLabel");

            spellSlotContainer = root.Q<IMGUIContainer>("SpellSlot");
            consumableSlotContainer = root.Q<IMGUIContainer>("ConsumableSlot");
            weaponSlotContainer = root.Q<IMGUIContainer>("WeaponSlot");
            shieldSlotContainer = root.Q<IMGUIContainer>("ShieldSlot");

            shieldBlockedIcon = shieldSlotContainer.Q<IMGUIContainer>("Blocked");

            equipmentContainer = root.Q<VisualElement>("EquipmentContainer");


            combatStanceIndicatorLabel = root.Q<Label>("CombatStanceIndicator");

            root.Q<VisualElement>("SwimmingIndicator").style.display = playerManager.thirdPersonController.water != null ? DisplayStyle.Flex : DisplayStyle.None;

            InputSystem.onDeviceChange += HandleDeviceChangeCallback;

            Load();
        }

        void Load()
        {
            UpdateEquipment();
            UpdateQuestTracking();
            EvaluatePlayerHUD();
            UpdateCombatStanceIndicator();
        }

        void UpdateCombatStanceIndicator()
        {
            if (equipmentDatabase.isTwoHanding)
            {
                combatStanceIndicatorLabel.text = twoHandIndicator_LocalizedString.GetLocalizedString();
            }
            else
            {
                combatStanceIndicatorLabel.text = oneHandIndicator_LocalizedString.GetLocalizedString();
            }
        }

        private void OnDisable()
        {
            InputSystem.onDeviceChange -= HandleDeviceChangeCallback;
        }

        void HandleDeviceChangeCallback(InputDevice device, InputDeviceChange change)
        {
            HandleDeviceChange();
        }

        void HandleDeviceChange()
        {
            UpdateEquipment();
        }


        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="value"></param>
        public void SetHUD_RootOpacity(float value)
        {
            root.style.opacity = value;
        }

        public void HideHUD()
        {
            SetHUD_RootOpacity(0);
        }

        public void ShowHUD()
        {
            SetHUD_RootOpacity(1);
        }

        public void FadeIn()
        {
            root.style.opacity = 0;
            root.style.display = DisplayStyle.Flex;
            DOTween.To(() => root.style.opacity.value, x => root.style.opacity = x, 1, 0.5f);
        }

        public void FadeOut()
        {
            root.style.opacity = 0;
            root.style.display = DisplayStyle.Flex;
            DOTween.To(() => root.style.opacity.value, x => root.style.opacity = x, 0, 0.5f);
        }

        private void Update()
        {
            healthContainer.style.width = (healthContainerBaseWidth +
                playerStatsBonusController.GetCurrentVitality() * _containerMultiplierPerLevel) * (playerHealth.hasHealthCutInHalf ? .5f : 1f);

            staminaContainer.style.width = staminaContainerBaseWidth + ((
                playerStatsBonusController.GetCurrentEndurance()) * _containerMultiplierPerLevel);

            manaContainer.style.width = manaContainerBaseWidth + ((
                playerStatsBonusController.GetCurrentIntelligence()) * _containerMultiplierPerLevel);

            this.healthFill.style.width = new Length(playerManager.health.GetCurrentHealthPercentage() * ((playerHealth.hasHealthCutInHalf ? .5f : 1f)), LengthUnit.Percent);
            this.staminaFill.style.width = new Length(playerManager.staminaStatManager.GetCurrentStaminaPercentage(), LengthUnit.Percent);
            this.manaFill.style.width = new Length(playerManager.manaManager.GetCurrentManaPercentage(), LengthUnit.Percent);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ShowEquipment()
        {
            equipmentContainer.visible = true;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void HideEquipment()
        {
            equipmentContainer.visible = false;
        }

        public void UpdateEquipment()
        {
            if (!this.isActiveAndEnabled)
            {
                return;
            }

            quickItemName.text = "";
            arrowsLabel.text = "";

            if (equipmentDatabase.IsRangeWeaponEquipped())
            {
                arrowsLabel.text = equipmentDatabase.GetCurrentArrow() != null
                    ? equipmentDatabase.GetCurrentArrow().GetName() + " (" + inventoryDatabase.GetItemAmount(equipmentDatabase.GetCurrentArrow()) + ")"
                    : "";

                spellSlotContainer.style.backgroundImage = equipmentDatabase.GetCurrentArrow() != null
                    ? new StyleBackground(equipmentDatabase.GetCurrentArrow().sprite)
                    : new StyleBackground(unequippedArrowSlot);
            }
            else
            {
                spellSlotContainer.style.backgroundImage = equipmentDatabase.GetCurrentSpell() != null
                    ? new StyleBackground(equipmentDatabase.GetCurrentSpell().sprite)
                    : new StyleBackground(unequippedSpellSlot);
            }

            shieldSlotContainer.style.backgroundImage = equipmentDatabase.GetCurrentLeftWeapon() != null
                ? new StyleBackground(equipmentDatabase.GetCurrentLeftWeapon().sprite)
                : new StyleBackground(unequippedShieldSlot);

            shieldSlotContainer.style.opacity = equipmentDatabase.isTwoHanding ? .25f : 1;

            /*
            shieldBlockedIcon.style.display = equipmentDatabase.IsRangeWeaponEquipped() || equipmentDatabase.IsStaffEquipped()
                ? DisplayStyle.Flex
                : DisplayStyle.None;*/

            weaponSlotContainer.style.backgroundImage = equipmentDatabase.GetCurrentWeapon() != null
                ? new StyleBackground(equipmentDatabase.GetCurrentWeapon().sprite)
                : new StyleBackground(unequippedWeaponSlot);

            quickItemName.text = equipmentDatabase.GetCurrentConsumable() != null ?
                equipmentDatabase.GetCurrentConsumable().GetName() + $" ({inventoryDatabase.GetItemAmount(equipmentDatabase.GetCurrentConsumable())})"
                : "";

            if (equipmentDatabase.GetCurrentConsumable() is Card)
            {
                consumableSlotContainer.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
                consumableSlotContainer.style.borderTopWidth = 0;
                consumableSlotContainer.style.borderBottomWidth = 0;
                consumableSlotContainer.style.borderLeftWidth = 0;
                consumableSlotContainer.style.borderRightWidth = 0;

            }
            else
            {
                consumableSlotContainer.style.unityBackgroundScaleMode = ScaleMode.ScaleAndCrop;
                consumableSlotContainer.style.borderTopWidth = new StyleFloat(1);
                consumableSlotContainer.style.borderBottomWidth = new StyleFloat(1);
                consumableSlotContainer.style.borderLeftWidth = new StyleFloat(1);
                consumableSlotContainer.style.borderRightWidth = new StyleFloat(1);
            }

            bool hasConsumable = equipmentDatabase.GetCurrentConsumable() != null;

            consumableSlotContainer.style.backgroundImage = hasConsumable
                ? new StyleBackground(equipmentDatabase.GetCurrentConsumable().sprite)
                : new StyleBackground(unequippedConsumableSlot);

            root.Q("ConsumableInfo").style.display = hasConsumable ? DisplayStyle.Flex : DisplayStyle.None;
        }

        public void OnSwitchWeapon()
        {
            UIUtils.PlayPopAnimation(weaponSlotContainer, popEffectWhenSwitchingSlots);
            UpdateEquipment();
        }
        public void OnSwitchShield()
        {
            UIUtils.PlayPopAnimation(shieldSlotContainer, popEffectWhenSwitchingSlots);
            UpdateEquipment();
        }
        public void OnSwitchConsumable()
        {
            UIUtils.PlayPopAnimation(consumableSlotContainer, popEffectWhenSwitchingSlots);
            UpdateEquipment();
        }
        public void OnSwitchSpell()
        {
            UIUtils.PlayPopAnimation(spellSlotContainer, popEffectWhenSwitchingSlots);
            UpdateEquipment();
        }

        public bool IsEquipmentDisplayed()
        {
            if (!root.visible)
            {
                return false;
            }

            return equipmentContainer.visible;
        }

        void UpdateQuestTracking()
        {
            root.Q("CurrentObjectives").style.display = questsDatabase.trackedQuests.Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;

            var highlightedMissionsContainer = root.Q("HighlightedMissions");
            highlightedMissionsContainer.Clear();

            foreach (QuestParent trackedQust in questsDatabase.trackedQuests)
            {
                VisualElement clone = highlightedMissinEntry.CloneTree();

                if (trackedQust.questProgress < trackedQust.questObjectives_LocalizedString.Length)
                {
                    clone.Q<Label>("QuestObjective").text = trackedQust.questObjectives_LocalizedString[trackedQust.questProgress].GetLocalizedString();
                }

                clone.Q<Label>("QuestType").text = trackedQust.questName_LocalizedString.GetLocalizedString();
                highlightedMissionsContainer.Add(clone);
            }
        }

        public void DisplayInsufficientStamina()
        {
            DisplayInsufficientBarBackgroundColor(staminaOriginalColor, staminaFill, staminaContainer);
        }

        public void DisplayInsufficientMana()
        {
            DisplayInsufficientBarBackgroundColor(manaOriginalColor, manaFill, manaContainer);
        }

        void DisplayInsufficientBarBackgroundColor(Color originalColor, VisualElement target, VisualElement targetContainer)
        {
            Color blinkColor = Color.red; // Change to Color.grey if needed

            // Sequence for the blink effect
            Sequence blinkSequence = DOTween.Sequence();
            blinkSequence.Append(
                DOTween.To(() => (Color)target.style.backgroundColor.value,
                           x => target.style.backgroundColor = new StyleColor(x),
                           blinkColor, 0.5f)
                       .SetEase(Ease.InOutFlash))
                       .OnComplete(() =>
                       {
                           target.style.backgroundColor = originalColor;
                       });
        }

        public enum ControlKey
        {
            None,
            Move,
            Interact,
            Sprint,
            Jump,
            Dodge,
            ToggleHands,
            Attack,
            BlockParryAim,
            LockOn,
            HeavyAttack,
            MainMenu,
        }

        public void HighlightKey(ControlKey controlKey)
        {
        }

        public void DisableHighlights()
        {
        }

    }
}
