using UnityEngine;
using System.Collections;
using TigerForge;
using AF.Events;

namespace AF.Music
{
    public class BGMManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        public AudioSource bgmAudioSource;
        public AudioSource ambienceAudioSource;
        public AudioSource sfxAudioSource;

        [Header("Settings")]
        public GameSettings gameSettings;
        public float fadeDuration = 1f;

        [Header("Components")]
        public SceneSettings sceneSettings;

        private Coroutine musicCoroutine;
        private Coroutine fadeCoroutine;

        private AudioClip mainMusic;

        private void Awake()
        {
            EventManager.StartListening(EventMessages.ON_MUSIC_VOLUME_CHANGED, ApplyVolumeSettings);
            ApplyVolumeSettings();
        }

        private void ApplyVolumeSettings()
        {
            float volume = gameSettings.GetMusicVolume();
            if (bgmAudioSource != null) bgmAudioSource.volume = volume;
            if (ambienceAudioSource != null) ambienceAudioSource.volume = volume;
        }

        public void PlayMusic(AudioClip clip)
        {
            if (clip == null || bgmAudioSource.clip == clip) return;

            StopCoroutineSafe(ref musicCoroutine);
            musicCoroutine = StartCoroutine(TransitionMusicCoroutine(clip));
        }

        public void StopMusic() => FadeOutMusic(clearClip: true);

        public void StopMusicImmediately()
        {
            StopAllCoroutines();
            bgmAudioSource.Stop();
            bgmAudioSource.clip = null;
        }

        private IEnumerator TransitionMusicCoroutine(AudioClip newClip)
        {
            yield return FadeMusic(toVolume: 0f);

            bgmAudioSource.clip = newClip;
            bgmAudioSource.Play();

            yield return FadeMusic(toVolume: gameSettings.GetMusicVolume());
        }

        private IEnumerator FadeMusic(float toVolume, bool clearClip = false)
        {
            float startVolume = bgmAudioSource.volume;
            float timeElapsed = 0f;

            while (timeElapsed < fadeDuration)
            {
                bgmAudioSource.volume = Mathf.Lerp(startVolume, toVolume, timeElapsed / fadeDuration);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            bgmAudioSource.volume = toVolume;

            if (toVolume == 0f)
            {
                bgmAudioSource.Stop();
                if (clearClip) bgmAudioSource.clip = null;
            }
        }

        private void FadeOutMusic(bool clearClip)
        {
            StopCoroutineSafe(ref fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeMusic(0f, clearClip));
        }

        public void PlayAmbience(AudioClip clip)
        {
            if (clip == null) return;
            ambienceAudioSource.clip = clip;
            ambienceAudioSource.Play();
        }

        public void StopAmbience()
        {
            ambienceAudioSource.Stop();
            ambienceAudioSource.clip = null;
        }

        public void PlaySound(AudioClip clip, AudioSource customSource = null, float volumeScale = 1f)
        {
            if (clip == null) return;
            (customSource ?? sfxAudioSource)?.PlayOneShot(clip, volumeScale);
        }

        public void PlaySoundWithPitchVariation(AudioClip clip, AudioSource source)
        {
            if (clip == null || source == null) return;
            source.pitch = Random.Range(0.99f, 1.01f);
            source.PlayOneShot(clip);
        }

        public bool IsPlayingMusicClip(string clipName) =>
            bgmAudioSource.clip != null && bgmAudioSource.clip.name == clipName;

        public bool IsPlayingMusicClip(AudioClip clip) =>
            bgmAudioSource.clip == clip;

        public bool IsNotPlayingMusic() =>
            bgmAudioSource.clip == null;

        public void PlayMusicalEffect(AudioClip effectClip)
        {
            if (effectClip == null) return;
            StopCoroutineSafe(ref musicCoroutine);
            musicCoroutine = StartCoroutine(MusicalEffectCoroutine(effectClip));
        }

        private IEnumerator MusicalEffectCoroutine(AudioClip effectClip)
        {
            if (bgmAudioSource.isPlaying)
                yield return FadeMusic(0f, clearClip: false);

            sfxAudioSource.PlayOneShot(effectClip);
            yield return new WaitForSeconds(effectClip.length);

            if (bgmAudioSource.clip != null)
            {
                bgmAudioSource.Play();
                yield return FadeMusic(gameSettings.GetMusicVolume(), clearClip: false);
            }
        }

        public bool IsBusy() => mainMusic != null;

        public void PlayMainMusic(AudioClip clip)
        {
            if (clip == null || clip == mainMusic) return;

            mainMusic = clip;
            PlayMusic(mainMusic);
        }

        public void ClearMainMusic()
        {
            if (mainMusic != null)
            {
                mainMusic = null;
                StopMusic();
            }
        }

        private void StopCoroutineSafe(ref Coroutine coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }
}
