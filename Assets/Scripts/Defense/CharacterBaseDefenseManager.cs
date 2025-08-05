using System.Collections.Generic;
using System.Linq;
using AF.Health;
using UnityEngine;

namespace AF
{

    public abstract class CharacterBaseDefenseManager : MonoBehaviour
    {
        [Header("Negated Damage")]
        [SerializeField] private Damage _baseDamagedAbsorbed = new();
        /// <summary>
        /// The damage absorption from the character, calculated from his / her stats
        /// </summary>
        public Damage BaseDamageAbsorbed => _baseDamagedAbsorbed;

        [SerializeField] private Damage _damagedAbsorbedFromEquipment = new();
        /// <summary>
        /// The damage absorption from currently equipped items
        /// </summary>
        public Damage DamageAbsorbedFromEquipment => _damagedAbsorbedFromEquipment;

        [SerializeField] private Damage _damageAbsorbed = new();

        /// <summary>
        /// The total current damage absorption
        /// </summary>
        public Damage CurrentDamageAbsorbed => _damageAbsorbed;

        [Header("Components")]
        public CharacterBaseManager character;

        // Call this whenever equipment is changed or stats are updated
        public void RecalculateDamageAbsorbed()
        {
            // Already includes bonuses from equipped items regarding the stats
            int vitality = character.characterBaseStats.GetVitality();
            int strength = character.characterBaseStats.GetStrength();
            int endurance = character.characterBaseStats.GetEndurance();

            _baseDamagedAbsorbed = new Damage
            {
                physical = GetPhysicalDamageAbsorption(vitality, endurance, strength),
                magic = GetElementalDefense(),
                fire = GetElementalDefense(),
                frost = GetElementalDefense(),
                water = GetElementalDefense(),
                darkness = GetDarknessDamageAbsorption(),
                lightning = GetLightningDamageAbsorption(),
                poiseDamage = 0,
                postureDamage = 0
            };

            _damagedAbsorbedFromEquipment = new Damage
            {
                physical = character.statsBonusController.equipmentPhysicalDefenseBonus,
                fire = character.statsBonusController.equipmentFireDefenseBonus,
                frost = character.statsBonusController.equipmentFrostDefenseBonus,
                lightning = character.statsBonusController.equipmentLightningDefenseBonus,
                magic = character.statsBonusController.equipmentMagicDefenseBonus,
                water = character.statsBonusController.equipmentWaterDefenseBonus,
                darkness = character.statsBonusController.equipmentDarkDefenseBonus,
                poiseDamage = character.statsBonusController.equipmentPoise,
                postureDamage = character.statsBonusController.postureBonus
            };

            _damageAbsorbed = new Damage();
            _damageAbsorbed.Combine(BaseDamageAbsorbed);
            _damageAbsorbed.Combine(DamageAbsorbedFromEquipment);
        }

        // Why do we pass the stats here? Because we also call this function in Level Up screen 
        public int GetPhysicalDamageAbsorption(int vitality, int endurance, int strength)
        {
            int defense = 0;

            // Defense from stats
            defense += DefenseUtils.GetPhysicalDefenseFromEndurance(endurance);
            defense += DefenseUtils.GetPhysicalDefenseFromVitaly(vitality);
            defense += DefenseUtils.GetPhysicalDefenseFromStrength(strength);

            return defense;
        }

        int GetLightningDamageAbsorption()
        {
            int defense = 0;

            // If the character has a reputation, they don't get a bonus to their defense from their stats, only from their equipment
            int reputation = character.characterBaseStats.GetReputation();

            // If is evil character, makes sense to get lightning damage because of negative reputation
            reputation = Mathf.Abs(character.characterBaseStats.GetReputation());

            // Defense from stats
            defense += DefenseUtils.GetElementalDefenseFromReputation(reputation);

            return defense;
        }

        int GetDarknessDamageAbsorption()
        {
            int defense = 0;

            // If the character has negative reputation, they don't get a bonus to their defense from their stats, only from their equipment
            int reputation = character.characterBaseStats.GetReputation();

            // Defense from stats
            defense += DefenseUtils.GetElementalDefenseFromReputation(reputation);

            return defense;
        }

        int GetElementalDefense()
        {
            int defense = 0;

            // Defense from stats
            defense += DefenseUtils.GetElementalDefenseFromIntelligence(character.characterBaseStats.GetIntelligence());

            return defense;
        }

        public int CompareHelmet(Helmet helmet)
        {
            int currentHelmetDamage = 0;

            Helmet equippedHelmet = character.characterBaseEquipment.GetEquippedHelmet();
            if (equippedHelmet != null)
            {
                currentHelmetDamage = equippedHelmet.GetDamageAbsorbedForCurrentLevel().GetTotalDamage();
            }

            int newHelmetDamage = 0;
            if (helmet != null)
            {
                newHelmetDamage = helmet.GetDamageAbsorbedForCurrentLevel().GetTotalDamage();
            }

            return CompareValues(currentHelmetDamage, newHelmetDamage);
        }

        public int CompareArmor(Armor armor)
        {
            int currentDamage = 0;

            Armor equippedArmor = character.characterBaseEquipment.GetEquippedArmor();
            if (equippedArmor != null)
            {
                currentDamage = equippedArmor.GetDamageAbsorbedForCurrentLevel().GetTotalDamage();
            }

            int itemDamage = 0;
            if (armor != null)
            {
                itemDamage = armor.GetDamageAbsorbedForCurrentLevel().GetTotalDamage();
            }

            return CompareValues(currentDamage, itemDamage);
        }

