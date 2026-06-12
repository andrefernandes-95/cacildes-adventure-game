namespace AF
{
    using AF.ModTools;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class UIDocumentModdingToolbar : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] Soundbank soundbank;
        [SerializeField] ModManager modManager;

        [Header("UI Documents")]
        [SerializeField] UIDocument uIDocument;

        VisualElement root;
        TextField ModNameTextField;
        Button SaveModButton;
        Button LoadOtherModButton;

        [Header("UI Components")]
        [SerializeField] UIDocumentModListModal uIDocumentModListModal;

        void Start()
        {
            root = uIDocument.rootVisualElement;

            ModNameTextField = root.Q<TextField>("ModNameTextField");
            SaveModButton = root.Q<Button>("SaveMod");
            LoadOtherModButton = root.Q<Button>("LoadOtherMod");

            SetupTextFields();
            SetupButtons();
        }

        void SetupTextFields()
        {
            ModNameTextField.value = modManager.currentModFile.modName;
            ModNameTextField.RegisterValueChangedCallback(ev =>
            {
                modManager.currentModFile.modName = ev.newValue;
            });
        }

        void SetupButtons()
        {
            UIUtils.SetupButton(SaveModButton, () =>
            {
                modManager.SaveMod();
            }, soundbank);

            UIUtils.SetupButton(LoadOtherModButton, () =>
            {
                uIDocumentModListModal.OpenMenu();
            }, soundbank);
        }
    }
}
