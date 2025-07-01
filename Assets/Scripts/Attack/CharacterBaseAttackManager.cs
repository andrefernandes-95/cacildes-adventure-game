namespace AF
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;
    using AF.Health;

    public abstract class CharacterBaseAttackManager : MonoBehaviour
    {
        // RECOMMENDED ATTACK FORMULA:
        // STR LEVEL * levelMultiplier * weaponScaling
        // A weapon that has S scaling with a levelmultiplier of 3.25 produces:
        // 1 * 3.25 * 2.4 = 8
        // 4 * 3.25 * 2.4 = 31
        // 8 * 3.25 * 2.4 = 62
        // 16 * 3.25 * 2.4 = 125
        // This gives good values, similar to Dark Souls

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

        [Header("Databases")]
        public PlayerStatsDatabase playerStatsDatabase;
        public EquipmentDatabase equipmentDatabase;

        [Header("Current Damage")]
        public Damage rightWeaponCurrentDamage;
        public Damage leftWeaponCurrentDamage;
        bool isAttackingWithLeftHand = false;

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
            return equipmentDatabase.IsRangeWeaponEquipped();
        }

        public Damage GetAttackDamage()
        {
            if (isAttackingWithLeftHand)
            {
                return leftWeaponCurrentDamage;
            }

            return rightWeaponCurrentDamage;
        }

        public void CalculateRightWeaponDamage(Weapon weapon)
        {
            if (weapon == null)
            {
                CalculateUnarmedDamage(true);
            }
            else
            {
                CalculateWeaponDamageForWeapon(weapon, true);
            }

            /*
            if (weapon != null)
            {
                Damage weaponDamage = new(
                    physical: GetWeaponAttack(weapon),
                    fire: (int)weapon.GetWeaponFireAttack(GetCharacter().characterBaseAttackManager),
                    frost: (int)weapon.GetWeaponFrostAttack(GetCharacter().characterBaseAttackManager),
                    magic: (int)weapon.GetWeaponMagicAttack(GetCharacter().characterBaseAttackManager),
                    lightning: (int)weapon.GetWeaponLightningAttack(GetCharacter().playerStatsDatabase.GetCurrentReputation(), GetCharacter().characterBaseAttackManager),
                    darkness: (int)weapon.GetWeaponDarknessAttack(GetCharacter().playerStatsDatabase.GetCurrentReputation(), GetCharacter().characterBaseAttackManager),
                    water: (int)weapon.GetWeaponWaterAttack(GetCharacter().characterBaseAttackManager),
                    postureDamage: (IsHeavyAttacking() || IsJumpAttacking())
                    ? (int)(weapon.damage.postureDamage * 1.1f)
                    : weapon.damage.postureDamage,
                    poiseDamage: weapon.damage.poiseDamage,
                    weaponAttackType: weapon.damage.weaponAttackType,
                    statusEffects: weapon.damage.statusEffects,
                    pushForce: weapon.damage.pushForce,
                    canNotBeParried: weapon.damage.canNotBeParried,
                    ignoreBlocking: weapon.damage.ignoreBlocking
                );

                if (rageBonus > 0)
                {
                    weaponDamage.damageType = DamageType.ENRAGED;
                }

                if (!weapon.AreRequirementsMet(GetCharacter()))
                {
                    weaponDamage.Multiply(.1f);
                }

                if (damageBonus != null)
                {
                    weaponDamage.Combine(damageBonus);
                    damageBonus = null;
                }

                return GetCharacter().playerWeaponsManager.GetBuffedDamage(weaponDamage); */
        }

        void CalculateUnarmedDamage(bool isRightHand)
        {
            if (isRightHand)
            {
                if (unarmedRightWeapon == null)
                {
                    rightWeaponCurrentDamage = new Damage();
                }
                else
                {
                    rightWeaponCurrentDamage = unarmedRightWeapon.damage;
                }
            }
            else
            {
                if (unarmedLeftWeapon == null)
                {
                    leftWeaponCurrentDamage = new Damage();
                }
                else
                {
                    leftWeaponCurrentDamage = unarmedLeftWeapon.damage;
                }
            }
        }

        void CalculateWeaponDamageForWeapon(Weapon weapon, bool isRightHand)
        {
            Damage weaponDamage = new();
            weaponDamage.physical =
                weapon.GetCurrentPhysicalAttackForLevel(weapon.level);

            if (isRightHand)
            {
            }
        }

        public int GetCurrentAttackForWeapon(Weapon weapon)
        {
            return weapon != null
                ? GetWeaponAttack(weapon)
                : GetCurrentPhysicalAttack();
        }

        int GetUnarmedPhysicalDamage()
        {
            int attackValue = GetCurrentPhysicalAttack();

            if (IsInAir() || IsJumpAttacking())
            {
                attackValue = Mathf.FloorToInt(attackValue * jumpAttackMultiplier);

                var jumpAttackBonuses = equipmentDatabase.accessories.Sum(x => x != null ? x.jumpAttackBonus : 0);
                attackValue += jumpAttackBonuses;
            }

            return GetAttackBuffs(attackValue);
        }

        int GetCurrentPhysicalAttack()
        {
            int heavyAttackBonus = 0;

            if (equipmentDatabase.GetCurrentWeapon() == null && IsHeavyAttacking())
            {
                heavyAttackBonus += heavyAttackBonusDamage;
            }

            var value = basePhysicalAttack;

            return (int)Mathf.Round(
                Mathf.Ceil(
                    value
                        + (playerStatsDatabase.strength * Formulas.levelMultiplier)
                        + (playerStatsDatabase.dexterity * Formulas.levelMultiplier)
                        + (GetCharacter().statsBonusController.strengthBonus * Formulas.levelMultiplier)
                        + (GetCharacter().statsBonusController.dexterityBonus * Formulas.levelMultiplier)
                    ) + physicalAttackBonus + heavyAttackBonus
                );
        }


        public int GetCurrentPhysicalAttackForGivenStrengthAndDexterity(int strength, int dexterity)
        {
            return (int)Mathf.Round(
                Mathf.Ceil(
                    basePhysicalAttack
                        + (strength * Formulas.levelMultiplier)
                        + (dexterity * Formulas.levelMultiplier)
                    )
                );
        }


        public int CompareWeapon(Weapon weaponToCompare)
        {
            if (equipmentDatabase.GetCurrentWeapon() == null)
            {
                return 1;
            }

            var weaponToCompareAttack = GetWeaponAttack(weaponToCompare);
            var currentWeaponAttack = GetWeaponAttack(equipmentDatabase.GetCurrentWeapon());

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

        int GetWeaponBaseDamage(Weapon weapon)
        {
            int playerBaseAttackValue = GetCurrentPhysicalAttack();

            if (weapon.damage.weaponAttackType == WeaponAttackType.Range)
            {
                playerBaseAttackValue = 0;
            }
            else if (weapon.damage.physical <= 0)
            {
                playerBaseAttackValue = 0;
            }

            playerBaseAttackValue += weapon.GetWeaponAttack(this);

            return playerBaseAttackValue;
        }

        public int GetTwoHandAttackBonus(Weapon weapon)
        {
            int weaponDamage = GetWeaponBaseDamage(weapon);

            if (HasRangeWeaponEquipped())
            {
                return 0;
            }

            int enhancedWeaponDamage = (int)(weaponDamage * twoHandAttackBonusMultiplier);
            return Mathf.Max(enhancedWeaponDamage - weaponDamage, 0);
        }

        public int GetWeaponAttack(Weapon weapon)
        {
            int value = GetWeaponBaseDamage(weapon);

            if (equipmentDatabase.isTwoHanding)
            {
                value += GetTwoHandAttackBonus(weapon);
            }

            if (IsHeavyAttacking())
            {
                value = (int)(value * heavyAttackBonusMultiplier);
            }

            if (IsInAir() || IsJumpAttacking())
            {
                value = Mathf.FloorToInt(value * jumpAttackMultiplier);

                var jumpAttackBonuses = equipmentDatabase.accessories.Sum(x => x != null ? x.jumpAttackBonus : 0);
                value += jumpAttackBonuses;
            }

            if (weapon.halveDamage && equipmentDatabase.isTwoHanding)
            {
                return (int)(value / 2);
            }

            return GetAttackBuffs(value);
        }

        public int GetAttackBuffs(int value)
        {
            // + Attack the lower the rep
            if (equipmentDatabase.accessories.FirstOrDefault(x => x != null && x.increaseAttackPowerTheLowerTheReputation) != null)
            {
                if (playerStatsDatabase.GetCurrentReputation() < 0)
                {
                    int extraAttackPower = Mathf.Min(150, (int)(Mathf.Abs(playerStatsDatabase.GetCurrentReputation()) * 2.25f));

                    value += extraAttackPower;
                }
            }

            // + Attack the lower the health
            if (equipmentDatabase.accessories.FirstOrDefault(x => x != null && x.increaseAttackPowerWithLowerHealth) != null)
            {
                int extraAttackPower = (int)(value * (GetCharacter().health as PlayerHealth).GetExtraAttackBasedOnCurrentHealth());

                value += extraAttackPower;
            }

            // Generic attack bonuses
            var attackBonuses = equipmentDatabase.accessories.Sum(x => x != null ? x.physicalAttackBonus : 0);
            value += attackBonuses;

            // Bonus for guard counters and parry attacks
            if (GetCharacter().characterBlockController.IsWithinCounterAttackWindow())
            {
                value = (int)(value * GetCharacter().characterBlockController.counterAttackMultiplier);
            }

            float attackMultiplierBonuses = 1;

            // Bonus for two handing attack accessories
            if (equipmentDatabase.isTwoHanding)
            {
                attackMultiplierBonuses += GetCharacter().statsBonusController.twoHandAttackBonusMultiplier;
            }

            Weapon currentWeapon = equipmentDatabase.GetCurrentWeapon();

            if (currentWeapon == null)
            {
                if (GetCharacter().statsBonusController.increaseAttackPowerWhenUnarmed)
                {
                    attackMultiplierBonuses *= 1.65f;
                }
            }
            else
            {
                if (currentWeapon.damage.weaponAttackType == WeaponAttackType.Pierce)
                {
                    attackMultiplierBonuses += GetCharacter().statsBonusController.pierceDamageMultiplier;
                }
                else if (currentWeapon.damage.weaponAttackType == WeaponAttackType.Slash)
                {
                    attackMultiplierBonuses += GetCharacter().statsBonusController.slashDamageMultiplier;
                }
                else if (currentWeapon.damage.weaponAttackType == WeaponAttackType.Blunt)
                {
                    attackMultiplierBonuses += GetCharacter().statsBonusController.bluntDamageMultiplier;
                }
            }

            /*
            if (GetCharacter().playerCombatController.isAttackingWithFoot)
            {
                value = (int)(value * (footDamageMultiplier + GetCharacter().statsBonusController.footDamageMultiplier));
            } */

            value = (int)(value * attackMultiplierBonuses);

            return value;
        }


        public int GetStrengthBonusFromWeapon(Weapon weapon)
        {
            if (weapon.damage.physical <= 0)
            {
                return 0;
            }

            return Formulas.GetBonusFromWeapon(
                GetCharacter().characterBaseStats.GetStrength(),
                 scalingDictionary[weapon.strengthScaling.ToString()]
            );
        }

        public float GetDexterityBonusFromWeapon(Weapon weapon)
        {
            if (weapon.damage.physical <= 0)
            {
                return 0;
            }

            return Formulas.GetBonusFromWeapon(
                GetCharacter().characterBaseStats.GetDexterity(),
                 scalingDictionary[weapon.dexterityScaling.ToString()]
            );
        }

        public float GetIntelligenceBonusFromWeapon(Weapon weapon)
        {
            return Formulas.GetBonusFromWeapon(
                GetCharacter().characterBaseStats.GetIntelligence(),
                 scalingDictionary[weapon.intelligenceScaling.ToString()]
            );
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

        Damage GetNextAttackBonusDamage(Damage damage)
        {
            if (increaseNextAttackDamage)
            {
                increaseNextAttackDamage = false;
                SetBuffDamageFXLoop(false);

                damage.physical = (int)(damage.physical * nextAttackMultiplierFactor);

                if (damage.fire > 0)
                {
                    damage.fire = (int)(damage.fire * nextAttackMultiplierFactor);
                }
                if (damage.frost > 0)
                {
                    damage.frost = (int)(damage.frost * nextAttackMultiplierFactor);
                }
                if (damage.lightning > 0)
                {
                    damage.lightning = (int)(damage.lightning * nextAttackMultiplierFactor);
                }
                if (damage.magic > 0)
                {
                    damage.magic = (int)(damage.magic * nextAttackMultiplierFactor);
                }
                if (damage.darkness > 0)
                {
                    damage.darkness = (int)(damage.darkness * nextAttackMultiplierFactor);
                }
                if (damage.water > 0)
                {
                    damage.water = (int)(damage.water * nextAttackMultiplierFactor);
                }
            }

            return damage;
        }

        public void SetIsAttackingWithLeftHand(bool isAttackingWithLeftHand)
        {
            this.isAttackingWithLeftHand = isAttackingWithLeftHand;
        }

        public abstract CharacterBaseManager GetCharacter();

        public int GetStrengthBonusForScale(Scaling scale)
        {
            return (int)(GetCharacter().characterBaseStats.GetStrength() * scalingDictionary[scale.ToString()]);
        }

        public int GetDexterityBonusForScale(Scaling scale)
        {
            return (int)(GetCharacter().characterBaseStats.GetDexterity() * scalingDictionary[scale.ToString()]);
        }

        public int GetIntelligenceBonusForScale(Scaling scale)
        {
            return (int)(GetCharacter().characterBaseStats.GetDexterity() * scalingDictionary[scale.ToString()]);
        }
    }
}
