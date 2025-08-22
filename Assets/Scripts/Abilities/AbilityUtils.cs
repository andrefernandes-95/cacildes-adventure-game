using AF.Health;

namespace AF
{
    public static class AbilityUtils
    {
        public static Damage GetAbilityDamageForAIAttack(CharacterBaseManager attacker, Damage abilityDamage)
        {
            Damage attackingWeaponDamage = null;
            HitboxType attackingHitbox = attacker.characterBaseAttackManager.attackingHitboxType;

            // Handle weapon-based attacks
            switch (attackingHitbox)
            {
                case HitboxType.RIGHT_HAND:
                    attackingWeaponDamage = attacker.characterBaseAttackManager.rightWeaponCurrentDamage;
                    break;
                case HitboxType.LEFT_HAND:
                    attackingWeaponDamage = attacker.characterBaseAttackManager.leftWeaponCurrentDamage;
                    break;
            }

            if (attackingWeaponDamage != null)
            {
                attackingWeaponDamage.Combine(abilityDamage);
                return attackingWeaponDamage;
            }

            // Handle unarmed attacks
            return GetUnarmedDamage(attacker, attackingHitbox, abilityDamage) ?? abilityDamage;
        }

        private static Damage GetUnarmedDamage(CharacterBaseManager attacker, HitboxType hitboxType, Damage abilityDamage)
        {
            var weaponsManager = attacker.characterBaseWeaponsManager;

            UnarmedHitbox unarmedHitbox = hitboxType switch
            {
                HitboxType.LEFT_HAND => weaponsManager.leftHandHitbox as UnarmedHitbox,
                HitboxType.RIGHT_HAND => weaponsManager.rightHandHitbox as UnarmedHitbox,
                HitboxType.LEFT_FOOT => weaponsManager.leftFootHitbox as UnarmedHitbox,
                HitboxType.RIGHT_FOOT => weaponsManager.rightFootHitbox as UnarmedHitbox,
                HitboxType.HEAD => weaponsManager.headHitbox as UnarmedHitbox,
                _ => null
            };

            if (unarmedHitbox?.unarmedWeapon?.damage != null)
            {
                Damage unarmedDamage = unarmedHitbox.unarmedWeapon.damage.Clone();
                unarmedDamage.Combine(abilityDamage);
                return unarmedDamage;
            }

            // No hitbox to combine damage, just return the ability damage itself
            return abilityDamage;
        }

    }
}
