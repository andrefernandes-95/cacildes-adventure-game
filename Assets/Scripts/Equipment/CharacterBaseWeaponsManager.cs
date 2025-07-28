namespace AF
{
    using System.Linq;
    using AF.Equipment;
    using AF.Health;
    using UnityEngine;

    public abstract class CharacterBaseWeaponsManager : MonoBehaviour
    {

        [Header("Unarmed Weapon References In-World")]
        public Hitbox leftHandHitbox;
        public Hitbox rightHandHitbox;
        public Hitbox leftFootHitbox;
        public Hitbox rightFootHitbox;
        public Hitbox headHitbox;


        [Header("Current Weapon")]
        public CharacterWeaponHitbox currentWeaponInstance;
        public CharacterWeaponHitbox currentShieldInstance;

        public float DEFAULT_WEAPON_BUFF_DURATION = 120f;

        private void Start()
        {
            UpdateEquipment();
        }

        protected void UpdateEquipment()
        {
            UpdateCurrentWeapon();
            UpdateCurrentLeftWeapon();

            GetCharacter().characterBaseAttackManager.CalculateCurrentDamage();
        }

        public void ResetStates()
        {
            CloseAllWeaponHitboxes();
            ShowEquipment();
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public virtual void CloseAllWeaponHitboxes()
        {
            currentWeaponInstance?.DisableHitbox();
            currentShieldInstance?.DisableHitbox();
            leftHandHitbox?.DisableHitbox();
            rightHandHitbox?.DisableHitbox();
            leftFootHitbox?.DisableHitbox();
            rightFootHitbox?.DisableHitbox();
            headHitbox?.DisableHitbox();
        }

        public abstract Weapon GetCurrentRightWeapon();
        public abstract Weapon GetCurrentLeftWeapon();
        public abstract bool IsTwoHanding();
        public abstract bool HasRangeWeapon();


        protected virtual void UpdateCurrentWeapon()
        {
            var CurrentWeapon = GetCurrentRightWeapon();

            if (currentWeaponInstance != null)
            {
                Destroy(currentWeaponInstance);
                currentWeaponInstance = null;
            }

            if (GetCharacter().characterTransformHelper.rightHand != null && GetCharacter().characterTransformHelper.rightHand.childCount > 0)
            {
                foreach (Transform child in GetCharacter().characterTransformHelper.rightHand.transform)
                {
                    Destroy(child.gameObject);
                }
            }

            if (CurrentWeapon is Weapon rightWeapon)
            {
                InstantiateWeapon(rightWeapon, true);
            }
        }

        protected virtual void UpdateCurrentLeftWeapon()
        {
            var CurrentShield = IsTwoHanding() ? null : GetCurrentLeftWeapon();

            if (currentShieldInstance != null)
            {
                Destroy(currentShieldInstance);

                currentShieldInstance = null;
            }

            if (GetCharacter().characterTransformHelper.leftHand != null && GetCharacter().characterTransformHelper.leftHand.childCount > 0)
            {
                foreach (Transform child in GetCharacter().characterTransformHelper.leftHand.transform)
                {
                    Destroy(child.gameObject);
                }
            }

            if (CurrentShield is Weapon leftWeapon)
            {
                InstantiateWeapon(leftWeapon, false);
            }
        }

        void InstantiateWeapon(Weapon weapon, bool isRightHand)
        {
            Transform grip = isRightHand ? GetCharacter().characterTransformHelper.rightHand : GetCharacter().characterTransformHelper.leftHand;

            if (weapon.weaponPrefab != null && grip != null)
            {
                foreach (Transform child in grip.transform)
                {
                    Destroy(child.gameObject);
                }

                GameObject instantiatedWeapon = Instantiate(weapon.weaponPrefab, grip).gameObject;
                CharacterWeaponHitbox instatiatedCharacterWeaponHitbox = instantiatedWeapon.GetComponent<CharacterWeaponHitbox>();

                // Important for dual wielding, to know which hitbox is hitting enemies
                instatiatedCharacterWeaponHitbox.hitboxType = isRightHand ? HitboxType.RIGHT_HAND : HitboxType.LEFT_HAND;

                // Replace the weapon from the prefab with the equipped one which contains itemIDs, level, etc.
                instatiatedCharacterWeaponHitbox.weapon = weapon;

                // Shields
                if (instatiatedCharacterWeaponHitbox is ShieldInstance shieldInstance1 && weapon is Shield shield)
                {
                    shieldInstance1.shield = shield;
                }

                // TOOD: Remove this code, its just to always have ref positions up to date
                Weapon weaponTemplate = Resources.Load<Weapon>("Items/Weapons/" + weapon.name.Replace("(Clone)", ""));
                if (weaponTemplate == null)
                {
                    weaponTemplate = Resources.Load<Weapon>("Items/Shields/" + weapon.name.Replace("(Clone)", ""));
                }

                if (weaponTemplate != null)
                {
                    instatiatedCharacterWeaponHitbox.weapon.rightHandPosition = weaponTemplate.rightHandPosition;
                    instatiatedCharacterWeaponHitbox.weapon.rightHandRotation = weaponTemplate.rightHandRotation;
                    instatiatedCharacterWeaponHitbox.weapon.leftHandPosition = weaponTemplate.leftHandPosition;
                    instatiatedCharacterWeaponHitbox.weapon.leftHandRotation = weaponTemplate.leftHandRotation;
                    instatiatedCharacterWeaponHitbox.weapon.twoHandingPosition = weaponTemplate.twoHandingPosition;
                    instatiatedCharacterWeaponHitbox.weapon.twoHandingRotation = weaponTemplate.twoHandingRotation;
                    instatiatedCharacterWeaponHitbox.weapon.th_BlockPosition = weaponTemplate.th_BlockPosition;
                    instatiatedCharacterWeaponHitbox.weapon.th_BlockRotation = weaponTemplate.th_BlockRotation;
                    instatiatedCharacterWeaponHitbox.weapon.aimingPosition = weaponTemplate.aimingPosition;
                    instatiatedCharacterWeaponHitbox.weapon.aimingRotation = weaponTemplate.aimingRotation;
                }

                UpdateWorldReferences(instatiatedCharacterWeaponHitbox);

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

        void UpdateWorldReferences(CharacterWeaponHitbox characterWeaponHitbox)
        {
            if (characterWeaponHitbox == null)
            {
                return;
            }

            if (characterWeaponHitbox.TryGetComponent(out CharacterTwoHandRef twoHandRef))
            {
                twoHandRef.characterBaseManager = GetCharacter();
            }
        }

        protected abstract CharacterBaseManager GetCharacter();

        void UnassignShield()
        {
            if (currentShieldInstance != null && currentShieldInstance is ShieldInstance shieldInstance)
            {
                currentShieldInstance.gameObject.SetActive(false);
                shieldInstance.shieldInTheBack.gameObject.SetActive(false);
                currentShieldInstance = null;
            }
        }

        public virtual void ShowEquipment()
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

        public virtual void HideEquipment()
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

        public virtual void ShowRightWeapon()
        {
            if (currentWeaponInstance != null)
            {
                currentWeaponInstance.ShowWeapon();
            }
        }

        public virtual void HideShield()
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

        public virtual void ShowShield()
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

        protected virtual bool CanApplyBuff()
        {
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

        public virtual int GetCurrentBlockStaminaCost()
        {
            if (currentShieldInstance != null && currentShieldInstance is ShieldInstance shieldInstance)
            {

                return (int)shieldInstance.shield.blockStaminaCost;
            }

            return 0;
        }

        public Damage GetCurrentShieldDefenseAbsorption(Damage incomingDamage)
        {
            ShieldInstance _currentShieldInstance = currentShieldInstance as ShieldInstance;

            if (IsTwoHanding() && GetCurrentRightWeapon() != null)
            {
                incomingDamage.physical = (int)(incomingDamage.physical * GetCurrentRightWeapon().weaponBlockAbsorption);
                return incomingDamage;
            }
            else if (_currentShieldInstance == null || _currentShieldInstance.shield == null)
            {
                incomingDamage.physical = (int)(incomingDamage.physical * GetCharacterUnarmedDefenseAbsorption());
                return incomingDamage;
            }

            return _currentShieldInstance.shield.FilterDamage(incomingDamage);
        }

        protected abstract float GetCharacterUnarmedDefenseAbsorption();

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

        /*
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
        }*/

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ThrowWeapon()
        {

        }

        public void UpdateRangeWeaponTransformToIdle()
        {
            Weapon leftWeapon = GetCurrentLeftWeapon();
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
            Weapon leftWeapon = GetCurrentLeftWeapon();
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

        public bool CanPowerStance()
        {
            if (IsTwoHanding())
            {
                return false;
            }

            if (GetCurrentRightWeapon() == null || GetCurrentLeftWeapon() == null)
            {
                return false;
            }

            return GetCurrentRightWeapon().weaponType == GetCurrentLeftWeapon().weaponType;
        }

        public bool IsAttacking()
        {
            if (currentWeaponInstance != null && currentWeaponInstance.IsHitboxOpen())
            {
                return true;
            }

            return currentShieldInstance != null && currentShieldInstance.IsHitboxOpen();
        }
    }
}
