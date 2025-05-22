
using UnityEngine;
using UnityEngine.UIElements;
using DG.Tweening;

namespace AF
{
    [RequireComponent(typeof(UIDocument))]
    public class UIDocumentAlert : MonoBehaviour
    {
        VisualElement root => GetComponent<UIDocument>().rootVisualElement;

        VisualElement icon;
        Label alertText;

        private void Start()
        {
            icon = root.Q<VisualElement>("Icon");
            alertText = root.Q<Label>("AlertText");
            root.style.display = DisplayStyle.None;
        }

        public void ShowAlert(string message)
        {
            alertText.text = message;

            root.style.display = DisplayStyle.Flex;
            root.style.opacity = 0f;
            root.transform.scale = new Vector3(0.5f, 0.5f, 1f);

            // Animate opacity and scale using DOTween
            DOTween.Kill(root); // Kill any previous tweens on this element

            Sequence seq = DOTween.Sequence();
            seq.Append(DOTween.To(() => root.style.opacity.value, x => root.style.opacity = x, 1f, 0.2f));
            seq.Join(DOTween.To(() => root.transform.scale, x => root.transform.scale = x, new Vector3(1.1f, 1.1f, 1f), 0.2f));
            seq.Append(DOTween.To(() => root.transform.scale, x => root.transform.scale = x, Vector3.one, 0.1f));
            seq.AppendInterval(1f); // Stay visible for 1 second
            seq.Append(DOTween.To(() => root.style.opacity.value, x => root.style.opacity = x, 0f, 0.3f));
            seq.OnComplete(() => root.style.display = DisplayStyle.None);
        }

    }

}
