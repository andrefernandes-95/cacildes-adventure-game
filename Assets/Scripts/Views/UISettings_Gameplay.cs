using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace AF
{
    [RequireComponent(typeof(UIDocument))]
    public class UISettings_Gameplay : MonoBehaviour
    {
        UIDocument uIDocument => GetComponent<UIDocument>();
        VisualElement root => uIDocument.rootVisualElement;

        [Header("Key Rebinding")]
        VisualElement pressAnyKeyModal;
        public StarterAssetsInputs inputs;

        [Header("Databases")]
        public GameSettings gameSettings;

        void OnEnable()
        {
            SetupUI();
        }

        void SetupUI()
        {
            Slider cameraSensitivity = root.Q<Slider>("CameraSensitivity");

            cameraSensitivity.lowValue = gameSettings.minimumMouseSensitivity;
            cameraSensitivity.highValue = gameSettings.maximumMouseSensitivity;
            cameraSensitivity.value = gameSettings.GetCameraSensitivity();
            cameraSensitivity.label =
                (Utils.IsPortuguese() ? "Sensibilidade da Câmara: " : "Camera Sensitivity: ") + gameSettings.GetCameraSensitivity();

            cameraSensitivity.RegisterValueChangedCallback(ev =>
            {
                gameSettings.SetCameraSensitivity(ev.newValue);
                cameraSensitivity.label =
                    (Utils.IsPortuguese() ? "Sensibilidade da Câmara: " : "Camera Sensitivity: ") + gameSettings.GetCameraSensitivity();
            });

            Toggle invertYAxis = root.Q<Toggle>("InvertYAxis");
            invertYAxis.value = gameSettings.GetInvertYAxis();
            invertYAxis.RegisterValueChangedCallback(ev =>
            {
                gameSettings.SetInvertYAxis(ev.newValue);
            });

        }
    }
}
