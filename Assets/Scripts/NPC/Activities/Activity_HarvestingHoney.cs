namespace AF
{
    using UnityEngine;

    public class Activity_HarvestingHoney : NPCActivity
    {
        [SerializeField] Transform beeHiveLookReference;
        [SerializeField] Transform transformRef;
        [SerializeField] GameObject beeHiveFxPrefab;

        public override void OnActivityStart(CharacterBaseManager activityTarget)
        {
            activityTarget.transform.position = transformRef.transform.position;
            activityTarget.PlayBusyAnimationWithRootMotion("Bee Harvest");
            activityTarget.FaceObject(beeHiveLookReference);
        }

        public override void OnActivityPerformed(CharacterBaseManager activityTarget)
        {
            Instantiate(beeHiveFxPrefab, activityTarget.characterTransformHelper.rightHand);
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
