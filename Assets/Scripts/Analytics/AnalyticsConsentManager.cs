namespace AF
{
    using UnityEngine;
    using Unity.Services.Core;
    using System.Threading.Tasks;
    using Unity.Services.Analytics;

    public class AnalyticsConsentManager : MonoBehaviour
    {
        public static AnalyticsConsentManager Instance;

        private const string ConsentKey = "AnalyticsConsent";

        public AnalyticsConsentState ConsentState
        {
            get => (AnalyticsConsentState)PlayerPrefs.GetInt(ConsentKey, 0);
            private set => PlayerPrefs.SetInt(ConsentKey, (int)value);
        }

        private async void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (ConsentState == AnalyticsConsentState.Accepted)
            {
                await InitializeAnalytics();
            }
        }

        public async void AcceptAnalytics()
        {
            ConsentState = AnalyticsConsentState.Accepted;
            await InitializeAnalytics();
        }

        public void DeclineAnalytics()
        {
            ConsentState = AnalyticsConsentState.Declined;
            Debug.Log("Analytics disabled.");
        }

        private async Task InitializeAnalytics()
        {
            if (UnityServices.State == ServicesInitializationState.Initialized)
                return;

            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
            Debug.Log("Analytics enabled.");
        }

        public bool CanTrack() => ConsentState == AnalyticsConsentState.Accepted;
    }
}
