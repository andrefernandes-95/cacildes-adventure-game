namespace AF.ModTools
{
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class UIDocumentMomentEditor : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] Soundbank soundbank;
        [SerializeField] ModManager modManager;

        [Header("UI Documents")]
        [SerializeField] UIDocument uIDocument;

        VisualElement root;

        Button Close;
        Button DeleteMomentButton;
        Label SelectedActionLabel;
        ScrollView MomentActionsList;


        VisualElement SelectedActionEditor;
        VisualElement DialogueEditor;
        TextField DialogueTextField;

        Button AddDialogue;

        MomentPlaceholder SelectedMoment;

        string selectedModEventUuid;

        private void Awake()
        {
            root = uIDocument.rootVisualElement;

            Close = root.Q<Button>("Close");

            SelectedActionLabel = root.Q<Label>("SelectedActionLabel");
            DeleteMomentButton = root.Q<Button>("DeleteButton");

            MomentActionsList = root.Q<ScrollView>("MomentActionsList");

            SelectedActionEditor = root.Q<VisualElement>("SelectedActionEditor");
            SelectedActionEditor.style.display = DisplayStyle.None;

            DialogueEditor = root.Q<VisualElement>("DialogueEditor");
            DialogueTextField = root.Q<TextField>("DialogueTextField");

            AddDialogue = root.Q<Button>("AddDialogue");

            SetupButtons();

            Hide();
        }

        void SetupButtons()
        {
            UIUtils.SetupButton(Close, () =>
            {
                Hide();
            }, soundbank);

            UIUtils.SetupButton(DeleteMomentButton, () =>
            {
                if (!string.IsNullOrEmpty(selectedModEventUuid))
                {
                    DeleteModEvent();
                }
            }, soundbank);

            UIUtils.SetupButton(AddDialogue, () =>
            {
                HandleAddDialogue();
            }, soundbank);
        }

        void Show() => root.style.display = DisplayStyle.Flex;
        void Hide() => root.style.display = DisplayStyle.None;

        public void Open(MomentPlaceholder momentPlaceholder)
        {
            this.SelectedMoment = momentPlaceholder;
            UpdateScrollList();
            Show();
        }

        void UpdateScrollList()
        {
            MomentActionsList.Clear();

            if (SelectedMoment == null || SelectedMoment.modEvents.Count <= 0)
            {
                return;
            }

            foreach (ModEvent modEvent in SelectedMoment.modEvents)
            {
                Button newButton = new Button();

                newButton.AddToClassList("primary-button");
                newButton.text = modEvent.GetActionType();
                newButton.style.backgroundColor = IsSelected(modEvent) ? Color.violet : Color.black;
                UIUtils.SetupButton(newButton, () =>
                {
                    SetSelectedModEvent(modEvent);
                    UpdateScrollList();
                }, soundbank);

                MomentActionsList.Add(newButton);
            }
        }

        void SetSelectedModEvent(ModEvent modEvent)
        {
            this.selectedModEventUuid = modEvent.uuid;
            DrawActionMenu();
        }

        bool IsSelected(ModEvent modEvent)
        {
            if (modEvent == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(selectedModEventUuid))
            {
                return false;
            }

            return modEvent.uuid == selectedModEventUuid;
        }

        void DrawActionMenu()
        {
            SelectedActionEditor.style.display = DisplayStyle.Flex;

            DrawDialogueForm();
        }

        void DeleteModEvent()
        {
            ModEvent tmp = GetCurrentModEvent();
            selectedModEventUuid = null;
            SelectedMoment.modEvents.Remove(tmp);
            SelectedActionEditor.style.display = DisplayStyle.None;
            UpdateScrollList();
        }

        public bool IsOpen() => root.style.display == DisplayStyle.Flex;

        void HandleAddDialogue()
        {
            if (SelectedMoment == null)
            {
                return;
            }

            DialogueModEvent newModEvent = new();
            SelectedMoment.modEvents.Add(newModEvent);
            UpdateScrollList();
        }

        void DrawDialogueForm()
        {
            if (GetCurrentModEvent() is DialogueModEvent dialogueModEvent)
            {
                DialogueEditor.style.display = DisplayStyle.None;
                DialogueEditor.style.display = DisplayStyle.Flex;
                DialogueTextField.SetValueWithoutNotify(dialogueModEvent.GetMessage());
                DialogueTextField.RegisterValueChangedCallback(OnDialogueChange);
            }

        }

        void OnDialogueChange(ChangeEvent<string> ev)
        {
            if (GetCurrentModEvent() is DialogueModEvent dialogueModEvent)
            {
                dialogueModEvent.SetData(ev.newValue);
            }
        }

        ModEvent GetCurrentModEvent()
        {
            if (SelectedMoment == null || SelectedMoment.modEvents.Count <= 0 || string.IsNullOrEmpty(selectedModEventUuid))
            {
                return null;
            }

            return SelectedMoment.modEvents.FirstOrDefault(modEvent => modEvent != null && modEvent.uuid == selectedModEventUuid);
        }
    }
}
