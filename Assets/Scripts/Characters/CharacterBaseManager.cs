using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AF.Animations;
using AF.Characters;
using AF.Combat;
using AF.Health;
using AF.Stats;
using AF.StatusEffects;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace AF
{
    public abstract class CharacterBaseManager : MonoBehaviour
    {
        [Header("Combatant Info")]
        public Combatant combatant;

        [Header("Components")]
        public Animator animator;
        public NavMeshAgent agent;
        public CharacterController characterController;

        [Header("Audio Sources")]
        public AudioSource combatAudioSource;

        [Header("Faction")]
        public CharacterFaction[] characterFactions;

        [Header("Flags")]
        public bool isBusy = false;

        public bool isConfused = false;
        public bool isInGhostForm = false;

        [Header("Components")]
        public StatusController statusController;
        public CharacterBaseHealth health;
        public CharacterAbstractPosture characterPosture;
        public CharacterAbstractPoise characterPoise;
        public CharacterAbstractBlockController characterAbstractBlockController;
        public CharacterBaseDamageReceiver characterBaseDamageReceiver;
        public CharacterPushController characterPushController;
        public CharacterTransformHelper characterTransformHelper;
        public CharacterBaseWeaponsManager characterBaseWeaponsManager;
        public CharacterBaseAppearance characterBaseAppearance;
        public SyntyCharacterModelManager syntyCharacterModelManager;
        public CharacterBaseEquipment characterBaseEquipment;
        public CharacterBaseInventory characterBaseInventory;
        public CharacterBaseStats characterBaseStats;
        public StatsBonusController statsBonusController;
        public CharacterBaseAttackManager characterBaseAttackManager;
        public CharacterBaseDefenseManager characterBaseDefenseManager;
        public CharacterBaseWeight characterBaseWeight;
        public CharacterBaseDodgeController characterBaseDodgeController;
        public CharacterAbilityBaseManager characterAbilityBaseManager;
        public CharacterBaseActivityManager characterBaseActivityManager;
        public CharacterBaseConsumableManager characterBaseConsumableManager;
        public CharacterHUD characterHUD;
        public CharacterBaseBuffManager characterBaseBuffManager;
        public CharacterBaseWeaknessesManager characterBaseWeaknessesManager;
        public CharacterBaseWeaponBuffManager characterBaseWeaponBuffManager;

        Coroutine WaitForPetrificationToKillCoroutine;

        #region Public Events

        /// <summary>
        /// Damage, Attacker, Damage Receiver
        /// </summary>
        [HideInInspector] public UnityEvent<Damage, CharacterBaseManager, CharacterBaseManager> onEnhanceAttackDamageWithEquipmentEffect = new();

        [HideInInspector] public UnityEvent<CharacterBaseManager> onPreparingToDrinkConsumable;

        #endregion

        // Store original materials so they can be restored
        private Dictionary<Renderer, Material[]> originalMaterials;
        private bool originalMaterialsCached = false;

        public abstract void ResetStates();

        public bool IsBusy()
        {
            return isBusy;
        }

        public void SetIsBusy(bool value)
        {
            isBusy = value;
        }

        public void PlayAnimationWithCrossFade(string animationName)
        {
            PlayAnimationWithCrossFade(animationName, false, false, 0.2f);
        }

        public void PlayAnimationWithCrossFade(string animationName, bool isBusy, bool applyRootMotion, float crossFade)
        {
            this.isBusy = isBusy;
            animator.applyRootMotion = applyRootMotion;

            SafeCrossFade(animationName, 0.2f);
        }

        public void PlayBusyAnimation(string animationName)
        {
            isBusy = true;

            SafePlay(animationName);
        }

        public void PlayBusyAnimationWithRootMotion(string animationName)
        {
            animator.applyRootMotion = true;
            PlayBusyAnimation(animationName);
        }


        public void PlayCrossFadeBusyAnimationWithRootMotion(string animationName, float crossFade)
        {
            animator.applyRootMotion = true;
            isBusy = true;
            SafeCrossFade(animationName, crossFade);
        }

        #region Hashed Animations
        public void PlayBusyHashedAnimationWithRootMotion(int hashedAnimationName)
        {
            animator.applyRootMotion = true;
            PlayBusyHashedAnimation(hashedAnimationName);
        }

        public void PlayBusyHashedAnimation(int animationName)
        {
            isBusy = true;
            SafeHashedPlay(animationName);
        }
        #endregion

        public abstract Damage GetAttackDamage(CharacterBaseManager damageReceiver);

        public bool IsFromSameFaction(CharacterBaseManager target)
        {
            return target != null && characterFactions != null
                && characterFactions.Length > 0
                && characterFactions.Any(thisCharactersFaction =>
                    target.characterFactions != null && target.characterFactions.Length > 0 && target.characterFactions.Contains(thisCharactersFaction));

        }


        public void SetIsConfused(bool value)
        {
            this.isConfused = value;
        }

        public void ResetIsConfused()
        {
            this.isConfused = false;
        }

        protected void AddOrReplaceOverride(List<AnimationOverride> list, Dictionary<string, AnimationOverride> overrides)
        {
            if (list == null || list.Count == 0)
                return;

            foreach (var entry in list)
            {
                if (entry == null || string.IsNullOrEmpty(entry.animationName))
                    continue;

                overrides[entry.animationName] = entry; // Replace or add
            }
        }

        public void UpdateAnimatorOverrideControllerClipsUsingDictionary(Dictionary<string, AnimationClip> clips)
        {
            var clipOverrides = new AnimationClipOverrides(GetAnimatorOverrideController().overridesCount);
            GetAnimatorOverrideController().GetOverrides(clipOverrides);
            animator.runtimeAnimatorController = GetAnimatorOverrideController();

            foreach (var clip in clips)
            {
                clipOverrides[clip.Key] = clip.Value;
            }

            ApplyClipOverrides(clipOverrides);

            animator.runtimeAnimatorController = GetAnimatorOverrideController();
        }

        public abstract AnimatorOverrideController GetAnimatorOverrideController();
        public abstract CharacterBaseManager GetTarget();

        public bool IsUsingSyntyModularFantasyHeroModel()
        {
            if (syntyCharacterModelManager == null)
            {
                return false;
            }

            return syntyCharacterModelManager.isUsingSyntyModularFantasyHeroModel;
        }

        protected void ApplyClipOverrides(AnimationClipOverrides clipOverrides)
        {
            GetAnimatorOverrideController().ApplyOverrides(clipOverrides);

            if (
                combatant != null
                && combatant.isHumanoid
                // Only play switching animation if not in the middle of performing an ability
                && characterAbilityBaseManager.currentAbility == null)
            {
                // Fixes issue where player and AI get underneath the ground because of animator override logic messing up the current animation playing
                PlaySwitchEquipmentAnimation();
            }
        }

        public void PlaySwitchEquipmentAnimation()
        {
            PlayBusyAnimationWithRootMotion("Switch Equipment");
        }

        public void FaceObject(Transform target)
        {
            Vector3 targetRotation = target.position - transform.position;
            targetRotation.y = 0;
            transform.rotation = Quaternion.LookRotation(targetRotation);
        }

        void SafeHashedPlay(int animationName)
        {
            if (!animator.HasState(0, animationName))
            {
                Debug.Log($"{gameObject.name} tried playing animation with hash {animationName} but it does not exist on layer 0");
            }

            animator.Play(animationName);
        }

        void SafePlay(string animationName)
        {
            if (!animator.HasState(0, Animator.StringToHash(animationName)))
            {
                Debug.Log($"{gameObject.name} tried playing animation {animationName} but it does not exist on layer 0");
            }

            animator.Play(animationName);
        }

        void SafeCrossFade(string animationName, float crossFade)
        {
            if (!animator.HasState(0, Animator.StringToHash(animationName)))
            {
                Debug.Log($"{gameObject.name} tried playing animation {animationName} but it does not exist on layer 0");
            }

            animator.CrossFade(animationName, crossFade);
        }

        public abstract void OnParalyzedStart();
        public abstract void OnParalyzedEnd();

        public abstract float GetDefaultAnimatorSpeed();


        public void OnPetrified(Material petrifiedMaterial)
        {
            CacheOriginalMaterials();   // <-- IMPORTANT

            OnParalyzedStart();

            if (agent != null)
            {
                agent.speed = 0f;
                agent.isStopped = true;
            }

            ApplyMaterialToSkinnedMeshRenderers(petrifiedMaterial);

            if (WaitForPetrificationToKillCoroutine != null)
            {
                StopCoroutine(WaitForPetrificationToKillCoroutine);
            }

            WaitForPetrificationToKillCoroutine = StartCoroutine(WaitForPetrificationToKill());
        }

        IEnumerator WaitForPetrificationToKill()
        {
            yield return new WaitForSeconds(3f);
            this.health.TakeDamage(Mathf.Infinity);
        }

        private void CacheOriginalMaterials()
        {
            if (originalMaterialsCached)
                return;

            originalMaterials = new Dictionary<Renderer, Material[]>();

            // SkinnedMeshRenderers
            foreach (var skinned in Utils.CollectComponentsFromGameObject<SkinnedMeshRenderer>(this.gameObject))
            {
                originalMaterials[skinned] = skinned.sharedMaterials.ToArray();
            }

            // MeshRenderers
            foreach (var meshRenderer in Utils.CollectComponentsFromGameObject<MeshRenderer>(this.gameObject))
            {
                originalMaterials[meshRenderer] = meshRenderer.sharedMaterials.ToArray();
            }

            originalMaterialsCached = true;
        }


        void ApplyMaterialToSkinnedMeshRenderers(Material mat)
        {
            SkinnedMeshRenderer[] allSkinnedMeshRenderers =
                Utils.CollectComponentsFromGameObject<SkinnedMeshRenderer>(this.gameObject);

            foreach (SkinnedMeshRenderer skinnedMeshRenderer in allSkinnedMeshRenderers)
            {
                Material[] mats = skinnedMeshRenderer.materials;

                for (int i = 0; i < mats.Length; i++)
                    mats[i] = mat;

                skinnedMeshRenderer.materials = mats;
            }

            MeshRenderer[] meshRenderers =
                Utils.CollectComponentsFromGameObject<MeshRenderer>(this.gameObject);

            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                Material[] mats = meshRenderer.materials;

                for (int i = 0; i < mats.Length; i++)
                    mats[i] = mat;

                meshRenderer.materials = mats;
            }
        }
        public void RestoreOriginalMaterials()
        {
            if (!originalMaterialsCached || originalMaterials == null)
                return;

            foreach (var kvp in originalMaterials)
            {
                if (kvp.Key == null)
                    continue;

                kvp.Key.sharedMaterials = kvp.Value;
            }
        }

    }
}
