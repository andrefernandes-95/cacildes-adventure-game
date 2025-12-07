using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class Activity_PlayMusic : NPCActivity
    {
        [SerializeField] Transform lookReference;
        [SerializeField] Transform transformRef;

        [Header("Animations")]
        public string animationToPlay = "Play Music";
        public string animationToTransitionTo = "Idle";
        [SerializeField] AudioSource audioSource;

        [Header("Instrument Props")]
        [SerializeField] GameObject instrumentPrefab;
        private GameObject instantiatedInstrument;
        [SerializeField] Vector3 instrumentLocalPosition;
        [SerializeField] Vector3 instrumentLocalRotation;
        [SerializeField] Vector3 instrumentLocalScale = new(1, 1, 1);
        [SerializeField] bool isRightHand = false;

        [Header("Unity Events")]

        [SerializeField] float exitCrossFade = .1f;

        public override void OnActivityStart(CharacterBaseManager activityTarget)
        {
            activityTarget.transform.position = transformRef.transform.position;
            activityTarget.PlayBusyAnimationWithRootMotion(animationToPlay);
            activityTarget.FaceObject(lookReference);
            activityTarget.characterBaseWeaponsManager.HideEquipment();
            audioSource.Play();


            // Spawn drink graphic if assigned
            if (instrumentPrefab != null)
            {
                instantiatedInstrument = Instantiate(instrumentPrefab, isRightHand ? activityTarget.characterTransformHelper.rightHand : activityTarget.characterTransformHelper.leftHand);
                instantiatedInstrument.transform.SetLocalPositionAndRotation(instrumentLocalPosition, Quaternion.Euler(instrumentLocalRotation));
                instantiatedInstrument.transform.localScale = instrumentLocalScale;
            }

        }

        public override void OnActivityPerformed(CharacterBaseManager activityTarget)
        {
        }

        public override void OnActivityEnd(CharacterBaseManager activityTarget)
        {
            activityTarget.PlayCrossFadeBusyAnimationWithRootMotion(animationToTransitionTo, exitCrossFade);

            activityTarget.characterBaseWeaponsManager.ShowEquipment();

            audioSource.Stop();
            DestroyInstrument();
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


        void DestroyInstrument()
        {
            if (instantiatedInstrument != null)
            {
                Destroy(instantiatedInstrument);
                instantiatedInstrument = null;
            }
        }
    }
}
