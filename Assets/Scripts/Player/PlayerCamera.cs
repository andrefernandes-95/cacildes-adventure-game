namespace AF
{
    using AF.Events;
    using Cinemachine;
    using TigerForge;
    using UnityEngine;

    public class PlayerCamera : MonoBehaviour
    {
        private CinemachineVirtualCamera cinemachineVirtualCamera;
        public GameSettings gameSettings;

        void Start()
        {
            cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();

            UpdateCameraDistance();

            EventManager.StartListening(EventMessages.ON_CAMERA_DISTANCE_CHANGED, UpdateCameraDistance);
        }

        void UpdateCameraDistance()
        {
            cinemachineVirtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().CameraDistance = gameSettings.GetCameraDistance();
        }
    }

}
