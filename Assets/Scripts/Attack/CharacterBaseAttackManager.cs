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

        [Header("Physical Attack")]
        [HideInInspector] public float jumpAttackMultiplier = 1.2f;
        [HideInInspector] public float twoHandAttackBonusMultiplier = 1.2f;
        [HideInInspector] public float heavyAttackBonusMultiplier = 1.3f;

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

            int twoHandAttackBonus = ApplyWeaponBuffs(weapon, weaponDamage);

            // If character doesn't meet the requirements
            if (!DoesCharacterMeetWeaponRequirements(weapon))
            {
                weaponDamage.Multiply(.25f);
            }

            return (weaponDamage, STRBonus, DEXBonus, INTBonus, twoHandAttackBonus);
        }

        int ApplyWeaponBuffs(Weapon weapon, Damage weaponDamage)
        {
            // Apply Weapon Buffs
            int twoHandAttackBonus = 0;
            if (GetCharacter().characterBaseWeaponsManager.IsTwoHanding())
            {
                float twoHandMultiplier = twoHandAttackBonusMultiplier + GetCharacter().statsBonusController.twoHandAttackBonusMultiplier;

                weaponDamage.physical = (int)(weaponDamage.physical * twoHandMultiplier);
                weaponDamage.fire = (int)(weaponDamage.fire * twoHandMultiplier);
                weaponDamage.frost = (int)(weaponDamage.frost * twoHandMultiplier);
                weaponDamage.lightning = (int)(weaponDamage.lightning * twoHandMultiplier);
                weaponDamage.magic = (int)(weaponDamage.magic * twoHandMultiplier);
                weaponDamage.darkness = (int)(weaponDamage.darkness * twoHandMultiplier);
                weaponDamage.water = (int)(weaponDamage.water * twoHandMultiplier);
                weaponDamage.postureDamage = (int)(weaponDamage.postureDamage * twoHandMultiplier);
                weaponDamage.poiseDamage = (int)(weaponDamage.poiseDamage * twoHandMultiplier);
            }

            // + Attack the lower the rep
            /* if (GetAccessories().FirstOrDefault(x => x != null && x.increaseAttackPowerTheLowerTheReputation) != null)
             {
                 if (playerStatsDatabase.GetCurrentReputation() < 0)
                 {
                     int extraAttackPower = Mathf.Min(150, (int)(Mathf.Abs(playerStatsDatabase.GetCurrentReputation()) * 2.25f));

                     value += extraAttackPower;
                 }
             }*/

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

            return twoHandAttackBonus;
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

            int twoHandAttackBonus = ApplyWeaponBuffs(null, weaponDamage);

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
