namespace AF.UIExperimental
{
    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using DG.Tweening;

    /// <summary>
    /// Handles button hover/focus sounds, click sound, and subtle DOTween pop effect.
    /// Works with Soundbank class for audio playback.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class UIButton : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IPointerClickHandler, ISubmitHandler
    {

        [Header("Pop Effect Settings")]
        public float popScale = 1.05f;
        public float popDuration = 0.1f;
        private Vector3 originalScale;

        private Button button;

        Soundbank _soundbank;

        private void Awake()
        {
            button = GetComponent<Button>();
            originalScale = transform.localScale;
        }

        /// <summary>
        /// Called when the pointer enters the button
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData)
        {
            PlayHover();
        }

        /// <summary>
        /// Called when the button is selected (keyboard/controller navigation)
        /// </summary>
        public void OnSelect(BaseEventData eventData)
        {
            PlayHover();
        }

        /// <summary>
        /// Called when the button is clicked
        /// </summary>
        public void OnPointerClick(PointerEventData eventData)
        {
            PlayClick();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            PlayClick();
        }

        /// <summary>
        /// Play hover sound + pop effect
        /// </summary>
        private void PlayHover()
        {
            GetSoundbank().PlaySound(GetSoundbank().uiExperimental_hover);

            // Pop effect using DOTween
            transform.DOKill(); // stop any existing tween
            transform.localScale = originalScale;
            transform.DOScale(originalScale * popScale, popDuration).SetEase(Ease.OutBack)
                .OnComplete(() => transform.DOScale(originalScale, popDuration).SetEase(Ease.OutBack));
        }

        /// <summary>
        /// Play click sound
        /// </summary>
        private void PlayClick()
        {
            GetSoundbank().PlaySound(GetSoundbank().uiExperimental_click);
        }

        Soundbank GetSoundbank()
        {
            if (_soundbank == null)
            {
                _soundbank = FindAnyObjectByType<Soundbank>(FindObjectsInactive.Include);
            }

            return _soundbank;
        }

    }

}