using AF.Events;
using TigerForge;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

namespace AF
{
    [ExecuteInEditMode]
    public class DayNightManager : MonoBehaviour
    {
        public Light directionalLight;

        [Header("Scene Light")]
        public bool useOverride = false;
        public Gradient AmbientColor;
        public Gradient DirectionalColor;
        public bool useFog = true;
        public Gradient FogColor;

        public float fogDensity = 0.03f;

        [Header("Values")]
        [Range(0, 24)]
        [TextArea]
        public string comment = "Put timeOfDay at 0 after finishing playing around with it";
        public bool tick = true;

        [Header("Skyboxes")]
        public Material dawnSky;
        public Material daySky;
        public Material duskSky;
        public Material nightfallSky;
        public Material nightSky;

        [Header("UI")]
        public UIDocumentPlayerHUDV2 uIDocumentPlayerHUDV2;
        public Sprite dawnSprite;
        public Sprite daySprite;
        public Sprite eveningSprite;
        public Sprite nightSprite;

        private IMGUIContainer dayNightIcon;
        private Label dayNightText;

        private StyleBackground dawnBg;
        private StyleBackground dayBg;
        private StyleBackground eveningBg;
        private StyleBackground nightBg;

        public SceneSettings sceneSettings;
        public bool canUpdateLighting = true;

        [Header("Components")]
        [SerializeField] CavernManager cavernManager;

        [Header("Systems")]
        public GameSession gameSession;
        [SerializeField] GameSettings gameSettings;

        // Coroutine for smooth lighting
        private Coroutine lightingCoroutine;
        private float transitionSpeed = 2f; // can be tuned for smoothness

        // -------------------------------------------------
        // Cached UI Setup
        // -------------------------------------------------

        private void OnEnable()
        {
            CacheUI();
            EventManager.StartListening(EventMessages.ON_HOUR_CHANGED, OnHourChanged);
            CacheStyleBackgrounds();
        }

        private void OnDisable()
        {
            EventManager.StopListening(EventMessages.ON_HOUR_CHANGED, OnHourChanged);
        }

        private void CacheUI()
        {
            if (uIDocumentPlayerHUDV2 == null || uIDocumentPlayerHUDV2.root == null)
                return;

            var root = uIDocumentPlayerHUDV2.root;
            dayNightIcon = root.Q<IMGUIContainer>("DayTimeIcon");
            dayNightText = root.Q<VisualElement>("Clock")?.Q<Label>("Value");
        }

        private void CacheStyleBackgrounds()
        {
            dawnBg = new StyleBackground(dawnSprite);
            dayBg = new StyleBackground(daySprite);
            eveningBg = new StyleBackground(eveningSprite);
            nightBg = new StyleBackground(nightSprite);
        }

        private void Start()
        {
            SetFogDensity(GetFogDensity());
            CacheUI();
            UpdateClockUI();
            UpdateClockIcon();
            ForceLightingUpdate();
        }

        // -------------------------------------------------
        // Time & Update
        // -------------------------------------------------

        private void Update()
        {
            if (!Application.isPlaying || gameSession == null || !gameSettings.UseDayAndNightCycling())
                return;

            // update clock (minutes)
            UpdateClockUI();

            if (!tick || !TimePassageAllowed())
                return;

            float newTime = gameSession.timeOfDay + Time.deltaTime * gameSession.daySpeed;
            float copy = newTime % 25f;
            newTime %= 24f;

            if (copy >= 24f && newTime < 23f)
                gameSession.daysPassed++;

            SetInternalTime(newTime);
        }

        private void SetInternalTime(float newValue)
        {
            float oldHour = Mathf.Floor(gameSession.timeOfDay);
            gameSession.timeOfDay = newValue;

            float newHour = Mathf.Floor(newValue);
            if (newHour != oldHour)
            {
                EventManager.EmitEvent(EventMessages.ON_HOUR_CHANGED);
            }
        }

