using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class Activity_PlayAnimation : NPCActivity
    {
        [SerializeField] Transform lookReference;
        [SerializeField] Transform transformRef;

        [Header("Settings")]
        [SerializeField] float durationInSeconds = 20;
        Coroutine WaitCoroutine;

        [Header("Animations")]
        public string animationToPlay = "Sitting";
        public string animationToTransitionTo = "Idle";

        [Header("Unity Events")]
        [SerializeField] UnityEvent onActvitiyPerformed;

        [SerializeField] float exitCrossFade = .1f;

        public override void OnActivityStart(CharacterBaseManager activityTarget)
        {
            activityTarget.transform.position = transformRef.transform.position;
            activityTarget.PlayBusyAnimationWithRootMotion(animationToPlay);

            activityTarget.FaceObject(lookReference);

            if (WaitCoroutine != null)
            {
                StopCoroutine(WaitCoroutine);
            }

            WaitCoroutine = StartCoroutine(Wait(activityTarget));
        }

        public override void OnActivityPerformed(CharacterBaseManager activityTarget)
        {
            onActvitiyPerformed?.Invoke();
        }

        IEnumerator Wait(CharacterBaseManager activityTarget)
        {
            yield return new WaitForSeconds(durationInSeconds);
            activityTarget.PlayCrossFadeBusyAnimationWithRootMotion(animationToTransitionTo, exitCrossFade);
        }

        public override void OnActivityEnd(CharacterBaseManager activityTarget)
        {
        }

        public override bool HasReachedActivity(CharacterBaseManager activityTarget)
        {
            float distanceToTarget = Vector3.Distance(
               activityTarget.transform.position,
               transformRef.position
           );

            return distanceToTarget <= stoppingDistance;
        }

        public override Transform GetActivityDestination()
        {
            return transformRef;
        }
    }
}
