using System;
using System.Linq;
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

        [SerializeField] GameSettings gameSettings;

        void OnEnable()
        {
            SetupUI();
        }

        void SetupUI()
        {
            RadioButtonGroup gameLanguageOptions = root.Q<RadioButtonGroup>("GameLanguage");
            gameLanguageOptions.value = gameSettings.GetGameLanguage() == "pt" ? 1 : 0;
            gameLanguageOptions.Focus();

            gameLanguageOptions.RegisterValueChangedCallback(ev =>
            {
                if (ev.newValue == 0)
                {
                    gameSettings.SetGameLanguage("en");
                }
                else
                {
                    gameSettings.SetGameLanguage("pt");
                }
            });

            SetupAutoLanguageDropdown();
        }

        void SetupAutoLanguageDropdown()
        {
            // Assuming your DropdownField is already populated with GoogleLanguage enum values
            EnumField autoLanguageDropdown = root.Q<EnumField>("AutoTranslation");

            // Set initial value based on current locale
            string currentCode = gameSettings.automaticTranslationCode;

            GoogleLanguage currentLanguage = Enum.GetValues(typeof(GoogleLanguage))
                                                 .Cast<GoogleLanguage>()
                                                 .FirstOrDefault(lang => lang.ToCode() == currentCode);

            autoLanguageDropdown.value = currentLanguage;

            // Register callback for when the user selects a new language
            autoLanguageDropdown.RegisterValueChangedCallback(ev =>
            {
                GoogleLanguage currentLanguage = (GoogleLanguage)ev.newValue;

                gameSettings.SetAutomaticTranslation(currentLanguage.ToCode());
            });
        }
    }
}
