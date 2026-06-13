namespace AF
{
    using System.Collections;
    using UnityEngine;

    public abstract class CharacterAbstractGesture : MonoBehaviour
    {
        [Header("Animator")]
        [SerializeField] CharacterBaseManager characterBaseManager;
        protected Gesture currentGesture;

        public void ResetStates()
        {
            currentGesture = null;
        }

        public virtual void PlayGesture(Gesture gesture)
        {
            StartCoroutine(WaitForNextFrameThenPlayGestureCoroutine(gesture));
        }

        IEnumerator WaitForNextFrameThenPlayGestureCoroutine(Gesture gesture)
        {
            if (gesture == null || gesture.animationClip == null)
                yield break;

            yield return new WaitForEndOfFrame();

            currentGesture = gesture;

            characterBaseManager.UpdateAnimatorOverrideControllerClipsUsingDictionary(
                new()
                {
                    { "GestureToIdle", gesture.animationClip },
                    { "GestureLoop", gesture.animationClip },
                }
            );

            yield return new WaitForEndOfFrame();

            if (!gesture.loop)
            {
                characterBaseManager.PlayCrossFadeBusyAnimationWithRootMotion("GestureToIdle", gesture.crossFade);
            }
            else
            {
                characterBaseManager.PlayCrossFadeBusyAnimationWithRootMotion("GestureLoop", gesture.crossFade);
            }
        }

        public bool IsPlayingGesture()
        {
            return currentGesture != null;
        }

        public void StopGestureInLoop()
        {
            if (currentGesture != null)
            {
                currentGesture = null;
                characterBaseManager.PlayBusyAnimationWithRootMotion(GetIdleAnimationName());
            }
        }

        public abstract string GetIdleAnimationName();
    }
}
