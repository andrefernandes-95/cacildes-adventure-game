using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class Activity_TalkTo : NPCActivity
    {
        [Header("Drinking Spots")]
        [SerializeField] CharacterBaseManager characterToTalkTo;

        [Header("Settings")]
        [SerializeField] float durationInSeconds = 10f;
        Coroutine waitCoroutine;

        [Header("Animations")]
        public string ownerAnimation = "Talk";
        public string receiverAnimation = "Talk";
        public string animationToTransitionTo = "Idle";

        [Header("Unity Events")]
        [SerializeField] UnityEvent onActivityPerformed;

        public override void OnActivityStart(CharacterBaseManager activityTarget)
        {
            // Play drinking animation
            activityTarget.PlayBusyAnimationWithRootMotion(ownerAnimation);
            characterToTalkTo.PlayBusyAnimationWithRootMotion(receiverAnimation);

            // Face forward or optional target
            activityTarget.FaceObject(characterToTalkTo.transform);
            characterToTalkTo.FaceObject(activityTarget.transform);

            // Start wait coroutine
            if (waitCoroutine != null)
            {
                StopCoroutine(waitCoroutine);
            }
            waitCoroutine = StartCoroutine(Wait(activityTarget));
        }

        public override void OnActivityPerformed(CharacterBaseManager activityTarget)
        {
            onActivityPerformed?.Invoke();
        }

        IEnumerator Wait(CharacterBaseManager activityTarget)
        {
            yield return new WaitForSeconds(durationInSeconds);

            // Transition back to idle
            activityTarget.PlayCrossFadeBusyAnimationWithRootMotion(animationToTransitionTo, 0.25f);
            characterToTalkTo.PlayCrossFadeBusyAnimationWithRootMotion(animationToTransitionTo, 0.25f);
        }

        public override void OnActivityEnd(CharacterBaseManager activityTarget)
        {
        }

        public override bool HasReachedActivity(CharacterBaseManager activityTarget)
        {
            float distanceToTarget = Vector3.Distance(
               activityTarget.transform.position,
               characterToTalkTo.transform.position
           );

            return distanceToTarget <= stoppingDistance;
        }

        public override Transform GetActivityDestination()
        {
            return characterToTalkTo.transform;
        }
    }
}
