using System.Collections.Generic;
using System.Linq;
using AF.Events;
using AF.Health;
using AF.Stats;
using TigerForge;
using UnityEngine;
using UnityEngine.Localization;

namespace AF.Equipment
{
    public class PlayerWeaponsManager : MonoBehaviour
    {
        [Header("Unarmed Weapon References In-World")]
        public CharacterWeaponHitbox leftHandHitbox;
        public CharacterWeaponHitbox rightHandHitbox;
        public CharacterWeaponHitbox leftFootHitbox;
        public CharacterWeaponHitbox rightFootHitbox;

        [Header("Weapon References In-World")]
        public List<CharacterWeaponHitbox> weaponInstances;
        public List<ShieldInstance> shieldInstances;

        [Header("Current Weapon")]
        public CharacterWeaponHitbox currentWeaponInstance;
        public CharacterWeaponHitbox currentShieldInstance;

        [Header("Database")]
        public EquipmentDatabase equipmentDatabase;

        [Header("Components")]
        public PlayerManager playerManager;
        StatsBonusController statsBonusController;
        public NotificationManager notificationManager;

        [Header("Localization")]

        // "Can not apply buff to this weapon"
        public LocalizedString CanNotApplyBuffToThisWeapon;
        // "Weapon is already buffed"
        public LocalizedString WeaponIsAlreadyBuffed;
        // "Not enough mana to use weapon special"
        public LocalizedString NotEnoughManaToUseWeaponSpecial;

        public float DEFAULT_WEAPON_BUFF_DURATION = 120f;

        [Header("Transform References")]
        [SerializeField] Transform rightHandGrip;
        [SerializeField] Transform leftHandGrip;

        private void Awake()
        {
            statsBonusController = playerManager.statsBonusController;

            EventManager.StartListening(
                EventMessages.ON_EQUIPMENT_CHANGED,
                UpdateEquipment);

            EventManager.StartListening(EventMessages.ON_TWO_HANDING_CHANGED, UpdateEquipment);
        }

        private void Start()
        {
            UpdateEquipment();
        }

        void UpdateEquipment()
        {
            UpdateCurrentWeapon();
            UpdateCurrentLeftWeapon();
            UpdateCurrentArrows();
            UpdateCurrentSpells();
        }

