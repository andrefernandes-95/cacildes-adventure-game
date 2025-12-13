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
            if (creatureSound == null)
            {
                return;
            }

            pool = new AudioSource[audioPoolSize];
            for (int i = 0; i < audioPoolSize; i++)
            {
                pool[i] = gameObject.AddComponent<AudioSource>();
                pool[i].playOnAwake = false;
                pool[i].rolloffMode = AudioRolloffMode.Logarithmic;
                pool[i].spatialBlend = 1f;
            }
        }

        void Start()
        {
            SubscribeEvents();
        }

        void SubscribeEvents()
        {
            if (creatureSound == null)
            {
                return;
            }

            if (ambushState != null)
            {
                ambushState.onAmbushBegin.AddListener(OnAmbush);
            }

            if (targetManager != null && targetManager.onTargetSet_Event != null && creatureSound.targetSpotted?.Length > 0)
            {
                targetManager.onTargetSet_Event.AddListener(OnTargetSpotted);
            }

            if (characterAnimationEventListener != null)
            {
                if (creatureSound.attacks?.Length > 0)
                {
                    characterAnimationEventListener.onRightWeaponHitboxOpen.AddListener(OnAttack);
                    characterAnimationEventListener.onLeftWeaponHitboxOpen.AddListener(OnAttack);
                    characterAnimationEventListener.onHeadHitboxOpen.AddListener(OnAttack);
                }

                if (creatureSound.roars?.Length > 0)
                {
                    characterAnimationEventListener.onRoar.AddListener(OnRoar);
                }

                if (creatureSound.footstep?.Length > 0)
                {
                    characterAnimationEventListener.onLeftFootstep.AddListener(OnFootstep);
                    characterAnimationEventListener.onRightFootstep.AddListener(OnFootstep);
                }
            }

            if (characterBaseManager != null)
            {
                if (creatureSound.hurt?.Length > 0)
                {
                    characterBaseManager.health.onTakeDamage.AddListener(OnHurt);
                }

                if (creatureSound.death?.Length > 0)
                {
                    characterBaseManager.health.onDeath.AddListener(OnDeath);
                }
            }
        }
        void OnAttack() => PlayFromSoundpack(creatureSound.attacks);

        void OnAmbush() => PlayFromSoundpack(creatureSound.ambush);

        void OnTargetSpotted() => PlayFromSoundpack(creatureSound.targetSpotted);
        void OnRoar() => PlayFromSoundpack(creatureSound.roars);
        void OnHurt() => PlayFromSoundpack(creatureSound.hurt);
        void OnDeath() => PlayFromSoundpack(creatureSound.death);
        void OnFootstep() => PlayFromSoundpack(creatureSound.footstep);

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
