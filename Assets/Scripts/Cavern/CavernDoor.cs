namespace AF
{
    using UnityEngine;
    using System.Collections;

    public class CavernDoor : MonoBehaviour
    {
        [Header("Door Settings")]
        public float slideDistance = 3f;     // How far down the door moves
        public float slideSpeed = 2f;        // Speed of movement
        public float openDuration = 3f;      // Time to stay open

        [Header("Effects")]
        public AudioSource audioSource;      // Audio source for door sound
        public AudioClip doorSFX;            // The sound clip to play
        public ParticleSystem openVFX;       // VFX to play when opening

        private Vector3 closedPosition;
        private Vector3 openPosition;
        private bool isMoving = false;

        void Start()
        {
            closedPosition = transform.position;
            openPosition = closedPosition - new Vector3(0, slideDistance, 0);
        }

        public void TriggerDoor()
        {
            if (!isMoving)
                StartCoroutine(OpenCloseRoutine());
        }

        private IEnumerator OpenCloseRoutine()
        {
            isMoving = true;

            PlayVfx();

            // Move down (open)
            yield return StartCoroutine(MoveDoor(closedPosition, openPosition));

            // Wait for player to enter
            yield return new WaitForSeconds(openDuration);

            PlayVfx();

            // Move back up (close)
            yield return StartCoroutine(MoveDoor(openPosition, closedPosition));

            isMoving = false;
        }

        void PlayVfx()
        {
            // Play sound and VFX
            if (doorSFX && audioSource)
                audioSource.PlayOneShot(doorSFX);
            if (openVFX)
                openVFX.Play();
        }

        private IEnumerator MoveDoor(Vector3 start, Vector3 end)
        {
            float elapsed = 0f;
            float distance = Vector3.Distance(start, end);

            while (elapsed < distance / slideSpeed)
            {
                transform.position = Vector3.Lerp(start, end, elapsed / (distance / slideSpeed));
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = end;
        }
    }
}
