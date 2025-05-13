using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UIElements;

namespace AF
{
    [RequireComponent(typeof(UIDocument))]
    public class UISettings_Language : MonoBehaviour
    {
        UIDocument uIDocument => GetComponent<UIDocument>();
        VisualElement root => uIDocument.rootVisualElement;

        void OnEnable()
        {
            SetupUI();
        }

        void SetupUI()
        {
            RadioButtonGroup gameLanguageOptions = root.Q<RadioButtonGroup>("GameLanguage");
            gameLanguageOptions.value = Utils.IsPortuguese() ? 1 : 0;
            gameLanguageOptions.Focus();

            gameLanguageOptions.RegisterValueChangedCallback(ev =>
            {
                if (ev.newValue == 0)
                {
                    LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("en");
                }
                else
                {
                    LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("pt");
                }
            });
        }
    }
}
