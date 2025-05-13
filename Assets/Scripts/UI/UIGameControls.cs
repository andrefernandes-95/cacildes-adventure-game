
using System.Collections.Generic;
using AF.Events;
using TigerForge;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{


#if UNITY_EDITOR

    [CustomEditor(typeof(UIGameControls), editorForChildClasses: true)]
    public class UIGameControlsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            UIGameControls viewEquipmentMenu = target as UIGameControls;

            if (GUILayout.Button("Show Keyboard Buttons"))
            {
                viewEquipmentMenu.ShowKeyboardButtons();
            }
            if (GUILayout.Button("Show PS4 Buttons"))
            {
                viewEquipmentMenu.ShowPs4Buttons();
            }
            if (GUILayout.Button("Show Xbox Buttons"))
            {
                viewEquipmentMenu.ShowXboxButtons();
            }
        }
    }
#endif

    [RequireComponent(typeof(UIDocument))]
    public class UIGameControls : MonoBehaviour
    {

        UIDocument uIDocument => GetComponent<UIDocument>();
        VisualElement root => uIDocument.rootVisualElement;

        [SerializeField] StarterAssetsInputs starterAssetsInputs;
        [SerializeField] GameSettings gameSettings;

        List<VisualElement> ps4Buttons = new List<VisualElement>();
        List<VisualElement> keyboardButtons = new List<VisualElement>();
        List<VisualElement> xboxButtons = new List<VisualElement>();
        List<Button> rebindButtons = new List<Button>();


        [Header("Settings")]
        [SerializeField] bool isOptionsMenu = false;

        void Awake()
        {
            EventManager.StartListening(EventMessages.ON_USE_CUSTOM_INPUT_CHANGED, UpdateInputsHUD);
        }

        void OnEnable()
        {
            SetupFooterButtons();

            UpdateFooterButtons();

            if (isOptionsMenu && starterAssetsInputs.IsKeyboardMouse())
            {
                EnableRebindButtons();
            }
            else
            {
                DisableRebindButtons();
            }

            bool displayAdditionalKeys = isOptionsMenu || starterAssetsInputs.IsKeyboardMouse();

            root.Q("SaveGame").style.display = displayAdditionalKeys ? DisplayStyle.Flex : DisplayStyle.None;
            root.Q("LoadGame").style.display = displayAdditionalKeys ? DisplayStyle.Flex : DisplayStyle.None;
            root.Q("CustomizeCharacter").style.display = displayAdditionalKeys ? DisplayStyle.Flex : DisplayStyle.None;

            UpdateInputsHUD();
        }

        void SetupFooterButtons()
        {
            keyboardButtons.Clear();
            ps4Buttons.Clear();
            xboxButtons.Clear();

            root.Query<VisualElement>("Keyboard").ForEach(x => keyboardButtons.Add(x));
            root.Query<VisualElement>("PS4").ForEach(x => ps4Buttons.Add(x));
            root.Query<VisualElement>("Xbox").ForEach(x => xboxButtons.Add(x));
            root.Query<Button>("Rebind").ForEach(x => rebindButtons.Add(x));
        }

        void DisableAllFooterButtons()
        {
            foreach (var button in keyboardButtons)
            {
                button.style.display = DisplayStyle.None;
            }
            foreach (var button in ps4Buttons)
            {
                button.style.display = DisplayStyle.None;
            }
            foreach (var button in xboxButtons)
            {
                button.style.display = DisplayStyle.None;
            }
        }

        void DisableRebindButtons()
        {
            foreach (var button in rebindButtons)
            {
                button.style.display = DisplayStyle.None;
            }
        }

        void EnableRebindButtons()
        {
            foreach (var button in rebindButtons)
            {
                button.style.display = DisplayStyle.Flex;
            }
        }

        void UpdateFooterButtons()
        {
            if (starterAssetsInputs.IsPS4Controller())
            {
                ShowPs4Buttons();
            }
            else if (starterAssetsInputs.IsXboxController() || starterAssetsInputs.IsGamepad())
            {
                ShowXboxButtons();
            }
            else
            {
                ShowKeyboardButtons();
            }
        }

        public void ShowPs4Buttons()
        {
            DisableAllFooterButtons();
            EnableButtons(ps4Buttons);
        }

        public void ShowKeyboardButtons()
        {
            DisableAllFooterButtons();
            EnableButtons(keyboardButtons);
        }

        public void ShowXboxButtons()
        {
            DisableAllFooterButtons();
            EnableButtons(xboxButtons);
        }

        void EnableButtons(List<VisualElement> buttons)
        {
            foreach (var button in buttons)
            {
                button.style.display = DisplayStyle.Flex;
            }
        }


        public void UpdateInputsHUD()
        {
            UpdateInputLabel("Dodge",
                Utils.IsPortuguese() ? "Esquivar" : "Dodge",
                gameSettings.GetDodgeOverrideBindingPayload(),
                "Dodge");

            UpdateInputLabel("Jump",
                Utils.IsPortuguese() ? "Pular" : "Jump",
                gameSettings.GetJumpOverrideBindingPayload(),
                "Jump");

            UpdateInputLabel("ToggleHands",
                Utils.IsPortuguese() ? "Alternar Mãos" : "Toggle Hands",
                gameSettings.GetTwoHandModeOverrideBindingPayload(),
                "Tab");

            UpdateInputLabel("HeavyAttack",
                Utils.IsPortuguese() ? "Ataque Pesado" : "Heavy Attack",
                gameSettings.GetHeavyAttackOverrideBindingPayload(),
                "HeavyAttack");

            UpdateInputLabel("Sprint",
                Utils.IsPortuguese() ? "Correr" : "Sprint",
                gameSettings.GetSprintOverrideBindingPayload(),
                "Sprint");
        }

        void UpdateInputLabel(string containerName, string labelText, string overrideBindingPayload, string actionName)
        {
            if (root == null)
            {
                Debug.Log("Root is null");
                return;
            }
            VisualElement container = root.Q(containerName);
            Label label = container.Q<Label>($"{containerName}KeyLabel");
            VisualElement icon = container.Q<VisualElement>("Keyboard");

            if (!string.IsNullOrEmpty(overrideBindingPayload))
            {
                label.text = $"{labelText}: " + starterAssetsInputs.GetCurrentKeyBindingForAction(actionName);
                icon.style.display = DisplayStyle.None;
            }
            else
            {
                label.text = labelText;
                icon.style.display = DisplayStyle.Flex;
            }
        }
    }
}
