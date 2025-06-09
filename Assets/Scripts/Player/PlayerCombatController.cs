using System.Collections;
using AF.Combat;
using AF.Ladders;
using UnityEngine;

namespace AF
{
    public class PlayerCombatController : MonoBehaviour
    {
        public float crossFade = 0.1f;
        public readonly string hashLightAttack1 = "Light Attack 1";
        public readonly string hashLightAttack2 = "Light Attack 2";
        public readonly string hashLightAttack3 = "Light Attack 3";
        public readonly string hashLightAttack4 = "Light Attack 4";
        public readonly string hashLeftLightAttack1 = "Left Light Attack 1";
        public readonly string hashLeftLightAttack2 = "Left Light Attack 2";
        public readonly string hashPowerStanceAttack1 = "Power Stance Attack 1";
        public readonly string hashPowerStanceAttack2 = "Power Stance Attack 2";
        public readonly string hashHeavyAttack1 = "Heavy Attack 1";
        public readonly string hashHeavyAttack2 = "Heavy Attack 2";
        public readonly string hashHeavyPowerStanceAttack1 = "Heavy Power Stance Attack 1";
        public readonly string hashHeavyPowerStanceAttack2 = "Heavy Power Stance Attack 2";
        public readonly int hashSpecialAttack = Animator.StringToHash("Special Attack");
        public readonly string hashJumpAttack = "Jump Attack";

        [Header("Attack Combo Index")]
        public float maxIdleCombo = 2f;
        [SerializeField] int lightAttackComboIndex, heavyAttackComboIndex = 0;

        [Header("Flags")]
        public bool isCombatting = false;
        public bool isLightAttacking = false;
        public bool isAttackingWithLeftHand = false;

        [Header("Components")]
        public PlayerManager playerManager;
        public Animator animator;
        public UIManager uIManager;

        [Header("Heavy Attacks")]
        public int unarmedHeavyAttackBonus = 35;

        [Header("UI")]
        public MenuManager menuManager;

        [Header("Databases")]
        public EquipmentDatabase equipmentDatabase;

        [Header("Flags")]
        public bool isHeavyAttacking = false;
        public bool isJumpAttacking = false;

        // Coroutines
        Coroutine ResetLightAttackComboIndexCoroutine;

        public readonly string SpeedMultiplierHash = "SpeedMultiplier";

        [Header("Foot Damage")]
        public bool isAttackingWithFoot = false;

        [Header("Flags")]
        bool canAttack = false;

        [Header("Combos")]
        [SerializeField] int oh_unarmedLightAttackCombos = 3;
        [SerializeField] int th_unarmedLightAttackCombos = 3;

        [SerializeField] int oh_unarmedHeavyAttackCombos = 2;
        [SerializeField] int th_unarmedHeavyAttackCombos = 2;

        [Header("Components")]
        [SerializeField] StarterAssetsInputs starterAssetsInputs;

        private void Start()
        {
            animator.SetFloat(SpeedMultiplierHash, 1f);

            starterAssetsInputs.onLightAttackInput.AddListener(OnAttackWithRightWeapon);
            starterAssetsInputs.onBlock_Start.AddListener(OnAttackWithLeftWeapon);
            starterAssetsInputs.onHeavyAttackInput.AddListener(OnHeavyAttack);
        }

        public void ResetStates()
        {
            isJumpAttacking = false;
            isHeavyAttacking = false;
            isLightAttacking = false;
            isAttackingWithFoot = false;
            isAttackingWithLeftHand = false;
            animator.SetFloat(SpeedMultiplierHash, 1f);
            canAttack = true;
        }

        public void OnLightAttack()
        {
            if (CanLightAttack())
            {
                HandleLightAttack();
                canAttack = false;
            }
        }

        public void OnHeavyAttack()
        {
            if (CanHeavyAttack())
            {
                HandleHeavyAttack(false);
                canAttack = false;
            }
        }

        public bool IsAttacking()
        {
            return isLightAttacking || isHeavyAttacking || isJumpAttacking;
        }

