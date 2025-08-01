namespace AF
{
    using UnityEngine;

    public class AlchemyGenericTrigger : GenericTrigger
    {
        UIDocumentCraftScreen _uIDocumentCraftScreen;

        private void Awake()
        {
            onActivate.AddListener(OnActivate);
        }

        public void OnActivate()
        {
            GetUIDocumentCraftScreen().OpenAlchemyMenu();
        }

        UIDocumentCraftScreen GetUIDocumentCraftScreen()
        {
            if (_uIDocumentCraftScreen == null)
            {
                _uIDocumentCraftScreen = FindAnyObjectByType<UIDocumentCraftScreen>(FindObjectsInactive.Include);
            }

            return _uIDocumentCraftScreen;
        }

        public override string GetAction()
        {
            if (Utils.IsPortuguese())
            {
                return $"Usar mesa de alquimia";
            }

            return $"Use alchemy table";
        }
    }
}
