using System;
using AF.Combat;
using AF.Health;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class PlayerDamageReceiver : CharacterBaseDamageReceiver, IDamageable
    {

        [Header("Character")]
        public PlayerManager playerManager;

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

        public override bool CanTakeDamage(CharacterBaseManager attacker)
        {
            if (!base.CanTakeDamage(attacker))
            {
                return false;
            }

            // Do not take damage while climbing ladders. 
            // This is because damage animations can break the climbing state
            if (playerManager.climbController.climbState != Ladders.ClimbState.NONE)
            {
                return false;
            }

            return true;
        }


        public override void HandleIncomingDamage(CharacterBaseManager attacker, UnityAction<Damage> onTakeDamage)
        {
            playerManager.playerWeaponsManager.CloseAllWeaponHitboxes();

            HandleAttackWhileFlatulent();

            Damage incomingDamage = attacker.GetAttackDamage()?.Clone();

            if (incomingDamage == null)
            {
                LogIncomingDamageNullError(attacker);
                return;
            }

            if (playerManager != null)
            {
                RecoverFromStunnedStateWhenAttacked();

                if (TryParryIncomingDamage(attacker, incomingDamage))
                {
                    return;
                }

                // For AI, we should return and not run any more logic, but for player, we can continue
                TryBlockIncomingDamageForPlayer(playerManager, attacker, ref incomingDamage);
                HandlePlayerReactionToEnemyAttack(attacker, playerManager);
                HandlePlayerArmorAttacks(attacker);
                HandlePlayerRage();
                HandlePlayerHealthBack(attacker);

                HandleAngleHitFrom(attacker);
            }

            onDamageModifierEvent?.Invoke(incomingDamage, attacker, GetCharacter());
            ApplyDamage(incomingDamage);

            onTakeDamage?.Invoke(incomingDamage);
            isTakingDamage = true;
        }

        void HandlePlayerArmorAttacks(CharacterBaseManager damageOwner)
        {
            CharacterManager enemy = damageOwner as CharacterManager;
            if (enemy == null)
            {
                return;
            }

            if (playerManager.equipmentDatabase.helmet != null && playerManager.equipmentDatabase.helmet.canDamageEnemiesUponAttack)
            {
                playerManager.equipmentDatabase.helmet.AttackEnemy(enemy);
            }
            if (playerManager.equipmentDatabase.armor != null && playerManager.equipmentDatabase.armor.canDamageEnemiesUponAttack)
            {
                playerManager.equipmentDatabase.armor.AttackEnemy(enemy);
            }
            if (playerManager.equipmentDatabase.gauntlet != null && playerManager.equipmentDatabase.gauntlet.canDamageEnemiesUponAttack)
            {
                playerManager.equipmentDatabase.gauntlet.AttackEnemy(enemy);
            }
            if (playerManager.equipmentDatabase.legwear != null && playerManager.equipmentDatabase.legwear.canDamageEnemiesUponAttack)
            {
                playerManager.equipmentDatabase.legwear.AttackEnemy(enemy);
            }
        }

        void HandlePlayerRage()
        {
            playerManager.rageManager.IncrementRage();
        }

        void HandlePlayerHealthBack(CharacterBaseManager damageOwner)
        {
            if (damageOwner is PlayerManager playerManager)
            {
                if (
                    playerManager.playerWeaponsManager?.currentWeaponInstance != null
                    && playerManager.playerWeaponsManager?.currentWeaponInstance?.weapon != null
                    && playerManager.playerWeaponsManager?.currentWeaponInstance?.weapon?.healthRestoredWithEachHit > 0)
                {
                    playerManager.health.RestoreHealth(playerManager.playerWeaponsManager.currentWeaponInstance.weapon.healthRestoredWithEachHit);
                }
            }
        }

        /// <summary>
        /// Unity Event
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

            if (GetCharacter() != null)
            {
                if (playerManager.health.GetCurrentHealth() <= 0)
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

        void HandlePlayerReactionToEnemyAttack(CharacterBaseManager damageOwner, CharacterBaseManager target)
        {
            if (damageOwner is CharacterManager aiCharacter)
            {
                if (aiCharacter.characterCombatController.currentCombatAction != null && aiCharacter.characterCombatController.currentCombatAction.targetHitReaction != null)
                {
                    target.PlayBusyAnimationWithRootMotion(aiCharacter.characterCombatController.currentCombatAction.targetHitReaction.name);
                }
            }
        }

        public override CharacterBaseManager GetCharacter()
        {
            return playerManager;
        }
    }
}
