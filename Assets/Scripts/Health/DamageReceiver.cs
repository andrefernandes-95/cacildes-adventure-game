using System;
using AF.Combat;
using AF.Health;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{

    public class CharacterDamageReceiver : CharacterBaseDamageReceiver, IDamageable
    {

        [Header("Character")]
        public CharacterManager character;

        public void OnDamage(CharacterBaseManager attacker, Action onDamageInflicted)
        {
            if (!CanTakeDamage(attacker))
            {
                return;
            }

            HandleIncomingDamage(attacker, (incomingDamage) =>
            {
                onDamageInflicted();
            });
        }


        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="value"></param>
        public void SetHasFlatulence(bool value)
        {
            this.hasFlatulence = value;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        /// <param name="value"></param>
        public override void SetCanTakeDamage(bool value)
        {
            canTakeDamage = value;
        }

        public override void HandleIncomingDamage(CharacterBaseManager attacker, UnityAction<Damage> onTakeDamage)
        {

            HandleAttackWhileFlatulent();

            Damage incomingDamage = attacker.GetAttackDamage();
            if (incomingDamage == null)
            {
                LogIncomingDamageNullError(attacker);
                return;
            }

            if (character != null)
            {
                character.characterBaseWeaponsManager.CloseAllWeaponHitboxes();

                if (character.targetManager != null)
                {
                    character.targetManager.SetTarget(attacker);
                }

                RecoverFromStunnedStateWhenAttacked();


                if (TryParryIncomingDamage(attacker, incomingDamage))
                {
                    return;
                }

                if (TryBlockIncomingDamageForAI(attacker, incomingDamage))
                {
                    return;
                }

                HandleAngleHitFrom(attacker);
            }

            ApplyDamage(incomingDamage);

            onTakeDamage?.Invoke(incomingDamage);
            isTakingDamage = true;
        }

        /// <summary>
        /// Unity Event
        /// 
        /// </summary>
        /// <param name="damage"></param>
        public override void TakeDamage(Damage damage)
        {
            TakeDamage(damage, true);
        }

        public override void ApplyDamage(Damage damage, bool callOnDamageReceivedEvent)
        {
            // Always clone damage before modifying it
            damage = damage.Clone();
            HandlePushForce(damage);
            FilterDamageAbsorption(damage);
            HandleEquipmentPassiveFilterEffects(damage);

            if (character != null)
            {
                if (character.health.GetCurrentHealth() <= 0)
                {
                    return;
                }

                if (HandleDamageFromBackstab(damage))
                {
                    damage.damageType = DamageType.BACKSTAB;
                }
                else if (HandleDamageFromAttack(damage))
                {
                    damage.damageType = DamageType.CRITICAL_ATTACK;
                }

                HandleDamageFromStatusEffects(damage);

                HandleDamageEvents(damage);
            }

            if (callOnDamageReceivedEvent)
            {
                onDamageReceived?.Invoke();
            }
        }

        public override CharacterBaseManager GetCharacter()
        {
            return character;
        }
    }
}