        public int CompareGauntlets(Gauntlet gauntlet)
        {
            int currentDamage = 0;

            Gauntlet equippedGauntlet = character.characterBaseEquipment.GetEquippedGauntlet();
            if (equippedGauntlet != null)
            {
                currentDamage = equippedGauntlet.GetDamageAbsorbedForCurrentLevel().GetTotalDamage();
            }

            int itemDamage = 0;
            if (gauntlet != null)
            {
                itemDamage = gauntlet.GetDamageAbsorbedForCurrentLevel().GetTotalDamage();
            }

            return CompareValues(currentDamage, itemDamage);
        }


        public int CompareLegwears(Legwear legwear)
        {
            int currentDamage = 0;

            Legwear equippedLegwear = character.characterBaseEquipment.GetEquippedLegwear();
            if (equippedLegwear != null)
            {
                currentDamage = equippedLegwear.GetDamageAbsorbedForCurrentLevel().GetTotalDamage();
            }

            int itemDamage = 0;
            if (legwear != null)
            {
                itemDamage = legwear.GetDamageAbsorbedForCurrentLevel().GetTotalDamage();
            }

            return CompareValues(currentDamage, itemDamage);
        }

        int CompareValues(int current, int next)
        {

            (bool isBetter, bool isWorse, bool isEqual) = DefenseUtils.CompareDamageNegation(current, next);

            if (isBetter)
            {
                return 1;
            }
            else if (isWorse)
            {
                return -1;
            }

            // Is Equal
            return 0;
        }

        public void FilterIncomingDamage(Damage incomingDamage)
        {
            if (incomingDamage.physical > 0)
            {
                incomingDamage.physical -= Mathf.Max(1, CurrentDamageAbsorbed.physical);

                // Apply weapon type multiplier after flat reduction
                switch (incomingDamage.weaponAttackType)
                {
                    case WeaponAttackType.Slash:
                        incomingDamage.physical = Mathf.Max(1, (int)(incomingDamage.physical
                            * (character.combatant != null ? character.combatant.slashAbsorption : 1f)));
                        break;
                    case WeaponAttackType.Blunt:
                        incomingDamage.physical = Mathf.Max(1, (int)(incomingDamage.physical
                            * (character.combatant != null ? character.combatant.bluntAbsorption : 1f)));
                        break;
                    case WeaponAttackType.Pierce:
                        incomingDamage.physical = Mathf.Max(1, (int)(incomingDamage.physical
                            * (character.combatant != null ? character.combatant.pierceAbsorption : 1f)));
                        break;
                }
            }

            if (incomingDamage.fire > 0)
            {
                incomingDamage.fire -= Mathf.Max(0, CurrentDamageAbsorbed.fire);

                if (character.combatant != null)
                {
                    incomingDamage.fire = (int)(incomingDamage.fire * character.combatant.fireAbsorption * character.combatant.fireBonus);
                }
            }

            if (incomingDamage.frost > 0)
            {
                incomingDamage.frost -= Mathf.Max(0, CurrentDamageAbsorbed.frost);

                if (character.combatant != null)
                {
                    incomingDamage.frost = (int)(incomingDamage.frost * character.combatant.frostAbsorption * character.combatant.frostBonus);
                }
            }

            if (incomingDamage.water > 0)
            {
                incomingDamage.water -= Mathf.Max(0, CurrentDamageAbsorbed.water);

                if (character.combatant != null)
                {
                    incomingDamage.water = (int)(incomingDamage.water * character.combatant.waterAbsorption * character.combatant.waterBonus);
                }
            }

            if (incomingDamage.darkness > 0)
            {
                incomingDamage.darkness -= Mathf.Max(0, CurrentDamageAbsorbed.darkness);

                if (character.combatant != null)
                {
                    incomingDamage.darkness = (int)(incomingDamage.darkness * character.combatant.darknessAbsorption * character.combatant.darknessBonus);
                }
            }

            if (incomingDamage.lightning > 0)
            {
                incomingDamage.lightning -= Mathf.Max(0, CurrentDamageAbsorbed.lightning);

                if (character.combatant != null)
                {
                    incomingDamage.lightning = (int)(incomingDamage.lightning * character.combatant.lightningAbsorption * character.combatant.lightningBonus);
                }
            }

            if (incomingDamage.magic > 0)
            {
                incomingDamage.magic -= Mathf.Max(0, CurrentDamageAbsorbed.magic);

                if (character.combatant != null)
                {
                    incomingDamage.magic = (int)(incomingDamage.magic * character.combatant.magicAbsorption * character.combatant.magicBonus);
                }
            }

            if (incomingDamage.postureDamage > 0)
            {
                incomingDamage.postureDamage = Mathf.Max(1, incomingDamage.postureDamage - CurrentDamageAbsorbed.postureDamage);
            }

            if (incomingDamage.poiseDamage > 0)
            {
                incomingDamage.poiseDamage = Mathf.Max(1, incomingDamage.poiseDamage - CurrentDamageAbsorbed.poiseDamage);
            }

            if (incomingDamage.pushForce > 0)
            {
                incomingDamage.pushForce = CurrentDamageAbsorbed.pushForce;
                incomingDamage.pushForce = Mathf.Max(1, incomingDamage.pushForce - incomingDamage.pushForce);
            }
        }

    }
}
