using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
#if UNITY_EDITOR

    [CustomEditor(typeof(UICloseIndicator), editorForChildClasses: true)]
    public class CloseIndicatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            UICloseIndicator closeIndicator = target as UICloseIndicator;

            if (GUILayout.Button("Show Keyboard Buttons"))
            {
                closeIndicator.EnableKeyboardButton();
            }
            if (GUILayout.Button("Show PS4 Buttons"))
            {
                closeIndicator.EnablePS4Button();
            }
            if (GUILayout.Button("Show Xbox Buttons"))
            {
                closeIndicator.EnableXboxButton();
            }
        }
    }
#endif

    [RequireComponent(typeof(UIDocument))]
    public class UICloseIndicator : MonoBehaviour
    {
        UIDocument uIDocument => GetComponent<UIDocument>();

        [SerializeField] StarterAssetsInputs starterAssetsInputs;

        VisualElement closeIndicator => uIDocument.rootVisualElement.Q("CloseIndicator");

        private void OnEnable()
        {
            DisableButtons();

            if (starterAssetsInputs.IsPS4Controller())
            {
                EnablePS4Button();
            }
            else if (starterAssetsInputs.IsXboxController() || starterAssetsInputs.IsGamepad())
            {
                EnableXboxButton();
            }
            else if (starterAssetsInputs.IsKeyboardMouse())
            {
                EnableKeyboardButton();
            }
        }

        void DisableButtons()
        {
            closeIndicator.Q<IMGUIContainer>("PS4").style.display = DisplayStyle.None;
            closeIndicator.Q<IMGUIContainer>("Xbox").style.display = DisplayStyle.None;
            closeIndicator.Q<IMGUIContainer>("Keyboard").style.display = DisplayStyle.None;
        }

        public void EnableKeyboardButton()
        {
            DisableButtons();

            closeIndicator.Q<IMGUIContainer>("Keyboard").style.display = DisplayStyle.Flex;
        }
        public void EnablePS4Button()
        {
            DisableButtons();

            closeIndicator.Q<IMGUIContainer>("PS4").style.display = DisplayStyle.Flex;
        }
        public void EnableXboxButton()
        {
            DisableButtons();

            closeIndicator.Q<IMGUIContainer>("Xbox").style.display = DisplayStyle.Flex;
        }
    }
}
