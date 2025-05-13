using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;
using UnityEngine.Localization;
using UnityEngine.InputSystem;
using UnityEditor;

namespace AF.UI
{
#if UNITY_EDITOR

    [CustomEditor(typeof(ViewMenu), editorForChildClasses: true)]
    public class ViewMenuEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            ViewMenu viewMenu = target as ViewMenu;

            if (GUILayout.Button("Show Keyboard Buttons"))
            {
                viewMenu.EnableKeyboardNavbarButtons();
            }
            if (GUILayout.Button("Show PS4 Buttons"))
            {
                viewMenu.EnablePS4NavbarButtons();
            }
            if (GUILayout.Button("Show Xbox Buttons"))
            {
                viewMenu.EnableXboxNavbarButtons();
            }
        }
    }
#endif


    public class ViewMenu : MonoBehaviour
    {
        [HideInInspector]
        public VisualElement root;
        public const string EQUIPMENT_BUTTON = "EquipmentButton";
        public const string OBJECTIVES_BUTTON = "ObjectivesButton";
        public const string OPTIONS_BUTTON = "OptionsGameButton";
        public const string LABEL_DESCRIPTOR = "Descriptor";

        Button equipmentButton;
        Button objectivesButton;
        Button optionsButton;
        public Button EquipmentButton => equipmentButton;
        public Button ObjectivesButton => objectivesButton;
        public Button OptionsButton => optionsButton;
        Label descriptor;

        // Components
        protected MenuManager menuManager;

        [Header("Components")]
        public CursorManager cursorManager;
        public Soundbank soundbank;
        public FadeManager fadeManager;
        public SaveManager saveManager;
        public GameSession gameSession;
        [SerializeField] StarterAssetsInputs starterAssetsInputs;

        [Header("Localization")]
        public LocalizedString equipment_LocalizedString;
        public LocalizedString quests_LocalizedString;
        public LocalizedString settings_LocalizedString;

        VisualElement previousKey, nextKey;

        // Components References
        protected VisualElement navbar;
        public const string NAVBAR_ID = "Navbar";

        protected virtual void OnEnable()
        {
            SetupRefs();

            equipmentButton.RemoveFromClassList("active");
            objectivesButton.RemoveFromClassList("active");
            optionsButton.RemoveFromClassList("active");

            switch (menuManager.viewMenuIndex)
            {
                case 0:
                    equipmentButton.AddToClassList("active");
                    descriptor.text = equipment_LocalizedString.GetLocalizedString();
                    break;
                case 1:
                    objectivesButton.AddToClassList("active");
                    descriptor.text = quests_LocalizedString.GetLocalizedString();
                    break;
                case 2:
                    optionsButton.AddToClassList("active");
                    descriptor.text = settings_LocalizedString.GetLocalizedString();
                    break;
                default:
                    return;
            }

            previousKey = root.Q<VisualElement>("PreviousKey");
            nextKey = root.Q<VisualElement>("NextKey");

            UpdateSwitchNavbarButtons();
        }

        void UpdateSwitchNavbarButtons()
        {
            if (starterAssetsInputs.IsPS4Controller())
            {
                EnablePS4NavbarButtons();
            }
            else if (starterAssetsInputs.IsXboxController() || starterAssetsInputs.IsGamepad())
            {
                EnableXboxNavbarButtons();
            }
            else
            {
                EnableKeyboardNavbarButtons();
            }
        }

        void DisableNavbarSwitchButtons()
        {
            previousKey.Q("Keyboard").style.display = DisplayStyle.None;
            previousKey.Q("PS4Button").style.display = DisplayStyle.None;
            previousKey.Q("XboxButton").style.display = DisplayStyle.None;

            nextKey.Q("Keyboard").style.display = DisplayStyle.None;
            nextKey.Q("PS4Button").style.display = DisplayStyle.None;
            nextKey.Q("XboxButton").style.display = DisplayStyle.None;
        }

        public void EnablePS4NavbarButtons()
        {
            DisableNavbarSwitchButtons();

            previousKey.Q("PS4Button").style.display = DisplayStyle.Flex;
            nextKey.Q("PS4Button").style.display = DisplayStyle.Flex;
        }
        public void EnableXboxNavbarButtons()
        {
            DisableNavbarSwitchButtons();

            previousKey.Q("XboxButton").style.display = DisplayStyle.Flex;
            nextKey.Q("XboxButton").style.display = DisplayStyle.Flex;
        }
        public void EnableKeyboardNavbarButtons()
        {
            DisableNavbarSwitchButtons();

            previousKey.Q("Keyboard").style.display = DisplayStyle.Flex;
            nextKey.Q("Keyboard").style.display = DisplayStyle.Flex;
        }

        private void Update()
        {
            if (UnityEngine.Cursor.visible == false)
            {
                cursorManager.ShowCursor();
            }
        }

        void SetupRefs()
        {
            menuManager = menuManager != null ? menuManager : FindAnyObjectByType<MenuManager>(FindObjectsInactive.Include);
            root = GetComponent<UIDocument>().rootVisualElement;

            if (root != null && menuManager.hasPlayedFadeIn == false)
            {
                menuManager.hasPlayedFadeIn = true;

                soundbank.PlaySound(soundbank.mainMenuOpen);

                DOTween.To(
                      () => root.contentContainer.style.opacity.value,
                      (value) => root.contentContainer.style.opacity = value,
                      1,
                      .25f
                );
            }

            descriptor = root.Q<Label>(LABEL_DESCRIPTOR);

            equipmentButton = root.Q<Button>(EQUIPMENT_BUTTON);
            UIUtils.SetupButton(equipmentButton, () =>
            {
                menuManager.viewMenuIndex = 0;
                menuManager.SetMenuView();
            }, soundbank);

            objectivesButton = root.Q<Button>(OBJECTIVES_BUTTON);
            UIUtils.SetupButton(objectivesButton, () =>
            {
                menuManager.viewMenuIndex = 1;
                menuManager.SetMenuView();
            }, soundbank);

            optionsButton = root.Q<Button>(OPTIONS_BUTTON);
            UIUtils.SetupButton(optionsButton, () =>
            {
                menuManager.viewMenuIndex = 2;
                menuManager.SetMenuView();
            }, soundbank);

            navbar = root.Q<VisualElement>(NAVBAR_ID);
        }
    }
}
