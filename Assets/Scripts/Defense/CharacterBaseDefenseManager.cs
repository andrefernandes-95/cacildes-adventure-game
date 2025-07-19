using System.Collections.Generic;
using System.Linq;
using AF.Health;
using AF.Stats;
using UnityEngine;

namespace AF
{

    public abstract class CharacterBaseDefenseManager : MonoBehaviour
    {
        [Header("Negated Damage")]
        public Damage damagedAbsorbed = new();

        [Header("Damage Type Absorptions")]
        public float pierceDamageAbsorption = 1f;
        public float slashDamageAbsorption = 1f;
        public float bluntDamageAbsorption = 1f;

        [Header("Components")]
        public CharacterBaseManager character;

        // Call this whenever equipment is changed or stats are updated
        public void RecalculateDamageAbsorbed()
        {
            int vitality = character.characterBaseStats.GetVitality();
            int strength = character.characterBaseStats.GetStrength();
            int endurance = character.characterBaseStats.GetEndurance();

            damagedAbsorbed.physical = GetPhysicalDamageAbsorption(vitality, endurance, strength);
            damagedAbsorbed.magic = GetMagicDamageAbsorption();
            damagedAbsorbed.fire = GetFireDamageAbsorption();
            damagedAbsorbed.frost = GetFrostDamageAbsorption();
            damagedAbsorbed.water = GetWaterDamageAbsorption();
            damagedAbsorbed.darkness = GetDarknessDamageAbsorption();
            damagedAbsorbed.lightning = GetLightningDamageAbsorption();

            //TODO: Poise, Posture, Status Effects, etc.
        }

        // Why do we pass the stats here? Because we also call this function in Level Up screen 
        public int GetPhysicalDamageAbsorption(int vitality, int endurance, int strength)
        {
            int defense = 0;

            // Defense from stats
            defense += DefenseUtils.GetPhysicalDefenseFromEndurance(endurance);
            defense += DefenseUtils.GetPhysicalDefenseFromVitaly(vitality);
            defense += DefenseUtils.GetPhysicalDefenseFromStrength(strength);

            // Defense from equipment
            defense += character.statsBonusController.equipmentPhysicalDefenseBonus;

            // TODO: Lowered damage bonus from status effect like weaken enemy to attacks

            return defense;
        }

        public int GetMagicDamageAbsorption()
        {
            return GetElementalDefense(character.statsBonusController.equipmentMagicDefenseBonus);
        }

        public int GetFireDamageAbsorption()
        {
            return GetElementalDefense(character.statsBonusController.equipmentMagicDefenseBonus);
        }

        public int GetFrostDamageAbsorption()
        {
            return GetElementalDefense(character.statsBonusController.equipmentMagicDefenseBonus);
        }

        public int GetWaterDamageAbsorption()
        {
            return GetElementalDefense(character.statsBonusController.equipmentMagicDefenseBonus);
        }

        int GetElementalDefense(int defenseFromEquipment)
        {
            int defense = 0;

            // Defense from stats
            defense += DefenseUtils.GetElementalDefenseFromIntelligence(character.characterBaseStats.GetIntelligence());

            defense += defenseFromEquipment;

            return defense;
        }

        int GetLightningDamageAbsorption()
        {
            int defense = 0;
            int defenseFromEquipment = character.statsBonusController.equipmentLightningDefenseBonus;

            // If the character has a reputation, they don't get a bonus to their defense from their stats, only from their equipment
            int reputation = character.characterBaseStats.GetReputation();
            if (reputation >= 0)
            {
                return defenseFromEquipment;
            }

            // If is evil character, makes sense to get lightning damage because of negative reputation
            reputation = Mathf.Abs(character.characterBaseStats.GetReputation());

            // Defense from stats
            defense += DefenseUtils.GetElementalDefenseFromReputation(reputation);
            defense += defenseFromEquipment;

            return defense;
        }

        int GetDarknessDamageAbsorption()
        {
            int defense = 0;
            int defenseFromEquipment = character.statsBonusController.equipmentDarkDefenseBonus;

            // If the character has negative reputation, they don't get a bonus to their defense from their stats, only from their equipment
            int reputation = character.characterBaseStats.GetReputation();
            if (reputation < 0)
            {
                return defenseFromEquipment;
            }

            // If is good / neutral character, makes sense to get darkness damage defense because of positive reputation
            reputation = character.characterBaseStats.GetReputation();

            // Defense from stats
            defense += DefenseUtils.GetElementalDefenseFromReputation(reputation);
            defense += defenseFromEquipment;

            return defense;
        }

        public int CompareHelmet(Helmet helmet)
        {
            int currentHelmetDamage = 0;

            Helmet equippedHelmet = character.characterBaseEquipment.GetEquippedHelmet();
            if (equippedHelmet != null)
            {
                currentHelmetDamage = equippedHelmet.damageAbsorbed.GetTotalDamage();
            }

            int newHelmetDamage = 0;
            if (helmet != null)
            {
                newHelmetDamage = helmet.damageAbsorbed.GetTotalDamage();
            }

            return CompareValues(currentHelmetDamage, newHelmetDamage);
        }

        public int CompareArmor(Armor armor)
        {
            int currentDamage = 0;

            Armor equippedArmor = character.characterBaseEquipment.GetEquippedArmor();
            if (equippedArmor != null)
            {
                currentDamage = equippedArmor.damageAbsorbed.GetTotalDamage();
            }

            int itemDamage = 0;
            if (armor != null)
            {
                itemDamage = armor.damageAbsorbed.GetTotalDamage();
            }

            return CompareValues(currentDamage, itemDamage);
        }

