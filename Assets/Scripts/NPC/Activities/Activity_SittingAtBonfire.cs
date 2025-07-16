namespace AF
{
    using System.Collections;
    using UnityEngine;

    public class Activity_SittingAtBonfire : NPCActivity
    {
        [SerializeField] Transform bonfireLookReference;
        [SerializeField] Transform transformRef;
        [SerializeField] GameObject bonfireFireToLitUp;

        [Header("Settings")]
        [SerializeField] float durationInSeconds = 20;

        Coroutine WaitOnBonfireCoroutine;

        public override void OnActivityStart(CharacterBaseManager activityTarget)
        {
            activityTarget.transform.position = transformRef.transform.position;
            activityTarget.PlayBusyAnimationWithRootMotion("Sitting At Bonfire");
            activityTarget.FaceObject(bonfireLookReference);
            bonfireFireToLitUp.SetActive(true);

            if (WaitOnBonfireCoroutine != null)
            {
                StopCoroutine(WaitOnBonfireCoroutine);
            }

            WaitOnBonfireCoroutine = StartCoroutine(WaitOnBonfire(activityTarget));
        }

        public override void OnActivityPerformed(CharacterBaseManager activityTarget)
        {
        }

        IEnumerator WaitOnBonfire(CharacterBaseManager activityTarget)
        {
            yield return new WaitForSeconds(durationInSeconds);
            activityTarget.PlayCrossFadeBusyAnimationWithRootMotion("Exit Bonfire", .1f);
        }

        public override void OnActivityEnd(CharacterBaseManager activityTarget)
        {
            bonfireFireToLitUp.SetActive(false);
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