        // -------------------------------------------------
        // Hour Event (UI + Lighting)
        // -------------------------------------------------

        private void OnHourChanged()
        {
            UpdateClockIcon();
            SmoothLightingTransition();
        }

        // -------------------------------------------------
        // CLOCK UI
        // -------------------------------------------------

        private void UpdateClockUI()
        {
            if (!gameSettings.UseDayAndNightCycling() && uIDocumentPlayerHUDV2)
            {
                if (uIDocumentPlayerHUDV2 != null && uIDocumentPlayerHUDV2.root != null)
                {
                    uIDocumentPlayerHUDV2.root.Q<IMGUIContainer>("DayTimeClockContainer").style.display = DisplayStyle.None;

                }
                return;
            }

            if (dayNightText == null || gameSession == null)
                return;

            int hour = Mathf.FloorToInt(gameSession.timeOfDay);
            int minutes = Mathf.FloorToInt((gameSession.timeOfDay - hour) * 60);
            dayNightText.text = $"{hour:00}:{minutes:00}";
        }

        private void UpdateClockIcon()
        {
            if (dayNightIcon == null)
                return;

            float t = gameSession.timeOfDay;

            if (t >= 5 && t < 8) dayNightIcon.style.backgroundImage = dawnBg;
            else if (t >= 8 && t < 17) dayNightIcon.style.backgroundImage = dayBg;
            else if (t >= 17 && t < 21) dayNightIcon.style.backgroundImage = eveningBg;
            else dayNightIcon.style.backgroundImage = nightBg;
        }

        // -------------------------------------------------
        // LIGHTING (Smooth Coroutine)
        // -------------------------------------------------

        void ForceLightingUpdate()
        {
            if (!gameSettings.UseDayAndNightCycling() || !canUpdateLighting)
            {
                return;
            }

            if (lightingCoroutine != null)
                StopCoroutine(lightingCoroutine);

            lightingCoroutine = StartCoroutine(LightingTransitionCoroutine(1f));
        }

        public void UpdateLighting()
        {
            SmoothLightingTransition();
        }

        private void SmoothLightingTransition()
        {
            if (!gameSettings.UseDayAndNightCycling() || !canUpdateLighting)
            {
                return;
            }

            if (lightingCoroutine != null)
                StopCoroutine(lightingCoroutine);

            lightingCoroutine = StartCoroutine(LightingTransitionCoroutine(0f));
        }

        private IEnumerator LightingTransitionCoroutine(float instant)
        {
            float timePercent = gameSession.timeOfDay / 24f;

            Color startAmbient = RenderSettings.ambientLight;
            Color targetAmbient = (ShouldOverride() ? GetAmbientColor() : gameSession.AmbientColor).Evaluate(timePercent);

            Color startFog = RenderSettings.fogColor;
            Color targetFog = (ShouldOverride() ? GetFogColor() : gameSession.FogColor).Evaluate(timePercent);

            Color startLight = directionalLight != null ? directionalLight.color : Color.white;
            Color targetLight = (ShouldOverride() ? GetDirectionalColor() : gameSession.DirectionalColor).Evaluate(timePercent);

            Quaternion startRot = directionalLight != null ? directionalLight.transform.localRotation : Quaternion.identity;
            Quaternion targetRot = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, -170f, 0));

            // Skybox immediate (cannot lerp)
            UpdateSkybox();

            bool fogEnabled = ShouldUseFog();
            RenderSettings.fog = fogEnabled;
            if (fogEnabled)
                RenderSettings.fogDensity = GetFogDensity();

            if (instant >= 1f)
            {
                // immediate
                RenderSettings.ambientLight = targetAmbient;
                if (fogEnabled) RenderSettings.fogColor = targetFog;
                if (directionalLight != null)
                {
                    directionalLight.color = targetLight;
                    directionalLight.transform.localRotation = targetRot;
                }

                yield break;
            }

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * transitionSpeed;
                RenderSettings.ambientLight = Color.Lerp(startAmbient, targetAmbient, t);
                if (fogEnabled)
                    RenderSettings.fogColor = Color.Lerp(startFog, targetFog, t);

