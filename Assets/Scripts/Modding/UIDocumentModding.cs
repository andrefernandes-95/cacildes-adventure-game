namespace AF
{
    using AF.ModTools;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class UIDocumentModding : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] Soundbank soundbank;
        [SerializeField] ModManager modManager;

        [Header("UI Documents")]
        [SerializeField] UIDocument uIDocument;
        VisualElement root;
        VisualElement editModeContainer;
        VisualElement playModeContainer;
        Button playMode;

        void Awake()
        {
            root = uIDocument.rootVisualElement;

            editModeContainer = root.Q<VisualElement>("EditModeContainer");
            playModeContainer = root.Q<VisualElement>("PlayModeContainer");

            playMode = root.Q<Button>("PlayMode");
            UIUtils.SetupButton(playMode, OnPlay, soundbank);

            modManager.onEditModeEnter.AddListener(OnEditModeEnter);
            modManager.onPlayModeEnter.AddListener(OnPlayModeEnter);

            modManager.modCamera.onLockEvent.AddListener(isLocked =>
            {
                root.focusable = isLocked == false;
            });
        }

        void OnEditModeEnter()
        {
            editModeContainer.style.display = DisplayStyle.Flex;
            playModeContainer.style.display = DisplayStyle.None;
        }

        void OnPlayModeEnter()
        {
            editModeContainer.style.display = DisplayStyle.None;
            playModeContainer.style.display = DisplayStyle.Flex;
        }

        void Start()
        {
        }

        void OnPlay()
        {
            modManager.ExitEditMode();
        }
    }
}
