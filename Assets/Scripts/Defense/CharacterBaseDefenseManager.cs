namespace AF
{
    using AF.Health;
    using UnityEngine;

    public abstract class CharacterBaseDefenseManager : MonoBehaviour
    {
        [Header("Components")]
        public CharacterBaseManager character;

        /* =========================
         * PUBLIC API
         * ========================= */

        public Damage GetCurrentDefense(int vit, int end, int str, int intelligence)
        {
            return CalculateDefense(vit, end, str, intelligence, GetEquippedArmorSet());
        }

        public void FilterIncomingDamage(Damage incomingDamage)
        {
            CharacterBaseStats stats = character.characterBaseStats;
            Damage defense = GetCurrentDefense(stats.GetVitality(), stats.GetEndurance(), stats.GetStrength(), stats.GetIntelligence());

            if (incomingDamage.physical > 0)
            {
                incomingDamage.physical -= Mathf.Max(1, defense.physical);

                switch (incomingDamage.weaponAttackType)
                {
                    case WeaponAttackType.Slash:
                        incomingDamage.physical = Mathf.Max(1,
                            (int)(incomingDamage.physical * GetAbsorption(character.combatant?.slashAbsorption)));
                        break;

                    case WeaponAttackType.Blunt:
                        incomingDamage.physical = Mathf.Max(1,
                            (int)(incomingDamage.physical * GetAbsorption(character.combatant?.bluntAbsorption)));
                        break;

                    case WeaponAttackType.Pierce:
                        incomingDamage.physical = Mathf.Max(1,
                            (int)(incomingDamage.physical * GetAbsorption(character.combatant?.pierceAbsorption)));
                        break;
                }
            }

            ApplyElemental(ref incomingDamage.fire, defense.fire,
                character.combatant?.fireAbsorption,
                character.combatant?.fireBonus);

            ApplyElemental(ref incomingDamage.frost, defense.frost,
                character.combatant?.frostAbsorption,
                character.combatant?.frostBonus);

            ApplyElemental(ref incomingDamage.water, defense.water,
                character.combatant?.waterAbsorption,
                character.combatant?.waterBonus);

            ApplyElemental(ref incomingDamage.darkness, defense.darkness,
                character.combatant?.darknessAbsorption,
                character.combatant?.darknessBonus);

            ApplyElemental(ref incomingDamage.lightning, defense.lightning,
                character.combatant?.lightningAbsorption,
                character.combatant?.lightningBonus);

            ApplyElemental(ref incomingDamage.magic, defense.magic,
                character.combatant?.magicAbsorption,
                character.combatant?.magicBonus);

            if (incomingDamage.postureDamage > 0)
            {
                float postureDamageAbsorption = (character != null && character.combatant != null) ? character.combatant.postureDamageAbsorption : 1f;
                incomingDamage.postureDamage = Mathf.Max(1, (int)(incomingDamage.postureDamage * postureDamageAbsorption));
            }

            if (incomingDamage.poiseDamage > 0)
            {
                float poiseDamageAbsorption = (character != null && character.combatant != null) ? character.combatant.poiseDamageAbsorption : 1f;
                incomingDamage.poiseDamage = Mathf.Max(1, (int)(incomingDamage.poiseDamage * poiseDamageAbsorption));
            }

            if (incomingDamage.pushForce > 0)
            {
                float pushForceDamageAbsorption = (character != null && character.combatant != null) ? character.combatant.pushForceAbsorption : 1f;
                incomingDamage.pushForce = Mathf.Max(1, (int)(incomingDamage.pushForce * pushForceDamageAbsorption));
            }

            character.characterBaseWeaknessesManager
                .ModifyBasedOnCurrentWeaknesses(incomingDamage);
        }


        public DefenseComparisonResult CompareArmorPiece(ArmorBase newItem, int slot = 0)
        {
            CharacterBaseStats stats = character.characterBaseStats;

            Damage current = GetCurrentDefense(stats.GetVitality(), stats.GetEndurance(), stats.GetStrength(), stats.GetIntelligence());

            ArmorSet modifiedSet = GetEquippedArmorSet();
            modifiedSet.Replace(newItem, slot);

            Damage withItem = CalculateDefense(stats.GetVitality(), stats.GetEndurance(), stats.GetStrength(), stats.GetIntelligence(), modifiedSet);

            return new DefenseComparisonResult
            {
                current = current,
                withItem = withItem,
                comparison = CompareTotal(current, withItem)
            };
        }

        /* =========================
         * CORE CALCULATION
         * ========================= */
        float GetAbsorption(float? value)
        {
            return value.HasValue ? value.Value : 1f;
        }

        void ApplyElemental(
            ref int damage,
            int defense,
            float? absorption,
            float? bonus)
        {
            if (damage <= 0) return;

            damage -= Mathf.Max(0, defense);

            if (damage <= 0)
            {
                damage = 0;
                return;
            }

            float finalAbsorption = GetAbsorption(absorption);
            float finalBonus = bonus ?? 1f;

            damage = (int)(damage * finalAbsorption * finalBonus);
        }

        Damage CalculateDefense(int vit, int end, int str, int intelligence, ArmorSet armorSet)
        {
            Damage result = CalculateBaseDefense(vit, end, str, intelligence);
            result.Combine(CalculateEquipmentDefense(armorSet));
            return result;
        }

        Damage CalculateBaseDefense(int vit, int end, int str, int intelligence)
        {

            return new Damage
            {
                physical = GetPhysicalDefense(vit, end, str),
                fire = GetElementalDefense(intelligence),
                frost = GetElementalDefense(intelligence),
                magic = GetElementalDefense(intelligence),
                water = GetElementalDefense(intelligence),
                lightning = GetElementalDefense(intelligence),
                darkness = GetElementalDefense(intelligence),
                poiseDamage = character.characterPoise.GetMaxPoiseHits(),
                postureDamage = character.characterPosture.GetMaxPostureDamage()
            };
        }

        Damage CalculateEquipmentDefense(ArmorSet set)
        {
            Damage total = new Damage();

            foreach (var piece in set.All)
            {
                if (piece == null) continue;
                total.Combine(piece.GetDamageAbsorbedForCurrentLevel());
            }

            return total;
        }

        /* =========================
         * STAT FORMULAS
         * ========================= */

        int GetPhysicalDefense(int vit, int end, int str)
        {
            return
                DefenseUtils.GetPhysicalDefenseFromVitaly(vit) +
                DefenseUtils.GetPhysicalDefenseFromEndurance(end) +
                DefenseUtils.GetPhysicalDefenseFromStrength(str);
        }

        int GetElementalDefense(int intelligence)
        {
            return DefenseUtils.GetElementalDefenseFromIntelligence(
                intelligence);
        }

        /* =========================
         * HELPERS
         * ========================= */

        int CompareTotal(Damage a, Damage b)
        {
            (bool isBetter, bool isWorse, bool isEqual) = DefenseUtils.CompareDamageNegation(
                a.GetTotalDamage(),
                b.GetTotalDamage()
            );

            if (isBetter)
            {
                return 1;
            }

            if (isWorse)
            {
                return -1;
            }

            return 0;
        }

        ArmorSet GetEquippedArmorSet()
        {
            var eq = character.characterBaseEquipment;
            return new ArmorSet(
                eq.GetEquippedHelmet(),
                eq.GetEquippedArmor(),
                eq.GetEquippedGauntlet(),
                eq.GetEquippedLegwear(),
                eq.GetAccessoryInSlot(0),
                eq.GetAccessoryInSlot(1),
                eq.GetAccessoryInSlot(2),
                eq.GetAccessoryInSlot(3)
            );
        }
    }
}
