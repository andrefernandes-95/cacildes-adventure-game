using UnityEngine;

namespace AF
{

    public class WorldMapCameraController : MonoBehaviour
    {
        public float panSpeed = 20f;
        public float zoomSpeed = 500f;
        public float minZoom = 20f;
        public float maxZoom = 120f;

        private Camera cam;

        void Start()
        {
            cam = Camera.main;
        }

        void Update()
        {
            Vector3 pos = transform.position;

            // Keyboard input for panning
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                pos += transform.forward * panSpeed * Time.deltaTime;

            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                pos -= transform.forward * panSpeed * Time.deltaTime;

            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                pos += transform.right * panSpeed * Time.deltaTime;

            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                pos -= transform.right * panSpeed * Time.deltaTime;

            // Mouse scroll for zoom
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            cam.fieldOfView -= scroll * zoomSpeed * Time.deltaTime;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minZoom, maxZoom);

            transform.position = pos;
        }
    }

}