using AF.Animations;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Animations
{
    public class PlayerAnimationEventListener : MonoBehaviour, IAnimationEventListener
    {
        public PlayerManager playerManager;

        [Header("Unity Events")]
        public UnityEvent onLeftFootstep;
        public UnityEvent onRightFootstep;
        public Cinemachine.CinemachineImpulseSource cinemachineImpulseSource;

        [Header("Components")]
        public AudioSource combatAudioSource;
        public Soundbank soundbank;

        [Header("Settings")]
        public float animatorSpeed = 1f;
        float defaultAnimatorSpeed;

        private void Awake()
        {
            playerManager.animator.speed = animatorSpeed;
            defaultAnimatorSpeed = animatorSpeed;
        }

        public void OpenLeftWeaponHitbox()
        {
            if (playerManager.playerWeaponsManager.currentShieldInstance != null)
            {
                playerManager.playerWeaponsManager.currentShieldInstance.EnableHitbox();
            }
            else if (playerManager.playerWeaponsManager.leftHandHitbox != null)
            {
                playerManager.playerWeaponsManager.leftHandHitbox.EnableHitbox();
            }

            DisableRotation();
        }

        public void CloseLeftWeaponHitbox()
        {
            if (playerManager.playerWeaponsManager.currentShieldInstance != null)
            {
                playerManager.playerWeaponsManager.currentShieldInstance.DisableHitbox();
            }
            else if (playerManager.playerWeaponsManager.leftHandHitbox != null)
            {
                playerManager.playerWeaponsManager.leftHandHitbox.DisableHitbox();
            }

            playerManager.playerCombatController.ResetCanAttack();
        }

        public void OpenRightWeaponHitbox()
        {
            if (playerManager.playerWeaponsManager.currentWeaponInstance != null)
            {
                playerManager.playerWeaponsManager.currentWeaponInstance.EnableHitbox();
            }
            else if (playerManager.playerWeaponsManager.rightHandHitbox != null)
            {
                playerManager.playerWeaponsManager.rightHandHitbox.EnableHitbox();
            }

            DisableRotation();
        }

        public void CloseRightWeaponHitbox()
        {
            if (playerManager.playerWeaponsManager.currentWeaponInstance != null)
            {
                playerManager.playerWeaponsManager.currentWeaponInstance.DisableHitbox();
            }
            if (playerManager.playerWeaponsManager.rightHandHitbox != null)
            {
                playerManager.playerWeaponsManager.rightHandHitbox.DisableHitbox();
            }

            playerManager.playerCombatController.ResetCanAttack();
        }

        public void OpenLeftFootHitbox()
        {
            if (playerManager.playerWeaponsManager.leftFootHitbox != null)
            {
                playerManager.playerWeaponsManager.leftFootHitbox.EnableHitbox();
                playerManager.playerCombatController.isAttackingWithFoot = true;
            }

            DisableRotation();
        }

        public void CloseLeftFootHitbox()
        {
            if (playerManager.playerWeaponsManager.leftFootHitbox != null)
            {
                playerManager.playerWeaponsManager.leftFootHitbox.DisableHitbox();
                playerManager.playerCombatController.isAttackingWithFoot = false;
            }

            playerManager.playerCombatController.ResetCanAttack();
        }

        public void OpenRightFootHitbox()
        {
            if (playerManager.playerWeaponsManager.rightFootHitbox != null)
            {
                playerManager.playerWeaponsManager.rightFootHitbox.EnableHitbox();
                playerManager.playerCombatController.isAttackingWithFoot = true;
            }

            DisableRotation();
        }

        public void CloseRightFootHitbox()
        {
            if (playerManager.playerWeaponsManager.rightFootHitbox != null)
            {
                playerManager.playerWeaponsManager.rightFootHitbox.DisableHitbox();
                playerManager.playerCombatController.isAttackingWithFoot = false;
            }

            playerManager.playerCombatController.ResetCanAttack();
        }

        public void EnableRotation()
        {
            playerManager.thirdPersonController.canRotateCharacter = true;
        }

        public void DisableRotation()
        {
            playerManager.thirdPersonController.canRotateCharacter = false;
        }

        public void EnableRootMotion()
        {
            playerManager.animator.applyRootMotion = true;
        }

        public void DisableRootMotion()
        {
            playerManager.animator.applyRootMotion = false;
        }

        public void FaceTarget()
        {

        }

        public void SetAnimatorBool_True(string parameterName)
        {
            playerManager.animator.SetBool(parameterName, true);
        }

        public void SetAnimatorBool_False(string parameterName)
        {
            playerManager.animator.SetBool(parameterName, false);
        }

        public void OnSpellCast()
        {
            playerManager.playerShootingManager.CastSpell();
        }

        public void OnFireArrow()
        {
            playerManager.playerShootingManager.OnShoot();
            playerManager.projectileSpawner.HideArrowPlaceholders();
        }

        public void OnFireMultipleArrows()
        {
            playerManager.playerShootingManager.ShootWithoutClearingProjectilesAndSpells(false);
        }

        public void OnLeftFootstep()
        {

            onLeftFootstep?.Invoke();
        }

        public void OnRightFootstep()
        {
            onRightFootstep?.Invoke();
        }

        public void OnCloth()
        {
            if (playerManager.thirdPersonController.Grounded)
            {
                soundbank.PlaySound(soundbank.dodge, combatAudioSource);
            }
            else
            {
                soundbank.PlaySound(soundbank.cloth, combatAudioSource);
            }
        }

        public void OnImpact()
        {
            soundbank.PlaySound(soundbank.impact, combatAudioSource);
        }

        public void OnBuff()
        {

        }

        public void OpenCombo()
        {

        }

        public void OnThrow()
        {
            playerManager.projectileSpawner.ThrowProjectile();
        }

        public void OnBlood()
        {
            throw new System.NotImplementedException();
        }

        public void RestoreDefaultAnimatorSpeed()
        {
            this.animatorSpeed = defaultAnimatorSpeed;
            playerManager.animator.speed = animatorSpeed;

        }

        public void SetAnimatorSpeed(float speed)
        {
            this.animatorSpeed = speed;
            playerManager.animator.speed = animatorSpeed;

        }

        public void OnShakeCamera()
        {
            cinemachineImpulseSource.GenerateImpulse();
        }

        public void ShowShield()
        {
            playerManager.playerWeaponsManager.ShowShield();
        }

        public void DropIKHelper()
        {
            playerManager.SetCanUseIK_False();
        }

        public void UseIKHelper()
        {
            playerManager.SetCanUseIK_True();
        }

        public void SetCanTakeDamage_False()
        {
            playerManager.damageReceiver.SetCanTakeDamage(false);
        }

        public void OnWeaponSpecial()
        {
            playerManager.playerWeaponsManager.HandleWeaponSpecial();
        }

        public void MoveTowardsTarget()
        {
        }

        public void StopMoveTowardsTarget()
        {
        }

        public void OnSwim()
        {
            playerManager.thirdPersonController.OnSwimAnimationEvent();
        }

        public void PauseAnimation()
        {
        }

        public void ResumeAnimation()
        {
        }


        public void StopIframes()
        {
            playerManager.dodgeController.StopIframes();
        }
        public void EnableIframes()
        {
            playerManager.dodgeController.EnableIframes();
        }

        public void OnCard()
        {
            playerManager.playerCardManager.UseCurrentCard();
        }

        public void OnExecuted()
        {
        }

        public void OnExecuting()
        {
            playerManager.executionerManager.OnExecuting();
        }

        public void ShowRifleWeapon()
        {
        }

        public void HideRifleWeapon()
        {
        }

        public void OnBuffWeaponWithFire()
        {
            playerManager.playerWeaponsManager.ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName
            .FIRE, playerManager.playerWeaponsManager.DEFAULT_WEAPON_BUFF_DURATION);
        }

        public void OnPrepareAbility()
        {
            playerManager.playerAbilityManager.OnPrepareAbility();
        }

        public void OnUseAbility()
        {
            playerManager.playerAbilityManager.OnUseAbility();
        }
    }
}
