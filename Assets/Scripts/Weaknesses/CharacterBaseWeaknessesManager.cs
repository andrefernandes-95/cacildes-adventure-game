using System.Collections.Generic;
using AF.Health;
using UnityEngine;

namespace AF
{
    public abstract class CharacterBaseWeaknessesManager : MonoBehaviour
    {
        [Header("Buffs")]
        [SerializeField] List<WeaknessOverTime> appliedWeaknesses = new();

        public abstract CharacterBaseManager GetCharacter();

        public void Add(WeaknessOverTime weaknessOverTime)
        {
            appliedWeaknesses.Add(weaknessOverTime);
        }

        public void Remove(WeaknessOverTime weaknessOverTime)
        {
            appliedWeaknesses.Remove(weaknessOverTime);
        }

        public Damage ModifyBasedOnCurrentWeaknesses(Damage baseDamage)
        {
            foreach (WeaknessOverTime weaknessOverTime in appliedWeaknesses)
            {
                baseDamage = ModifyDamageWithWeakness(baseDamage, weaknessOverTime);
            }

            return baseDamage;
        }

        Damage ModifyDamageWithWeakness(Damage damage, WeaknessOverTime weaknessOverTime)
        {
            switch (weaknessOverTime.weaponElementType)
            {
                case WeaponElementType.Physical:
                    damage.physical += (int)(damage.physical * weaknessOverTime.attackMultiplier);
                    break;
                case WeaponElementType.Fire:
                    damage.fire += (int)(damage.fire * weaknessOverTime.attackMultiplier);
                    break;
                case WeaponElementType.Frost:
                    damage.frost += (int)(damage.frost * weaknessOverTime.attackMultiplier);
                    break;
                case WeaponElementType.Lightning:
                    damage.lightning += (int)(damage.lightning * weaknessOverTime.attackMultiplier);
                    break;
                case WeaponElementType.Magic:
                    damage.magic += (int)(damage.magic * weaknessOverTime.attackMultiplier);
                    break;
                case WeaponElementType.Darkness:
                    damage.darkness += (int)(damage.darkness * weaknessOverTime.attackMultiplier);
                    break;
                case WeaponElementType.Water:
                    damage.water += (int)(damage.water * weaknessOverTime.attackMultiplier);
                    break;
                default:
                    break;
            }

            return damage;
        }

    }
}
