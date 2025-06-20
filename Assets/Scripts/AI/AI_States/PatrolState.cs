using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Splines;

namespace AF
{
    public class PatrolState : State
    {
        [Header("Waypoints")]
        public SplineContainer splineContainer;
        [SerializeField] List<Vector3> waypoints = new List<Vector3>();
        int currentWaypointIndex;

        [Header("Random Points")]
        public bool chooseRandomWaypoint = false;
        public float minimumRadiusToChooseFromCurrentPosition = 1f;
        public float maximumRadiusToChooseFromCurrentPosition = 5f;

        [Header("Settings")]
        public float waitOnWaypoints = 0f;
        bool isWaitingOnWaypoint = false;
        Coroutine waitCoroutine;

        [Header("Components")]
        public CharacterManager characterManager;

        [Header("Events")]
        public UnityEvent onStateEnter;
        public UnityEvent onStateUpdate;
        public UnityEvent onStateExit;

        public UnityEvent onWaitOnWaypoint_Start;
        public UnityEvent onWaitOnWaypoint_End;

        Vector3 originalPosition;



        private void Awake()
        {
            originalPosition = transform.position;

            SetupWaypoints();
        }

        private void Start()
        {
            SetDestinationToWaypoint();
        }

        void SetupWaypoints()
        {
            if (splineContainer == null)
            {
                Debug.LogError("Please assign a spline container to the patrol state");
                return;
            }

            foreach (var spline in splineContainer.Splines)
            {
                foreach (var s in spline)
                {
                    Vector3 worldPosition = splineContainer.transform.TransformPoint(s.Position);
                    waypoints.Add(worldPosition);
                }
            }
        }

        public override void OnStateEnter(StateManager stateManager)
        {
            onStateEnter?.Invoke();
            DecideNextWaypoint();
        }

        public override void OnStateExit(StateManager stateManager)
        {
            onStateExit?.Invoke();
        }

        bool ShouldDecideNextWaypoint()
        {
            if (isWaitingOnWaypoint)
            {
                return false;
            }

            return characterManager.agent.remainingDistance <= characterManager.agent.stoppingDistance;
        }

        public override State Tick(StateManager stateManager)
        {
            onStateUpdate?.Invoke();

            // Check if the agent has reached its current destination
            if (ShouldDecideNextWaypoint())
            {
                DecideNextWaypoint();
            }
            else
            {
                characterManager.MoveUsingNavmeshAgent(false);
            }

            return this;
        }

        void DecideNextWaypoint()
        {
            isWaitingOnWaypoint = true;

            if (waitCoroutine != null)
            {
                StopCoroutine(waitCoroutine);
            }

            waitCoroutine = StartCoroutine(Wait());
        }

        public void SetNextWaypoint()
        {
            if (!chooseRandomWaypoint)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
            }

            SetDestinationToWaypoint();
        }

        private void SetDestinationToWaypoint()
        {
            if (chooseRandomWaypoint)
            {
                characterManager.SetAgentDestination(GetRandomPointOnNavMesh());
            }
            else if (characterManager.agent.enabled && waypoints.Count > 0)
            {
                characterManager.SetAgentDestination(waypoints[currentWaypointIndex]);
            }
            else
            {
                Debug.LogWarning($"No waypoints assigned to {gameObject.name}.");
            }
        }

        IEnumerator Wait()
        {
            onWaitOnWaypoint_Start?.Invoke();
            yield return new WaitForSeconds(waitOnWaypoints);
            onWaitOnWaypoint_End?.Invoke();

            SetNextWaypoint();
            isWaitingOnWaypoint = false;
        }

        Vector3 GetRandomPointOnNavMesh()
        {
            Vector3 randomDirection = Random.insideUnitSphere * maximumRadiusToChooseFromCurrentPosition;
            randomDirection += originalPosition + transform.forward * minimumRadiusToChooseFromCurrentPosition;
            Vector3 randomPoint = Vector3.zero;

            // Attempt to find a random point on the NavMesh in the random direction
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, maximumRadiusToChooseFromCurrentPosition, NavMesh.AllAreas))
            {
                // If a valid point is found, return it
                randomPoint = hit.position;
            }

            return randomPoint;
        }
    }
}
