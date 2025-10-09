using System.Collections;
using System.Linq;
using AF.Characters;
using AF.Companions;
using AF.Events;
using TigerForge;
using UnityEngine;
using UnityEngine.Events;

namespace AF.Combat
{

    public class TargetManager : MonoBehaviour
    {
        [Header("Events")]
        public UnityEvent onTargetSet_Event;
        public UnityEvent onAgressiveTowardsPlayer_Event;
        public UnityEvent onClearTarget_Event;


        [Header("Components")]
        public CharacterBaseManager currentTarget;

        public CharacterManager characterManager;

        [Header("Faction Settings")]
        public UnityAction<bool> onAgressiveTowardsPlayer;

        [Header("Combat Start Settings")]
        bool hasBeenInCombat = false;
        public float delayWhenBeginningCombatForFirstTime = 1f;

        // Scene Reference
        PlayerManager playerManager;
        CompanionsSceneManager companionsSceneManager;

        private void Awake()
        {
            EventManager.StartListening(EventMessages.ON_LEAVING_BONFIRE, () =>
            {
                hasBeenInCombat = false;
            });
        }

        bool CanFightTarget(CharacterBaseManager target)
        {
            if (currentTarget == target)
            {
                return false;
            }

            if (characterManager.transform.root == target.transform.root)
            {
                return false;
            }

            if (characterManager.IsFromSameFaction(target))
            {
                return false;
            }

            return true;
        }

        public void SetTarget(CharacterBaseManager target)
        {
            if (!CanFightTarget(target))
            {
                return;
            }

            HandleSetTarget(target);

            HandleBossEvent(target);
        }

        void HandleSetTarget(CharacterBaseManager target)
        {
            SetTargetInternally(target);

            if (characterManager != null && characterManager.partners != null && characterManager.partners.Length > 0)
            {
                foreach (var combatPartner in characterManager.partners)
                {
                    if (combatPartner != null && combatPartner.targetManager != null && combatPartner.isActiveAndEnabled)
                    {
                        combatPartner.targetManager.SetTarget(target);
                    }
                }
            }

            // Edge case to check if it's player
            if (target is PlayerManager)
            {
                NotifyCompanions();

                if (onAgressiveTowardsPlayer != null)
                {
                    onAgressiveTowardsPlayer(true);
                }
                onAgressiveTowardsPlayer_Event?.Invoke();
            }
        }

        void NotifyCompanions()
        {
            foreach (var companionInstance in GetCompanionsSceneManager().companionInstancesInScene)
            {
                companionInstance.Value.GetComponent<CharacterManager>().targetManager.SetTarget(this.characterManager);
            }

            Minion[] minionsInScene = FindObjectsByType<Minion>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var minion in minionsInScene)
            {
                if (minion.TryGetComponent<CharacterManager>(out var charManager))
                {
                    charManager.targetManager.SetTarget(this.characterManager);
                }
            }
        }

        public bool IsTargetBusy()
        {
            if (currentTarget == null)
            {
                return false;
            }

            return currentTarget.IsBusy();
        }

        public bool IsTargetShooting()
        {
            if (currentTarget is PlayerManager playerManager)
            {
                return playerManager.playerShootingManager.isShooting;
            }

            return false;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void SetPlayerAsTarget()
        {
            SetTarget(GetPlayerManager());
        }

        PlayerManager GetPlayerManager()
        {
            if (playerManager == null)
            {
                playerManager = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include);
            }

            return playerManager;
        }

        CompanionsSceneManager GetCompanionsSceneManager()
        {
            if (companionsSceneManager == null)
            {
                companionsSceneManager = FindAnyObjectByType<CompanionsSceneManager>(FindObjectsInactive.Include);
            }

            return companionsSceneManager;
        }

        public bool IsTargetOutOfMeleeRange()
        {
            if (currentTarget == null)
            {
                return false;
            }

            float maxDistance = characterManager.agent.stoppingDistance;
            if (characterManager.characterCombatController.currentCombatAction != null)
            {
                maxDistance += characterManager.characterCombatController.currentCombatAction.maximumDistanceToTarget;
            }

            return Vector3.Distance(currentTarget.transform.position, characterManager.transform.position) > maxDistance;
        }

        void SetTargetInternally(CharacterBaseManager target)
        {
            if (currentTarget != null)
            {
                currentTarget.health.onDeath.RemoveListener(OnTargetDeath);
                currentTarget = null;
            }

            if (target != null)
            {
                target.health.onDeath.AddListener(OnTargetDeath);
                currentTarget = target;
                onTargetSet_Event?.Invoke();
            }
        }

        void OnTargetDeath()
        {
            ClearTarget();
        }

        public void ClearTarget()
        {
            SetTargetInternally(null);

            onAgressiveTowardsPlayer?.Invoke(false);

            onClearTarget_Event?.Invoke();
        }

        void HandleBossEvent(CharacterBaseManager target)
        {
            if (target is not PlayerManager)
            {
                return;
            }

            if (characterManager.characterBossController.isBoss)
            {
                characterManager.characterBossController.BeginBossBattle();
            }
        }
    }
}
