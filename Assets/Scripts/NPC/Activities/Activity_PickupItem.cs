namespace AF
{
    using System.Collections;
    using UnityEngine;

    public class Activity_PickupItem : NPCActivity
    {
        [SerializeField] Transform alchemyTableLookReference;
        [SerializeField] Transform transformRef;

        [SerializeField] GameObject itemGraphic;
        [SerializeField] GameObject itemPickupVfxPrefab;


        private const string PICKUP_ANIMATION = "Pickup Item";

        void Start()
        {
            itemGraphic.SetActive(false);
        }

        public override void OnActivityStart(CharacterBaseManager activityTarget)
        {
            activityTarget.transform.position = transformRef.transform.position;
            activityTarget.PlayBusyAnimationWithRootMotion(PICKUP_ANIMATION);
            activityTarget.FaceObject(alchemyTableLookReference);
            itemGraphic.SetActive(true);
        }

        public override void OnActivityPerformed(CharacterBaseManager activityTarget)
        {
            Instantiate(itemPickupVfxPrefab, activityTarget.characterTransformHelper.rightHand);
            itemGraphic.SetActive(false);
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