                if (directionalLight != null)
                {
                    directionalLight.color = Color.Lerp(startLight, targetLight, t);
                    directionalLight.transform.localRotation = Quaternion.Slerp(startRot, targetRot, t);
                }

                yield return null;
            }
        }

        private void UpdateSkybox()
        {
            if (!gameSettings.UseDayAndNightCycling())
            {
                return;
            }

            float tod = gameSession.timeOfDay;

            if (tod >= 7 && tod < 18) RenderSettings.skybox = daySky;
            else if (tod >= 18 && tod < 20) RenderSettings.skybox = duskSky;
            else if (tod >= 20 && tod < 22) RenderSettings.skybox = nightfallSky;
            else if (tod >= 22 || tod < 5) RenderSettings.skybox = nightSky;
            else if (tod >= 5 && tod < 7) RenderSettings.skybox = dawnSky;
        }

        // -------------------------------------------------
        // Helpers
        // -------------------------------------------------

        public void AdvanceOneHour() => SetHour((int)(gameSession.timeOfDay + 1));
        public void GoBackOneHour() => SetHour((int)(gameSession.timeOfDay + 23) % 24);
        public void SetHour(int hour) => SetTimeOfDay(hour, 0);
        public void SetTimeOfDay(int hours, int minutes) => SetInternalTime(hours + (minutes / 60f));

        public bool TimePassageAllowed()
        {
            if (!gameSettings.UseDayAndNightCycling())
            {
                return false;
            }

            if (sceneSettings?.sceneLocation != null)
                return !sceneSettings.sceneLocation.isInterior;

            return sceneSettings == null || !sceneSettings.isInterior;
        }

        // -------------------------------------------------
        // Environment Values
        // -------------------------------------------------

        bool ShouldUseFog()
        {
            if (cavernManager.IsInCavern()) return true;
            if (sceneSettings?.sceneLocation != null && sceneSettings.sceneLocation.useFog) return true;
            return useFog;
        }

        bool ShouldOverride()
        {
            return (sceneSettings?.sceneLocation != null && sceneSettings.sceneLocation.useSceneLightSettings) || useOverride;
        }

        float GetFogDensity()
        {
            if (cavernManager.IsInCavern())
                return cavernManager.currentCavern.CavernFogDensity;

            if (sceneSettings?.sceneLocation != null && sceneSettings.sceneLocation.useSceneLightSettings)
                return sceneSettings.sceneLocation.fogDensity;

            return fogDensity;
        }

        Gradient GetAmbientColor() =>
            cavernManager.IsInCavern()
                ? cavernManager.currentCavern.CavernAmbientColor
                : sceneSettings?.sceneLocation != null && sceneSettings.sceneLocation.useSceneLightSettings
                    ? sceneSettings.sceneLocation.AmbientColor
                    : AmbientColor;

        Gradient GetDirectionalColor() =>
            cavernManager.IsInCavern()
                ? cavernManager.currentCavern.CavernDirectionalColor
                : sceneSettings?.sceneLocation != null && sceneSettings.sceneLocation.useSceneLightSettings
                    ? sceneSettings.sceneLocation.DirectionalColor
                    : DirectionalColor;

        Gradient GetFogColor() =>
            cavernManager.IsInCavern()
                ? cavernManager.currentCavern.CavernFogColor
                : sceneSettings?.sceneLocation != null && sceneSettings.sceneLocation.useSceneLightSettings
                    ? sceneSettings.sceneLocation.FogColor
                    : FogColor;

        public void SetFogDensity(float value)
        {
            if (!gameSettings.UseDayAndNightCycling())
            {
                return;
            }

            RenderSettings.fogDensity = value;
        }
    }
}