        public void HandleLightAttack()
        {
            isHeavyAttacking = false;
            isLightAttacking = true;

            if (playerManager.thirdPersonController.Grounded)
            {
                if (playerManager.playerBackstabController.PerformBackstab())
                {
                    return;
                }

                if (lightAttackComboIndex > GetMaxLightCombo())
                {
                    lightAttackComboIndex = 0;
                }

                string hashAttack = "";
                if (lightAttackComboIndex == 0)
                {
                    if (isAttackingWithLeftHand)
                    {
                        hashAttack = equipmentDatabase.CanPowerStance() ? hashPowerStanceAttack1 : hashLeftLightAttack1;
                    }
                    else
                    {
                        hashAttack = hashLightAttack1;
                    }
                }
                else if (lightAttackComboIndex == 1)
                {
                    if (isAttackingWithLeftHand)
                    {
                        hashAttack = equipmentDatabase.CanPowerStance() ? hashPowerStanceAttack2 : hashLeftLightAttack2;
                    }
                    else
                    {
                        hashAttack = hashLightAttack2;
                    }
                }
                else if (lightAttackComboIndex == 2)
                {
                    if (isAttackingWithLeftHand)
                    {
                        hashAttack = equipmentDatabase.CanPowerStance() ? hashPowerStanceAttack1 : hashLeftLightAttack1;
                    }
                    else
                    {
                        hashAttack = hashLightAttack3;
                    }
                }
                else if (lightAttackComboIndex == 3)
                {
                    if (isAttackingWithLeftHand)
                    {
                        hashAttack = equipmentDatabase.CanPowerStance() ? hashPowerStanceAttack2 : hashLeftLightAttack2;
                    }
                    else
                    {
                        hashAttack = hashLightAttack4;
                    }
                }

                // If first attack, do not crossfade
                if (lightAttackComboIndex == 0)
                {
                    playerManager.PlayBusyAnimationWithRootMotion(hashAttack);
                }
                else
                {
                    playerManager.PlayCrossFadeBusyAnimationWithRootMotion(hashAttack, crossFade);
                }

                HandleAttackSpeed();
            }
            else
            {
                HandleJumpAttack();
            }

            lightAttackComboIndex++;

            // Stamina
            playerManager.staminaStatManager.DecreaseLightAttackStamina(isAttackingWithLeftHand);

            if (ResetLightAttackComboIndexCoroutine != null)
            {
                StopCoroutine(ResetLightAttackComboIndexCoroutine);
            }
            ResetLightAttackComboIndexCoroutine = StartCoroutine(_ResetLightAttackComboIndex());
        }

        int GetMaxLightCombo()
        {
            int maxCombo = (equipmentDatabase.isTwoHanding ? th_unarmedLightAttackCombos : oh_unarmedLightAttackCombos) - 1;

            Weapon currentWeapon = equipmentDatabase.GetCurrentWeapon();

            if (currentWeapon != null)
            {
                maxCombo = (equipmentDatabase.isTwoHanding ? currentWeapon.th_lightAttackCombos : currentWeapon.lightAttackCombos) - 1;
            }

            return maxCombo;
        }

        IEnumerator _ResetLightAttackComboIndex()
        {
            yield return new WaitForSeconds(maxIdleCombo);
            lightAttackComboIndex = 0;
        }

        void HandleAttackSpeed()
        {
            Weapon currentWeapon = equipmentDatabase.GetCurrentWeapon();
            if (equipmentDatabase.isTwoHanding == false && currentWeapon != null && currentWeapon.oneHandAttackSpeedPenalty != 1)
            {
                animator.SetFloat(SpeedMultiplierHash, currentWeapon.oneHandAttackSpeedPenalty);
            }
            else if (equipmentDatabase.isTwoHanding && currentWeapon != null && currentWeapon.twoHandAttackSpeedPenalty != 1)
            {
                animator.SetFloat(SpeedMultiplierHash, currentWeapon.twoHandAttackSpeedPenalty);
            }
            else
            {
                animator.SetFloat(SpeedMultiplierHash, 1f);
            }
        }

        void HandleJumpAttack()
        {
            isHeavyAttacking = false;
            isLightAttacking = false;
            isJumpAttacking = true;

            playerManager.playerWeaponsManager.HideShield();

            playerManager.playerAnimationEventListener.OpenRightWeaponHitbox();

            playerManager.PlayCrossFadeBusyAnimationWithRootMotion(hashJumpAttack, crossFade);
            playerManager.playerComponentManager.DisableCollisionWithEnemies();
        }

