namespace AF
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Throw Spell Ability", menuName = "Abilities / Spells / New Throw Spell Ability", order = 0)]
    public class ThrowSpellAbility : Ability
    {
        [Header("FX")]
        public GameObject chargingSpellFX;
        public GameObject releaseSpellFX;
        public GameObject explosionSpellFX;

        [Header("SFX")]
        public AudioClip chargingSfx;
        public AudioClip chargingLoopSfx;
        public AudioClip releaseSfx;
        public AudioClip explosionSfx;

        [Header("Trajectory")]
        [SerializeField] float projectileSpeed = 1f;
        [SerializeField] float arcHeight = 1f;

        // Private
        Transform spawnRef;
        Transform origin;
        Transform target;

        public override void OnPrepare(CharacterManager characterManager)
        {
        }

        public override void OnPrepare(PlayerManager playerManager)
        {
            if (!playerManager.playerAbilityManager.CanUseAbility())
            {
                return;
            }

            playerManager.playerAbilityManager.PrepareAbility(this);
            playerManager.playerWeaponsManager.HideEquipment();
            GameObject chargingAbilityFXInstance = PrepareSpellGameObject(playerManager);
            playerManager.playerAbilityManager.chargingAbilityFX = chargingAbilityFXInstance;
            playerManager.PlayCrossFadeBusyAnimationWithRootMotion("Cast Spell", 0.1f);
        }

        public override void OnUse(PlayerManager playerManager)
        {
            float chargingAbilityMultiplier = 1 + playerManager.playerAbilityManager.chargingAbilityAmount;
            damage.ScaleDamage(chargingAbilityMultiplier);
            spawnRef = playerManager.characterTransformHelper.rightHand;
            origin = playerManager.transform;
            target = playerManager.lockOnManager.nearestLockOnTarget != null
                ? playerManager.lockOnManager.nearestLockOnTarget.transform : null;

            ReleaseSpellGameObject(new string[] { "Enemy" });
        }

        public override void OnUse(CharacterManager characterManager)
        {
        }

        GameObject PrepareSpellGameObject(CharacterBaseManager characterBaseManager)
        {
            GameObject instance = Instantiate(chargingSpellFX, characterBaseManager.characterTransformHelper.rightHand);

            if (chargingSfx != null)
            {
                AudioSource audioSource = Utils.CreateAudioSource(instance, chargingSfx);
                audioSource.Play();
            }

            if (chargingLoopSfx != null)
            {
                AudioSource audioSource = Utils.CreateAudioSource(instance, chargingLoopSfx);
                audioSource.loop = true;
                audioSource.Play();
            }

            return instance;
        }

        void ReleaseSpellGameObject(string[] tagsToDetect)
        {
            GameObject instance = Instantiate(releaseSpellFX, origin.transform.position + origin.transform.up, Quaternion.identity);
            instance.transform.parent = null;

            DestroyableParticle destroyableParticle = instance.AddComponent<DestroyableParticle>();
            destroyableParticle.destroyAfter = 5;

            OnDamageTriggerManager onDamageTriggerManager = instance.AddComponent<OnDamageTriggerManager>();
            onDamageTriggerManager.damage = damage;
            onDamageTriggerManager.tagsToDetect = tagsToDetect;

            SphereCollider sphereCollider = instance.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;

            if (releaseSfx != null)
            {
                AudioSource audioSource = Utils.CreateAudioSource(instance, releaseSfx);
                audioSource.Play();
            }

            // Assign destination
            TrackingProjectile trackingProjectile = instance.AddComponent<TrackingProjectile>();
            trackingProjectile.travelDuration = projectileSpeed;
            trackingProjectile.arcHeight = arcHeight;

            trackingProjectile.explosionFx = explosionSpellFX;
            trackingProjectile.explosionSfx = explosionSfx;

            if (target != null)
            {
                Vector3 offsetTarget = target.position; // Optional vertical aim
                trackingProjectile.destination = offsetTarget;
            }
            else
            {
                trackingProjectile.destination = origin.position + origin.forward * 20f; // Arbitrary forward target
            }

            trackingProjectile.Initialize(origin.position);

        }
    }
}
