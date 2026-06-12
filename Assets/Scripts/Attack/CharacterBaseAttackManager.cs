namespace AF
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AF.Health;

    public abstract class CharacterBaseAttackManager : MonoBehaviour
    {
        private Dictionary<string, float> scalingDictionary = new();

        [Header("Current Damage")]
        public Damage rightWeaponCurrentDamage;
        public Damage leftWeaponCurrentDamage;

        [HideInInspector] public HitboxType attackingHitboxType;

        [Header("Unarmed Hitboxes")]
        [SerializeField] UnarmedHitbox unarmedRightWeapon;
        [SerializeField] UnarmedHitbox unarmedLeftWeapon;

        public enum AttackSource
        {
            WEAPON,
            SHIELD,
            UNARMED
        }

        public AttackSource attackSource = AttackSource.UNARMED;

        [HideInInspector] public Damage damageBonus;



        private void Start()
        {
            scalingDictionary.Add("E", Formulas.E);
            scalingDictionary.Add("D", Formulas.D);
            scalingDictionary.Add("C", Formulas.C);
            scalingDictionary.Add("B", Formulas.B);
            scalingDictionary.Add("A", Formulas.A);
            scalingDictionary.Add("S", Formulas.S);
        }

        public void ResetStates()
        {
            damageBonus = null;
        }

        public abstract bool IsHeavyAttacking();

        public abstract bool IsJumpAttacking();

        public abstract bool IsInAir();

        public bool HasRangeWeaponEquipped()
        {
            return GetCharacter().characterBaseWeaponsManager.HasRangeWeapon();
        }

        public Damage GetAttackDamage()
        {
            Damage clonedDamage;

            if (attackingHitboxType == HitboxType.LEFT_HAND)
            {
                clonedDamage = leftWeaponCurrentDamage.Clone();
            }
            else
            {
                clonedDamage = rightWeaponCurrentDamage.Clone();
            }

            if (damageBonus != null)
            {
                clonedDamage.Combine(damageBonus);
            }

            Damage enhancedDamageWithCharacterBuffs = GetCharacter().characterBaseBuffManager.EnhanceAttackDamage(clonedDamage);
            Damage enhancedDamageWithWeaponBuffs = GetCharacter().characterBaseWeaponBuffManager.EnhanceAttackDamage(enhancedDamageWithCharacterBuffs);
            return enhancedDamageWithWeaponBuffs;
        }

        public void CalculateCurrentDamage()
        {
            if (GetCharacter().characterBaseWeaponsManager.currentWeaponInstance != null)
            {
                rightWeaponCurrentDamage = CalculateWeaponDamageForWeapon(GetCharacter().characterBaseWeaponsManager.currentWeaponInstance.weapon).weaponDamage;
            }
            else if (unarmedRightWeapon != null)
            {
                rightWeaponCurrentDamage = CalculateUnarmedDamage(unarmedRightWeapon.unarmedWeapon.damage).weaponDamage;
            }

            if (GetCharacter().characterBaseWeaponsManager.currentShieldInstance != null)
            {
                leftWeaponCurrentDamage = CalculateWeaponDamageForWeapon(GetCharacter().characterBaseWeaponsManager.currentShieldInstance.weapon).weaponDamage;
            }
            else if (unarmedLeftWeapon != null)
            {
                leftWeaponCurrentDamage = CalculateUnarmedDamage(unarmedLeftWeapon.unarmedWeapon.damage).weaponDamage;
            }
        }

        public (Damage weaponDamage, int STRBonus, int DEXBonus, int INTBonus,
        int TwoHandAttackBonus) CalculateWeaponDamageForWeapon(Weapon weapon)
        {
            Damage weaponDamage = weapon.damage.Clone();

            int STRBonus = weaponDamage.GetStrengthBonus(GetCharacter().characterBaseStats.GetStrength());
            int DEXBonus = weaponDamage.GetDexterityBonus(GetCharacter().characterBaseStats.GetDexterity());
            int INTBonus = weaponDamage.GetIntelligenceBonus(GetCharacter().characterBaseStats.GetIntelligence());

            // Override with appropriate values for current weapon level
            weaponDamage.physical =
                weapon.GetCurrentPhysicalAttackForLevel(weapon.level);

            // Store the weapon's current base physical damage for UI purposes
            weaponDamage.basePhysicalDamage = weaponDamage.physical;

            if (weaponDamage.physical > 0)
            {
                weaponDamage.physical += STRBonus + DEXBonus;
            }

            weaponDamage.fire = weapon.GetFireAttackForLevel(weapon.level);
            if (weaponDamage.fire > 0) weaponDamage.fire += INTBonus;

            weaponDamage.frost = weapon.GetFrostAttackForLevel(weapon.level);
            if (weaponDamage.frost > 0) weaponDamage.frost += INTBonus;

            weaponDamage.lightning = weapon.GetLightningAttackForLevel(weapon.level);
            if (weaponDamage.lightning > 0) weaponDamage.lightning += INTBonus;

            weaponDamage.magic = weapon.GetMagicAttackForLevel(weapon.level);
            if (weaponDamage.magic > 0) weaponDamage.magic += INTBonus;

            weaponDamage.darkness = weapon.GetDarknessAttackForLevel(weapon.level);
            if (weaponDamage.darkness > 0) weaponDamage.darkness += INTBonus;

            weaponDamage.water = weapon.GetWaterAttackForLevel(weapon.level);
            if (weaponDamage.water > 0) weaponDamage.water += INTBonus;

            weaponDamage.poiseDamage = weapon.GetBonusPoisePerLevel(weaponDamage.poiseDamage, weapon.level);
            weaponDamage.postureDamage = weapon.GetBonusPosturePerLevel(weaponDamage.postureDamage, weapon.level);

            weaponDamage.statusEffects = new StatusEffectEntry[0];

            List<StatusEffectEntry> weaponStatusEffectsToAdd = new();
            foreach (var weaponStatusEffectToClone in weapon.damage.statusEffects)
            {
                if (weaponStatusEffectToClone != null)
                {
                    StatusEffectEntry statusEffectEntry = new();
                    statusEffectEntry.statusEffect = weaponStatusEffectToClone.statusEffect;
                    statusEffectEntry.amountPerHit = weapon.GetBonusStatusEffectAmountPerHitPerLevel(weaponStatusEffectToClone.amountPerHit, weapon.level);
                    weaponStatusEffectsToAdd.Add(statusEffectEntry);
                }
            }
            weaponDamage.statusEffects = weaponStatusEffectsToAdd.ToArray();

            // Copy other stats from the weapon
            weaponDamage.pushForce = weapon.damage.pushForce;
            weaponDamage.ignoreBlocking = weapon.damage.ignoreBlocking;
            weaponDamage.canNotBeParried = weapon.damage.canNotBeParried;

            int twoHandAttackBonus = GetTwoHandBonus();

            // If two handing, increase the posture damage too
            if (GetCharacter().characterBaseWeaponsManager.IsTwoHanding())
            {
                weaponDamage.postureDamage += twoHandAttackBonus;
            }

            ApplyWeaponBuffs(weapon, weaponDamage);

            // If character doesn't meet the requirements
            if (!DoesCharacterMeetWeaponRequirements(weapon))
            {
                weaponDamage.Multiply(.25f);
            }

            return (weaponDamage, STRBonus, DEXBonus, INTBonus, twoHandAttackBonus);
        }

        int GetTwoHandBonus()
        {
            if (!GetCharacter().characterBaseWeaponsManager.IsTwoHanding())
            {
                return 0;
            }

            float str = GetCharacter().characterBaseStats.GetStrength();

            // Front-loaded bonus with diminishing returns
            // STR 0  -> 20
            // STR 5  -> ~27
            // STR 10 -> ~30
            // STR 25 -> ~35
            // STR 50 -> ~39
            return (int)(
                20 +
                Mathf.Pow(str, 0.5f) * 3.0f +
                GetCharacter().statsBonusController.twoHandAttackBonusMultiplier
            );
        }

        int GetJumpAttackBonus()
        {
            float str = GetCharacter().characterBaseStats.GetDexterity();
            int jumpAttackBonus = (int)(10 + Mathf.Pow(str, 0.9f) * 1.5f + GetCharacter().statsBonusController.jumpAttackBonusMultiplier);
            return jumpAttackBonus;
        }

        int GetHeavyAttackBonus()
        {
            // Get the character's Strength stat
            int str = GetCharacter().characterBaseStats.GetStrength();
            int dex = GetCharacter().characterBaseStats.GetDexterity();

            int strengthMultiplier = (int)(10 + Mathf.Pow(str, 0.9f) * 1.5f + GetCharacter().statsBonusController.heavyAttackBonusMultiplier);
            int dexterityMultiplier = (int)(10 + Mathf.Pow(dex, 0.9f) * 1.25f + GetCharacter().statsBonusController.heavyAttackBonusMultiplier);

            return strengthMultiplier + dexterityMultiplier;
        }

        void EnhanceDamage(Damage damage, int bonus)
        {
            int postureDamageBonus = bonus / 20;
            int poiseDamageBonus = bonus / 40;

            // Prioritize physical damage over elemental
            damage.physical += bonus;
            if (damage.fire > 0) damage.fire += bonus / 4;
            if (damage.frost > 0) damage.frost += bonus / 4;
            if (damage.lightning > 0) damage.lightning += bonus / 4;
            if (damage.magic > 0) damage.magic += bonus / 4;
            if (damage.darkness > 0) damage.darkness += bonus / 4;
            if (damage.water > 0) damage.water += bonus / 4;
            if (damage.postureDamage > 0) damage.postureDamage += postureDamageBonus;
            if (damage.poiseDamage > 0) damage.poiseDamage += poiseDamageBonus;
        }

        public void EnhanceWithTwoHandingDamage(Damage damage)
        {
            int damageBonus = GetTwoHandBonus();
            EnhanceDamage(damage, damageBonus);
        }

        public void EnhanceWithHeavyAttackDamage(Damage damage)
        {
            int damageBonus = GetHeavyAttackBonus();
            EnhanceDamage(damage, damageBonus);
        }

        public void EnhanceWithJumpAttackDamage(Damage damage)
        {
            int damageBonus = GetJumpAttackBonus();
            EnhanceDamage(damage, damageBonus);
        }

        void ApplyWeaponBuffs(Weapon weapon, Damage weaponDamage)
        {
            if (GetCharacter().characterBaseWeaponsManager.IsTwoHanding())
            {
                EnhanceWithTwoHandingDamage(weaponDamage);
            }

            // + Attack the lower the health
            if (GetAccessories().FirstOrDefault(x => x != null && x.increaseAttackPowerWithLowerHealth) != null)
            {
                int extraAttackPower = GetCharacter().health.GetExtraAttackBasedOnCurrentHealth();
                weaponDamage.physical += extraAttackPower;
            }

            // Generic attack bonuses
            var attackBonuses = GetAccessories().Sum(x => x != null ? x.physicalAttackBonus : 0);
            weaponDamage.physical += attackBonuses;

            float attackMultiplierBonuses = 1f;
            if (weapon == null)
            {
                if (GetCharacter().statsBonusController.increaseAttackPowerWhenUnarmed)
                {
                    attackMultiplierBonuses += 0.5f;
                }
            }
            else
            {
                if (weaponDamage.weaponAttackType == WeaponAttackType.Pierce)
                {
                    attackMultiplierBonuses += GetCharacter().statsBonusController.pierceDamageMultiplier;
                }
                else if (weaponDamage.weaponAttackType == WeaponAttackType.Slash)
                {
                    attackMultiplierBonuses += GetCharacter().statsBonusController.slashDamageMultiplier;
                }
                else if (weaponDamage.weaponAttackType == WeaponAttackType.Blunt)
                {
                    attackMultiplierBonuses += GetCharacter().statsBonusController.bluntDamageMultiplier;
                }
            }

            weaponDamage.physical = (int)(weaponDamage.physical * attackMultiplierBonuses);
        }


        public (Damage weaponDamage, int STRBonus, int DEXBonus, int INTBonus, int TwoHandAttackBonus) CalculateUnarmedDamage(Damage unarmedDamage)
        {
            Damage weaponDamage = unarmedDamage.Clone();

            int STRBonus = weaponDamage.GetStrengthBonus(GetCharacter().characterBaseStats.GetStrength());
            int DEXBonus = weaponDamage.GetDexterityBonus(GetCharacter().characterBaseStats.GetDexterity());
            int INTBonus = weaponDamage.GetIntelligenceBonus(GetCharacter().characterBaseStats.GetIntelligence());

            // Store the weapon's current base physical damage for UI purposes
            weaponDamage.basePhysicalDamage = weaponDamage.physical;

            if (weaponDamage.physical > 0)
            {
                weaponDamage.physical += STRBonus + DEXBonus;
            }

            ApplyWeaponBuffs(null, weaponDamage);

            int twoHandAttackBonus = GetTwoHandBonus();

            return (weaponDamage, STRBonus, DEXBonus, INTBonus, twoHandAttackBonus);
        }

        public Accessory[] GetAccessories()
        {
            return GetCharacter().characterBaseEquipment.GetEquippedAccessories();
        }

        public int CompareWeapon(Weapon weaponToCompare, bool isRightHand)
        {
            if (isRightHand && GetCharacter().characterBaseWeaponsManager.GetCurrentRightWeapon() == null)
            {
                return 1;
            }
            if (!isRightHand && GetCharacter().characterBaseWeaponsManager.GetCurrentLeftWeapon() == null)
            {
                return 1;
            }

            var weaponToCompareAttack = GetWeaponAttack(weaponToCompare);
            var currentWeaponAttack = GetWeaponAttack(isRightHand
                ? GetCharacter().characterBaseWeaponsManager.GetCurrentRightWeapon() : GetCharacter().characterBaseWeaponsManager.GetCurrentLeftWeapon());

            if (weaponToCompareAttack > currentWeaponAttack)
            {
                return 1;
            }

            if (weaponToCompareAttack == currentWeaponAttack)
            {
                return 0;
            }

            return -1;
        }

        public int GetWeaponAttack(Weapon weapon)
        {
            Damage damage = CalculateWeaponDamageForWeapon(weapon).weaponDamage;
            if (damage != null)
            {
                return damage.GetTotalDamage();
            }

            return 0;
        }

        public abstract CharacterBaseManager GetCharacter();

        public abstract bool DoesCharacterMeetWeaponRequirements(Weapon weapon);

        float GetBonusStep(int level)
        {
            if (level % 3 == 0)
            {
                return 6;
            }
            else
            {
                return 3;
            }
        }

        public int GetBonusAttackPerLevel(int level)
        {
            if (level == 0)
            {
                return 0;
            }

            float total = 0;

            for (int i = 1; i <= level; i++)
            {
                total += GetBonusStep(i);
            }

            return Mathf.RoundToInt(total);
        }
    }
}
