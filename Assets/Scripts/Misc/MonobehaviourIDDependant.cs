namespace AF
{
    using System.Linq;
    using AF.Flags;
    using UnityEngine;
    using UnityEngine.Events;

    public class MonobehaviourIDDependant : MonoBehaviour
    {
        [SerializeField] MonoBehaviourID[] monoBehaviourIDs;
        [SerializeField] FlagsDatabase flagsDatabase;

        [SerializeField] UnityEvent onAllTrue;
        [SerializeField] UnityEvent onFalse;

        [Header("Debug Options")]
        [SerializeField] bool debug_activateAllTrue = false;

        /// <summary>
        /// Unity Event
        /// </summary>
        public void OnActivate()
        {
            if (debug_activateAllTrue || monoBehaviourIDs.All(x => flagsDatabase.ContainsFlag(x.ID)))
            {
                onAllTrue?.Invoke();
            }
            else
            {
                onFalse?.Invoke();
            }
        }

    }
}
