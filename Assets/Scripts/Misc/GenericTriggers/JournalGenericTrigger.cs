namespace AF
{
    using AF.Events;
    using Ink.Runtime;
    using TigerForge;
    using UnityEngine;
    using UnityEngine.Localization.Settings;

    public class JournalGenericTrigger : GenericTrigger
    {
        [SerializeField] TextAsset englishBook;
        [SerializeField] TextAsset portugueseBook;
        UIDocumentBookV2 _uiDocumentBookV2;

        string currentTitle = "";

        bool hasLoadedBookMetadata = false;

        private void Awake()
        {
            LocalizationSettings.SelectedLocaleChanged += (value) =>
            {
                hasLoadedBookMetadata = false;
            };

            onActivate.AddListener(LoadInkBook);
        }

        [ContextMenu("Load Ink Book")]
        public void LoadInkBook()
        {
            GetUIDocumentBookV2().BeginReadInk(Utils.IsPortuguese() ? portugueseBook : englishBook);
        }

        UIDocumentBookV2 GetUIDocumentBookV2()
        {
            if (_uiDocumentBookV2 == null)
            {
                _uiDocumentBookV2 = FindAnyObjectByType<UIDocumentBookV2>(FindObjectsInactive.Include);
            }

            return _uiDocumentBookV2;
        }

        public override string GetAction()
        {
            TryLoadBookMetadata();

            if (Utils.IsPortuguese())
            {
                return $"Ler '{currentTitle}'";
            }

            return $"Read '{currentTitle}'";
        }

        void TryLoadBookMetadata()
        {
            if (hasLoadedBookMetadata)
            {
                return;
            }

            currentTitle = "";
            hasLoadedBookMetadata = true;

            Story story = new(Utils.IsPortuguese() ? portugueseBook.text : englishBook.text);

            while (story.canContinue && string.IsNullOrEmpty(currentTitle))
            {
                string raw = story.Continue().Trim();

                if (string.IsNullOrWhiteSpace(raw)) continue;

                if (raw.Contains(":"))
                {
                    if (raw.StartsWith("Title:", System.StringComparison.OrdinalIgnoreCase))
                    {
                        currentTitle = raw.Substring("Title:".Length).Trim();
                        continue;
                    }
                }
            }
        }
    }
}
