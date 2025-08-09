using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AF.Health
{

    [RequireComponent(typeof(Slider))]
    [RequireComponent(typeof(CanvasGroup))]
    public class CharacterPostureUI : MonoBehaviour
    {
        CharacterBaseManager characterBaseManager;
        Slider slider => GetComponent<Slider>();
        CanvasGroup canvasGroup => GetComponent<CanvasGroup>();

        [Header("UI Options")]
        [SerializeField] TextMeshProUGUI currentAndMaxValue;

        [Header("Options")]
        [SerializeField] float canvasFadeSpeed = 1f;

        public void SetupEvents(CharacterBaseManager characterBaseManager)
        {
            this.characterBaseManager = characterBaseManager;

            characterBaseManager.characterPosture.onShowPostureBar.AddListener(OnUpdatePostureBar);
            characterBaseManager.characterPosture.onUpdatePostureBar.AddListener(OnUpdatePostureBar);
            characterBaseManager.characterPosture.onHidePostureBar.AddListener(OnHidePostureBar);

            OnHidePostureBar();
        }

        void ShowPostureBar()
        {
            if (!this.isActiveAndEnabled)
            {
                gameObject.SetActive(true);
                UIUtils.FadeIn(canvasGroup, canvasFadeSpeed);
            }
        }

        void OnHidePostureBar()
        {
            if (this.isActiveAndEnabled)
            {
                UIUtils.FadeOut(canvasGroup, canvasFadeSpeed, () =>
                {
                    gameObject.SetActive(false);
                });
            }
        }

        void OnUpdatePostureBar()
        {
            if (characterBaseManager.health.GetCurrentHealth() <= 0)
            {
                OnHidePostureBar();
                return;
            }

            float currentValue = characterBaseManager.characterPosture.currentPostureDamage;
            int maxValue = characterBaseManager.characterPosture.GetMaxPostureDamage();
            slider.value = currentValue / maxValue;
            slider.maxValue = 1f;

            currentAndMaxValue.text = $"{(int)currentValue}/{maxValue}";

            if (!gameObject.activeSelf)
            {
                ShowPostureBar();
            }
        }
    }
}
