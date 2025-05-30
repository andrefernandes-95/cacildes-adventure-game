using System;
using System.Collections;
using System.Linq;
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

        [Header("Conditions")]
        [SerializeField] int minimumStrength = 0;
        [SerializeField] int minimumDexterity = 0;
        [SerializeField] int minimumIntelligence = 0;

        public override IEnumerator Dispatch()
        {
            string message = strengthMessage;

            int strength = statsBonusController.GetCurrentStrength();
            int dexterity = statsBonusController.GetCurrentDexterity();
            int intelligence = statsBonusController.GetCurrentIntelligence();

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
