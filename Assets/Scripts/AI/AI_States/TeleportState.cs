using System.Collections;
using System.Collections.Generic;
using AF.Companions;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace AF
{
    public class TeleportState : State
    {

        [Header("Components")]
        public CharacterManager characterManager;


        [Header("Teleport Options")]
        public float delayBeforeTeleportationBegins = 1f;
        public float minimumTeleportRadiusFromTarget = 5f;
        public float maximumTeleportRadiusFromTarget = 10f;
        public float minimumTeleportTime = 1f;
        public float maximumTeleportTime = 4f;
        public bool teleportNearPlayer = false;
        public List<Transform> teleportPoints = new();

        PlayerManager _playerManager;
        public State chaseState;

        public const string hashTeleporting = "Teleport";

        [Header("Events")]
        public UnityEvent onStateEnter;
        public UnityEvent onDisappear;
        public UnityEvent onReappear;

        public override void OnStateEnter(StateManager stateManager)
        {
            onStateEnter?.Invoke();
            onDisappear?.Invoke();

            TeleportEnemy();

            characterManager.PlayBusyAnimationWithRootMotion(hashTeleporting);
        }

        public override void OnStateExit(StateManager stateManager)
        {
        }

        public override State Tick(StateManager stateManager)
        {

            return this;
        }

        void TeleportEnemy()
        {
            // characterManager.agent.Warp(randomPoint);
        }

        PlayerManager GetPlayerManager()
        {
            if (_playerManager == null) { _playerManager = FindAnyObjectByType<PlayerManager>(FindObjectsInactive.Include); }
            return _playerManager;
        }

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnTeleportEnd()
        {
            onReappear?.Invoke();
        }
    }
}
