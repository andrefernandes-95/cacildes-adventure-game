using UnityEngine;

namespace AF
{

    public class Elevator : MonoBehaviour
    {
        [Header("Positions")]
        public float topPoint;         // Highest point
        public float bottomPoint;      // Lowest point

        [Header("Movement Settings")]
        public float speed = 2f;           // Lerp speed
        public bool startAtTop = false;    // Optional
        [SerializeField] bool goingUp = false;      // Direction toggle

        [Header("Audio")]
        public AudioSource audioSource;    // Optional audio
        public AudioClip startSound;       // Plays when movement begins
        public AudioClip stopSound;        // Plays when elevator arrives

        private bool moving = false;
        private Vector3 targetPos;

        void Start()
        {
            // Initialize position
            transform.localPosition = new Vector3(transform.localPosition.x, startAtTop ? topPoint : bottomPoint, transform.localPosition.z);
        }

        void Update()
        {
            if (moving)
            {
                transform.localPosition = Vector3.MoveTowards(
                    transform.localPosition, // current position
                    targetPos,               // target position
                    speed * Time.deltaTime    // maximum distance to move this frame
                );

                // Check arrival
                if (Vector3.Distance(transform.localPosition, targetPos) < 0.05f)
                {
                    transform.localPosition = targetPos;
                    moving = false;

                    if (audioSource && stopSound)
                        audioSource.PlayOneShot(stopSound);
                }
            }
        }

        // Call this from lever, switch, or trigger
        public void ToggleElevator()
        {
            if (moving) return;

            goingUp = !goingUp;
            targetPos = new Vector3(transform.localPosition.x, goingUp ? topPoint : bottomPoint, transform.localPosition.z);
            moving = true;

            if (audioSource && startSound)
                audioSource.PlayOneShot(startSound);
        }
    }
}
