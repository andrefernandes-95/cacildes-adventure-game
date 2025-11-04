using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentTitleScreenSaveFiles : MonoBehaviour
    {
        VisualElement root;
        ScrollView scrollPanel;

        public VisualTreeAsset saveFileButtonPrefab;

        [Header("Components")]
        public UIManager uiManager;
        public Soundbank soundbank;
        public SaveManager saveManager;

        [Header("Localization")]
        public LocalizedString ReturnToTitleScreen_LocalizedString;
        public LocalizedString OpenSavesFolder_LocalizedString;

        [Header("UI Components")]
        public UIDocumentTitleScreen uIDocumentTitleScreen;

        // Pagination
        public int FILES_PER_PAGE = 25;
        int currentPage = 0;
        List<string> allSaveFiles = new();

        // Last scroll position
        int lastScrollElementIndex = -1;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void OnClose()
        {
            if (this.isActiveAndEnabled)
            {
                Close();
            }
        }

        private void OnEnable()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            scrollPanel = root.Q<ScrollView>("SaveFilesContainer");

            // Load all file names once
            allSaveFiles = new List<string>(SaveUtils.GetSaveFileNames(saveManager.SAVE_FILES_FOLDER));
            currentPage = 0;

            DrawUI();
        }

        void DrawUI()
        {
            scrollPanel.Clear();

            // Title screen return button
            Button exitButton = new()
            {
                text = ReturnToTitleScreen_LocalizedString.GetLocalizedString()
            };
            exitButton.AddToClassList("primary-button");
            scrollPanel.Add(exitButton);

            UIUtils.SetupButton(exitButton,
                () => Close(),
                () =>
                {
                    scrollPanel.ScrollTo(exitButton);
                },
                () => { },
                false,
                soundbank);

            // Open saves folder button
            Button openSavesFolder = new()
            {
                text = OpenSavesFolder_LocalizedString.GetLocalizedString()
            };
            openSavesFolder.AddToClassList("primary-button");

            UIUtils.SetupButton(openSavesFolder, () =>
            {
                Process.Start(Application.persistentDataPath + "/" + saveManager.SAVE_FILES_FOLDER);
            },
            () =>
            {
                scrollPanel.ScrollTo(exitButton);
            },
            () => { },
            false,
            soundbank);

            scrollPanel.Add(openSavesFolder);

            // Draw first batch
            DrawSaveFileBatch();

            if (lastScrollElementIndex == -1)
            {
                root.Q<ScrollView>().ScrollTo(exitButton);
            }
            else
            {
                Invoke(nameof(GiveFocus), 0f);
            }
        }

        void DrawSaveFileBatch()
        {
            int startIndex = currentPage * FILES_PER_PAGE;
            int endIndex = Mathf.Min(startIndex + FILES_PER_PAGE, allSaveFiles.Count);

            for (int i = startIndex; i < endIndex; i++)
            {
                string saveFileName = allSaveFiles[i];
                var saveFileInstance = saveFileButtonPrefab.CloneTree();

                saveFileInstance.Q<Label>("SaveFileName").text = saveFileName;

                Texture2D screenshotThumbnail = SaveUtils.GetScreenshotFilePath(saveManager.SAVE_FILES_FOLDER, saveFileName);
                var screenshotElement = saveFileInstance.Q<VisualElement>("SaveScreenshot");
                screenshotElement.style.backgroundImage = screenshotThumbnail;
                screenshotElement.Q<Label>("SaveFileNotFoundLabel").style.display =
                    screenshotThumbnail == null ? DisplayStyle.Flex : DisplayStyle.None;

                UIUtils.SetupButton(saveFileInstance.Q<Button>("Button"), () =>
                {
                    saveManager.LoadSaveFile(saveFileName);
                }, () =>
                {
                    scrollPanel.ScrollTo(saveFileInstance.Q<Button>());
                }, () => { }, true, soundbank);

                scrollPanel.Add(saveFileInstance);
            }

            // Remove any existing "Load More" before adding a new one
            var existingLoadMore = scrollPanel.Q<Button>("LoadMoreButton");
            existingLoadMore?.RemoveFromHierarchy();

            // Add "Load More" button if there are more files
            if (endIndex < allSaveFiles.Count)
            {
                Button loadMoreButton = new()
                {
                    name = "LoadMoreButton",
                    text = Utils.IsPortuguese() ? "Mostrar mais..." : "Load More Saves..."
                };
                loadMoreButton.AddToClassList("primary-button");

                UIUtils.SetupButton(
                    loadMoreButton,
                () =>
                {
                    currentPage++;
                    loadMoreButton.SetEnabled(false); // prevent double-click
                    DrawSaveFileBatch(); // append next batch
                    lastScrollElementIndex = scrollPanel.childCount - 1;
                    Invoke(nameof(GiveFocus), 0f);
                },
                () =>
                {
                    scrollPanel.ScrollTo(loadMoreButton);
                },
                () =>
                {

                }, false, soundbank);

                scrollPanel.Add(loadMoreButton);
            }
        }

        void Close()
        {
            uIDocumentTitleScreen.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }

        void GiveFocus()
        {
            UIUtils.ScrollToLastPosition(
                lastScrollElementIndex,
                scrollPanel,
                () => { lastScrollElementIndex = -1; }
            );
        }
    }
}
