namespace AF
{
    using UnityEngine;

    public class GameSettingsManager : MonoBehaviour
    {
        [SerializeField] private GameSettings gameSettings;

        private void Awake()
        {
            // Load settings before anything else
            gameSettings.LoadSettings();
        }


        private void OnApplicationQuit()
        {
            gameSettings.SaveSettings();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                gameSettings.SaveSettings();
            }
        }
    }
}