        public void HandleHeavyAttack(bool isCardAttack)
        {
            if (isCombatting || playerManager.thirdPersonController.Grounded == false)
            {
                return;
            }

            isLightAttacking = false;
            isHeavyAttacking = true;

            playerManager.playerWeaponsManager.HideShield();


            if (heavyAttackComboIndex > GetMaxHeavyCombo())
            {
                heavyAttackComboIndex = 0;
            }

            if (isCardAttack)
            {
                playerManager.PlayBusyHashedAnimationWithRootMotion(hashSpecialAttack);
            }
            else
            {
                if (heavyAttackComboIndex == 0)
                {
                    playerManager.PlayCrossFadeBusyAnimationWithRootMotion(
                        equipmentDatabase.CanPowerStance() ? hashHeavyPowerStanceAttack1 : hashHeavyAttack1, crossFade);
                }
                else if (heavyAttackComboIndex == 1)
                {
                    playerManager.PlayCrossFadeBusyAnimationWithRootMotion(
                        equipmentDatabase.CanPowerStance() ? hashHeavyPowerStanceAttack2 : hashHeavyAttack2, crossFade);
                }
            }

            // Stamina
            playerManager.staminaStatManager.DecreaseHeavyAttackStamina();

            HandleAttackSpeed();

            heavyAttackComboIndex++;
        }

        int GetMaxHeavyCombo()
        {
            int maxCombo = (equipmentDatabase.isTwoHanding ? th_unarmedHeavyAttackCombos : oh_unarmedHeavyAttackCombos) - 1;

            Weapon currentWeapon = equipmentDatabase.GetCurrentWeapon();

            if (currentWeapon != null)
            {
                maxCombo = (equipmentDatabase.isTwoHanding ? currentWeapon.th_heavyAttackCombos : currentWeapon.heavyAttackCombos) - 1;
            }

            return maxCombo;
        }

        public bool CanLightAttack()
        {
            if (!this.isActiveAndEnabled)
            {
                return false;
            }

            if (CanAttack() == false)
            {
                return false;
            }

            if (equipmentDatabase.IsStaffEquipped() || equipmentDatabase.IsRangeWeaponEquipped())
            {
                return false;
            }

            return playerManager.staminaStatManager.HasEnoughStaminaForLightAttack(isAttackingWithLeftHand);
        }

        public bool CanHeavyAttack()
        {
            if (CanAttack() == false)
            {
                return false;
            }

            return playerManager.staminaStatManager.HasEnoughStaminaForHeavyAttack();
        }

        bool CanAttack()
        {
            if (!canAttack)
            {
                return false;
            }

            if (playerManager.characterBlockController.isBlocking)
            {
                return false;
            }

            if (menuManager.isMenuOpen)
            {
                return false;
            }

            if (playerManager.playerShootingManager.isAiming)
            {
                return false;
            }

            if (playerManager.climbController.climbState != ClimbState.NONE)
            {
                return false;
            }

            if (playerManager.dodgeController.isDodging)
            {
                return false;
            }

            if (uIManager.IsShowingGUI())
            {
                return false;
            }

            if (playerManager.thirdPersonController.isSwimming)
            {
                return false;
            }

            return true;
        }

        private void OnDisable()
        {
            ResetStates();
        }


        public void HandlePlayerAttack(IDamageable damageable, Weapon weapon)
        {
            if (damageable is not DamageReceiver damageReceiver)
            {
                return;
            }

            if (playerManager.playerBlockController.isCounterAttacking)
            {
                playerManager.playerBlockController.onCounterAttack?.Invoke();
            }

            damageReceiver?.health?.onDamageFromPlayer?.Invoke();

            if (weapon != null && damageReceiver?.health?.weaponRequiredToKill != null && damageReceiver.health.weaponRequiredToKill == weapon)
            {
                damageReceiver.health.hasBeenHitWithRequiredWeapon = true;
            }

            if (weapon != null)
            {
                playerManager.attackStatManager.attackSource = AttackStatManager.AttackSource.WEAPON;
            }
            else
            {
                playerManager.attackStatManager.attackSource = AttackStatManager.AttackSource.UNARMED;
            }
        }

        public void ResetCanAttack()
        {
            canAttack = true;
            playerManager.thirdPersonController.canRotateCharacter = true;
        }

        void OnAttackWithRightWeapon()
        {
            isAttackingWithLeftHand = false;
            OnLightAttack();
        }

        void OnAttackWithLeftWeapon()
        {
            // if has no left weapon
            // or is shield
            // or is two handing
            // do not attack with left hand
            Weapon leftWeapon = equipmentDatabase.GetCurrentLeftWeapon();
            if (leftWeapon == null || leftWeapon is Shield || equipmentDatabase.isTwoHanding)
            {
                return;
            }

            isAttackingWithLeftHand = true;
            OnLightAttack();
        }
    }
}