        public int CompareGauntlets(Gauntlet gauntlet)
        {
            int currentDamage = 0;

            Gauntlet equippedGauntlet = character.characterBaseEquipment.GetEquippedGauntlet();
            if (equippedGauntlet != null)
            {
                currentDamage = equippedGauntlet.damageAbsorbed.GetTotalDamage();
            }

            int itemDamage = 0;
            if (gauntlet != null)
            {
                itemDamage = gauntlet.damageAbsorbed.GetTotalDamage();
            }

            return CompareValues(currentDamage, itemDamage);
        }


        public int CompareLegwears(Legwear legwear)
        {
            int currentDamage = 0;

            Legwear equippedLegwear = character.characterBaseEquipment.GetEquippedLegwear();
            if (equippedLegwear != null)
            {
                currentDamage = equippedLegwear.damageAbsorbed.GetTotalDamage();
            }

            int itemDamage = 0;
            if (legwear != null)
            {
                itemDamage = legwear.damageAbsorbed.GetTotalDamage();
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
                incomingDamage.physical -= Mathf.Max(1, damagedAbsorbed.physical);

                // Apply weapon type multiplier after flat reduction
                switch (incomingDamage.weaponAttackType)
                {
                    case WeaponAttackType.Slash:
                        incomingDamage.physical = Mathf.Max(1, (int)(incomingDamage.physical
                            * (character.combatant != null ? character.combatant.slashAbsorption : slashDamageAbsorption)));
                        break;
                    case WeaponAttackType.Blunt:
                        incomingDamage.physical = Mathf.Max(1, (int)(incomingDamage.physical
                            * (character.combatant != null ? character.combatant.bluntAbsorption : bluntDamageAbsorption)));
                        break;
                    case WeaponAttackType.Pierce:
                        incomingDamage.physical = Mathf.Max(1, (int)(incomingDamage.physical
                            * (character.combatant != null ? character.combatant.pierceAbsorption : pierceDamageAbsorption)));
                        break;
                }
            }

            if (incomingDamage.fire > 0)
            {
                incomingDamage.fire -= Mathf.Max(0, damagedAbsorbed.fire);

                if (character.combatant != null)
                {
                    incomingDamage.fire = (int)(incomingDamage.fire * character.combatant.fireAbsorption * character.combatant.fireBonus);
                }
            }

            if (incomingDamage.frost > 0)
            {
                incomingDamage.frost -= Mathf.Max(0, damagedAbsorbed.frost);

                if (character.combatant != null)
                {
                    incomingDamage.frost = (int)(incomingDamage.frost * character.combatant.frostAbsorption * character.combatant.frostBonus);
                }
            }

            if (incomingDamage.water > 0)
            {
                incomingDamage.water -= Mathf.Max(0, damagedAbsorbed.water);

                if (character.combatant != null)
                {
                    incomingDamage.water = (int)(incomingDamage.water * character.combatant.waterAbsorption * character.combatant.waterBonus);
                }
            }

            if (incomingDamage.darkness > 0)
            {
                incomingDamage.darkness -= Mathf.Max(0, damagedAbsorbed.darkness);

                if (character.combatant != null)
                {
                    incomingDamage.darkness = (int)(incomingDamage.darkness * character.combatant.darknessAbsorption * character.combatant.darknessBonus);
                }
            }

            if (incomingDamage.lightning > 0)
            {
                incomingDamage.lightning -= Mathf.Max(0, damagedAbsorbed.lightning);

                if (character.combatant != null)
                {
                    incomingDamage.lightning = (int)(incomingDamage.lightning * character.combatant.lightningAbsorption * character.combatant.lightningBonus);
                }
            }

            if (incomingDamage.magic > 0)
            {
                incomingDamage.magic -= Mathf.Max(0, damagedAbsorbed.magic);

                if (character.combatant != null)
                {
                    incomingDamage.magic = (int)(incomingDamage.magic * character.combatant.magicAbsorption * character.combatant.magicBonus);
                }
            }

            if (incomingDamage.postureDamage > 0)
            {
                incomingDamage.postureDamage -= Mathf.Max(1, damagedAbsorbed.postureDamage);
            }

            if (incomingDamage.poiseDamage > 0)
            {
                incomingDamage.poiseDamage -= Mathf.Max(1, damagedAbsorbed.poiseDamage);
            }

            if (incomingDamage.pushForce > 0)
            {
                incomingDamage.pushForce -= damagedAbsorbed.pushForce;
                incomingDamage.pushForce = Mathf.Max(0, incomingDamage.pushForce);
            }

            if (incomingDamage.statusEffects != null && incomingDamage.statusEffects.Length > 0)
            {
                List<StatusEffectEntry> filteredEffects = new();

                foreach (var effectEntry in incomingDamage.statusEffects)
                {
                    StatusEffectEntry match = damagedAbsorbed.statusEffects.FirstOrDefault(
                        entry => entry.statusEffect == effectEntry.statusEffect);

                    if (match == null)
                    {
                        // Do not apply filter to this effect since we do not absorb it
                        filteredEffects.Add(effectEntry);
                        continue;
                    }

                    float finalAmount = Mathf.Max(1, effectEntry.amountPerHit - match.amountPerHit);

                    /* // TODO: Add Status Effect classes for each status effect so we can easily check the resistance here
                    if (character.combatant != null)
                    {
                        incomingDamage.magic = (int)(incomingDamage.magic * character.combatant.poisonResistance);
                    } */

                    filteredEffects.Add(new StatusEffectEntry
                    {
                        statusEffect = effectEntry.statusEffect,
                        amountPerHit = finalAmount
                    });
                }

                incomingDamage.statusEffects = filteredEffects.ToArray();
            }
        }
    }
}
