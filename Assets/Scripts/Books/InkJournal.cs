using UnityEngine;
using Ink.Runtime;
using UnityEngine.Localization;

namespace AF.Journals
{
    public class InkJournalLoader : MonoBehaviour
    {
        public TextAsset portugueseBook;
        public TextAsset englishBook;

        UIDocumentBookV2 _uiDocumentBookV2;


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
    }
}
