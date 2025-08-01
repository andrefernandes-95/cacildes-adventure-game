namespace AF
{
    using System.Collections;
    using UnityEngine;

    public class Activity_UsingAlchemyTable : NPCActivity
    {
        [SerializeField] Transform alchemyTableLookReference;
        [SerializeField] Transform transformRef;

        [Header("Pestle")]
        [SerializeField] GameObject pestlePrefab;
        GameObject pestleInstance;
        [SerializeField] Vector3 pestleLocalPosition;
        [SerializeField] Vector3 pestleLocalRotation;
        [SerializeField] Vector3 pestleLocalScale = new(1, 1, 1);

        [Header("Mortar")]
        [SerializeField] GameObject mortarPrefab;
        GameObject mortarInstance;
        [SerializeField] Vector3 mortarLocalPosition;
        [SerializeField] Vector3 mortarLocalRotation;
        [SerializeField] Vector3 mortarLocalScale = new(1, 1, 1);

        [Header("Duration")]
        [SerializeField] float duration = 10;

        Coroutine PerformAlchemyCoroutine;

        private const string ALCHEMY_START = "Alchemy Start";
        private const string ALCHEMY_END = "Alchemy End";

        public override void OnActivityStart(CharacterBaseManager activityTarget)
        {
            activityTarget.transform.position = transformRef.transform.position;
            activityTarget.PlayBusyAnimationWithRootMotion(ALCHEMY_START);
            activityTarget.FaceObject(alchemyTableLookReference);

            if (PerformAlchemyCoroutine != null)
            {
                StopCoroutine(PerformAlchemyCoroutine);
            }

            PerformAlchemyCoroutine = StartCoroutine(PerformAlchemy(activityTarget));
        }


        public override void OnActivityPerformed(CharacterBaseManager activityTarget)
        {
        }

        public override void OnActivityEnd(CharacterBaseManager activityTarget)
        {
            DestroyCurrentMortarInstance();
            DestroyCurrentPestleInstance();

            activityTarget.characterBaseWeaponsManager.ShowEquipment();
        }

        public override bool HasReachedActivity(CharacterBaseManager activityTarget)
        {
            float distanceToTarget = Vector3.Distance(
               activityTarget.transform.position,
               transform.position
           );

            return distanceToTarget <= stoppingDistance + activityTarget.characterController.radius * 2;
        }

        public override Transform GetActivityDestination()
        {
            return transformRef;
        }

        IEnumerator PerformAlchemy(CharacterBaseManager activityTarget)
        {
            activityTarget.characterBaseWeaponsManager.HideEquipment();

            InstantiatePestle(activityTarget);
            InstantiateMortar(activityTarget);

            yield return new WaitForSeconds(duration);
            activityTarget.PlayCrossFadeBusyAnimationWithRootMotion(ALCHEMY_END, .1f);
        }

        void DestroyCurrentPestleInstance()
        {
            if (pestleInstance != null)
            {
                Destroy(pestleInstance);
            }
        }

        void DestroyCurrentMortarInstance()
        {
            if (mortarInstance != null)
            {
                Destroy(mortarInstance);
            }
        }

        void InstantiatePestle(CharacterBaseManager activityTarget)
        {
            DestroyCurrentPestleInstance();

            pestleInstance = Instantiate(pestlePrefab, activityTarget.characterTransformHelper.rightHand);
            pestleInstance.transform.SetLocalPositionAndRotation(pestleLocalPosition, Quaternion.Euler(pestleLocalRotation));
            pestleInstance.transform.localScale = pestleLocalScale;
        }
        void InstantiateMortar(CharacterBaseManager activityTarget)
        {
            DestroyCurrentMortarInstance();

            mortarInstance = Instantiate(mortarPrefab, activityTarget.characterTransformHelper.leftHand);
            mortarInstance.transform.SetLocalPositionAndRotation(mortarLocalPosition, Quaternion.Euler(mortarLocalRotation));
            mortarInstance.transform.localScale = mortarLocalScale;
        }

    }
}
