using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AF.Health
{
    [RequireComponent(typeof(Slider))]
    [RequireComponent(typeof(CanvasGroup))]
    public class CharacterHealthUI : MonoBehaviour
    {
        CharacterBaseManager characterBaseManager;

        Slider slider => GetComponent<Slider>();
        CanvasGroup canvasGroup => GetComponent<CanvasGroup>();

        [Header("Slider Background")]
        [SerializeField] Image sliderImage;
        [SerializeField] Color normalSliderImage;
        [SerializeField] Color cursedSliderImage;


        [Header("UI Options")]
        [SerializeField] TextMeshProUGUI currentAndMaxHealthValue;

        [Header("Options")]
        [SerializeField] float canvasFadeSpeed = 1f;

        public void SetupEvents(CharacterBaseManager characterBaseManager)
        {
            this.characterBaseManager = characterBaseManager;

            characterBaseManager.health.onShowHealthbar.AddListener(ShowHealthbar);
            characterBaseManager.health.onUpdateHealthbar.AddListener(OnUpdateHealthbar);
            characterBaseManager.health.onHideHealthbar.AddListener(OnHideHealthbar);

            OnHideHealthbar();
        }

        void ShowHealthbar()
        {
            if (IsBoss())
            {
                OnHideHealthbar();
                return;
            }

            if (!this.isActiveAndEnabled)
            {
                gameObject.SetActive(true);
                UIUtils.FadeIn(canvasGroup, canvasFadeSpeed);
            }
        }

        void OnHideHealthbar()
        {
            if (this.isActiveAndEnabled)
            {
                UIUtils.FadeOut(canvasGroup, canvasFadeSpeed, () =>
                {
                    gameObject.SetActive(false);
                });
            }
        }

        void OnUpdateHealthbar()
        {
            if (IsBoss())
            {
                OnHideHealthbar();
                return;
            }

            float currentHealth = characterBaseManager.health.GetCurrentHealth();
            int maxHealth = characterBaseManager.health.GetMaxHealth();
            slider.value = currentHealth / maxHealth;
            slider.maxValue = 1f;

            sliderImage.color = characterBaseManager.health.hasHealthCutInHalf ? cursedSliderImage : normalSliderImage;

            currentAndMaxHealthValue.text = $"{(int)currentHealth}/{maxHealth}";

            if (characterBaseManager.health.GetCurrentHealth() <= 0)
            {
                OnHideHealthbar();
            }
        }

        bool IsBoss()
        {
            if (characterBaseManager is CharacterManager characterManager)
            {
                return characterManager.characterBossController.IsBoss();
            }

            return false;
        }
    }
}
