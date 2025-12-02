using System.Collections;
using UnityEngine;

namespace AF
{
    public class EV_SetTransformRotation : EventBase
    {
        public Transform transformTarget;
        public Quaternion targetRotation;

        [Header("Local Rotation")]
        public bool isLocal = false;
        public Vector3 localRotation;

        public override IEnumerator Dispatch()
        {
            SetTransformRotation();
            yield return null;
        }

        public void SetTransformRotation()
        {
            if (isLocal)
            {
                transformTarget.localRotation = Quaternion.Euler(localRotation);
                return;
            }

            transformTarget.transform.rotation = targetRotation;
        }
    }
}
