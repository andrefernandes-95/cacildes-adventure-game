namespace AF
{
    using UnityEngine;
    using UnityEngine.UIElements;
    using DG.Tweening;

    public class MusicPlayerHUD : MonoBehaviour
    {
        [SerializeField] UIDocument uiDocument;

        VisualElement musicPlayerContainer;
        Label musicPlayingLabel;

        [SerializeField] float duration = 6f;
        [SerializeField] float initialDelay = 1f;

        private void Awake()
        {
            musicPlayerContainer = uiDocument.rootVisualElement.Q<VisualElement>("MusicPlaying");
            musicPlayingLabel = musicPlayerContainer.Q<Label>();

            musicPlayerContainer.style.opacity = 0f;
            musicPlayerContainer.style.translate = new StyleTranslate(new Translate(new Length(100, LengthUnit.Percent), 0));
        }

        private void OnEnable()
        {
            musicPlayerContainer.style.display = DisplayStyle.None;
        }

        public void DisplayMusic(AudioClip audioClip)
        {
            if (audioClip == null || string.IsNullOrEmpty(audioClip.name)) return;

            string clipName = audioClip.name;
            int start = clipName.IndexOf('(');
            int end = clipName.IndexOf(')');

            if (start == -1 || end == -1 || end <= start) return;

            string content = clipName.Substring(start + 1, end - start - 1);
            string[] parts = content.Split(',');

            if (parts.Length < 2) return;

            string title = parts[0].Trim();
            string artist = parts[1].Trim();

            musicPlayingLabel.text = $"{title} - {artist}";

            // Reset state
            musicPlayerContainer.style.opacity = 0f;
            musicPlayerContainer.style.translate = new StyleTranslate(new Translate(new Length(25, LengthUnit.Percent), 0));
            musicPlayerContainer.style.display = DisplayStyle.Flex;

            DOTween.Kill(musicPlayerContainer); // Cancel previous animations if any

            Sequence sequence = DOTween.Sequence();

            // Initial delay
            sequence.AppendInterval(initialDelay);

            // Slide in + fade in
            sequence.Append(DOVirtual.Float(100f, 0f, 0.5f, value =>
            {
                musicPlayerContainer.style.translate = new StyleTranslate(new Translate(new Length(value, LengthUnit.Percent), 0));
            }));

            sequence.Join(DOVirtual.Float(0f, 1f, 0.5f, value =>
            {
                musicPlayerContainer.style.opacity = value;
            }));

            // Wait before sliding out
            sequence.AppendInterval(duration);

            // Slide out + fade out
            sequence.Append(DOVirtual.Float(0f, -30f, 0.5f, value =>
            {
                musicPlayerContainer.style.translate = new StyleTranslate(new Translate(new Length(value, LengthUnit.Percent), 0));
            }));

            sequence.Join(DOVirtual.Float(1f, 0f, 0.5f, value =>
            {
                musicPlayerContainer.style.opacity = value;
            }));

            sequence.OnComplete(() =>
            {
                musicPlayerContainer.style.display = DisplayStyle.None;
            });
        }
    }
}
