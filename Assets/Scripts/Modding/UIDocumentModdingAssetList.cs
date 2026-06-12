namespace AF.ModTools
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class UIDocumentModdingAssetList : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] Soundbank soundbank;
        [SerializeField] ModManager modManager;

        [Header("UI Documents")]
        [SerializeField] UIDocument uIDocument;
        VisualElement root;

        [Header("Asset Lists")]
        [SerializeField] VisualTreeAsset assetItemButton;
        Button showEnvironmentAssets;
        Button showObjectAssets;
        Button showEntityAssets;

        ScrollView assetListScrollView;

        public enum Mode
        {
            Environment,
            Objects,
            Entities
        }

        public Mode mode = Mode.Environment;


        [Header("Fallbacks")]
        [SerializeField] Texture2D FallbackAssetImage;

        void Awake()
        {
            root = uIDocument.rootVisualElement;

            showEnvironmentAssets = root.Q<Button>("EnvironmentAssets");
            UIUtils.SetupButton(showEnvironmentAssets, OnShowEnvironmentalAssets, soundbank);

            showObjectAssets = root.Q<Button>("ObjectAssets");
            UIUtils.SetupButton(showObjectAssets, OnShowObjectAssets, soundbank);

            showEntityAssets = root.Q<Button>("EntityAssets");
            UIUtils.SetupButton(showEntityAssets, OnShowEntityAssets, soundbank);

            assetListScrollView = root.Q<ScrollView>("AssetList");

            modManager.modCamera.onLockEvent.AddListener(isLocked =>
            {
                root.focusable = isLocked == false;
            });
        }

        void Start()
        {
            RedrawScrollList();
            OnShowEnvironmentalAssets();
        }

        void OnPlay()
        {
            modManager.ExitEditMode();
        }

        void RedrawScrollList()
        {
            assetListScrollView.Clear();

            List<ModAsset> targetList = new();
            if (mode == Mode.Environment)
            {
                targetList = modManager.environmentModAssets.ToList();
            }
            else if (mode == Mode.Objects)
            {
                targetList = modManager.objectModAssets.ToList();
            }
            else if (mode == Mode.Entities)
            {
                targetList = modManager.entitiyModAssets.ToList();
            }

            foreach (ModAsset environmentModAsset in targetList)
            {
                VisualElement clone = assetItemButton.CloneTree();
                clone.Q<Image>().image = environmentModAsset.thumbnail != null ? environmentModAsset.thumbnail : FallbackAssetImage;
                clone.Q<Label>().text = environmentModAsset.GetName();
                UIUtils.SetupButton(clone.Q<Button>(), () => OnInsertAsset(environmentModAsset), soundbank);
                assetListScrollView.Add(clone);
            }
        }

        void OnInsertAsset(ModAsset modAsset)
        {
            modManager.SpawnModAsset(modAsset);
        }

        void OnShowEnvironmentalAssets()
        {
            mode = Mode.Environment;
            RedrawScrollList();

            UpdateButtonSelection();
        }

        void OnShowObjectAssets()
        {
            mode = Mode.Objects;
            RedrawScrollList();
            UpdateButtonSelection();
        }

        void OnShowEntityAssets()
        {
            mode = Mode.Entities;
            RedrawScrollList();
            UpdateButtonSelection();
        }

        void UpdateButtonSelection()
        {
            showEnvironmentAssets.style.backgroundColor = mode == Mode.Environment ? Color.violet : Color.black;
            showObjectAssets.style.backgroundColor = mode == Mode.Objects ? Color.violet : Color.black;
            showEntityAssets.style.backgroundColor = mode == Mode.Entities ? Color.violet : Color.black;
        }
    }
}