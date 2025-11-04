using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class EV_Fade : EventBase
    {
        public float duration = 1f;


        [Header("Unity Events")]
        public UnityEvent duringFadeTransitionsEventCallback;
        [Header("Settings")]
        public bool fadeIn = false;
        public bool fadeOut = false;

        [TextArea]
        public string comment;

        // Scene Refs    
        FadeManager fadeManager;

        UIDocumentDialogueWindow uIDocumentDialogueWindow;

        UIDocumentDialogueWindow GetUIDocumentDialogueWindow()
        {
            if (uIDocumentDialogueWindow == null)
            {
                uIDocumentDialogueWindow = FindAnyObjectByType<UIDocumentDialogueWindow>(FindObjectsInactive.Include);
            }

            return uIDocumentDialogueWindow;
        }

        public override IEnumerator Dispatch()
        {

            if (fadeIn)
            {
                GetFadeManager().FadeIn(duration);
                yield return new WaitForSeconds(duration);
                yield break;
            }
            else if (fadeOut)
            {
                GetFadeManager().FadeOut(duration);
                yield return new WaitForSeconds(duration);
                yield break;
            }

            GetFadeManager().FadeIn(duration);
            yield return new WaitForSeconds(duration);

            // Safely disable dialogue window when fading, for the bug where we talk to companions and they join the party
            // and the dialogue might be triggered by double keys from the player, causing the dialogue to become stale
            GetUIDocumentDialogueWindow().HideDialogueWindow();

            GetFadeManager().FadeOut(1f);
            duringFadeTransitionsEventCallback?.Invoke();

        }

        FadeManager GetFadeManager()
        {
            if (fadeManager == null)
            {
                fadeManager = FindAnyObjectByType<FadeManager>(FindObjectsInactive.Include);
            }

            return fadeManager;
        }
    }
}
