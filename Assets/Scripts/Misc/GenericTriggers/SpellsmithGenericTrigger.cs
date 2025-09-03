namespace AF
{
    using UnityEngine;

    public class SpellsmithGenericTrigger : GenericTrigger
    {
        UIDocumentBlacksmith _uiDocumentBlacksmith;

        private void Awake()
        {
            onActivate.AddListener(OpenSpellSmithingMenu);
        }

        [ContextMenu("Load Ink Book")]
        public void OpenSpellSmithingMenu()
        {
            GetUIDocumentBlacksmith().OpenSpellSmithingMenu();
        }

        UIDocumentBlacksmith GetUIDocumentBlacksmith()
        {
            if (_uiDocumentBlacksmith == null)
            {
                _uiDocumentBlacksmith = FindAnyObjectByType<UIDocumentBlacksmith>(FindObjectsInactive.Include);
            }

            return _uiDocumentBlacksmith;
        }

        public override string GetAction()
        {
            if (Utils.IsPortuguese())
            {
                return $"Usar fogo para melhorar feitiços";
            }

            return $"Use flames to improve spells";
        }
    }
}
