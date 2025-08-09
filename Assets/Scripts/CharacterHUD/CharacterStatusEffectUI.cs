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

        public void UpdateUI(string buildUpName, string appliedName, Sprite icon, Color barColor, float amount, float maxAmount, bool isApplied)
        {
            if (isApplied)
            {
                sliderBackground.color = Color.black;
                statusEffectLabel.text = appliedName;
            }
            else
            {
                sliderBackground.color = Color.white;
                statusEffectLabel.text = buildUpName;
            }

            sliderFill.color = barColor;
            slider.maxValue = 1f;
            slider.value = amount / maxAmount;
            effectIcon.sprite = icon;

            amountLabel.text = $"{(int)amount}/{(int)maxAmount}";
        }

    }
}
