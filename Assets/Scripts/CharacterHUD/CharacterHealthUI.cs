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

        [Header("UI Options")]
        [SerializeField] TextMeshProUGUI currentAndMaxHealthValue;

        [Header("Options")]
        [SerializeField] float canvasFadeSpeed = 1f;

        public void SetupEvents(CharacterBaseManager characterBaseManager)
        {
            this.characterBaseManager = characterBaseManager;

            characterBaseManager.health.onShowHealthbar.AddListener(OnUpdateHealthbar);
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
            if (characterBaseManager.health.GetCurrentHealth() <= 0 || IsBoss())
            {
                OnHideHealthbar();
                return;
            }

            float currentHealth = characterBaseManager.health.GetCurrentHealth();
            int maxHealth = characterBaseManager.health.GetMaxHealth();
            slider.value = currentHealth * 0.01f;
            slider.maxValue = characterBaseManager.health.GetMaxHealth() * 0.01f;

            currentAndMaxHealthValue.text = $"{(int)currentHealth}/{maxHealth}";

            if (!gameObject.activeSelf)
            {
                ShowHealthbar();
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
