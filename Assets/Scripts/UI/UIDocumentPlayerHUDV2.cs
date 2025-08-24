using System.Collections.Generic;
using System.Linq;
using AF.Events;
using AF.Inventory;
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
        Label healthCurrentValue;
        VisualElement staminaContainer;
        VisualElement staminaFill;
        Label staminaCurrentValue;
        VisualElement manaContainer;
        VisualElement manaFill;
        Label manaCurrentValue;

        [Header("Graphic Settings")]
        public float healthContainerBaseWidth = 180;
        public float staminaContainerBaseWidth = 150;
        public float manaContainerBaseWidth = 150;

        Label quickItemName, abilityName;
        IMGUIContainer shieldBlockedIcon;

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;
        public InventoryDatabase inventoryDatabase;
        public PlayerStatsDatabase playerStatsDatabase;
        public QuestsDatabase questsDatabase;
        public GameSettings gameSettings;
        public QuestManager questManager;

        [Header("Unequipped Textures")]
        public Texture2D unequippedSpellSlot;
        public Texture2D unequippedWeaponSlot;
        public Texture2D unequippedConsumableSlot;
        public Texture2D unequippedShieldSlot;
        public Texture2D unequippedArrowSlot;

        [Header("Combat Stance Icons")]
        public Sprite oneHandIcon;
        public Sprite twoHandIcon;
        VisualElement combatStanceSprite;

        [Header("Components")]
        public PlayerManager playerManager;

        UIGameControls uIGameControls => GetComponent<UIGameControls>();

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
                OnEquipmentChanged);

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

            EventManager.StartListening(EventMessages.ON_PLAYER_HEALTH_CHANGED, OnHealthChanged);
            EventManager.StartListening(EventMessages.ON_PLAYER_MANA_CHANGED, OnManaChanged);
            EventManager.StartListening(EventMessages.ON_PLAYER_STAMINA_CHANGED, OnStaminaChanged);
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
            healthFill = healthContainer.Q<VisualElement>("Fill");
            healthCurrentValue = healthContainer.Q<Label>("Value");
            staminaContainer = root.Q<VisualElement>("Stamina");
            staminaFill = staminaContainer.Q<VisualElement>("Fill");
            staminaCurrentValue = staminaContainer.Q<Label>("Value");
            manaContainer = root.Q<VisualElement>("Mana");
            manaFill = manaContainer.Q<VisualElement>("Fill");
            manaCurrentValue = manaContainer.Q<Label>("Value");

            quickItemName = root.Q<Label>("QuickItemName");
            abilityName = root.Q<Label>("AbilityName");

            spellSlotContainer = root.Q<IMGUIContainer>("SpellSlot");
            consumableSlotContainer = root.Q<IMGUIContainer>("ConsumableSlot");
            weaponSlotContainer = root.Q<IMGUIContainer>("WeaponSlot");
            shieldSlotContainer = root.Q<IMGUIContainer>("ShieldSlot");

            shieldBlockedIcon = shieldSlotContainer.Q<IMGUIContainer>("Blocked");

            equipmentContainer = root.Q<VisualElement>("EquipmentContainer");


            combatStanceIndicatorLabel = root.Q<Label>("CombatStanceIndicator");
            combatStanceSprite = root.Q<VisualElement>("CombatStanceSprite");

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

            OnHealthChanged();
            OnManaChanged();
            OnStaminaChanged();
        }

        void UpdateCombatStanceIndicator()
        {
            if (equipmentDatabase.isTwoHanding)
            {
                combatStanceIndicatorLabel.text = twoHandIndicator_LocalizedString.GetLocalizedString();
                combatStanceSprite.style.backgroundImage = new StyleBackground(twoHandIcon);
            }
            else
            {
                combatStanceIndicatorLabel.text = oneHandIndicator_LocalizedString.GetLocalizedString();
                combatStanceSprite.style.backgroundImage = new StyleBackground(oneHandIcon);
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

            uIGameControls.UpdateFooterButtons();
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


        void OnHealthChanged()
        {
            int width = (int)healthContainerBaseWidth;

            width += playerManager.playerStats.GetVitality() * 10;
            healthContainer.style.width = width;

            healthFill.style.width = new Length(playerManager.health.GetCurrentHealthPercentage(), LengthUnit.Percent);
            healthCurrentValue.text = $"{Mathf.RoundToInt(playerManager.health.GetCurrentHealth())}/{playerManager.health.GetMaxHealth()}";
        }

        void OnManaChanged()
        {
            int width = (int)manaContainerBaseWidth;

            width += playerManager.playerStats.GetIntelligence() * 10;
            manaContainer.style.width = width;

            manaFill.style.width = new Length(playerManager.manaManager.GetCurrentManaPercentage(), LengthUnit.Percent);
            manaCurrentValue.text = $"{Mathf.RoundToInt(playerManager.manaManager.GetCurrentMana())}/{playerManager.manaManager.GetMaxMana()}";
        }

        void OnStaminaChanged()
        {
            int width = (int)staminaContainerBaseWidth;

            width += playerManager.playerStats.GetEndurance() * 10;
            staminaContainer.style.width = width;

            staminaFill.style.width = new Length(playerManager.staminaStatManager.GetCurrentStaminaPercentage(), LengthUnit.Percent);
            staminaCurrentValue.text = $"{Mathf.RoundToInt(playerManager.staminaStatManager.GetCurrentStamina())}/{playerManager.staminaStatManager.GetMaxStamina()}";
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

        public void UpdateHealthManaAndStaminaBars()
        {
            OnHealthChanged();
            OnManaChanged();
            OnStaminaChanged();
        }

        void OnEquipmentChanged()
        {
            UpdateEquipment();
        }

        public void UpdateEquipment()
        {
            if (!this.isActiveAndEnabled)
            {
                return;
            }

            quickItemName.text = "";

            UpdateSpellSlot();

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
                equipmentDatabase.GetCurrentConsumable().GetName() + $" ({playerManager.playerInventory.GetAvailableConsumables(equipmentDatabase.GetCurrentConsumable()).Count})"
                : "";


            bool hasConsumable = equipmentDatabase.GetCurrentConsumable() != null;

            consumableSlotContainer.style.backgroundImage = hasConsumable
                ? new StyleBackground(equipmentDatabase.GetCurrentConsumable().sprite)
                : new StyleBackground(unequippedConsumableSlot);

            root.Q("ConsumableInfo").style.display = hasConsumable ? DisplayStyle.Flex : DisplayStyle.None;
        }

        void UpdateSpellSlot()
        {
            abilityName.text = "";
            spellSlotContainer.style.backgroundImage = new StyleBackground(unequippedSpellSlot);

            Spell currentSpell = equipmentDatabase.GetCurrentSpell();
            if (currentSpell != null)
            {
                spellSlotContainer.style.backgroundImage = new StyleBackground(equipmentDatabase.GetCurrentSpell().sprite);
                abilityName.text = currentSpell.GetName();
                root.Q<VisualElement>("AbilityInfo").style.display = DisplayStyle.Flex;
            }
            else
            {
                root.Q<VisualElement>("AbilityInfo").style.display = DisplayStyle.None;
            }
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

            if (root.style.opacity.value < 1)
            {
                return false;
            }

            return equipmentContainer.visible;
        }

        void UpdateQuestTracking()
        {
            root.Q("CurrentObjectives").style.display = questManager.GetTrackedQuests().Count > 0 ? DisplayStyle.Flex : DisplayStyle.None;

            var highlightedMissionsContainer = root.Q("HighlightedMissions");
            highlightedMissionsContainer.Clear();

            List<QuestParent> trackedQuests = questManager.GetTrackedQuests();
            foreach (QuestParent trackedQuest in trackedQuests)
            {
                VisualElement clone = highlightedMissinEntry.CloneTree();

                if (trackedQuest.GetCurrentObjective() != null)
                {
                    clone.Q<Label>("QuestObjective").text = trackedQuest.GetCurrentObjective().GetDescription();
                }

                clone.Q<Label>("QuestType").text = trackedQuest.questName_LocalizedString.GetLocalizedString();
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
                DOTween.To(() => (Color)target.style.unityBackgroundImageTintColor.value,
                           x => target.style.unityBackgroundImageTintColor = new StyleColor(x),
                           blinkColor, 0.5f)
                       .SetEase(Ease.InOutFlash))
                       .OnComplete(() =>
                       {
                           target.style.unityBackgroundImageTintColor = originalColor;
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
