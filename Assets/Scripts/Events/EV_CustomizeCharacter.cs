using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace AF
{
    public class EV_CustomizeCharacter : EventBase
    {
        [SerializeField] UIDocumentCharacterCustomization uIDocumentCharacterCustomization;

        public override IEnumerator Dispatch()
        {
            if (uIDocumentCharacterCustomization.isActiveAndEnabled)
            {
                uIDocumentCharacterCustomization.gameObject.SetActive(false);
            }
            else
            {
                uIDocumentCharacterCustomization.gameObject.SetActive(true);
            }

            yield return null;
        }

    }
}
