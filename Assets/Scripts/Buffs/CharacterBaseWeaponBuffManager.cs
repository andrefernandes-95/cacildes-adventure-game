using System.Collections.Generic;
using System.Linq;
using AF.Health;
using UnityEngine;

namespace AF
{
    public abstract class CharacterBaseWeaponBuffManager : MonoBehaviour
    {
        [System.Serializable]
        public class WeaponBuffInstance
        {
            public string weaponID;
            public WeaponBuffAttribute weaponBuffAttribute;
            public float timeRemaining;
        }

        public List<WeaponBuffInstance> currentBuffs = new();

        public abstract CharacterBaseManager GetCharacter();

        List<WeaponBuffInstance> buffsToRemove = new();

        CharacterWeaponHitbox GetTargetHitboxForBuff()
        {
            CharacterWeaponHitbox rightHandHitbox = GetCharacter().characterBaseWeaponsManager.currentWeaponInstance;

            // First, attempt to buff the right weapon
            if (rightHandHitbox != null && rightHandHitbox.weapon != null)
            {
                if (!HasBuffApplied(rightHandHitbox.weapon))
                {
                    return rightHandHitbox;
                }
            }

            // Left Hand Weapon
            CharacterWeaponHitbox leftHandHitbox = GetCharacter().characterBaseWeaponsManager.currentShieldInstance;
            if (leftHandHitbox != null && leftHandHitbox.weapon != null)
            {
                if (!HasBuffApplied(leftHandHitbox.weapon))
                {
                    return leftHandHitbox;
                }
            }

            return null;
        }

        bool HasBuffApplied(Weapon weapon)
        {
            return weapon != null
                 && currentBuffs.Any(currentBuff => weapon.itemID.Equals(currentBuff.weaponID));
        }

        public void AddBuff(WeaponBuffAttribute weaponBuffAttribute)
        {
            CharacterWeaponHitbox targetWeaponHitbox = GetTargetHitboxForBuff();

            if (targetWeaponHitbox == null)
            {
                return;
            }

            Weapon currentWeapon = targetWeaponHitbox.weapon;

            WeaponBuffInstance weaponBuffInstance = new()
            {
                weaponBuffAttribute = weaponBuffAttribute,
                timeRemaining = weaponBuffAttribute.durationInSeconds,
                weaponID = currentWeapon.itemID
            };

            currentBuffs.Add(weaponBuffInstance);

            HandleCurrentBuffsVfx();
        }

        public void RemoveBuff(WeaponBuffInstance instance)
        {
            if (instance != null)
            {
                RemoveBuffVfx(instance);
                currentBuffs.Remove(instance);
            }
        }

        private void Update()
        {
            if (currentBuffs.Count <= 0)
            {
                return;
            }

            buffsToRemove.Clear();
            foreach (WeaponBuffInstance weaponBuffInstance in currentBuffs)
            {
                weaponBuffInstance.timeRemaining -= Time.deltaTime;

                if (weaponBuffInstance.timeRemaining <= 0)
                {
                    buffsToRemove.Add(weaponBuffInstance);
                }
            }

            if (buffsToRemove.Count > 0)
            {
                foreach (WeaponBuffInstance buffToRemove in buffsToRemove)
                {
                    RemoveBuff(buffToRemove);
                }
            }
        }

        public Damage EnhanceAttackDamage(Damage baseDamage)
        {
            // Get Attacking Weapon
            if (GetCharacter().characterBaseAttackManager.attackingHitboxType == HitboxType.RIGHT_HAND)
            {
                Weapon rightHandWeapon = GetCharacter().characterBaseWeaponsManager.GetCurrentRightWeapon();
                if (rightHandWeapon != null)
                {
                    WeaponBuffInstance match = currentBuffs.FirstOrDefault(x => x.weaponID.Equals(rightHandWeapon.itemID));

                    if (match != null)
                    {
                        return EnhanceDamage(baseDamage, match);
                    }
                }
            }

            if (GetCharacter().characterBaseAttackManager.attackingHitboxType == HitboxType.LEFT_HAND)
            {
                Weapon leftHandWeapon = GetCharacter().characterBaseWeaponsManager.GetCurrentLeftWeapon();
                if (leftHandWeapon != null)
                {
                    WeaponBuffInstance match = currentBuffs.FirstOrDefault(x => x.weaponID.Equals(leftHandWeapon.itemID));

                    if (match != null)
                    {
                        return EnhanceDamage(baseDamage, match);
                    }
                }
            }

            return baseDamage;
        }

