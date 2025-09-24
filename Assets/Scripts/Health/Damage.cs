using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace AF.Health
{

    [System.Serializable]
    public class StatusEffectEntry
    {
        public StatusEffect statusEffect;
        public float amountPerHit;
    }

    [System.Serializable]
    public enum DamageType
    {
        NORMAL,
        COUNTER_ATTACK,
        ENRAGED,
        CRITICAL_ATTACK,
        BACKSTAB
    }

    [System.Serializable]
    public class Damage
    {
        [HideInInspector] public int basePhysicalDamage;
        public int physical;
        public int fire;
        public int frost;
        public int magic;
        public int lightning;
        public int darkness;
        public int water;
        public int postureDamage;
        public int poiseDamage;
        public float pushForce = 0;

        public WeaponAttackType weaponAttackType;

        public StatusEffectEntry[] statusEffects;

        public bool ignoreBlocking = false;
        public bool canNotBeParried = false;
        public DamageType damageType = DamageType.NORMAL;

        [Header("Scaling")]
        public Scaling strengthScaling = Scaling.E;
        public Scaling dexterityScaling = Scaling.E;
        public Scaling intelligenceScaling = Scaling.E;

        public const float SCALING_LEVEL_MULTIPLIER = 3.25f;
        public const float S = 2.4f;
        public const float A = 2;
        public const float B = 1.6f;
        public const float C = 1.2f;
        public const float D = 0.8f;
        public const float E = 0.4f;

        public Damage()
        {
        }

        public Damage(
            int physical,
            int fire,
            int frost,
            int magic,
            int lightning,
            int darkness,
            int water,
            int postureDamage,
            int poiseDamage,
            WeaponAttackType weaponAttackType,
            StatusEffectEntry[] statusEffects,
            float pushForce,
            bool ignoreBlocking,
            bool canNotBeParried)
        {
            this.physical = physical;
            this.fire = fire;
            this.frost = frost;
            this.magic = magic;
            this.lightning = lightning;
            this.darkness = darkness;
            this.water = water;
            this.postureDamage = postureDamage;
            this.poiseDamage = poiseDamage;
            this.weaponAttackType = weaponAttackType;
            this.statusEffects = statusEffects;
            this.pushForce = pushForce;
            this.ignoreBlocking = ignoreBlocking;
            this.canNotBeParried = canNotBeParried;
        }

        public int GetTotalDamage()
        {
            return physical + fire + frost + magic + lightning + darkness + water;
        }


        public int GetStrengthBonus(CharacterBaseManager characterBaseManager)
        {
            if (this.physical <= 0)
            {
                return 0;
            }

            // Apply bonus damage based on scaling and stats
            int bonusFromSTR = ScalingUtils.GetBonusAttackPerLevel(characterBaseManager.characterBaseStats.GetStrength(), ScalingUtils.StatType.STRENGTH, this.strengthScaling);

            return bonusFromSTR;
        }

        public int GetDexterityBonus(CharacterBaseManager characterBaseManager)
        {
            if (this.physical <= 0)
            {
                return 0;
            }

            // Apply bonus damage based on scaling and stats
            int bonusFromDEX = ScalingUtils.GetBonusAttackPerLevel(characterBaseManager.characterBaseStats.GetDexterity(), ScalingUtils.StatType.DEXTERITY, this.dexterityScaling);

            return bonusFromDEX;
        }

        public int GetIntelligenceBonus(CharacterBaseManager characterBaseManager)
        {
            if (this.fire <= 0 && this.frost <= 0 && this.lightning <= 0 && this.magic <= 0 && this.darkness <= 0 && this.water <= 0)
            {
                return 0;
            }

            // Apply bonus damage based on scaling and stats
            int bonusFromINT = ScalingUtils.GetBonusAttackPerLevel(characterBaseManager.characterBaseStats.GetIntelligence(), ScalingUtils.StatType.INTELLIGENCE, this.intelligenceScaling);

            return bonusFromINT;
        }


        public void Multiply(float multiplier)
        {
            this.physical = (int)(this.physical * multiplier);
            this.fire = (int)(this.fire * multiplier);
            this.frost = (int)(this.frost * multiplier);
            this.magic = (int)(this.magic * multiplier);
            this.lightning = (int)(this.lightning * multiplier);
            this.darkness = (int)(this.darkness * multiplier);
            this.water = (int)(this.water * multiplier);
            this.poiseDamage = (int)(this.poiseDamage * multiplier);
            this.pushForce = (int)(this.pushForce * multiplier);
            this.postureDamage = (int)(this.postureDamage * multiplier);

            if (this.statusEffects != null)
            {
                List<StatusEffectEntry> newEffects = new();

                foreach (var otherEffect in this.statusEffects)
                {
                    newEffects.Add(new StatusEffectEntry
                    {
                        statusEffect = otherEffect.statusEffect,
                        amountPerHit = (int)(otherEffect.amountPerHit * multiplier)
                    });
                }

                this.statusEffects = newEffects.ToArray();
            }
        }

        public void ScaleProjectile(CharacterBaseAttackManager attackStatManager, Weapon currentWeapon)
        {
            if (currentWeapon == null)
            {
                return;
            }

            // Steel arrow might inherit magic from a magical bow, hence don't check if base values are greater than zero
            this.physical += (int)currentWeapon.GetWeaponAttack(attackStatManager);

            if (attackStatManager.GetCharacter().statsBonusController.projectileMultiplierBonus > 0f)
            {
                this.physical = (int)(this.physical * attackStatManager.GetCharacter().statsBonusController.projectileMultiplierBonus);
            }

            this.fire += (int)currentWeapon.GetWeaponFireAttack(attackStatManager);
            this.frost += (int)currentWeapon.GetWeaponFrostAttack(attackStatManager);
            this.magic += (int)currentWeapon.GetWeaponMagicAttack(attackStatManager);
            this.lightning += (int)currentWeapon.GetWeaponLightningAttack(attackStatManager.GetCharacter().characterBaseStats.GetReputation(), attackStatManager);
            this.darkness += (int)currentWeapon.GetWeaponDarknessAttack(attackStatManager.GetCharacter().characterBaseStats.GetReputation(), attackStatManager);
            this.water += (int)currentWeapon.GetWeaponWaterAttack(attackStatManager);
        }


        public Damage Clone()
        {
            return (Damage)this.MemberwiseClone();
        }

        public void ScaleDamageForNewGamePlus(GameSession gameSession)
        {
            this.physical = Utils.ScaleWithCurrentNewGameIteration(this.physical, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
            this.fire = Utils.ScaleWithCurrentNewGameIteration(this.fire, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
            this.frost = Utils.ScaleWithCurrentNewGameIteration(this.frost, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
            this.lightning = Utils.ScaleWithCurrentNewGameIteration(this.lightning, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
            this.magic = Utils.ScaleWithCurrentNewGameIteration(this.magic, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
            this.darkness = Utils.ScaleWithCurrentNewGameIteration(this.darkness, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
            this.water = Utils.ScaleWithCurrentNewGameIteration(this.water, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
            this.poiseDamage = Utils.ScaleWithCurrentNewGameIteration(this.poiseDamage, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
            this.postureDamage = Utils.ScaleWithCurrentNewGameIteration(this.postureDamage, gameSession.currentGameIteration, gameSession.newGamePlusScalingFactor);
        }

        public Damage Copy()
        {
            Damage newDamage = new()
            {
                physical = this.physical,
                fire = this.fire,
                frost = this.frost,
                lightning = this.lightning,
                magic = this.magic,
                darkness = this.darkness,
                water = this.water,
                canNotBeParried = this.canNotBeParried,
                damageType = this.damageType,
                ignoreBlocking = this.ignoreBlocking,
                poiseDamage = this.poiseDamage,
                postureDamage = this.postureDamage,
                pushForce = this.pushForce,
                weaponAttackType = this.weaponAttackType,
                statusEffects = this.statusEffects
            };

            return newDamage;
        }

        public Damage ScaleWithStats(int STR, int DEX, int INT)
        {
            Damage copy = this.Clone();

            // Apply bonus damage based on scaling and stats
            int bonusFromSTR = ScalingUtils.GetBonusAttackPerLevel(STR, ScalingUtils.StatType.STRENGTH, this.strengthScaling);

            int bonusFromDEX = ScalingUtils.GetBonusAttackPerLevel(DEX, ScalingUtils.StatType.DEXTERITY, this.dexterityScaling);

            int bonusFromINT = ScalingUtils.GetBonusAttackPerLevel(INT, ScalingUtils.StatType.INTELLIGENCE, this.intelligenceScaling);

            if (copy.physical > 0)
            {
                copy.physical += bonusFromSTR + bonusFromDEX;
            }
            if (copy.magic > 0)
            {
                copy.magic += bonusFromINT;
            }
            if (copy.fire > 0)
            {
                copy.fire += bonusFromINT;
            }
            if (copy.frost > 0)
            {
                copy.frost += bonusFromINT;
            }
            if (copy.lightning > 0)
            {
                copy.lightning += bonusFromINT;
            }
            if (copy.darkness > 0)
            {
                copy.darkness += bonusFromINT;
            }
            if (copy.water > 0)
            {
                copy.water += bonusFromINT;
            }

            return copy;
        }


        public void Combine(Damage other)
        {
            if (other == null) return;

            this.physical += other.physical;
            this.fire += other.fire;
            this.frost += other.frost;
            this.magic += other.magic;
            this.lightning += other.lightning;
            this.darkness += other.darkness;
            this.water += other.water;

            this.poiseDamage += other.poiseDamage;
            this.postureDamage += other.postureDamage;
            this.pushForce += other.pushForce;

            // Combine status effects
            if (other.statusEffects != null)
            {
                if (this.statusEffects == null)
                {
                    this.statusEffects = new StatusEffectEntry[0];
                }

                List<StatusEffectEntry> combinedEffects = new List<StatusEffectEntry>(this.statusEffects);

                foreach (var otherEffect in other.statusEffects)
                {
                    var existing = combinedEffects.Find(x => x.statusEffect == otherEffect.statusEffect);
                    if (existing != null)
                    {
                        existing.amountPerHit += otherEffect.amountPerHit;
                    }
                    else
                    {
                        combinedEffects.Add(new StatusEffectEntry
                        {
                            statusEffect = otherEffect.statusEffect,
                            amountPerHit = otherEffect.amountPerHit
                        });
                    }
                }

                this.statusEffects = combinedEffects.ToArray();
            }
        }

    }
}
