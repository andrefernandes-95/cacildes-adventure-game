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

        [Header("Status attack bonus")]
        [Tooltip("Increased by buffs like potions, or equipment like accessories")]
        public float physicalAttackBonus = 0f;
        [SerializeField] int heavyAttackBonusDamage = 50;

        [Header("Unarmed Attack Options")]
        public int unarmedLightAttackPostureDamage = 18;
        public int unarmedPostureDamageBonus = 10;

        [Header("Physical Attack")]
        public int basePhysicalAttack = 100;

        public float jumpAttackMultiplier = 1.25f;
        public float twoHandAttackBonusMultiplier = 1.25f;
        public float heavyAttackBonusMultiplier = 1.35f;
        public float footDamageMultiplier = 1.2f;

        [Header("Buff Bonuses")]
        public ParticleSystem increaseNextAttackDamageFX;
        bool increaseNextAttackDamage = false;
        readonly float nextAttackMultiplierFactor = 1.3f;

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

        StatusEffectEntry[] unarmedStatusEffects = new List<StatusEffectEntry>().ToArray();


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
            Damage clonedDamage = null;

            if (attackingHitboxType == HitboxType.LEFT_HAND)
            {
                clonedDamage = leftWeaponCurrentDamage.Clone();

                if (damageBonus != null)
                {
                    clonedDamage.Combine(damageBonus);
                }

                return clonedDamage;
            }

            clonedDamage = rightWeaponCurrentDamage.Clone();

            if (damageBonus != null)
            {
                clonedDamage.Combine(damageBonus);
            }

            return clonedDamage;
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
            Damage weaponDamage = GetScaledDamageForStats(
                weapon.damage.Clone(),
                GetCharacter().characterBaseStats.GetStrength(),
                GetCharacter().characterBaseStats.GetDexterity()
                );

            int STRBonus = weaponDamage.GetStrengthBonus(GetCharacter());
            int DEXBonus = weaponDamage.GetDexterityBonus(GetCharacter());
            int INTBonus = weaponDamage.GetIntelligenceBonus(GetCharacter());

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

            int twoHandAttackBonus = ApplyWeaponBuffs(weapon, weaponDamage);

            // If character doesn't meet the requirements
            if (!DoesCharacterMeetWeaponRequirements(weapon))
            {
                weaponDamage.Multiply(.1f);
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

                twoHandAttackBonus = (int)(
                    weaponDamage.physical * twoHandMultiplier - weaponDamage.physical);

                weaponDamage.physical += twoHandAttackBonus;
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
            Damage weaponDamage = GetScaledDamageForStats(
                unarmedDamage.Clone(),
                GetCharacter().characterBaseStats.GetStrength(),
                GetCharacter().characterBaseStats.GetDexterity()
            );

            int STRBonus = weaponDamage.GetStrengthBonus(GetCharacter());
            int DEXBonus = weaponDamage.GetDexterityBonus(GetCharacter());
            int INTBonus = weaponDamage.GetIntelligenceBonus(GetCharacter());

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

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="value"></param>
        public void SetBonusPhysicalAttack(int value)
        {
            physicalAttackBonus = value;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void ResetBonusPhysicalAttack()
        {
            physicalAttackBonus = 0f;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="value"></param>
        public void SetIncreaseNextAttackDamage(bool value)
        {
            increaseNextAttackDamage = value;
            SetBuffDamageFXLoop(value);
        }

        void SetBuffDamageFXLoop(bool isLooping)
        {
            var main = increaseNextAttackDamageFX.main;
            main.loop = isLooping;
        }

        public abstract CharacterBaseManager GetCharacter();

        public abstract bool DoesCharacterMeetWeaponRequirements(Weapon weapon);

        /// <summary>
        /// Scales the given raw damage with the bonus from the character's strength and dexterity levels
        /// </summary>
        public Damage GetScaledDamageForStats(Damage unarmedDamageWeapon, int currentStrength, int currentDexterity)
        {
            int bonusFromStrength = GetBonusAttackPerLevel(currentStrength);
            int bonusFromDexterity = GetBonusAttackPerLevel(currentDexterity) / 2;

            unarmedDamageWeapon.physical += bonusFromStrength + bonusFromDexterity;

            return unarmedDamageWeapon;
        }

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

        int GetBonusAttackPerLevel(int level)
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
