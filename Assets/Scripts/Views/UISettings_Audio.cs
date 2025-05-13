using UnityEngine;
using UnityEngine.UIElements;

namespace AF
{
    [RequireComponent(typeof(UIDocument))]
    public class UISettings_Audio : MonoBehaviour
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
            Slider musicVolumeSlider = root.Q<Slider>("MusicVolume");

            musicVolumeSlider.lowValue = 0f;
            musicVolumeSlider.highValue = 1f;
            musicVolumeSlider.value = gameSettings.GetMusicVolume();
            musicVolumeSlider.label = (Utils.IsPortuguese()
                ? "Volume da Música:" : "Music Volume: ") + gameSettings.GetMusicVolume();

            musicVolumeSlider.RegisterValueChangedCallback(ev =>
            {
                gameSettings.SetMusicVolume(ev.newValue);
                musicVolumeSlider.label = (Utils.IsPortuguese()
                    ? "Volume da Música:" : "Music Volume: ") + gameSettings.GetMusicVolume();
            });
        }
    }
}
