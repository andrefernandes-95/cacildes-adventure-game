using System.Collections;
using AF.Dialogue;
using AF.Stats;
using UnityEngine;

namespace AF
{
    public class EV_MessageBasedOnStats : EventBase
    {
        [Header("Actor")]
        public Character character;

        public string strengthMessage;
        public string dexterityMessage;
        public string intelligenceMessage;

        // Scene Refs
        UIDocumentDialogueWindow uIDocumentDialogueWindow;

        [Header("Character Stats")]
        [SerializeField] StatsBonusController statsBonusController;

        public override IEnumerator Dispatch()
        {
            string message = strengthMessage;

            int strength = statsBonusController.character.characterBaseStats.GetStrength();
            int dexterity = statsBonusController.character.characterBaseStats.GetDexterity();
            int intelligence = statsBonusController.character.characterBaseStats.GetIntelligence();

            if (strength >= dexterity && strength >= intelligence)
            {
                message = strengthMessage;
            }
            else if (dexterity >= strength && dexterity >= intelligence)
            {
                message = dexterityMessage;
            }
            else if (intelligence >= strength && intelligence >= dexterity)
            {
                message = intelligenceMessage;
            }

            yield return GetUIDocumentDialogueWindow().DisplayMessage(
                character, message, new Response[0]);
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        UIDocumentDialogueWindow GetUIDocumentDialogueWindow()
        {
            if (uIDocumentDialogueWindow == null)
            {
                uIDocumentDialogueWindow = FindAnyObjectByType<UIDocumentDialogueWindow>(FindObjectsInactive.Include);
            }

            return uIDocumentDialogueWindow;
        }
    }
}
