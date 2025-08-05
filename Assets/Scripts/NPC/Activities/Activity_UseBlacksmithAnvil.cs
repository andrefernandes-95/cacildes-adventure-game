namespace AF
{
    using System.Collections;
    using UnityEngine;
    using UnityEngine.Events;

    public class Activity_UseBlacksmithAnvil : NPCActivity
    {
        [SerializeField] Transform lookReference;
        [SerializeField] Transform transformRef;

        [Header("Hammer")]
        [SerializeField] GameObject hammerPrefab;
        GameObject hammerPrefabInstance;
        [SerializeField] Vector3 hammerLocalPosition;
        [SerializeField] Vector3 hammerLocalRotation;
        [SerializeField] Vector3 hammerLocalScale = new(1, 1, 1);

        [Header("Weapon")]
        [SerializeField] GameObject weaponToImprovePrefab;
        GameObject weaponToImprovePrefabInstance;
        [SerializeField] Vector3 weaponToImproveLocalPosition;
        [SerializeField] Vector3 weaponToImproveLocalRotation;
        [SerializeField] Vector3 weaponToImproveLocalScale = new(1, 1, 1);

        [Header("Duration")]
        [SerializeField] float duration = 10;

        Coroutine PerformingCoroutine;

        private const string BLACKSMITH_START = "Blacksmith Start";
        private const string BLACKSMITH_END = "Blacksmith End";

        [Header("Events")]
        [SerializeField] UnityEvent onActivityPerformed;

        public override void OnActivityStart(CharacterBaseManager activityTarget)
        {
            activityTarget.transform.position = transformRef.transform.position;
            activityTarget.PlayBusyAnimationWithRootMotion(BLACKSMITH_START);
            activityTarget.FaceObject(lookReference);

            if (PerformingCoroutine != null)
            {
                StopCoroutine(PerformingCoroutine);
            }

            PerformingCoroutine = StartCoroutine(PerformingActivity(activityTarget));
        }


        public override void OnActivityPerformed(CharacterBaseManager activityTarget)
        {
            onActivityPerformed?.Invoke();
        }

        public override void OnActivityEnd(CharacterBaseManager activityTarget)
        {
            DestroyCurrentWeaponToImproveInstance();
            DestroyCurrentHammerInstance();

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

        IEnumerator PerformingActivity(CharacterBaseManager activityTarget)
        {
            activityTarget.characterBaseWeaponsManager.HideEquipment();

            InstantiateHammer(activityTarget);
            InstantiateWeaponToImprove(activityTarget);

            yield return new WaitForSeconds(duration);
            activityTarget.PlayCrossFadeBusyAnimationWithRootMotion(BLACKSMITH_END, .1f);
        }

        void DestroyCurrentWeaponToImproveInstance()
        {
            if (weaponToImprovePrefabInstance != null)
            {
                Destroy(weaponToImprovePrefabInstance);
            }
        }

        void DestroyCurrentHammerInstance()
        {
            if (hammerPrefabInstance != null)
            {
                Destroy(hammerPrefabInstance);
            }
        }

        void InstantiateHammer(CharacterBaseManager activityTarget)
        {
            DestroyCurrentHammerInstance();

            hammerPrefabInstance = Instantiate(hammerPrefab, activityTarget.characterTransformHelper.rightHand);
            hammerPrefabInstance.transform.SetLocalPositionAndRotation(hammerLocalPosition, Quaternion.Euler(hammerLocalRotation));
            hammerPrefabInstance.transform.localScale = hammerLocalScale;
        }
        void InstantiateWeaponToImprove(CharacterBaseManager activityTarget)
        {
            DestroyCurrentWeaponToImproveInstance();

            weaponToImprovePrefabInstance = Instantiate(weaponToImprovePrefab, activityTarget.characterTransformHelper.leftHand);
            weaponToImprovePrefabInstance.transform.SetLocalPositionAndRotation(weaponToImproveLocalPosition, Quaternion.Euler(weaponToImproveLocalRotation));
            weaponToImprovePrefabInstance.transform.localScale = weaponToImproveLocalScale;
        }

    }
}
