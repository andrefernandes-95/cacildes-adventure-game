using UnityEngine;

namespace AF
{
    public abstract class NPCActivity : MonoBehaviour
    {
        int hashIdle = Animator.StringToHash("Idle");

        public float stoppingDistance = 0.5f;

        public abstract void OnActivityStart(CharacterBaseManager activityTarget);

        public abstract void OnActivityEnd(CharacterBaseManager activityTarget);

        public abstract bool HasReachedActivity(CharacterBaseManager activityTarget);
        public abstract Transform GetActivityDestination();

        /// <summary>
        /// Animation Event Listener
        /// </summary>
        public abstract void OnActivityPerformed(CharacterBaseManager activityTarget);

        protected void ReturnToIdle(Animator animator, float crossFade = 0.1f)
        {
            animator.CrossFade(hashIdle, crossFade);
        }

    }
}
