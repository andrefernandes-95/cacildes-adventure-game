
using System.Collections.Generic;
using System.Linq;
using AF.Characters;
using AF.Combat;
using UnityEngine;
using UnityEngine.Events;
using Vector3 = UnityEngine.Vector3;

namespace AF.Detection
{
    public class Sight : MonoBehaviour
    {
        [Header("Radius")]
        [SerializeField] float detectionRadius = 15f;
        [SerializeField] float minimumDetectionAngle = -35;
        [SerializeField] float maximumDetectionAngle = 35;

        [Header("Detection")]
        public float viewDistance = 10f;
        public LayerMask targetLayer;
        public LayerMask environmentBlockLayer;

        [Header("Components")]
        public Transform origin;
        public TargetManager targetManager;

        [Header("Tags")]
        public List<string> tagsToDetect = new();

        [Header("Factions")]
        public List<CharacterFaction> factionsToIgnore = new();

        [Header("Events")]
        public UnityEvent OnTargetSighted;

        [Header("Settings")]
        public bool debug = false;

        [Header("Flags")]
        [SerializeField] bool isSighted = false;
        public bool canSight = true;

        public CharacterBaseManager GetTargetInSight()
        {
            if (targetManager.currentTarget != null)
            {
                return null;
            }

            CharacterManager sightOwner = targetManager.characterManager;

            Collider[] colliders = Physics.OverlapSphere(sightOwner.transform.position, detectionRadius, targetLayer);
            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterBaseManager targetCharacter = colliders[i].transform.GetComponent<CharacterBaseManager>();

                if (targetCharacter == null)
                {
                    continue;
                }

                if (tagsToDetect.Count > 0 && !tagsToDetect.Contains(colliders[i].transform.gameObject.tag))
                {
                    continue;
                }

                // Target is self? Ignore
                if (targetCharacter.transform.root == sightOwner.transform.root)
                {
                    continue;
                }

                if (targetCharacter.health.GetCurrentHealth() <= 0)
                {
                    continue;
                }

                if (sightOwner.IsFromSameFaction(targetCharacter))
                {
                    continue;
                }

                // If a potential target is found, it has to be in front of us
                Vector3 possibleTargetDirection = targetCharacter.transform.position - sightOwner.transform.position;
                float viewableAngleOfPossibleTarget = Vector3.Angle(possibleTargetDirection, sightOwner.transform.forward);

                if (viewableAngleOfPossibleTarget > minimumDetectionAngle && viewableAngleOfPossibleTarget < maximumDetectionAngle)
                {
                    // Lastly, check for environment blocks
                    bool isObstructed = Physics.Linecast(origin.position, origin.position, environmentBlockLayer);
                    if (!isObstructed)
                    {
                        return targetCharacter;
                    }
                }
            }

            return null;
        }
        public void CastSight()
        {
            if (canSight == false)
            {
                return;
            }

            CharacterBaseManager potentialTarget = GetTargetInSight();

            if (potentialTarget != null)
            {
                targetManager.SetTarget(potentialTarget, () =>
                {
                    OnTargetSighted?.Invoke();
                }, false);
            }
        }

        public void SetDetectionLayer(string layerName)
        {
            this.targetLayer = LayerMask.GetMask(layerName);
        }

        public void SetTagsToDetect(List<string> tagsToDetect)
        {
            this.tagsToDetect.Clear();
            this.tagsToDetect = tagsToDetect;
        }

        public void Set_CanSight(bool value)
        {
            canSight = value;
        }
    }
}
