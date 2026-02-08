namespace AF.ModTools
{
    using System.IO;
    using System.Linq;
    using CI.QuickSave;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class UIDocumentModListModal : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] ModManager modManager;
        [SerializeField] Soundbank soundbank;

        [SerializeField] UIDocument uIDocument;

        VisualElement root;

        VisualElement Container;
        Button CloseButton;
        TextField ModSearchTextField;
        ScrollView ModListScrollView;

        string searchFilter = "";

        void Awake()
        {
            root = uIDocument.rootVisualElement;

            Container = root.Q<VisualElement>("Container");
            ModSearchTextField = root.Q<TextField>("SearchField");
            ModListScrollView = root.Q<ScrollView>("Modlist");

            SetupButtons();

            SetupSearch();

            UpdateList();

            CloseMenu();
        }

        void SetupButtons()
        {
            CloseButton = root.Q<Button>("Close");
            UIUtils.SetupButton(CloseButton, () =>
            {
                CloseMenu();
            }, soundbank);
        }

        void SetupSearch()
        {
            ModSearchTextField.RegisterValueChangedCallback(ev =>
            {
                searchFilter = ev.newValue;
                UpdateList();
            });
        }

        void UpdateList()
        {
            ModListScrollView.Clear();

            string PreferencesFolderPath = Path.Combine(Application.persistentDataPath, "Mods");

            if (!Directory.Exists(PreferencesFolderPath))
            {
                Directory.CreateDirectory(PreferencesFolderPath);
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(PreferencesFolderPath);
            FileInfo[] files = directoryInfo.GetFiles();

            if (files.Length == 0)
            {
                return;
            }

            string[] modFiles = files
                .OrderByDescending(f => f.CreationTime)
                .Where(x =>
                {
                    if (x.Name.Contains(".json"))
                    {
                        if (!string.IsNullOrEmpty(searchFilter))
                        {
                            return x.Name.Contains(searchFilter);
                        }

                        return true;
                    }

                    return false;
                })
                .Select(x => x.Name.Replace(".json", ""))
                .ToArray();

            foreach (string modFile in modFiles)
            {
                Button button = new Button();
                button.AddToClassList("primary-button");
                button.text = modFile;
                UIUtils.SetupButton(button, () =>
                {
                    string FilePath = Path.Combine(PreferencesFolderPath, modFile + ".json");

                    QuickSaveReader quickSaveReader = QuickSaveReader.Create(FilePath);

                    modManager.LoadMod(quickSaveReader);

                    CloseMenu();
                }, soundbank);

                ModListScrollView.Add(button);
            }
        }

        public void CloseMenu()
        {
            Container.style.display = DisplayStyle.None;
        }

        public void OpenMenu()
        {
            Container.style.display = DisplayStyle.Flex;
            UpdateList();
        }
    }
}