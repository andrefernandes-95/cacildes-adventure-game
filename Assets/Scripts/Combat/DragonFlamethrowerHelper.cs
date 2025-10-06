using UnityEngine;

namespace AF
{
    [RequireComponent(typeof(ParticleSystem))]
    [RequireComponent(typeof(AudioSource))]
    public class DragonFlamethrowerHelper : MonoBehaviour
    {
        private ParticleSystem _particleSystem;
        private AudioSource _audioSource;

        private void Awake()
        {
            // Cache components for better performance
            _particleSystem = GetComponent<ParticleSystem>();
            _audioSource = GetComponent<AudioSource>();
        }

        public void EnableFlameThrower()
        {
            if (_particleSystem == null || _audioSource == null)
                return;
            var main = _particleSystem.main;
            main.loop = true;

            _particleSystem.Play();
            _audioSource.Play();
        }

        public void DisableFlameThrower()
        {
            var main = _particleSystem.main;
            main.loop = false;

            _particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            _audioSource.Stop();
        }
    }
}
