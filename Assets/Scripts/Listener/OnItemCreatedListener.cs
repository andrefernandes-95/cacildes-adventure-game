namespace AF
{
    using UnityEngine;
    using UnityEngine.Events;

    public class OnItemCreatedListener : MonoBehaviour
    {
        [Header("Item Created Listener")]
        [SerializeField] Item itemCreated;
        [SerializeField] UnityEvent onItemCreated;
        [SerializeField] UIDocumentCraftScreen uIDocumentCraftScreen;

        void OnEnable()
        {
            uIDocumentCraftScreen.onItemCreated.AddListener(OnItemCreated);
        }

        void OnDisable()
        {
            uIDocumentCraftScreen.onItemCreated.RemoveListener(OnItemCreated);
        }

        void OnItemCreated(Item itemCreated)
        {
            if (itemCreated != null && itemCreated.EqualsTo(itemCreated))
            {
                onItemCreated?.Invoke();
            }
        }
    }
}
