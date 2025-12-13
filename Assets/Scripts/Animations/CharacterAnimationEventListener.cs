using System.Collections.Generic;
using AF.Events;
using AYellowpaper.SerializedCollections;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Animations
{
    public class CharacterAnimationEventListener : MonoBehaviour, IAnimationEventListener
    {

        [Header("Components")]
        public CharacterManager characterManager;

        [Header("Animator Settings")]
        public string speedParameter = "Speed";
        public float animatorSpeed = 1f;
        public bool ignoreAnimatorSpeed = false;
        public float overrideChaseSpeed = -1f;

        [Header("Animation Clip Overrides")]
        public SerializedDictionary<string, AnimationClip> clipOverrides;

        [Header("Unity Events")]
        public UnityEvent onLeftFootstep;
        public UnityEvent onRightFootstep;
        public UnityEvent onLeftWeaponHitboxOpen;
        public UnityEvent onLeftWeaponHitboxClose;
        public UnityEvent onRightWeaponHitboxOpen;
        public UnityEvent onRightWeaponHitboxClose;
        public UnityEvent onLeftFootHitboxOpen;
        public UnityEvent onLeftFootHitboxClose;
        public UnityEvent onRightFootHitboxOpen;
        public UnityEvent onRightFootHitboxClose;
        public UnityEvent onHeadHitboxOpen;
        public UnityEvent onBuff;
        public UnityEvent onCloth;
        public UnityEvent onImpact;
        public UnityEvent onOpenCombo;
        public UnityEvent onBlood;
        [HideInInspector] public UnityEvent onRoar;

        float defaultAnimatorSpeed;

        private void Awake()
        {
            characterManager.animator.speed = animatorSpeed;
            defaultAnimatorSpeed = animatorSpeed;
        }


        private void Start()
        {
            OverrideAnimatorClips();

            if (ignoreAnimatorSpeed)
            {
                characterManager.animator.SetFloat(speedParameter, 0f);
            }

            characterManager.onResetStates.AddListener(ResetAnimationSpeed);
        }

        void ResetAnimationSpeed()
        {
            characterManager.animator.speed = defaultAnimatorSpeed;
        }

        void OverrideAnimatorClips()
        {
            foreach (var entry in clipOverrides)
            {
                characterManager.UpdateAnimatorOverrideControllerClips(entry.Key, entry.Value);
            }
        }

        public void OnLeftFootstep()
        {
            onLeftFootstep?.Invoke();
        }

        public void OnRightFootstep()
        {
            onRightFootstep?.Invoke();
        }

        public void OpenLeftWeaponHitbox()
        {
            onLeftWeaponHitboxOpen?.Invoke();

            if (characterManager.characterWeaponsManager.currentShieldInstance != null)
            {
                characterManager.characterWeaponsManager.currentShieldInstance.EnableHitbox();
            }
            else if (characterManager.characterWeaponsManager.leftHandHitbox != null)
            {
                characterManager.characterWeaponsManager.leftHandHitbox.EnableHitbox();
            }
        }

        public void CloseLeftWeaponHitbox()
        {
            onLeftWeaponHitboxClose?.Invoke();

            if (characterManager.characterWeaponsManager.currentShieldInstance != null)
            {
                characterManager.characterWeaponsManager.currentShieldInstance.DisableHitbox();
            }
            else if (characterManager.characterWeaponsManager.leftHandHitbox != null)
            {
                characterManager.characterWeaponsManager.leftHandHitbox.DisableHitbox();
            }
        }

        public void OpenRightWeaponHitbox()
        {
            onRightWeaponHitboxOpen?.Invoke();

            if (characterManager.characterWeaponsManager.currentWeaponInstance != null)
            {
                characterManager.characterWeaponsManager.currentWeaponInstance.EnableHitbox();
            }
            else if (characterManager.characterWeaponsManager.rightHandHitbox != null)
            {
                characterManager.characterWeaponsManager.rightHandHitbox.EnableHitbox();
            }
        }

        public void CloseRightWeaponHitbox()
        {
            onRightWeaponHitboxClose?.Invoke();

            if (characterManager.characterWeaponsManager.currentWeaponInstance != null)
            {
                characterManager.characterWeaponsManager.currentWeaponInstance.DisableHitbox();
            }
            else if (characterManager.characterWeaponsManager.rightHandHitbox != null)
            {
                characterManager.characterWeaponsManager.rightHandHitbox.DisableHitbox();
            }
        }

        public void OpenLeftFootHitbox()
        {
            onLeftFootHitboxOpen?.Invoke();

            if (characterManager.characterWeaponsManager.leftFootHitbox != null)
            {
                characterManager.characterWeaponsManager.leftFootHitbox.EnableHitbox();
            }
        }

        public void CloseLeftFootHitbox()
        {
            onLeftFootHitboxClose?.Invoke();

            if (characterManager.characterWeaponsManager.leftFootHitbox != null)
            {
                characterManager.characterWeaponsManager.leftFootHitbox.DisableHitbox();
            }
        }

        public void OpenRightFootHitbox()
        {
            onRightFootHitboxOpen?.Invoke();

            if (characterManager.characterWeaponsManager.rightFootHitbox != null)
            {
                characterManager.characterWeaponsManager.rightFootHitbox.EnableHitbox();
            }
        }

        public void CloseRightFootHitbox()
        {
            onRightFootHitboxClose?.Invoke();

            if (characterManager.characterWeaponsManager.rightFootHitbox != null)
            {
                characterManager.characterWeaponsManager.rightFootHitbox.DisableHitbox();
            }
        }

        public void EnableRotation()
        {
        }

        public void DisableRotation()
        {
        }

        public void FaceTarget()
        {
            if (characterManager.targetManager.currentTarget == null)
            {
                return;
            }

            characterManager.FaceTarget();
        }


        public void FaceTargetImmediately()
        {
            if (characterManager.targetManager.currentTarget == null)
            {
                return;
            }

            characterManager.FaceTargetImmediately();
        }

        public void EnableRootMotion()
        {
            characterManager.animator.applyRootMotion = true;
        }

        public void DisableRootMotion()
        {
            characterManager.animator.applyRootMotion = false;
        }

        public void OnFireArrow()
        {
            characterManager.characterBaseShooter.FireArrow();
        }

        public void OnCloth()
        {
            onCloth?.Invoke();
        }

        public void OnImpact()
        {
            onImpact?.Invoke();
        }

        public void OnBuff()
        {
            onBuff?.Invoke();
        }

        public void OpenCombo()
        {
            onOpenCombo?.Invoke();
        }

        public void OnThrow()
        {
        }

        public void OnBlood()
        {
            onBlood?.Invoke();
        }

        public void RestoreDefaultAnimatorSpeed()
        {
            this.animatorSpeed = defaultAnimatorSpeed;
            characterManager.animator.speed = animatorSpeed;
        }

        public void SetAnimatorSpeed(float speed)
        {
            this.animatorSpeed = speed;
            characterManager.animator.speed = animatorSpeed;
        }

        public void OnShakeCamera()
        {
        }

        public void DropIKHelper()
        {
        }

        public void UseIKHelper()
        {
        }

        public void SetCanTakeDamage_False()
        {
            if (characterManager == null || characterManager.characterBaseDamageReceiver == null)
            {
                return;
            }
            characterManager.characterBaseDamageReceiver.SetCanTakeDamage(false);
        }

        public void OnFireMultipleArrows()
        {
            characterManager.characterBaseShooter.FireArrow();

        }

        public void OnWeaponSpecial()
        {
            characterManager.characterWeaponsManager.OnWeaponSpecial();
        }

        public void MoveTowardsTarget()
        {
            characterManager.isCuttingDistanceToTarget = true;
        }

        public void StopMoveTowardsTarget()
        {
            characterManager.isCuttingDistanceToTarget = false;
        }

        public void OnSwim()
        {

        }

        public void PauseAnimation()
        {
            // Allow a chance to not do the slow down
            if (Random.Range(0, 1f) > 0.5)
            {
                return;
            }

            // TODO: Do not pause animation, it creates lots of bugs for now
            // SetAnimatorSpeed(Random.Range(0.1f, 0.3f));
        }

        public void ResumeAnimation()
        {
            RestoreDefaultAnimatorSpeed();
        }

        public bool ShouldResetAnimationSpeed()
        {
            return defaultAnimatorSpeed != characterManager.animator.speed;
        }

        public void StopIframes()
        {
            characterManager.characterDodgeController.StopIframes();
        }

        public void EnableIframes()
        {
            characterManager.characterDodgeController.EnableIframes();
        }

        public void ShowShield()
        {
        }

        public void OnExecuted()
        {
            characterManager.executionManager.OnExecuted();
        }

        public void OnExecuting()
        {
        }

        public void ShowRifleWeapon()
        {
            characterManager.characterBaseShooter.ShowRifleWeapon();
        }
        public void HideRifleWeapon()
        {
            characterManager.characterBaseShooter.HideRifleWeapon();
        }

        public void OnBuffWeaponWithFire()
        {
        }

        public void OnCombo()
        {
            characterManager.characterAbilityManager.ComboToNextAbility();
        }
        public void OnPrepareAbility()
        {
            characterManager.characterAbilityManager.OnPrepareAbility();
        }

        public void OnUseAbility()
        {
            characterManager.characterAbilityManager.OnUseAbility();
        }

        public void OnActivityPerformed()
        {
            characterManager.characterActivityManager.OnActivityPerformed();
        }


        public void OpenHeadHitbox()
        {
            if (characterManager.characterWeaponsManager.headHitbox != null)
            {
                characterManager.characterWeaponsManager.headHitbox.EnableHitbox();

                onHeadHitboxOpen?.Invoke();
            }
        }

        public void CloseHeadHitbox()
        {
            if (characterManager.characterWeaponsManager.headHitbox != null)
            {
                characterManager.characterWeaponsManager.headHitbox.DisableHitbox();
            }
        }

        public void OnUseConsumable()
        {
            characterManager.characterConsumableManager.OnConsumableUse();
        }

        public void ShowEquipment()
        {
            characterManager.characterWeaponsManager.ShowEquipment();
        }

        public void HideEquipment()
        {
            characterManager.characterWeaponsManager.HideEquipment();
        }

        public void OnRoar()
        {
            onRoar?.Invoke();
        }
    }
}
