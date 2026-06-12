namespace AF.ModTools
{
    using UnityEngine;
    using UnityEngine.UIElements;

    public class UIDocumentModdingSelectedObject : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] Soundbank soundbank;
        [SerializeField] ModManager modManager;

        [Header("UI Documents")]
        [SerializeField] UIDocument uIDocument;

        VisualElement root;
        VisualElement container;

        Label objectName;
        Button deleteButton;
        Vector3Field positionField;

        Button EditMomentButton;

        EditableObject currentObject;

        EventCallback<ChangeEvent<Vector3>> positionChangedCallback;

        private void Awake()
        {
            root = uIDocument.rootVisualElement;
            container = root.Q<VisualElement>("SelectedObjectContainer");

            objectName = container.Q<Label>("ObjectName");
            deleteButton = container.Q<Button>("DeleteButton");
            positionField = container.Q<Vector3Field>("Position");

            EditMomentButton = container.Q<Button>("EditMomentButton");

            SetupButtons();
            Hide();

            // Cache callback once
            positionChangedCallback = OnPositionChanged;

            modManager.selectionTool.onObjectSelected.AddListener(OnObjectSelected);
            modManager.selectionTool.onObjectDeselected.AddListener(OnObjectDeselected);
            modManager.selectionTool.onObjectDragged.AddListener(OnObjectDragged);
        }

        void SetupButtons()
        {
            UIUtils.SetupButton(deleteButton, () =>
            {
                modManager.selectionTool.DeleteCurrent();
            }, soundbank);

            UIUtils.SetupButton(EditMomentButton, () =>
            {
                HandleEditMoment();
            }, soundbank);
        }

        void Show()
        {
            container.style.display = DisplayStyle.Flex;

            UpdateContextActions();
        }

        void Hide() => container.style.display = DisplayStyle.None;

        void OnObjectSelected(EditableObject editableObject)
        {
            currentObject = editableObject;

            objectName.text = editableObject.name;

            // Sync UI without triggering callback
            positionField.SetValueWithoutNotify(editableObject.transform.position);

            positionField.RegisterValueChangedCallback(positionChangedCallback);

            Show();
        }

        void OnObjectDeselected()
        {
            positionField.UnregisterValueChangedCallback(positionChangedCallback);

            currentObject = null;
            Hide();
        }

        void OnObjectDragged(Vector3 newPos)
        {
            positionField.SetValueWithoutNotify(newPos);
        }

        void OnPositionChanged(ChangeEvent<Vector3> ev)
        {
            if (currentObject == null)
                return;

            currentObject.transform.position = ev.newValue;
        }

        void UpdateContextActions()
        {
            if (IsSelectedObjectAMoment(out _))
            {
                EditMomentButton.style.display = DisplayStyle.Flex;
            }
            else
            {
                EditMomentButton.style.display = DisplayStyle.None;
            }
        }

        void HandleEditMoment()
        {
            if (IsSelectedObjectAMoment(out MomentPlaceholder momentPlaceholder))
            {
                modManager.uIDocumentMomentEditor.Open(momentPlaceholder);
            }
        }

        bool IsSelectedObjectAMoment(out MomentPlaceholder result)
        {
            result = null;

            if (modManager.selectionTool.Current != null && modManager.selectionTool.Current.TryGetComponent<MomentPlaceholder>(out var momentPlaceholder))
            {
                result = momentPlaceholder;
                return true;
            }

            return false;
        }
    }
}
