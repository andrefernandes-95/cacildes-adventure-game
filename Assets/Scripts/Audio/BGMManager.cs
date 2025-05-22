using UnityEngine;
using System.Collections;
using System.Linq;
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
        public float fadeMusicSpeed = .1f;

        [Header("Components")]
        public SceneSettings sceneSettings;

        // Internal
        Coroutine HandleMusicChangeCoroutine;
        Coroutine FadeInCoreCoroutine;
        Coroutine FadeOutCoreCoroutine;

        // Flags
        public bool isPlayingBossMusic = false;

        private void Awake()
        {
            EventManager.StartListening(EventMessages.ON_MUSIC_VOLUME_CHANGED, HandleVolume);
        }

        private void Start()
        {
            HandleVolume();
        }

        void HandleVolume()
        {
            bgmAudioSource.volume = gameSettings.GetMusicVolume();
            ambienceAudioSource.volume = gameSettings.GetMusicVolume();
        }

        public void PlayMusic(AudioClip musicToPlay)
        {
            if (this.bgmAudioSource.clip != null)
            {
                if (HandleMusicChangeCoroutine != null)
                {
                    StopCoroutine(HandleMusicChangeCoroutine);
                }

                HandleMusicChangeCoroutine = StartCoroutine(HandleMusicChange_Coroutine(musicToPlay));
            }
            else
            {
                // No music playing before, lets do a fade in
                this.bgmAudioSource.volume = 0;
                this.bgmAudioSource.clip = musicToPlay;
                this.bgmAudioSource.Play();

                if (FadeInCoreCoroutine != null)
                {
                    StopCoroutine(FadeInCoreCoroutine);
                }

                FadeInCoreCoroutine = StartCoroutine(FadeCore(false));
            }
        }

        IEnumerator HandleMusicChange_Coroutine(AudioClip musicToPlay)
        {
            yield return FadeCore(fadeOut: true);

            bgmAudioSource.clip = musicToPlay;
            bgmAudioSource.Play();

            yield return FadeCore(fadeOut: false);
        }

        IEnumerator FadeCore(bool fadeOut, float fadeDuration = 1, bool clearMusic = true)
        {
            float targetVolume = fadeOut ? 0f : gameSettings.GetMusicVolume();
            float startVolume = bgmAudioSource.volume;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                float newVolume = Mathf.Lerp(startVolume, targetVolume, elapsedTime / fadeDuration);
                bgmAudioSource.volume = newVolume;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            bgmAudioSource.volume = targetVolume;

            if (fadeOut)
            {
                bgmAudioSource.Stop();

                if (clearMusic)
                {
                    bgmAudioSource.clip = null;
                }
            }
        }

        void StopCoroutines()
        {
            if (FadeInCoreCoroutine != null)
            {
                StopCoroutine(FadeInCoreCoroutine);
            }

            if (HandleMusicChangeCoroutine != null)
            {
                StopCoroutine(HandleMusicChangeCoroutine);
            }

            if (FadeOutCoreCoroutine != null)
            {
                StopCoroutine(FadeOutCoreCoroutine);
            }
        }

        public void StopMusic()
        {
            StopCoroutines();

            if (this.bgmAudioSource.clip != null)
            {
                FadeOutCoreCoroutine = StartCoroutine(FadeCore(true));
            }
            else
            {
                this.bgmAudioSource.Stop();
                this.bgmAudioSource.clip = null;
            }
        }

        public void StopMusicImmediately()
        {
            this.bgmAudioSource.Stop();
            this.bgmAudioSource.clip = null;
        }

        public void PlayAmbience(AudioClip ambience)
        {
            this.ambienceAudioSource.clip = ambience;
            this.ambienceAudioSource.Play();
        }

        public void StopAmbience()
        {
            this.ambienceAudioSource.clip = null;
            this.ambienceAudioSource.Stop();
        }

        public void PlaySound(AudioClip sfxToPlay, AudioSource customAudioSource, float volumeScale = 1f)
        {
            if (customAudioSource != null)
            {
                customAudioSource.PlayOneShot(sfxToPlay);
                return;
            }

            this.sfxAudioSource.PlayOneShot(sfxToPlay, volumeScale);
        }

        public void PlaySoundWithPitchVariation(AudioClip sfxToPlay, AudioSource customAudioSource)
        {
            float pitch = UnityEngine.Random.Range(0.99f, 1.01f);
            customAudioSource.pitch = pitch;
            customAudioSource.PlayOneShot(sfxToPlay);
        }

        public void PlayMapMusicAfterKillingEnemy()
        {
            sceneSettings.HandleSceneSound(true);
        }

        public bool IsPlayingMusicClip(string clipName)
        {
            if (this.bgmAudioSource.clip == null)
            {
                return false;
            }

            if (this.bgmAudioSource.clip.name == clipName)
            {
                return true;
            }

            return false;
        }

        public bool IsNotPlayingMusic()
        {
            return this.bgmAudioSource.clip == null;
        }

        public void PlayMusicalEffect(AudioClip musicEffect)
        {
            StartCoroutine(PauseMusicPlayEffectThenResume_Coroutine(musicEffect));
        }

        IEnumerator PauseMusicPlayEffectThenResume_Coroutine(AudioClip musicEffect)
        {
            if (bgmAudioSource.isPlaying)
            {
                // Fade out the music before pausing
                yield return FadeCore(fadeOut: true, .1f, false);
            }

            // Play the one-shot musical effect
            sfxAudioSource.PlayOneShot(musicEffect);

            // Wait until the sound finishes playing
            yield return new WaitForSeconds(musicEffect.length);

            // Resume the original music clip with fade-in
            if (bgmAudioSource.clip != null)
            {
                bgmAudioSource.Play();
                FadeInCoreCoroutine = StartCoroutine(FadeCore(fadeOut: false));
            }
        }
    }
}