        public void ResetStates()
        {
            CloseAllWeaponHitboxes();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void CloseAllWeaponHitboxes()
        {
            currentWeaponInstance?.DisableHitbox();
            leftFootHitbox?.DisableHitbox();
            rightFootHitbox?.DisableHitbox();
            leftHandHitbox?.DisableHitbox();
            rightHandHitbox?.DisableHitbox();
            currentShieldInstance?.DisableHitbox();
        }

        void UpdateCurrentWeapon()
        {
            var CurrentWeapon = equipmentDatabase.GetCurrentWeapon();

            if (currentWeaponInstance != null)
            {
                Destroy(currentWeaponInstance);
                currentWeaponInstance = null;
            }

            if (rightHandGrip.childCount > 0)
            {
                foreach (Transform child in rightHandGrip.transform)
                {
                    Destroy(child.gameObject);
                }
            }

            if (CurrentWeapon is Weapon rightWeapon)
            {
                InstantiateWeapon(rightWeapon, true);
            }

            playerManager.UpdateAnimatorOverrideControllerClips();

            playerManager.statsBonusController.RecalculateEquipmentBonus();
        }

        void UpdateCurrentLeftWeapon()
        {
            var CurrentShield = equipmentDatabase.isTwoHanding ? null : equipmentDatabase.GetCurrentLeftWeapon();

            if (currentShieldInstance != null)
            {
                Destroy(currentShieldInstance);

                currentShieldInstance = null;
            }

            if (leftHandGrip.childCount > 0)
            {
                foreach (Transform child in leftHandGrip.transform)
                {
                    Destroy(child.gameObject);
                }
            }

            if (CurrentShield is Weapon leftWeapon)
            {
                InstantiateWeapon(leftWeapon, false);
            }

            playerManager.UpdateAnimatorOverrideControllerClips();
            statsBonusController.RecalculateEquipmentBonus();
        }

        void UpdateCurrentArrows()
        {
            if (equipmentDatabase.IsRangeWeaponEquipped() == false)
            {
                return;
            }

            UnassignShield();
        }

        void UpdateCurrentSpells()
        {
            if (equipmentDatabase.IsStaffEquipped() == false)
            {
                return;
            }

            UnassignShield();
        }

        void InstantiateWeapon(Weapon weapon, bool isRightHand)
        {
            string weaponName = weapon.name.Replace("(Clone)", "");

            // Find weapon in the weapons list
            var weaponPrefab = weapon is Shield
                    ? null
                    : weaponInstances.FirstOrDefault(weaponInstance => weaponInstance.name == weaponName);

            if (weaponPrefab == null)
            {
                weaponPrefab = shieldInstances.FirstOrDefault(shieldInstance => shieldInstance.name == weaponName);
            }

            Transform grip = isRightHand ? rightHandGrip : leftHandGrip;

            if (weaponPrefab != null)
            {
                foreach (Transform child in grip.transform)
                {
                    Destroy(child.gameObject);
                }

                GameObject instantiatedWeapon = Instantiate(weaponPrefab, grip).gameObject;
                CharacterWeaponHitbox instatiatedCharacterWeaponHitbox = instantiatedWeapon.GetComponent<CharacterWeaponHitbox>();

                if (isRightHand)
                {
                    currentWeaponInstance = instatiatedCharacterWeaponHitbox;
                }
                else
                {
                    currentShieldInstance = instatiatedCharacterWeaponHitbox;
                }

                instantiatedWeapon.transform.localPosition = isRightHand ? weapon.rightHandPosition : weapon.leftHandPosition;
                instantiatedWeapon.transform.localRotation = Quaternion.Euler(isRightHand ? weapon.rightHandRotation : weapon.leftHandRotation);

                if (instatiatedCharacterWeaponHitbox.TryGetComponent<CharacterTwoHandRef>(out var characterTwoHandRef))
                {
                    characterTwoHandRef.SetOriginalPositionAndRotation(
                       instantiatedWeapon.transform.localPosition,
                       instantiatedWeapon.transform.localRotation
                    );
                }

                if (instatiatedCharacterWeaponHitbox.TryGetComponent<ShieldInstance>(out var shieldInstance))
                {
                    shieldInstance.shouldHide = false;
                }

                instantiatedWeapon.SetActive(true);
            }
        }

        void UnassignShield()
        {
            if (currentShieldInstance != null && currentShieldInstance is ShieldInstance shieldInstance)
            {
                currentShieldInstance.gameObject.SetActive(false);
                shieldInstance.shieldInTheBack.gameObject.SetActive(false);
                currentShieldInstance = null;
            }
        }

        public void EquipWeapon(Weapon weaponToEquip, int slot)
        {
            equipmentDatabase.EquipWeapon(weaponToEquip, slot);
        }

        public void UnequipWeapon(int slot)
        {
            equipmentDatabase.UnequipWeapon(slot);
        }

        public void EquipShield(Weapon shieldToEquip, int slot)
        {
            equipmentDatabase.EquipShield(shieldToEquip, slot);
        }

        public void UnequipShield(int slot)
        {
            equipmentDatabase.UnequipShield(slot);
        }

        public void ShowEquipment()
        {
            ShowRightWeapon();

            if (currentShieldInstance != null && currentShieldInstance is ShieldInstance shieldInstance)
            {
                shieldInstance.ResetStates();
            }
            if (currentShieldInstance != null && currentShieldInstance is CharacterWeaponHitbox characterWeaponHitbox)
            {
                characterWeaponHitbox.ShowWeapon();
            }
        }

        public void HideEquipment()
        {
            if (currentWeaponInstance != null)
            {
                currentWeaponInstance.HideWeapon();
            }

            if (currentShieldInstance != null)
            {
                if (currentShieldInstance is ShieldInstance shieldInstance)
                {
                    shieldInstance.HideShield();
                }

                if (currentShieldInstance is CharacterWeaponHitbox characterWeaponHitbox)
                {
                    characterWeaponHitbox.HideWeapon();
                }
            }
        }

        public void HideRightWeapon()
        {
            if (currentWeaponInstance != null)
            {
                currentWeaponInstance.HideWeapon();
            }
        }
        public void ShowRightWeapon()
        {
            if (playerManager.playerShootingManager.isAiming)
            {
                return;
            }

            if (currentWeaponInstance != null)
            {
                currentWeaponInstance.ShowWeapon();
            }
        }

        public void HideShield()
        {
            if (currentShieldInstance != null)
            {
                if (currentShieldInstance is ShieldInstance shieldInstance)
                {
                    shieldInstance.HideShield();
                }

                if (currentShieldInstance is CharacterWeaponHitbox characterWeaponHitbox)
                {
                    characterWeaponHitbox.HideWeapon();
                }
            }

        }

        public void ShowShield()
        {
            if (currentShieldInstance != null)
            {
                if (currentShieldInstance is ShieldInstance shieldInstance)
                {
                    shieldInstance.ShowShield();
                }

                if (currentShieldInstance is CharacterWeaponHitbox characterWeaponHitbox)
                {
                    characterWeaponHitbox.ShowWeapon();
                }
            }
        }

        bool CanApplyBuff()
        {
            if (currentWeaponInstance == null || currentWeaponInstance.characterWeaponBuffs == null)
            {
                notificationManager.ShowNotification(
                    CanNotApplyBuffToThisWeapon.GetLocalizedString(), notificationManager.systemError);
                return false;
            }
            else if (currentWeaponInstance.characterWeaponBuffs.HasOnGoingBuff())
            {
                notificationManager.ShowNotification(
                    WeaponIsAlreadyBuffed.GetLocalizedString(), notificationManager.systemError);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ApplyFireToWeapon(float customDuration)
        {
            ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName.FIRE, customDuration);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ApplyFrostToWeapon(float customDuration)
        {
            ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName.FROST, customDuration);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ApplyLightningToWeapon(float customDuration)
        {
            ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName.LIGHTNING, customDuration);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ApplyMagicToWeapon(float customDuration)
        {
            ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName.MAGIC, customDuration);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ApplyDarknessToWeapon(float customDuration)
        {
            ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName.DARKNESS, customDuration);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ApplyPoisonToWeapon(float customDuration)
        {
            ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName.POISON, customDuration);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ApplyBloodToWeapon(float customDuration)
        {
            ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName.BLOOD, customDuration);
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ApplySharpnessToWeapon(float customDuration)
        {
            ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName.SHARPNESS, customDuration);
        }


        public void ApplyWeaponBuffToWeapon(CharacterWeaponBuffs.WeaponBuffName weaponBuffName, float customDuration)
        {
            if (!CanApplyBuff())
            {
                return;
            }

            if (customDuration > 0)
            {
                currentWeaponInstance.characterWeaponBuffs.ApplyBuff(weaponBuffName, customDuration);
            }
            else
            {
                currentWeaponInstance.characterWeaponBuffs.ApplyBuff(weaponBuffName);
            }
        }

        public Damage GetBuffedDamage(Damage weaponDamage)
        {
            if (currentWeaponInstance == null || currentWeaponInstance.characterWeaponBuffs == null || currentWeaponInstance.characterWeaponBuffs.HasOnGoingBuff() == false)
            {
                return weaponDamage;
            }

            if (currentWeaponInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.FIRE)
            {
                weaponDamage.fire += currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.FIRE].damageBonus;


                if (currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.FIRE].statusEffect != null)
                {
                    StatusEffectEntry statusEffectToApply = new()
                    {
                        statusEffect = currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.FIRE].statusEffect,
                        amountPerHit = currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.FIRE].statusEffectAmountToApply,
                    };

                    if (weaponDamage.statusEffects == null)
                    {
                        weaponDamage.statusEffects = new StatusEffectEntry[] {
                        statusEffectToApply
                    };
                    }
                    else
                    {
                        weaponDamage.statusEffects = weaponDamage.statusEffects.Append(statusEffectToApply).ToArray();
                    }
                }
            }

