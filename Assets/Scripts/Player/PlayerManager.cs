using System.Collections.Generic;
using System.Linq;
using AF.Animations;
using AF.Companions;
using AF.Equipment;
using AF.Events;
using AF.Footsteps;
using AF.Health;
using AF.Inventory;
using AF.Ladders;
using AF.Reputation;
using AF.Shooting;
using AF.Stats;
using TigerForge;
using UnityEngine;

namespace AF
{
    public class PlayerManager : CharacterBaseManager
    {
        public ThirdPersonController thirdPersonController;
        public PlayerWeaponsManager playerWeaponsManager;
        public ClimbController climbController;
        public PlayerDodgeController playerDodgeController;
        public PlayerLevelManager playerLevelManager;
        public PlayerAchievementsManager playerAchievementsManager;
        public PlayerCombatController playerCombatController;
        public StaminaStatManager staminaStatManager;
        public ManaManager manaManager;
        public PlayerInventory playerInventory;
        public FavoriteItemsManager favoriteItemsManager;
        public PlayerShooter playerShootingManager;
        public ProjectileSpawner projectileSpawner;
        public FootstepListener footstepListener;
        public PlayerComponentManager playerComponentManager;
        public EventNavigator eventNavigator;
        public PlayerBlockInput playerBlockInput;
        public PlayerBlockController playerBlockController;
        public StarterAssetsInputs starterAssetsInputs;
        public PlayerAnimationEventListener playerAnimationEventListener;
        public PlayerBackstabController playerBackstabController;
        public TwoHandingController twoHandingController;
        public LockOnManager lockOnManager;
        public PlayerReputation playerReputation;
        public RageManager rageManager;
        public ExecutionerManager executionerManager;
        public UIDocumentPlayerHUDV2 uIDocumentPlayerHUDV2;
        public UIDocumentAlert uIDocumentAlert;
        public PlayerAbilityManager playerAbilityManager;
        public PlayerStats playerStats;
        public CompanionsSceneManager companionsSceneManager;
        public PlayerActivityManager playerActivityManager;
        public PlayerConsumableManager playerConsumableManager;
        public PlayerBuffManager playerBuffManager;

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;

        public EquipmentDatabase equipmentDatabase;
        public GameSettings gameSettings;
        public CompanionsDatabase companionsDatabase;

        [Header("Other Components")]
        [SerializeField] UIDocumentCharacterCustomization uIDocumentCharacterCustomization;

        // Animator Overrides
        protected AnimatorOverrideController animatorOverrideController;
        RuntimeAnimatorController defaultAnimatorController;

        [Header("IK Helpers")]
        bool _canUseWeaponIK = true;

        [Header("Unarmed Animations Overrides")]
        [SerializeField] List<AnimationOverride> oh_unarmedAnimationOverrides = new();
        [SerializeField] List<AnimationOverride> th_unarmedAnimationOverrides = new();

        private void Awake()
        {
            SetupAnimRefs();
        }

        void SetupAnimRefs()
        {
            if (defaultAnimatorController == null)
            {
                defaultAnimatorController = animator.runtimeAnimatorController;
            }
            if (animatorOverrideController == null)
            {
                animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            }
        }

        public override void ResetStates()
        {
            // First, reset all flags before calling the handlers
            isBusy = false;
            animator.applyRootMotion = false;
            SetCanUseIK_True();

            thirdPersonController.canRotateCharacter = true;

            playerInventory.FinishItemConsumption();
            playerCombatController.ResetStates();
            playerShootingManager.ResetStates();

            playerDodgeController.ResetStates();
            playerInventory.ResetStates();
            characterPosture.ResetStates();
            characterPoise.ResetStates();
            characterBaseDamageReceiver.ResetStates();

            rageManager.ResetStates();

            playerComponentManager.ResetStates();

            playerWeaponsManager.ResetStates();

            playerBlockInput.CheckQueuedInput();


            playerBlockController.ResetStates();

            characterBaseAttackManager.ResetStates();

            playerAbilityManager.ResetStates();
            playerActivityManager.ResetStates();
            playerConsumableManager.ResetStates();
        }

        public override Damage GetAttackDamage()
        {
            Damage attackDamage = characterBaseAttackManager.GetAttackDamage().Clone();

            if (playerCombatController.isHeavyAttacking)
            {
                attackDamage.Multiply(characterBaseAttackManager.heavyAttackBonusMultiplier);
            }

            if (playerCombatController.isJumpAttacking)
            {
                attackDamage.Multiply(characterBaseAttackManager.jumpAttackMultiplier);
            }

            if (playerBlockController.isCounterAttacking)
            {
                attackDamage.damageType = DamageType.COUNTER_ATTACK;
            }

            return attackDamage;
        }

