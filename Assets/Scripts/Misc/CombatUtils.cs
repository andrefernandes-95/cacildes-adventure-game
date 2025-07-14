using UnityEngine;

namespace AF
{
    public static class CombatUtils
    {
        static readonly string hashLightAttack1 = "Weapon Light Attack 1";
        static readonly string hashLightAttack2 = "Weapon Light Attack 2";
        static readonly string hashLightAttack3 = "Weapon Light Attack 3";
        static readonly string hashLightAttack4 = "Weapon Light Attack 4";
        static readonly string hashLeftLightAttack1 = "Weapon Left Light Attack 1";
        static readonly string hashLeftLightAttack2 = "Weapon Left Light Attack 2";
        static readonly string hashPowerStanceAttack1 = "Weapon Power Stance Attack 1";
        static readonly string hashPowerStanceAttack2 = "Weapon Power Stance Attack 2";
        static readonly string hashHeavyAttack1 = "Weapon Heavy Attack 1";
        static readonly string hashHeavyAttack2 = "Weapon Heavy Attack 2";
        static readonly string hashHeavyPowerStanceAttack1 = "Weapon Heavy Power Stance Attack 1";
        static readonly string hashHeavyPowerStanceAttack2 = "Weapon Heavy Power Stance Attack 2";

        public static string GetLightAttackAnimationName(int lightAttackComboIndex, bool isAttackingWithLeftHand, bool canPowerStance)
        {
            string hashAttack = "";

            if (lightAttackComboIndex == 0)
            {
                if (isAttackingWithLeftHand)
                {
                    hashAttack = canPowerStance ? hashPowerStanceAttack1 : hashLeftLightAttack1;
                }
                else
                {
                    hashAttack = hashLightAttack1;
                }
            }
            else if (lightAttackComboIndex == 1)
            {
                if (isAttackingWithLeftHand)
                {
                    hashAttack = canPowerStance ? hashPowerStanceAttack2 : hashLeftLightAttack2;
                }
                else
                {
                    hashAttack = hashLightAttack2;
                }
            }
            else if (lightAttackComboIndex == 2)
            {
                if (isAttackingWithLeftHand)
                {
                    hashAttack = canPowerStance ? hashPowerStanceAttack1 : hashLeftLightAttack1;
                }
                else
                {
                    hashAttack = hashLightAttack3;
                }
            }
            else if (lightAttackComboIndex == 3)
            {
                if (isAttackingWithLeftHand)
                {
                    hashAttack = canPowerStance ? hashPowerStanceAttack2 : hashLeftLightAttack2;
                }
                else
                {
                    hashAttack = hashLightAttack4;
                }
            }

            return hashAttack;
        }

        public static string GetHeavyAttackAnimationName(int heavyAttackComboIndex, bool canPowerStance)
        {
            string hashAttack = "";

            if (heavyAttackComboIndex == 0)
            {
                hashAttack = canPowerStance ? hashHeavyPowerStanceAttack1 : hashHeavyAttack1;
            }
            else if (heavyAttackComboIndex == 1)
            {
                hashAttack = canPowerStance ? hashHeavyPowerStanceAttack2 : hashHeavyAttack2;
            }

            return hashAttack;
        }

        public static void ThrowWeapon(CharacterWeaponHitbox currentWeapon, GameObject weaponThrowProjectilePrefab, CharacterBaseManager attacker, CharacterBaseManager target)
        {
            if (currentWeapon == null)
            {
                return;
            }

            currentWeapon.gameObject.SetActive(false);

            // Clone our current weapon and put it as a child of the weaponProjectilePrefab
            GameObject clonedWeapon = GameObject.Instantiate(currentWeapon.gameObject, attacker.transform.position + attacker.transform.up, attacker.transform.rotation);

            // Set the local scale, position, and rotation of the cloned weapon
            clonedWeapon.transform.parent = null;
            clonedWeapon.transform.localScale = new Vector3(1, 1, 1);

            clonedWeapon.GetComponent<Hitbox>().character = attacker;
            clonedWeapon.AddComponent<AttachCameraShakeToSpell>();
            clonedWeapon.AddComponent<Rigidbody>();
            clonedWeapon.AddComponent<ThrowWeaponHelper>().Initialize(attacker);
        }

        public static Projectile ThrowProjectile(GameObject projectile, CharacterBaseManager attacker, CharacterBaseManager target)
        {
            if (target != null)
            {
                var rotation = target.transform.position - attacker.transform.position;
                rotation.y = 0;
                attacker.transform.rotation = Quaternion.LookRotation(rotation);
            }

            GameObject instanceGO = GameObject.Instantiate(projectile, attacker.transform.position + attacker.transform.up, Quaternion.identity);
            Projectile instance = instanceGO.GetComponent<Projectile>();
            instance.shooter = attacker;

            if (target != null)
            {
                Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - instance.transform.position);
                instance.transform.rotation = targetRotation;
            }
            else
            {
                instance.transform.rotation = attacker.transform.rotation;
            }

            instance.Shoot(attacker, instance.GetForwardVelocity() * instance.transform.forward, instance.forceMode);
            return instance;
        }


    }
}