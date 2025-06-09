using UnityEngine;
namespace AF
{

    public class CharacterTwoHandRef : MonoBehaviour
    {

        public bool useTwoHandingTransform = true;

        [Header("Idle Settings")]
        public Vector3 twoHandingPosition;
        public Vector3 twoHandingRotation;

        [Header("Block Settings")]
        public bool useCustomBlockRefs = false;
        public Vector3 blockPosition;
        public Vector3 blockRotation;

        Vector3 originalPosition;
        Quaternion originalRotation;

        [Header("Components")]
        public PlayerManager playerManager;
        public EquipmentDatabase equipmentDatabase;

        public void SetOriginalPositionAndRotation(Vector3 initialLocalPosition, Quaternion initialLocalRotation)
        {
            this.originalPosition = initialLocalPosition;
            this.originalRotation = initialLocalRotation;
        }

        private void OnEnable()
        {
            playerManager.twoHandingController.onTwoHandingModeChanged += EvaluateTwoHandingUpdate;
            playerManager.characterBlockController.onBlockChanged += EvaluateTwoHandingUpdate;
            playerManager.characterBlockController.onBlockChanged += UseBlockTransform;

            EvaluateTwoHandingUpdate();
        }

        private void OnDisable()
        {

            playerManager.twoHandingController.onTwoHandingModeChanged -= EvaluateTwoHandingUpdate;
            playerManager.characterBlockController.onBlockChanged -= EvaluateTwoHandingUpdate;
            playerManager.characterBlockController.onBlockChanged -= UseBlockTransform;
        }


        public void EvaluateTwoHandingUpdate()
        {
            if (equipmentDatabase.isTwoHanding == false)
            {
                UseOneHandTransform();
                return;
            }

            if (playerManager.characterBlockController.isBlocking && equipmentDatabase.isTwoHanding)
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
            Weapon currentWeapon = equipmentDatabase.GetCurrentWeapon();
            if (currentWeapon == null || currentWeapon.useTwoHandingTransform == false)
            {
                return;
            }

            transform.localPosition = currentWeapon.twoHandingPosition;
            transform.localEulerAngles = currentWeapon.twoHandingRotation;
        }

        public void UseBlockTransform()
        {
            Weapon currentWeapon = equipmentDatabase.GetCurrentWeapon();

            if (
                equipmentDatabase.isTwoHanding == false ||
                playerManager.characterBlockController.isBlocking == false ||
                equipmentDatabase.isUsingShield ||
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
