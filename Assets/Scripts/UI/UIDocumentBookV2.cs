using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace AF
{
    public struct PageElement
    {
        public string title;
        public List<string> images;
        public List<List<string>> groupedImages;
        public string content;
    }

    public class UIDocumentBookV2 : MonoBehaviour
    {

        VisualElement root;
        VisualElement bookFront, bookPage, bookBack, notePage;
        VisualElement leftPage, rightPage;
        VisualElement leftPageContent, rightPageContent;
        Label bookTitle, bookAuthor, notePageTitle, notePageText, leftPageTitle, rightPageTitle;


        enum BookState
        {
            Cover,
            PageContent,
            Back,
            Note
        }

        BookState currentState = BookState.Cover;


        [Header("Indexes")]
        public int currentPage = 0;

        [Header("Events")]
        public UnityEvent onJournalOpen;
        public UnityEvent onJournalClose;

        [Header("Components")]
        [SerializeField] PlayerManager playerManager;
        public Soundbank soundbank;
        public CursorManager cursorManager;

        bool isReading = false;
        Coroutine SetIsReadingCoroutine;

        Story inkStory;
        List<PageElement> inkPages = new(); // text or image path)> inkPages = new();

        string bookTitleText = "Untitled";
        string bookAuthorText = "Unknown";
        Color coverColor = Color.white;

        private void Awake()
        {
            this.gameObject.SetActive(false);
            playerManager.starterAssetsInputs.onMenuEvent.AddListener(OnClose);
            playerManager.starterAssetsInputs.onInteract.AddListener(OnClose);
            playerManager.starterAssetsInputs.onSwitchShieldInput.AddListener(OnSwitchPreviousPage);
            playerManager.starterAssetsInputs.onSwitchWeaponInput.AddListener(OnSwitchNextPage);
        }

        private void OnEnable()
        {
            this.root = GetComponent<UIDocument>().rootVisualElement;
            SetupRefs();
        }

        void SetupRefs()
        {
            bookFront = root.Q<VisualElement>("BookFront");
            bookPage = root.Q<VisualElement>("BookPage");
            bookBack = root.Q<VisualElement>("BookBack");
            notePage = root.Q<VisualElement>("NotePage");

            bookTitle = root.Q<Label>("BookTitle");
            bookAuthor = root.Q<Label>("BookAuthor");

            notePageTitle = notePage.Q<Label>("ChapterTitle");
            notePageText = notePage.Q<Label>("PageText");

            leftPage = root.Q<VisualElement>("LeftPage");
            rightPage = root.Q<VisualElement>("RightPage");

            leftPageTitle = leftPage.Q<Label>("ChapterTitle");
            rightPageTitle = rightPage.Q<Label>("ChapterTitle");

            leftPageContent = leftPage.Q<VisualElement>("PageContent");
            rightPageContent = rightPage.Q<VisualElement>("PageContent");
        }

        void ClearState()
        {
            currentPage = 0;
            if (inkPages.Count <= 1)
            {
                currentState = BookState.Note;
            }
            else
            {
                currentState = BookState.Cover;
            }
        }

        public void BeginReadInk(TextAsset inkJSON)
        {
            inkStory = new Story(inkJSON.text);
            inkPages.Clear();
            ParseInkBook();

            ClearState();

            gameObject.SetActive(true);

            ShowCurrentState();

            onJournalOpen?.Invoke();

            if (SetIsReadingCoroutine != null)
                StopCoroutine(SetIsReadingCoroutine);

            SetIsReadingCoroutine = StartCoroutine(HandleIsReading());
            OnReadStart();
        }

        IEnumerator HandleIsReading()
        {
            yield return new WaitForEndOfFrame();
            isReading = true;
        }

        void ParseInkBook()
        {
            bookTitleText = "";
            bookAuthorText = "";
            inkPages.Clear();

            bool parsingMetadata = true;

            while (inkStory.canContinue)
            {
                string raw = inkStory.Continue();

                if (string.IsNullOrWhiteSpace(raw)) continue;

                if (parsingMetadata && raw.Contains(":"))
                {
                    if (raw.StartsWith("Title:", System.StringComparison.OrdinalIgnoreCase))
                    {
                        bookTitleText = raw.Substring("Title:".Length).Trim();
                        continue;
                    }
                    else if (raw.StartsWith("Author:", System.StringComparison.OrdinalIgnoreCase))
                    {
                        bookAuthorText = raw.Substring("Author:".Length).Trim();
                        continue;
                    }
                    else if (raw.StartsWith("Color:", System.StringComparison.OrdinalIgnoreCase))
                    {
                        string currentColor = raw.Substring("Color:".Length).Trim();
                        if (ColorUtility.TryParseHtmlString($"#{currentColor}", out Color newCoverColor))
                        {
                            coverColor = newCoverColor;
                        }
                        continue;
                    }
                }

                parsingMetadata = false;
                string currentTitle = "";

                PageElement newPageElement = new();
                List<string> images = new();
                List<List<string>> groupedImages = new();

                foreach (var tag in inkStory.currentTags)
                {
                    if (tag.StartsWith("Chapter:", System.StringComparison.OrdinalIgnoreCase))
                    {
                        currentTitle = tag.Substring("Chapter:".Length).Trim();
                    }
                    else if (tag.StartsWith("Image:", System.StringComparison.OrdinalIgnoreCase))
                    {
                        string imagePath = tag.Substring("Image:".Length).Trim();
                        images.Add(imagePath);
                    }
                    else if (tag.StartsWith("Images:", System.StringComparison.OrdinalIgnoreCase))
                    {
                        string imagePath = tag.Substring("Images:".Length).Trim();
                        string[] separatedImages = imagePath.Split(",");

                        List<string> groupedImagesToAdd = new();
                        foreach (var img in separatedImages)
                        {
                            groupedImagesToAdd.Add(img);
                        }

                        groupedImages.Add(groupedImagesToAdd);
                    }
                }

                newPageElement.title = currentTitle;

                if (!string.IsNullOrEmpty(raw))
                {
                    inkPages.Add(new()
                    {
                        title = currentTitle,
                        content = raw,
                        images = images,
                        groupedImages = groupedImages
                    });
                }
            }
        }

        void OnClose()
        {
            if (!isActiveAndEnabled || inkStory == null || !isReading)
                return;

            onJournalClose.Invoke();
            inkStory = null;
            inkPages.Clear();
            cursorManager.HideCursor();
            isReading = false;
            OnReadEnd();
            gameObject.SetActive(false);
        }

        void OnReadStart()
        {
            playerManager.uIDocumentPlayerHUDV2.FadeOut();
            playerManager.thirdPersonController.SetLockCameraPosition(true);
        }

        void OnReadEnd()
        {
            playerManager.uIDocumentPlayerHUDV2.FadeIn();
            playerManager.thirdPersonController.SetLockCameraPosition(false);
        }

        public void OnSwitchNextPage() => NavigateBook(true);
        public void OnSwitchPreviousPage() => NavigateBook(false);

        void NavigateBook(bool forward)
        {
            if (inkPages.Count == 0) return;

            switch (currentState)
            {
                case BookState.Cover:
                    if (forward)
                    {
                        currentPage = 0;
                        currentState = BookState.PageContent;
                    }
                    break;

                case BookState.PageContent:
                    if (forward)
                    {
                        currentPage += 2;

                        if (currentPage >= inkPages.Count)
                        {
                            currentState = BookState.Back;
                        }
                    }
                    else
                    {
                        currentPage -= 2;
                        if (currentPage < 0)
                        {
                            currentState = BookState.Cover;
                            currentPage = 0;
                        }
                    }
                    break;

                case BookState.Back:
                    if (!forward)
                    {
                        currentState = BookState.PageContent;
                        currentPage = inkPages.Count - (inkPages.Count % 2 == 0 ? 2 : 1);
                    }
                    break;

                case BookState.Note:
                    currentPage = 0;
                    break;
            }

            ShowCurrentState();
        }

        void ShowCurrentState()
        {
            switch (currentState)
            {
                case BookState.Cover:
                    ShowCover();
                    break;

                case BookState.PageContent:
                    if (currentPage + 1 >= inkPages.Count)
                    {
                        ShowSingleInkPage(inkPages[currentPage], false);
                    }
                    else
                    {
                        ShowInkPages(inkPages[currentPage], inkPages[currentPage + 1]);
                    }
                    break;

                case BookState.Back:
                    ShowBack();
                    break;

                case BookState.Note:
                    ShowNotePage(inkPages[currentPage]);
                    break;
            }
        }

        public void ShowNotePage(PageElement pageElements)
        {
            bookFront.style.display = DisplayStyle.None;
            bookPage.style.display = DisplayStyle.None;
            bookBack.style.display = DisplayStyle.None;
            notePage.style.display = DisplayStyle.Flex;

            notePageTitle.text = pageElements.title;
            notePageText.text = pageElements.content;
        }

        void ShowCover()
        {
            soundbank.PlaySound(soundbank.bookFlip);
            bookFront.style.display = DisplayStyle.Flex;
            bookPage.style.display = DisplayStyle.None;
            bookBack.style.display = DisplayStyle.None;
            notePage.style.display = DisplayStyle.None;

            bookTitle.text = bookTitleText;
            bookAuthor.text = bookAuthorText;
            bookFront.style.backgroundColor = coverColor;
        }

        void ShowBack()
        {
            soundbank.PlaySound(soundbank.bookFlip);
            bookFront.style.display = DisplayStyle.None;
            bookPage.style.display = DisplayStyle.None;
            bookBack.style.display = DisplayStyle.Flex;
            notePage.style.display = DisplayStyle.None;
            bookBack.style.backgroundColor = coverColor;
        }

        void ShowSingleInkPage(PageElement pageElements, bool renderOnRight)
        {
            soundbank.PlaySound(soundbank.bookFlip);
            bookFront.style.display = DisplayStyle.None;
            bookPage.style.display = DisplayStyle.Flex;
            bookBack.style.display = DisplayStyle.None;
            notePage.style.display = DisplayStyle.None;
            rightPageContent.Clear();
            leftPageContent.Clear();

            if (renderOnRight)
            {
                RenderPageContent(pageElements, rightPageContent, rightPageTitle, pageElements.title);
            }
            else
            {
                RenderPageContent(pageElements, leftPageContent, leftPageTitle, pageElements.title);
            }
        }

        void ShowInkPages(PageElement left, PageElement right)
        {
            soundbank.PlaySound(soundbank.bookFlip);
            bookFront.style.display = DisplayStyle.None;
            bookPage.style.display = DisplayStyle.Flex;
            bookBack.style.display = DisplayStyle.None;
            notePage.style.display = DisplayStyle.None;

            RenderPageContent(left, leftPageContent, leftPageTitle, left.title);
            RenderPageContent(right, rightPageContent, rightPageTitle, right.title);
        }

        void RenderPageContent(PageElement elements, VisualElement container, Label titleLabel, string title)
        {
            container.Clear();

            if (!string.IsNullOrEmpty(title))
            {
                var label = new Label(title);
                label.style.whiteSpace = WhiteSpace.Normal;
                label.style.marginBottom = 6;
                label.AddToClassList("label-text");
                label.AddToClassList("book-chapter-title");
                container.Add(label);
            }

            if (elements.images.Count > 0)
            {
                VisualElement imageContainer = new VisualElement();
                imageContainer.style.display = DisplayStyle.Flex;
                imageContainer.style.flexDirection = FlexDirection.Row;
                imageContainer.style.alignItems = Align.Center;

                foreach (var element in elements.images)
                {
                    var image = new Image();
                    var cleanPath = element.Replace("Assets/Resources/", "").Replace(".png", "");
                    var sprite = Resources.Load<Sprite>(cleanPath);

                    if (sprite != null)
                    {
                        image.image = sprite.texture;
                        image.style.width = sprite.texture.width;
                        image.style.height = sprite.texture.height;
                        image.scaleMode = ScaleMode.ScaleToFit;
                        image.style.marginTop = 8;
                        image.style.marginBottom = 8;
                        imageContainer.Add(image);
                    }
                }

                container.Add(imageContainer);
            }

            if (elements.groupedImages.Count > 0)
            {
                foreach (var group in elements.groupedImages)
                {
                    VisualElement imageContainer = new VisualElement();
                    imageContainer.style.display = DisplayStyle.Flex;
                    imageContainer.style.flexDirection = FlexDirection.Row;
                    imageContainer.style.alignItems = Align.Center;

                    foreach (var imageInGroup in group)
                    {
                        var image = new Image();
                        var cleanPath = imageInGroup.Replace("Assets/Resources/", "").Replace(".png", "");
                        var sprite = Resources.Load<Sprite>(cleanPath);

                        if (sprite != null)
                        {
                            image.image = sprite.texture;
                            image.style.width = sprite.texture.width * .75f;
                            image.style.height = sprite.texture.height * .75f;
                            image.scaleMode = ScaleMode.ScaleToFit;
                            image.style.marginTop = 4;
                            image.style.marginBottom = 4;
                            image.style.marginRight = 4;
                            image.style.marginLeft = 4;
                            imageContainer.Add(image);
                        }
                    }

                    container.Add(imageContainer);
                }
            }

            if (!string.IsNullOrEmpty(elements.content))
            {
                string textContent = elements.content;
                textContent = textContent.Replace("[br]", "\n");
                var label = new Label(textContent);

                label.style.whiteSpace = WhiteSpace.PreWrap;
                label.style.marginBottom = 6;
                label.AddToClassList("label-text");
                label.AddToClassList("book-chapter-paragraph");
                container.Add(label);
            }
        }

    }
}
