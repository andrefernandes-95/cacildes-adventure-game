using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace AF
{
    public class CharacterDodgeController : CharacterBaseDodgeController
    {
        // Animation hash values
        public readonly int hashBackstep = Animator.StringToHash("Back Step");
        public readonly int hashRightRoll = Animator.StringToHash("Right Roll");
        public readonly int hashLeftRoll = Animator.StringToHash("Left Roll");
        public readonly int hashForwardRoll = Animator.StringToHash("Forward Roll");
        public readonly int hashBackwardRoll = Animator.StringToHash("Back Roll");

        [SerializeField] CharacterManager characterManager;
        public float dodgeDistance = 3f;

        public void EnableIframes()
        {
            isDodging = true;
        }

        public void StopIframes()
        {
            isDodging = false;
        }

        public override void HandleDodge()
        {
            Dictionary<int, Vector3> hash = GetValidDodgeDirections();

            if (hash.Count <= 0)
            {
                return;
            }

            var randomEntry = hash.ElementAt(Random.Range(0, hash.Count));
            characterManager.PlayBusyHashedAnimationWithRootMotion(randomEntry.Key);
            isDodging = true;
        }

        public override bool CanDodge()
        {
            if (isDodging)
            {
                return false;
            }

            return true;
        }

        public Dictionary<int, Vector3> GetValidDodgeDirections()
        {
            Dictionary<int, Vector3> validDodgeDirections = new();

            Vector3 origin = characterManager.transform.position;
            Vector3 forward = characterManager.transform.forward;
            Vector3 right = characterManager.transform.right;

            TryAddDirection(origin + forward * dodgeDistance, hashForwardRoll, ref validDodgeDirections);
            TryAddDirection(origin - forward * dodgeDistance, hashBackwardRoll, ref validDodgeDirections);
            TryAddDirection(origin - forward * dodgeDistance, hashBackstep, ref validDodgeDirections);
            TryAddDirection(origin + right * dodgeDistance, hashRightRoll, ref validDodgeDirections);
            TryAddDirection(origin - right * dodgeDistance, hashLeftRoll, ref validDodgeDirections);

            return validDodgeDirections;
        }

        private void TryAddDirection(Vector3 targetPosition, int hash, ref Dictionary<int, Vector3> dict)
        {
            if (NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, dodgeDistance, NavMesh.AllAreas))
            {
                dict[hash] = hit.position;
            }
        }
    }
}
