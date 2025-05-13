using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{

#if UNITY_EDITOR

    [CustomEditor(typeof(UIFooterButtons), editorForChildClasses: true)]
    public class ViewEquipmentMenuEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            UIFooterButtons viewEquipmentMenu = target as UIFooterButtons;

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
    public class UIFooterButtons : MonoBehaviour
    {
        UIDocument uIDocument => GetComponent<UIDocument>();
        VisualElement root => uIDocument.rootVisualElement;

        [SerializeField] StarterAssetsInputs starterAssetsInputs;

        // Footer
        public string FOOTER_ID = "MenuFooter";
        VisualElement footer;
        List<VisualElement> ps4Buttons = new List<VisualElement>();
        List<VisualElement> keyboardButtons = new List<VisualElement>();
        List<VisualElement> xboxButtons = new List<VisualElement>();

        void OnEnable()
        {

            SetupFooterButtons();

            UpdateFooterButtons();
        }

        void SetupFooterButtons()
        {
            footer = root.Q(FOOTER_ID);

            keyboardButtons.Clear();
            ps4Buttons.Clear();
            xboxButtons.Clear();

            footer.Query<VisualElement>("Keyboard").ForEach(x => keyboardButtons.Add(x));
            footer.Query<VisualElement>("PS4").ForEach(x => ps4Buttons.Add(x));
            footer.Query<VisualElement>("Xbox").ForEach(x => xboxButtons.Add(x));
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
    }
}
