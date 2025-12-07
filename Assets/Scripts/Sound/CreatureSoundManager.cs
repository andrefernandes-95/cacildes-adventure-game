namespace AF
{
    using AF.Animations;
    using AF.Combat;
    using UnityEngine;

    public class CreatureSoundManager : MonoBehaviour
    {
        [SerializeField] CreatureSound creatureSound;

        [Header("Components")]
        [SerializeField] CharacterBaseManager characterBaseManager;
        [SerializeField] CharacterAnimationEventListener characterAnimationEventListener;
        [SerializeField] AmbushState ambushState;
        [SerializeField] TargetManager targetManager;

        [Header("Audio")]
        [SerializeField] int audioPoolSize = 5;
        private AudioSource[] pool;
        private int poolIndex = 0;

        void Awake()
        {
            pool = new AudioSource[audioPoolSize];
            for (int i = 0; i < audioPoolSize; i++)
            {
                pool[i] = gameObject.AddComponent<AudioSource>();
                pool[i].playOnAwake = false;
                pool[i].rolloffMode = AudioRolloffMode.Logarithmic;
                pool[i].spatialBlend = 1f;
            }
        }

        private void OnEnable()
        {
            if (creatureSound == null)
            {
                return;
            }

            SubscribeEvents();
        }

        private void OnDisable()
        {
            if (creatureSound == null)
            {
                return;
            }
            UnsubscribeEvents();
        }

        void SubscribeEvents()
        {
            if (creatureSound.attacks.Length > 0)
            {
                characterAnimationEventListener.onRightWeaponHitboxOpen.AddListener(OnAttack);
                characterAnimationEventListener.onLeftWeaponHitboxOpen.AddListener(OnAttack);
            }

            if (ambushState != null)
            {
                ambushState.onAmbushBegin.AddListener(OnAmbush);
            }

            if (targetManager != null && creatureSound.targetSpotted.Length > 0)
            {
                targetManager.onTargetSet_Event.AddListener(OnTargetSpotted);
            }

            if (characterAnimationEventListener != null)
            {
                if (creatureSound.roars.Length > 0)
                {
                    characterAnimationEventListener.onRoar.AddListener(OnRoar);
                }
            }

            if (creatureSound.hurt.Length > 0)
            {
                characterBaseManager.health.onTakeDamage.AddListener(OnHurt);
            }

            if (creatureSound.death.Length > 0)
            {
                characterBaseManager.health.onDeath.AddListener(OnDeath);
            }
        }

        void UnsubscribeEvents()
        {
            if (creatureSound.attacks.Length > 0)
            {
                characterAnimationEventListener.onRightWeaponHitboxOpen.RemoveListener(OnAttack);
                characterAnimationEventListener.onLeftWeaponHitboxOpen.RemoveListener(OnAttack);
            }

            if (ambushState != null)
            {
                ambushState.onAmbushBegin.RemoveListener(OnAmbush);
            }

            if (targetManager != null && creatureSound.targetSpotted.Length > 0)
            {
                targetManager.onTargetSet_Event.RemoveListener(OnTargetSpotted);
            }

            if (characterAnimationEventListener != null)
            {
                if (creatureSound.roars.Length > 0)
                {
                    characterAnimationEventListener.onRoar.RemoveListener(OnRoar);
                }
            }

            if (creatureSound.hurt.Length > 0)
            {
                characterBaseManager.health.onTakeDamage.RemoveListener(OnHurt);
            }

            if (creatureSound.death.Length > 0)
            {
                characterBaseManager.health.onDeath.RemoveListener(OnDeath);
            }
        }

        void OnAttack() => PlayFromSoundpack(creatureSound.attacks);

        void OnAmbush() => PlayFromSoundpack(creatureSound.ambush);

        void OnTargetSpotted() => PlayFromSoundpack(creatureSound.targetSpotted);
        void OnRoar() => PlayFromSoundpack(creatureSound.roars);
        void OnHurt() => PlayFromSoundpack(creatureSound.hurt);
        void OnDeath() => PlayFromSoundpack(creatureSound.death);


        void PlayFromSoundpack(AudioClip[] sounds)
        {
            if (sounds == null || sounds.Length == 0) return;

            var clip = sounds[Random.Range(0, sounds.Length)];
            var source = pool[poolIndex];

            source.PlayOneShot(clip);
            poolIndex = (poolIndex + 1) % pool.Length;
        }
    }
}
