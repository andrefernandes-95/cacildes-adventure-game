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

        [HideInInspector] public UnityEvent<Damage, CharacterBaseManager> onEnhanceAttackDamageWithEquipmentEffect = new();

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

            animator.CrossFade(animationName, 0.2f);
        }

        public void PlayBusyAnimation(string animationName)
        {
            isBusy = true;
            animator.Play(animationName);
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
            animator.CrossFade(animationName, crossFade);
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
            animator.Play(animationName);
        }
        #endregion

        public abstract Damage GetAttackDamage();

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
    }
}