            if (currentWeaponInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.FROST)
            {
                weaponDamage.frost += currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.FROST].damageBonus;

                if (currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.FROST].statusEffect != null)
                {
                    StatusEffectEntry statusEffectToApply = new()
                    {
                        statusEffect = currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.FROST].statusEffect,
                        amountPerHit = currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.FROST].statusEffectAmountToApply,
                    };

                    if (weaponDamage.statusEffects == null)
                    {
                        weaponDamage.statusEffects = new StatusEffectEntry[] {
                        statusEffectToApply
                    };
                    }
                    else
                    {
                        weaponDamage.statusEffects = weaponDamage.statusEffects.Append(statusEffectToApply).ToArray();
                    }
                }

            }

            if (currentWeaponInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.LIGHTNING)
            {
                if (currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.LIGHTNING].statusEffect != null)
                {
                    StatusEffectEntry statusEffectToApply = new()
                    {
                        statusEffect = currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.LIGHTNING].statusEffect,
                        amountPerHit = currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.LIGHTNING].statusEffectAmountToApply,
                    };

                    if (weaponDamage.statusEffects == null)
                    {
                        weaponDamage.statusEffects = new StatusEffectEntry[] {
                        statusEffectToApply
                    };
                    }
                    else
                    {
                        weaponDamage.statusEffects = weaponDamage.statusEffects.Append(statusEffectToApply).ToArray();
                    }
                }

                weaponDamage.lightning += currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.LIGHTNING].damageBonus;
            }

            if (currentWeaponInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.MAGIC)
            {
                weaponDamage.magic += currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.MAGIC].damageBonus;
            }

            if (currentWeaponInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.DARKNESS)
            {
                weaponDamage.darkness += currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.DARKNESS].damageBonus;
            }

            if (currentWeaponInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.WATER)
            {
                weaponDamage.water += currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.WATER].damageBonus;
            }

            if (currentWeaponInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.SHARPNESS)
            {
                weaponDamage.physical += currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.SHARPNESS].damageBonus;
            }

            if (currentWeaponInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.POISON)
            {
                StatusEffectEntry statusEffectToApply = new()
                {
                    statusEffect = currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.POISON].statusEffect,
                    amountPerHit = currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.POISON].statusEffectAmountToApply,
                };

                if (weaponDamage.statusEffects == null)
                {
                    weaponDamage.statusEffects = new StatusEffectEntry[] {
                        statusEffectToApply
                    };
                }
                else
                {
                    weaponDamage.statusEffects = weaponDamage.statusEffects.Append(statusEffectToApply).ToArray();
                }
            }

            if (currentWeaponInstance.characterWeaponBuffs.appliedBuff == CharacterWeaponBuffs.WeaponBuffName.BLOOD)
            {
                StatusEffectEntry statusEffectToApply = new()
                {
                    statusEffect = currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.BLOOD].statusEffect,
                    amountPerHit = currentWeaponInstance.characterWeaponBuffs.weaponBuffs[CharacterWeaponBuffs.WeaponBuffName.BLOOD].statusEffectAmountToApply,
                };

                if (weaponDamage.statusEffects == null)
                {
                    weaponDamage.statusEffects = new StatusEffectEntry[] {
                        statusEffectToApply
                    };
                }
                else
                {
                    weaponDamage.statusEffects = weaponDamage.statusEffects.Append(statusEffectToApply).ToArray();
                }
            }

            return weaponDamage;
        }

        public int GetCurrentBlockStaminaCost()
        {
            if (playerManager.playerWeaponsManager.currentShieldInstance == null)
            {
                return playerManager.characterBlockController.unarmedStaminaCostPerBlock;
            }

            if (currentShieldInstance != null && currentShieldInstance is ShieldInstance shieldInstance)
            {

                return (int)shieldInstance.shield.blockStaminaCost;
            }

            return 0;
        }

        public Damage GetCurrentShieldDefenseAbsorption(Damage incomingDamage)
        {
            ShieldInstance _currentShieldInstance = currentShieldInstance as ShieldInstance;

            if (equipmentDatabase.isTwoHanding && equipmentDatabase.GetCurrentWeapon() != null)
            {
                incomingDamage.physical = (int)(incomingDamage.physical * equipmentDatabase.GetCurrentWeapon().blockAbsorption);
                return incomingDamage;
            }
            else if (_currentShieldInstance == null || _currentShieldInstance.shield == null)
            {
                incomingDamage.physical = (int)(incomingDamage.physical * playerManager.characterBlockController.unarmedDefenseAbsorption);
                return incomingDamage;
            }

            return _currentShieldInstance.shield.FilterDamage(incomingDamage);
        }
        public Damage GetCurrentShieldPassiveDamageFilter(Damage incomingDamage)
        {
            ShieldInstance _currentShieldInstance = currentShieldInstance as ShieldInstance;

            if (_currentShieldInstance == null || _currentShieldInstance.shield == null)
            {
                return incomingDamage;
            }

            return _currentShieldInstance.shield.FilterPassiveDamage(incomingDamage);
        }

        public void ApplyShieldDamageToAttacker(CharacterManager attacker)
        {
            ShieldInstance _currentShieldInstance = currentShieldInstance as ShieldInstance;

            if (_currentShieldInstance == null || _currentShieldInstance.shield == null)
            {
                return;
            }

            _currentShieldInstance.shield.AttackShieldAttacker(attacker);
        }

        public void HandleWeaponSpecial()
        {
            if (
                playerManager.playerWeaponsManager.currentWeaponInstance == null
                || playerManager.playerWeaponsManager.currentWeaponInstance.onWeaponSpecial == null
                || playerManager.playerWeaponsManager.currentWeaponInstance.weapon == null
                )
            {
                return;
            }

            if (playerManager.manaManager.playerStatsDatabase.currentMana < playerManager.playerWeaponsManager.currentWeaponInstance.weapon.manaCostToUseWeaponSpecialAttack)
            {
                //                notificationManager.ShowNotification(NotEnoughManaToUseWeaponSpecial.GetLocalizedString());
                return;
            }

            playerManager.manaManager.DecreaseMana(
                playerManager.playerWeaponsManager.currentWeaponInstance.weapon.manaCostToUseWeaponSpecialAttack
            );

            playerManager.playerWeaponsManager.currentWeaponInstance.onWeaponSpecial?.Invoke();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ThrowWeapon()
        {

        }

        public void UpdateRangeWeaponTransformToIdle()
        {
            Weapon leftWeapon = equipmentDatabase.GetCurrentLeftWeapon();
            if (leftWeapon == null || leftWeapon.damage.weaponAttackType != WeaponAttackType.Range)
            {
                return;
            }

            if (currentShieldInstance != null)
            {
                currentShieldInstance.transform.localPosition = leftWeapon.leftHandPosition;
                currentShieldInstance.transform.localEulerAngles = leftWeapon.leftHandRotation;
            }
        }

        public void UpdateRangeWeaponTransformToAim()
        {
            Weapon leftWeapon = equipmentDatabase.GetCurrentLeftWeapon();
            if (leftWeapon == null || leftWeapon.damage.weaponAttackType != WeaponAttackType.Range)
            {
                return;
            }

            if (currentShieldInstance != null)
            {
                currentShieldInstance.transform.localPosition = leftWeapon.aimingPosition;
                currentShieldInstance.transform.localEulerAngles = leftWeapon.aimingRotation;
            }
        }

    }
}
