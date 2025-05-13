using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    [RequireComponent(typeof(UIDocument))]

    public class UISettings_Graphics : MonoBehaviour
    {

        UIDocument uIDocument => GetComponent<UIDocument>();
        VisualElement root => uIDocument.rootVisualElement;

        [SerializeField] private GameSettings gameSettings;

        void OnEnable()
        {
            SetupUI();
        }

        void SetupUI()
        {
            RadioButtonGroup graphicsOptions = root.Q<RadioButtonGroup>("GraphicsQuality");

            graphicsOptions.value = gameSettings.GetGraphicsQuality();
            graphicsOptions.Focus();

            graphicsOptions.RegisterValueChangedCallback(ev =>
            {
                gameSettings.SetGameQuality(ev.newValue);
            });
        }
    }
}
