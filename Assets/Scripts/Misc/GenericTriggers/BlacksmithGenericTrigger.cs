namespace AF
{
    using UnityEngine;

    public class BlacksmithGenericTrigger : GenericTrigger
    {
        UIDocumentBlacksmith _uiDocumentBlacksmith;

        private void Awake()
        {
            onActivate.AddListener(OpenBlacksmith);
        }

        [ContextMenu("Load Ink Book")]
        public void OpenBlacksmith()
        {
            GetUIDocumentBlacksmith().OpenBlacksmithMenu();
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
                return $"Usar bigorna para melhorar equipamento";
            }

            return $"Use anvil to improve equipment";
        }
    }
}
