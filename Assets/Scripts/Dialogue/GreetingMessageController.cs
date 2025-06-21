using System.Collections;
using UnityEngine;

namespace AF.Dialogue
{
    public class GreetingMessageController : MonoBehaviour
    {
        [Header("Flags")]
        bool isGreeting = false;
        public bool IsGreeting() => isGreeting;

        [Header("Components")]
        [SerializeField] GreetingMessageUI greetingMessageUI;
        [SerializeField] CharacterManager characterManager;

        const string GESTURE_ANIMATION_OVERRIDE = "Gesture";

        Coroutine GreetingCoroutine;

        public void ShowGreeting(CharacterGreeting characterGreeting)
        {
            isGreeting = true;

            if (characterGreeting.shouldFacePlayerWhenGreeting)
            {
                characterManager.FacePlayer();
            }

            if (GreetingCoroutine != null)
            {
                StopCoroutine(GreetingCoroutine);
            }

            GreetingCoroutine = StartCoroutine(ShowGreeting_Coroutine(characterGreeting));
        }

        IEnumerator ShowGreeting_Coroutine(CharacterGreeting characterGreeting)
        {
            if (characterGreeting.gesture != null)
            {
                HandleGesture(characterGreeting.gesture);
            }

            HandleGreetingSound();

            greetingMessageUI.Display(characterGreeting.greeting);

            yield return new WaitForSeconds(characterGreeting.duration);

            greetingMessageUI.Hide();

            if (characterGreeting.shouldFacePlayerWhenGreeting)
            {
                characterManager.FaceInitialRotation();
            }

            isGreeting = false;
        }

        void HandleGreetingSound()
        {
            if (characterManager.combatant != null && characterManager.combatant.greeting != null)
            {
                characterManager.combatAudioSource.PlayOneShot(characterManager.combatant.greeting);
            }
        }

        void HandleGesture(AnimationClip gestureClip)
        {
            if (gestureClip == null)
            {
                return;
            }

            characterManager.UpdateAnimatorOverrideControllerClips(GESTURE_ANIMATION_OVERRIDE, gestureClip);
            characterManager.PlayBusyAnimationWithRootMotion("Gesture");
        }
    }
}
