using UnityEngine;
using System.Collections.Generic;

namespace AF
{
    public class MovingPlatform : MonoBehaviour
    {
        private Vector3 lastPosition;
        private readonly List<CharacterBaseManager> riders = new List<CharacterBaseManager>();

        private void Start()
        {
            lastPosition = transform.position;
        }

        private void Update()
        {
            Vector3 delta = transform.position - lastPosition;

            if (delta != Vector3.zero)
            {
                foreach (var rider in riders)
                {
                    if (rider != null && rider.enabled)
                    {
                        if (rider.health.GetCurrentHealth() > 0)
                        {
                            rider.characterController.Move(delta); // apply platform movement
                        }
                    }
                }
            }

            lastPosition = transform.position;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Enemy"))
            {
                CharacterBaseManager cc = other.GetComponent<CharacterBaseManager>();
                if (cc != null && !riders.Contains(cc))
                {
                    riders.Add(cc);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("Enemy"))
            {
                CharacterBaseManager cc = other.GetComponent<CharacterBaseManager>();
                if (cc != null && riders.Contains(cc))
                {
                    riders.Remove(cc);
                }
            }
        }
    }
}
