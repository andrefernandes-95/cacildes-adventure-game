namespace AF
{
    using System.Collections.Generic;
    using System.Linq;
    using AF.Combat;
    using Cinemachine;
    using UnityEngine;
    using UnityEngine.Events;

    [RequireComponent(typeof(CinemachineImpulseSource))]
    [RequireComponent(typeof(AudioSource))]
    public abstract class Hitbox : MonoBehaviour
    {
        [Header("Owner")]
        [HideInInspector] public CharacterBaseManager character;

        [Header("Trails")]
        [HideInInspector] public TrailRenderer trailRenderer;
        public BoxCollider hitCollider => GetComponent<BoxCollider>();

        [Header("Tags To Ignore")]
        public List<string> tagsToIgnore = new();

        [Header("SFX")]
        AudioSource combatAudioSource => GetComponent<AudioSource>();
        readonly List<IDamageable> damageReceiversHit = new();

        // Camera Shake
        CinemachineImpulseSource cinemachineImpulseSource => GetComponent<CinemachineImpulseSource>();

        [Header("Events")]
        public UnityEvent onOpenHitbox;
        public UnityEvent onCloseHitbox;
        public UnityEvent onDamageInflicted;
        public UnityEvent onWeaponSpecial;

        [Header("Character Weapon Addons")]
        [HideInInspector] public CharacterTwoHandRef characterTwoHandRef;
        [HideInInspector] public CharacterWeaponBuffs characterWeaponBuffs;

        // Scene References
        Soundbank soundbank;
        WeaponCollisionFXManager weaponCollisionFXManager;

        // Internal flags
        bool canPlayHitSfx = true;

        List<BoxCollider> ownColliders => GetComponents<BoxCollider>()?.ToList();

        // Useful for throwable weapon situation
        [HideInInspector] public bool shouldDisableHitboxOnStart = true;

        [HideInInspector] public bool isOnRightHand;

        private void Awake()
        {
            SetupRefs();
        }

        void SetupRefs()
        {
            character = GetComponentInParent<CharacterBaseManager>();

            AssignTrailRenderer();

            characterWeaponBuffs = GetComponent<CharacterWeaponBuffs>();

            characterTwoHandRef = GetComponent<CharacterTwoHandRef>();
            if (characterTwoHandRef != null)
            {
                characterTwoHandRef.characterBaseManager = character;
            }
        }

        void AssignTrailRenderer()
        {
            trailRenderer = GetComponent<TrailRenderer>();

            if (trailRenderer == null)
            {
                trailRenderer = GetComponentInChildren<TrailRenderer>(true);
            }
        }

        void Start()
        {
            if (shouldDisableHitboxOnStart)
            {
                DisableHitbox();
            }
        }

        public void ShowWeapon()
        {
            gameObject.SetActive(true);

            if (characterTwoHandRef != null)
            {
                characterTwoHandRef.EvaluateTwoHandingUpdate();
            }
        }

        public void HideWeapon()
        {
            gameObject.SetActive(false);
        }

        public void EnableHitbox()
        {
            canPlayHitSfx = true;

            if (trailRenderer != null)
            {
                trailRenderer.Clear();

                trailRenderer.enabled = true;
            }

            if (hitCollider != null)
            {
                hitCollider.enabled = true;
            }

            if (GetSwingSFX() != null && HasSoundbank())
            {
                combatAudioSource.pitch = Random.Range(0.9f, 1.1f);
                combatAudioSource.Stop();

                soundbank.PlaySound(GetSwingSFX(), combatAudioSource);
            }

            onOpenHitbox?.Invoke();
        }

        public void DisableHitbox()
        {
            if (trailRenderer != null)
            {
                trailRenderer.enabled = false;
            }

            if (hitCollider != null)
            {
                hitCollider.enabled = false;
            }

            if (ownColliders?.Count > 1)
            {
                foreach (var collider in ownColliders)
                {
                    collider.enabled = false;
                }
            }

            damageReceiversHit.Clear();
            onCloseHitbox?.Invoke();
        }

        public void OnTriggerEnter(Collider other)
        {
            if (HasWeaponCollisionManager())
            {
                weaponCollisionFXManager.EvaluateCollision(other, this.gameObject);
            }

            if (ShouldIgnoreCollision(other))
            {
                return;
            }

            if (other.TryGetComponent(out IDamageable damageable) && !damageReceiversHit.Contains(damageable))
            {
                character.characterBaseAttackManager.SetIsAttackingWithLeftHand(!isOnRightHand);

                damageReceiversHit.Add(damageable);

                damageable.OnDamage(character, () =>
                {
                    onDamageInflicted?.Invoke();

                    PlayCameraShake();

                    if (GetImpactSFX() != null && canPlayHitSfx && character != null)
                    {
                        canPlayHitSfx = false;
                        PlayHitSound();
                    }
                });

                HandleCharacterAttack(damageable);
            }
        }

        protected abstract void HandleCharacterAttack(IDamageable damageable);

        private bool ShouldIgnoreCollision(Collider other)
        {
            if (tagsToIgnore.Contains(other.tag))
            {
                return true;
            }

            return false;
        }

        private void PlayHitSound()
        {
            if (HasSoundbank() && combatAudioSource != null)
            {
                combatAudioSource.pitch = Random.Range(0.9f, 1.1f);
                soundbank.PlaySound(GetImpactSFX(), combatAudioSource);
            }
        }

        bool HasSoundbank()
        {
            if (soundbank == null)
            {
                soundbank = FindAnyObjectByType<Soundbank>(FindObjectsInactive.Include);

                return soundbank != null;
            }

            return true;
        }

        public bool UseCustomTwoHandTransform()
        {
            return characterTwoHandRef != null;
        }

        bool HasWeaponCollisionManager()
        {
            if (weaponCollisionFXManager == null)
            {
                weaponCollisionFXManager = FindAnyObjectByType<WeaponCollisionFXManager>(FindObjectsInactive.Include);

                return weaponCollisionFXManager != null;
            }

            return true;
        }

        void PlayCameraShake()
        {
            cinemachineImpulseSource.GenerateImpulse(GetWeaponImpactImpulse());
        }

        public abstract float GetWeaponImpactImpulse();

        public abstract AudioClip GetSwingSFX();
        public abstract AudioClip GetImpactSFX();

    }
}
