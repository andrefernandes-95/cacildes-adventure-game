using System.Collections;
using UnityEngine;

namespace AF.Events
{
    public class EV_MoveTowards : EventBase
    {
        public CharacterManager characterManager;
        public bool shouldRun = false;
        public Transform targetDestination;

        [Header("Settings")]
        public bool shouldWaitUntilReachingWaypoint = true;
        float elapsedTime = 0f;
        public float maxTimeToTryReachingThePlace = 5f;

        public override IEnumerator Dispatch()
        {
            characterManager.SetAgentDestination(targetDestination.transform.position);

            yield return new WaitUntil(() =>
            {
                elapsedTime += Time.deltaTime;

                return !shouldWaitUntilReachingWaypoint || characterManager.agent.remainingDistance <= characterManager.agent.stoppingDistance || elapsedTime >= maxTimeToTryReachingThePlace;
            });

            if (shouldWaitUntilReachingWaypoint)
            {
                characterManager.agent.speed = 0f;
            }
        }
    }
}