        Damage EnhanceDamage(Damage baseDamage, WeaponBuffInstance weaponBuffInstance)
        {
            if (weaponBuffInstance.weaponBuffAttribute.weaponElementType == WeaponBuffType.Physical)
            {
                int physicalBonus = weaponBuffInstance.weaponBuffAttribute.baseDamage
                    + GetCharacter().characterBaseAttackManager.GetBonusAttackPerLevel(GetCharacter().characterBaseStats.GetStrength())
                    + GetCharacter().characterBaseAttackManager.GetBonusAttackPerLevel(GetCharacter().characterBaseStats.GetDexterity());

                baseDamage.physical += physicalBonus;
            }

            if (weaponBuffInstance.weaponBuffAttribute.weaponElementType == WeaponBuffType.Fire)
            {
                int fireBonus = weaponBuffInstance.weaponBuffAttribute.baseDamage
                    + GetCharacter().characterBaseAttackManager.GetBonusAttackPerLevel(GetCharacter().characterBaseStats.GetIntelligence());

                baseDamage.fire += fireBonus;
            }

            if (weaponBuffInstance.weaponBuffAttribute.weaponElementType == WeaponBuffType.Frost)
            {
                int frostBonus = weaponBuffInstance.weaponBuffAttribute.baseDamage
                    + GetCharacter().characterBaseAttackManager.GetBonusAttackPerLevel(GetCharacter().characterBaseStats.GetIntelligence());

                baseDamage.frost += frostBonus;
            }

            if (weaponBuffInstance.weaponBuffAttribute.weaponElementType == WeaponBuffType.Lightning)
            {
                int lightningBonus = weaponBuffInstance.weaponBuffAttribute.baseDamage
                    + GetCharacter().characterBaseAttackManager.GetBonusAttackPerLevel(GetCharacter().characterBaseStats.GetIntelligence());

                baseDamage.lightning += lightningBonus;
            }

            if (weaponBuffInstance.weaponBuffAttribute.weaponElementType == WeaponBuffType.Magic)
            {
                int magicBonus = weaponBuffInstance.weaponBuffAttribute.baseDamage
                    + GetCharacter().characterBaseAttackManager.GetBonusAttackPerLevel(GetCharacter().characterBaseStats.GetIntelligence());

                baseDamage.magic += magicBonus;
            }

            if (weaponBuffInstance.weaponBuffAttribute.weaponElementType == WeaponBuffType.Darkness)
            {
                int darknessBonus = weaponBuffInstance.weaponBuffAttribute.baseDamage
                    + GetCharacter().characterBaseAttackManager.GetBonusAttackPerLevel(GetCharacter().characterBaseStats.GetIntelligence());

                baseDamage.darkness += darknessBonus;
            }

            if (weaponBuffInstance.weaponBuffAttribute.weaponElementType == WeaponBuffType.Water)
            {
                int waterBonus = weaponBuffInstance.weaponBuffAttribute.baseDamage
                    + GetCharacter().characterBaseAttackManager.GetBonusAttackPerLevel(GetCharacter().characterBaseStats.GetIntelligence());

                baseDamage.water += waterBonus;
            }

            if (weaponBuffInstance.weaponBuffAttribute.statusEffect != null)
            {
                StatusEffectEntry statusEffectToApply = new()
                {
                    statusEffect = weaponBuffInstance.weaponBuffAttribute.statusEffect,
                    amountPerHit = weaponBuffInstance.weaponBuffAttribute.statusEffectAmountApplied
                };
                if (baseDamage.statusEffects == null)
                {
                    baseDamage.statusEffects = new StatusEffectEntry[] {
                        statusEffectToApply
                    };
                }
                else
                {
                    baseDamage.statusEffects = baseDamage.statusEffects.Append(statusEffectToApply).ToArray();
                }
            }

            return baseDamage;
        }

        public void OnEquipmentChanged()
        {
            HandleCurrentBuffsVfx();
        }

        void HandleCurrentBuffsVfx()
        {
            var character = GetCharacter();
            if (character == null || currentBuffs == null || currentBuffs.Count == 0)
            {
                return;
            }

            for (int i = 0; i < currentBuffs.Count; i++)
            {
                var buff = currentBuffs[i];

                CharacterWeaponHitbox targetTransform = GetTargetWeaponHitboxByWeaponID(buff.weaponID);

                if (targetTransform != null)
                {
                    HandleWeaponBuffEffectVFX(buff.weaponBuffAttribute.weaponElementType, targetTransform.transform, true);
                }
            }
        }

        CharacterWeaponHitbox GetTargetWeaponHitboxByWeaponID(string weaponID)
        {

            var weaponManager = GetCharacter().characterBaseWeaponsManager;
            if (weaponManager == null)
            {
                return null;
            }

            // Check against current weapon
            var weaponInstance = weaponManager.currentWeaponInstance;
            if (weaponInstance?.weapon?.itemID == weaponID)
            {
                return weaponInstance;
            }
            // Check against shield
            else
            {
                var shieldInstance = weaponManager.currentShieldInstance;
                if (shieldInstance?.weapon?.itemID == weaponID)
                {
                    return shieldInstance;
                }
            }

            return null;
        }

        void RemoveBuffVfx(WeaponBuffInstance weaponBuffInstance)
        {
            CharacterWeaponHitbox targetTransform = GetTargetWeaponHitboxByWeaponID(weaponBuffInstance.weaponID);

            if (targetTransform != null)
            {
                HandleWeaponBuffEffectVFX(weaponBuffInstance.weaponBuffAttribute.weaponElementType, targetTransform.transform, false);
            }
        }

        void HandleWeaponBuffEffectVFX(WeaponBuffType weaponBuffType, Transform targetTransform, bool enable)
        {
            WeaponBuffEffect[] weaponBuffEffects = targetTransform.GetComponentsInChildren<WeaponBuffEffect>(true);
            WeaponBuffEffect targetWeaponBuffEffect = weaponBuffEffects.FirstOrDefault(
                x => x != null && x.weaponElementType == weaponBuffType);

            if (targetWeaponBuffEffect != null)
            {
                targetWeaponBuffEffect.gameObject.SetActive(enable);
            }
        }

    }
}
