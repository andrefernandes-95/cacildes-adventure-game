using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace AF
{
    public class UIDocumentBookV2 : MonoBehaviour
    {
        enum PageElementType { Text, Image }

        struct PageElement
        {
            public PageElementType type;
            public string content; // text or image path
        }

        VisualElement root;
        VisualElement bookFront, bookPage, bookBack, notePage;
        VisualElement leftPage, rightPage;
        VisualElement leftPageContent, rightPageContent;
        Label bookTitle, bookAuthor, notePageTitle, notePageText, leftPageTitle, rightPageTitle;

        [Header("Indexes")]
        public int currentPage = -1;

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
        List<(string title, List<PageElement> elements)> inkPages = new();

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

        public void BeginReadInk(TextAsset inkJSON)
        {
            inkStory = new Story(inkJSON.text);
            inkPages.Clear();
            currentPage = -1;

            ParseInkBook();

            gameObject.SetActive(true);
            ShowCover();
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
                string raw = inkStory.Continue().Trim();

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

                foreach (var tag in inkStory.currentTags)
                {
                    if (tag.StartsWith("Chapter:", System.StringComparison.OrdinalIgnoreCase))
                    {
                        currentTitle = tag.Substring("Chapter:".Length).Trim();
                    }
                }

                List<PageElement> elements = new();

                System.Text.StringBuilder currentText = new();

                var parts = raw.Split(new[] { "image:" }, StringSplitOptions.None);

                for (int i = 0; i < parts.Length; i++)
                {
                    var part = parts[i];

                    if (i == 0)
                    {
                        // First part, always text before any image
                        currentText.Append(part.Trim());
                        continue;
                    }

                    // For subsequent parts, extract image path and remaining text
                    int pngIndex = part.IndexOf(".png");

                    if (pngIndex != -1)
                    {
                        // Extract image path: from start of part up to .png
                        string imagePath = part.Substring(0, pngIndex + 4).Trim();

                        // Flush accumulated text before image
                        if (currentText.Length > 0)
                        {
                            elements.Add(new PageElement
                            {
                                type = PageElementType.Text,
                                content = currentText.ToString().Trim()
                            });
                            currentText.Clear();
                        }

                        // Add image element
                        elements.Add(new PageElement
                        {
                            type = PageElementType.Image,
                            content = imagePath
                        });

                        // Append rest of the part (after image path) as text
                        string remainingText = part.Substring(pngIndex + 4).Trim();
                        if (!string.IsNullOrEmpty(remainingText))
                        {
                            currentText.Append(remainingText);
                        }
                    }
                    else
                    {
                        // No image in this part, just text
                        currentText.Append(part.Trim());
                    }
                }

                // Flush any remaining text
                if (currentText.Length > 0)
                {
                    elements.Add(new PageElement
                    {
                        type = PageElementType.Text,
                        content = currentText.ToString().Trim()
                    });
                }

                inkPages.Add(new() { title = currentTitle, elements = elements });
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

            if (forward)
            {
                currentPage = Mathf.Clamp(currentPage == -1 ? 0 : currentPage + 2, -1, inkPages.Count);
            }
            else
            {
                currentPage = Mathf.Clamp(currentPage - 2, -1, inkPages.Count);
            }

            if (currentPage == -1)
            {
                ShowCover();
            }
            else if (currentPage > inkPages.Count - 1)
            {
                ShowBack();
            }
            else
            {
                if (currentPage + 1 > inkPages.Count - 1)
                {
                    ShowSingleInkPage(inkPages[currentPage], false);
                }
                else
                {
                    ShowInkPages(inkPages[currentPage], inkPages[currentPage + 1]);
                }
            }
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

        void ShowSingleInkPage((string title, List<PageElement> elements) page, bool renderOnRight)
        {
            soundbank.PlaySound(soundbank.bookFlip);
            bookFront.style.display = DisplayStyle.None;
            bookPage.style.display = DisplayStyle.Flex;
            bookBack.style.display = DisplayStyle.None;
            notePage.style.display = DisplayStyle.None;

            if (renderOnRight)
            {
                leftPageContent.Clear();
                RenderPageContent(page.elements, rightPageContent, rightPageTitle, page.title);
            }
            else
            {
                rightPageContent.Clear();
                RenderPageContent(page.elements, leftPageContent, leftPageTitle, page.title);
            }
        }

        void ShowInkPages((string title, List<PageElement> elements) left, (string title, List<PageElement> elements) right)
        {
            soundbank.PlaySound(soundbank.bookFlip);
            bookFront.style.display = DisplayStyle.None;
            bookPage.style.display = DisplayStyle.Flex;
            bookBack.style.display = DisplayStyle.None;
            notePage.style.display = DisplayStyle.None;

            RenderPageContent(left.elements, leftPageContent, leftPageTitle, left.title);
            RenderPageContent(right.elements, rightPageContent, rightPageTitle, right.title);
        }

        void RenderPageContent(List<PageElement> elements, VisualElement container, Label titleLabel, string title)
        {
            container.Clear();

            if (!string.IsNullOrEmpty(title))
            {
                var label = new Label(FormatText(title));
                label.style.whiteSpace = WhiteSpace.Normal;
                label.style.marginBottom = 6;
                label.AddToClassList("label-text");
                label.AddToClassList("book-chapter-title");
                container.Add(label);
            }

            foreach (var element in elements)
            {
                if (element.type == PageElementType.Text)
                {
                    var label = new Label(FormatText(element.content));
                    label.style.whiteSpace = WhiteSpace.Normal;
                    label.style.marginBottom = 6;
                    label.AddToClassList("label-text");
                    label.AddToClassList("book-chapter-paragraph");
                    container.Add(label);
                }
                else if (element.type == PageElementType.Image)
                {
                    var image = new Image();
                    var cleanPath = element.content.Replace("Assets/Resources/", "").Replace(".png", "");
                    var sprite = Resources.Load<Sprite>(cleanPath);
                    if (sprite != null)
                    {
                        image.image = sprite.texture;
                        image.style.width = sprite.texture.width;
                        image.style.height = sprite.texture.height;
                        image.scaleMode = ScaleMode.ScaleToFit;
                        image.style.marginTop = 8;
                        image.style.marginBottom = 8;
                        container.Add(image);
                    }
                    else
                    {
                        Debug.LogWarning($"[Book] Image not found: {element.content}");
                    }
                }
            }
        }

        string FormatText(string text) => text.Replace("\r", "");
    }
}