        private void OnTriggerStay(Collider other)
        {
            if (!playerDodgeController.isDodging)
            {
                return;
            }

            if (other.TryGetComponent<CharacterBaseDamageReceiver>(out var damageReceiver) && damageReceiver.damageOnDodge)
            {
                damageReceiver.TakeDamage(new Damage(
                    physical: 1,
                    fire: 0,
                    frost: 0,
                    lightning: 0,
                    darkness: 0,
                    magic: 0,
                    water: 0,
                    poiseDamage: 0,
                    postureDamage: 0,
                    weaponAttackType: WeaponAttackType.Blunt,
                    statusEffects: null,
                    pushForce: 0,
                    canNotBeParried: false,
                    ignoreBlocking: false
                ));
            }

            if (other.TryGetComponent<Destroyable>(out var destroyable))
            {
                destroyable.OnDamage(this, () => { });
            }
        }

        public void UpdateAnimatorOverrideControllerClips()
        {
            SetupAnimRefs();

            animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            var clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
            animatorOverrideController.GetOverrides(clipOverrides);
            animator.runtimeAnimatorController = defaultAnimatorController;

            Dictionary<string, AnimationOverride> overrides = new();


            // Always apply unarmed first
            AddOrReplaceOverride(oh_unarmedAnimationOverrides, overrides);

            if (equipmentDatabase.isTwoHanding)
            {
                AddOrReplaceOverride(th_unarmedAnimationOverrides, overrides);
            }

            // Apply right-hand weapon overrides
            Weapon currentWeapon = equipmentDatabase.GetCurrentWeapon();
            if (currentWeapon != null)
            {
                AddOrReplaceOverride(currentWeapon.GetOneHandAnimations(), overrides);

                if (equipmentDatabase.isTwoHanding)
                {
                    AddOrReplaceOverride(currentWeapon.GetTwoHandAnimations(), overrides);
                }
            }

            // Apply left-hand weapon overrides if not two-handing
            Weapon leftWeapon = equipmentDatabase.GetCurrentLeftWeapon();
            if (leftWeapon != null && !equipmentDatabase.isTwoHanding)
            {
                AddOrReplaceOverride(leftWeapon.GetLeftHandAnimations(), overrides);

                // If left weapons is a range weapon, override the animations for shooting
                if (leftWeapon.weaponRangeAnimation != null)
                {
                    AddOrReplaceOverride(leftWeapon.weaponRangeAnimation.GetAnimations(false), overrides);
                }
            }

            // Lastly, check for any additional weapon animation overrides that have the highest priority
            if (currentWeapon != null)
            {
                if (equipmentDatabase.isTwoHanding && currentWeapon.th_weaponAnimationOverrides.Count > 0)
                {
                    AddOrReplaceOverride(currentWeapon.th_weaponAnimationOverrides, overrides);
                }
                else if (currentWeapon.oh_weaponAnimationOverrides.Count > 0)
                {
                    AddOrReplaceOverride(currentWeapon.oh_weaponAnimationOverrides, overrides);
                }
            }

            // Apply all collected overrides in one go
            UpdateAnimationOverrides(animator, clipOverrides, overrides.Values.ToList());
        }

        void UpdateAnimationOverrides(Animator animator, AnimationClipOverrides clipOverrides, List<AnimationOverride> clips)
        {
            foreach (var animationOverride in clips)
            {
                clipOverrides[animationOverride.animationName] = animationOverride.animationClip;
            }

            ApplyClipOverrides(clipOverrides);

            animator.runtimeAnimatorController = animatorOverrideController;

            RefreshAnimationOverrideState();
        }

        public void RefreshAnimationOverrideState()
        {
            // Hack to refresh lock on while switching animations
            if (lockOnManager.isLockedOn)
            {
                LockOnRef tmp = lockOnManager.nearestLockOnTarget;
                lockOnManager.DisableLockOn();
                lockOnManager.nearestLockOnTarget = tmp;
                lockOnManager.EnableLockOn();
            }
        }

        public void UpdateAnimatorOverrideControllerClip(string animationName, AnimationClip animationClip)
        {
            var clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
            animatorOverrideController.GetOverrides(clipOverrides);

            animator.runtimeAnimatorController = defaultAnimatorController;

            clipOverrides[animationName] = animationClip;

            ApplyClipOverrides(clipOverrides);
            animator.runtimeAnimatorController = animatorOverrideController;
        }

        public void SetCanUseIK_False()
        {
            _canUseWeaponIK = false;
        }

        public void SetCanUseIK_True()
        {
            _canUseWeaponIK = true;

            EventManager.EmitEvent(EventMessages.ON_CAN_USE_IK_IS_TRUE);
        }

        public bool CanUseIK()
        {
            return _canUseWeaponIK;
        }

        public override AnimatorOverrideController GetAnimatorOverrideController()
        {
            return animatorOverrideController;
        }

        public override CharacterBaseManager GetTarget()
        {
            return lockOnManager.nearestLockOnTarget != null ? lockOnManager.nearestLockOnTarget.characterManager : null;
        }

        public Sprite GetPlayerPortrait() => uIDocumentCharacterCustomization.portraits[gameSettings.playerPortrait];
    }
}
