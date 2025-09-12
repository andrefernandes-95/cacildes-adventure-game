using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class Activity_Drink : NPCActivity
    {
        [Header("Drinking Spots")]
        [SerializeField] Transform lookReference;
        [SerializeField] Transform transformRef;

        [Header("Settings")]
        [SerializeField] float durationInSeconds = 10f;
        Coroutine waitCoroutine;

        [Header("Animations")]
        public string drinkAnimation = "Drink";
        public string animationToTransitionTo = "Idle";

        [Header("Drink Props")]
        [SerializeField] GameObject drinkGraphicPrefab;
        private GameObject instantiatedDrink;
        [SerializeField] Vector3 drinkLocalPosition;
        [SerializeField] Vector3 drinkLocalRotation;
        [SerializeField] Vector3 drinkLocalScale = new(1, 1, 1);

        [Header("Unity Events")]
        [SerializeField] UnityEvent onActivityPerformed;

        public override void OnActivityStart(CharacterBaseManager activityTarget)
        {
            // Play drinking animation
            activityTarget.PlayBusyAnimationWithRootMotion(drinkAnimation);

            // Face forward or optional target
            activityTarget.FaceObject(lookReference);

            // Spawn drink graphic if assigned
            if (drinkGraphicPrefab != null)
            {
                instantiatedDrink = Instantiate(drinkGraphicPrefab, activityTarget.characterTransformHelper.rightHand);
                instantiatedDrink.transform.SetLocalPositionAndRotation(drinkLocalPosition, Quaternion.Euler(drinkLocalRotation));
                instantiatedDrink.transform.localScale = drinkLocalScale;
            }

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
            activityTarget.PlayCrossFadeBusyAnimationWithRootMotion(animationToTransitionTo, 0f);

            DestroyDrink();
        }

        public override void OnActivityEnd(CharacterBaseManager activityTarget)
        {
            DestroyDrink();
        }

        void DestroyDrink()
        {
            if (instantiatedDrink != null)
            {
                Destroy(instantiatedDrink);
                instantiatedDrink = null;
            }
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
