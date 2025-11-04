using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace AF
{
    public static class UIUtils
    {
        public static void SetupButton(Button button, UnityAction callback, Soundbank soundbank)
        {
            SetupButton(button, callback, null, null, true, soundbank);
        }

        public static void SetupButton(
            Button button,
            UnityAction callback,
            UnityAction onFocusInCallback,
            UnityAction onFocusOutCallback,
            bool hasPopupAnimation,
            Soundbank soundbank)
        {
            button.RegisterCallback<ClickEvent>(ev =>
            {
                if (hasPopupAnimation)
                {
                    PlayPopAnimation(button);
                }


                soundbank.PlaySound(soundbank.uiDecision);
                callback.Invoke();
            });
            button.RegisterCallback<NavigationSubmitEvent>(ev =>
            {
                if (hasPopupAnimation)
                {
                    PlayPopAnimation(button);
                }

                soundbank.PlaySound(soundbank.uiDecision);
                callback.Invoke();
            });

            button.RegisterCallback<FocusInEvent>(ev =>
            {
                if (hasPopupAnimation)
                {
                    PlayPopAnimation(button);
                }

                soundbank.PlayUIHoverSound();
                onFocusInCallback?.Invoke();
            });

            button.RegisterCallback<PointerEnterEvent>(ev =>
            {
                soundbank.PlayUIHoverSound();
                onFocusInCallback?.Invoke();
            });

            button.RegisterCallback<FocusOutEvent>(ev =>
            {
                onFocusOutCallback?.Invoke();
            });
            button.RegisterCallback<PointerOutEvent>(ev =>
            {
                onFocusOutCallback?.Invoke();
            });
        }

        public static void PlayPopAnimation(VisualElement button)
        {
            PlayPopAnimation(button, Vector3.zero);
        }

        public static void PlayPopAnimation(VisualElement button, Vector3 startingScale)
        {
            button.transform.scale = Vector3.one;

            DOTween.To(
                () => startingScale,
                scale => button.transform.scale = scale,
                Vector3.one,
                0.5f
            ).SetEase(Ease.OutElastic);
        }

        public static void ScrollToLastPosition(int currentIndex, ScrollView scrollView, UnityAction onFinish)
        {
            if (scrollView == null || scrollView.childCount == 0)
            {
                onFinish?.Invoke();
                return;
            }

            // Clamp index so we never go out of bounds
            int index = Mathf.Clamp(currentIndex, 0, scrollView.childCount - 1);

            // Try to get the element
            VisualElement element = scrollView.ElementAt(index);

            if (element != null)
            {
                element.Focus();
                scrollView.ScrollTo(element);
            }

            onFinish?.Invoke();
        }

        public static void FadeIn(VisualElement element, float duration = 0.5f, UnityAction onComplete = null)
        {
            if (element == null) return;

            // Set initial opacity
            element.style.opacity = 0f;
            element.style.display = DisplayStyle.Flex;

            float currentOpacity = 0f;

            DOTween.To(() => currentOpacity, x =>
            {
                currentOpacity = x;
                element.style.opacity = currentOpacity;
            }, 1f, duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                element.style.opacity = 1f;
                onComplete?.Invoke();
            });
        }
        public static void FadeIn(CanvasGroup canvasGroup, float duration = 0.5f, UnityAction onComplete = null)
        {
            if (canvasGroup == null || canvasGroup.alpha >= 1f) return;

            canvasGroup.DOKill(); // kill any existing tweens
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            canvasGroup.DOFade(1f, duration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => onComplete?.Invoke());
        }

        public static void FadeOut(CanvasGroup canvasGroup, float duration = 0.5f, UnityAction onComplete = null)
        {
            if (canvasGroup == null || canvasGroup.alpha <= 0f) return;

            canvasGroup.DOKill(); // kill any existing tweens

            canvasGroup.DOFade(0f, duration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                    onComplete?.Invoke();
                });
        }
    }
}
