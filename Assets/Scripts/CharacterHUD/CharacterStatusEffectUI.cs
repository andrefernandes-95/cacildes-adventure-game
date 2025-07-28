namespace AF
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class CharacterStatusEffectUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] Slider slider;
        [SerializeField] TextMeshProUGUI statusEffectLabel;
        [SerializeField] TextMeshProUGUI amountLabel;
        [SerializeField] Image sliderBackground;
        [SerializeField] Image sliderFill;
        [SerializeField] Image effectIcon;

        public void UpdateUI(StatusEffect statusEffect, float amount, float maxAmount, bool isApplied)
        {
            if (isApplied)
            {
                sliderBackground.color = Color.black;
                statusEffectLabel.text = statusEffect.GetAppliedName();
            }
            else
            {
                sliderBackground.color = Color.white;
                statusEffectLabel.text = statusEffect.GetName();
            }

            sliderFill.color = statusEffect.barColor;
            slider.maxValue = maxAmount * 0.01f;
            slider.value = amount * 0.01f;
            effectIcon.sprite = statusEffect.icon;

            amountLabel.text = $"{(int)amount}/{(int)maxAmount}";
        }

    }
}
