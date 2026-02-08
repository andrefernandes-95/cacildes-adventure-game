namespace AF
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class UIDocumentLoadingSpinner : MonoBehaviour
    {
        [SerializeField] UIDocument uIDocument;

        private VisualElement container;
        private VisualElement spinner;
        private bool spinning;

        Coroutine SpinCoroutine;

        private void Awake()
        {
            container = uIDocument.rootVisualElement.Q<VisualElement>("SpinnerContainer");
            spinner = uIDocument.rootVisualElement.Q<VisualElement>("Spinner");
            Hide();
        }
        public void Show()
        {
            container.style.display = DisplayStyle.Flex;
            spinning = true;

            if (SpinCoroutine != null)
            {
                StopCoroutine(SpinCoroutine);
            }

            SpinCoroutine = StartCoroutine(Spin());
        }


        public void Hide()
        {
            spinning = false;

            if (SpinCoroutine != null)
            {
                StopCoroutine(SpinCoroutine);
            }

            container.style.display = DisplayStyle.None;
        }

        private IEnumerator Spin()
        {
            float rotation = 0f;
            while (spinning)
            {
                rotation += 360f * Time.deltaTime;
                spinner.style.rotate = new Rotate(new Angle(rotation));
                yield return null;
            }
        }
    }
}
