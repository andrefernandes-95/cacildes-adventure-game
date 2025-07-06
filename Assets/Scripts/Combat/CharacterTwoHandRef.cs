using UnityEngine;
namespace AF
{

    public class CharacterTwoHandRef : MonoBehaviour
    {
        Vector3 originalPosition;
        Quaternion originalRotation;

        [Header("Components")]
        [HideInInspector] public CharacterBaseManager characterBaseManager;

        private void Awake()
        {
            characterBaseManager = GetComponentInParent<CharacterBaseManager>();
        }

        public void SetOriginalPositionAndRotation(Vector3 initialLocalPosition, Quaternion initialLocalRotation)
        {
            this.originalPosition = initialLocalPosition;
            this.originalRotation = initialLocalRotation;
        }

        private void OnEnable()
        {
            if (characterBaseManager is PlayerManager playerManager)
            {
                playerManager.twoHandingController.onTwoHandingModeChanged += EvaluateTwoHandingUpdate;
                playerManager.characterBlockController.onBlockChanged += EvaluateTwoHandingUpdate;
                playerManager.characterBlockController.onBlockChanged += UseBlockTransform;
            }

            EvaluateTwoHandingUpdate();
        }

        private void OnDisable()
        {

            if (characterBaseManager is PlayerManager playerManager)
            {
                playerManager.twoHandingController.onTwoHandingModeChanged -= EvaluateTwoHandingUpdate;
                playerManager.characterBlockController.onBlockChanged -= EvaluateTwoHandingUpdate;
                playerManager.characterBlockController.onBlockChanged -= UseBlockTransform;
            }
        }


        public void EvaluateTwoHandingUpdate()
        {
            if (characterBaseManager.characterBaseWeaponsManager.IsTwoHanding() == false)
            {
                UseOneHandTransform();
                return;
            }

            if (characterBaseManager.characterBlockController.isBlocking && characterBaseManager.characterBaseWeaponsManager.IsTwoHanding())
            {
                UseBlockTransform();
                return;
            }

            UseTwoHandTransform();
        }

        public void UseOneHandTransform()
        {
            transform.SetLocalPositionAndRotation(originalPosition, originalRotation);
        }

        public void UseTwoHandTransform()
        {
            Weapon currentWeapon = characterBaseManager.characterBaseWeaponsManager.GetCurrentRightWeapon();
            if (currentWeapon == null || currentWeapon.useTwoHandingTransform == false)
            {
                return;
            }

            transform.localPosition = currentWeapon.twoHandingPosition;
            transform.localEulerAngles = currentWeapon.twoHandingRotation;
        }

        public void UseBlockTransform()
        {
            Weapon currentWeapon = characterBaseManager.characterBaseWeaponsManager.GetCurrentRightWeapon();

            if (
                characterBaseManager.characterBaseWeaponsManager.IsTwoHanding() == false ||
                characterBaseManager.characterBlockController.isBlocking == false ||
                currentWeapon == null ||
                currentWeapon.useCustomTwoHandingBlockTransforms == false)
            {
                return;
            }

            this.transform.localPosition = currentWeapon.th_BlockPosition;
            this.transform.localEulerAngles = currentWeapon.th_BlockRotation;
        }
    }
}
